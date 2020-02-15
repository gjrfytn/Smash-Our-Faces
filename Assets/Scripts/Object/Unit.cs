using Sof.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class Unit : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private SpriteRenderer _MoveTile;

        [SerializeField]
        private int _Speed;
        [SerializeField]
        private int _Health;
        [SerializeField]
        private int _Damage;
        [SerializeField]
        private int _AttackRange;
#pragma warning restore 0649

        public Model.Unit ModelUnit { get; private set; }

        public int Speed => _Speed;
        public int Health => _Health;
        public int Damage => _Damage;

        private GameManager _GameManager;
        private Map _Map;

        public void Initialize(GameManager gameManager, Map map, Faction faction)
        {
            if (gameManager == null)
                throw new System.ArgumentNullException(nameof(gameManager));

            if (map == null)
                throw new System.ArgumentNullException(nameof(map));

            _GameManager = gameManager;
            _Map = map;

            ModelUnit = new Model.Unit(gameManager, map.ModelMap, _Speed, _Health, _Damage, _AttackRange, faction, true);
            ModelUnit.UnitMovedAlongPath += ModelUnit_UnitMovedAlongPath;
            ModelUnit.Attacked += ModelUnit_Attacked;
            ModelUnit.TookHit += ModelUnit_TookHit;
            ModelUnit.Died += ModelUnit_Died;
        }

        public void ShowMoveArea()
        {
            var moveArea = ModelUnit.GetMoveRange().ToArray();

            if (moveArea.Length > 1)
                foreach (var moveTile in moveArea)
                    Instantiate(_MoveTile, new Vector3(moveTile.Pos.X, moveTile.Pos.Y, 0), Quaternion.identity, transform);
        }

        public void HideMoveArea()
        {
            for (var i = 0; i < transform.childCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
        }

        private void ModelUnit_UnitMovedAlongPath(IEnumerable<Model.Tile> path)
        {
            StartCoroutine(FollowPath(path));
        }

        private void ModelUnit_Attacked()
        {
            StartCoroutine(PlayAttack());
        }

        private void ModelUnit_TookHit(int damage)
        {
            _GameManager.OnUnitHit(this, damage);
        }

        private void ModelUnit_Died()
        {
            if (ModelUnit.Critical)
                _GameManager.OnCriticalUnitDeath(ModelUnit.Faction);

            Destroy(gameObject);
        }

        private IEnumerator FollowPath(IEnumerable<Model.Tile> path)
        {
            _GameManager.DisableUIInteraction = true;
            HideMoveArea();

            foreach (var tile in path)
            {
                Vector3 tilePos = _Map.GetWorldPos(tile);
                while (transform.position != tilePos)
                {
                    transform.position = Vector3.MoveTowards(transform.position, tilePos, Time.deltaTime);

                    yield return null;
                }
            }

            ShowMoveArea();
            _GameManager.DisableUIInteraction = false;
        }

        private IEnumerator PlayAttack()
        {
            _GameManager.DisableUIInteraction = true;
            HideMoveArea();

            transform.localRotation = Quaternion.Euler(0, 0, -30);

            yield return new WaitForSeconds(0.5f);

            transform.localRotation = Quaternion.Euler(0, 0, 0);

            ShowMoveArea();
            _GameManager.DisableUIInteraction = false;
        }
    }
}