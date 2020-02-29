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
        private SpriteRenderer _AttackTile;

        [SerializeField]
        private int _Speed;
        [SerializeField]
        private int _Health;
        [SerializeField]
        private int _Damage;
        [SerializeField]
        private int _AttackRange;
        [SerializeField]
        private int _GoldCost;
        [SerializeField]
        private SpriteRenderer _OwnerFactionSprite;
#pragma warning restore 0649

        private List<SpriteRenderer> _UI_Sprites = new List<SpriteRenderer>();

        public Model.Unit ModelUnit { get; private set; }

        public int Speed => _Speed;
        public int Health => _Health;
        public int Damage => _Damage;

        private GameManager _GameManager;
        private Map _Map;

        public void Initialize(GameManager gameManager, Map map, Faction faction)
        {
            _GameManager = gameManager != null ? gameManager : throw new System.ArgumentNullException(nameof(gameManager));
            _Map = map != null ? map : throw new System.ArgumentNullException(nameof(map));

            if (faction == null)
                throw new System.ArgumentNullException(nameof(faction));

            ModelUnit = new Model.Unit(gameManager, map.ModelMap, _Speed, _Health, _Damage, _AttackRange, faction, true, _GoldCost);
            ModelUnit.MovedAlongPath += ModelUnit_MovedAlongPath;
            ModelUnit.Attacked += ModelUnit_Attacked;
            ModelUnit.TookHit += ModelUnit_TookHit;
            ModelUnit.Healed += ModelUnit_Healed;
            ModelUnit.Died += ModelUnit_Died;

            SetOwnerColor();
        }

        public void ShowUI()
        {
            var moveArea = ModelUnit.GetMoveRange().ToArray();

            if (moveArea.Length > 1)
                foreach (var movePoint in moveArea)
                    _UI_Sprites.Add(Instantiate(_MoveTile, _Map.GetWorldPos(movePoint.Tile), Quaternion.identity, transform));

            var attackArea = ModelUnit.GetAttackArea().ToArray();

            if (attackArea.Length > 1)
                foreach (var attackTile in attackArea)
                    _UI_Sprites.Add(Instantiate(_AttackTile, _Map.GetWorldPos(attackTile), Quaternion.identity, transform));
        }

        public void HideUI()
        {
            foreach (var @object in _UI_Sprites)
                Destroy(@object.gameObject);

            _UI_Sprites.Clear();
        }

        private void ModelUnit_MovedAlongPath(IEnumerable<Model.Tile> path)
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

        private void ModelUnit_Healed(int heal)
        {
            _GameManager.OnUnitHeal(this, heal);
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
            HideUI();

            foreach (var tile in path)
            {
                Vector3 tilePos = _Map.GetWorldPos(tile);
                while (transform.position != tilePos)
                {
                    transform.position = Vector3.MoveTowards(transform.position, tilePos, Time.deltaTime);

                    yield return null;
                }
            }

            ShowUI();
            _GameManager.DisableUIInteraction = false;
        }

        private IEnumerator PlayAttack()
        {
            _GameManager.DisableUIInteraction = true;
            HideUI();

            transform.localRotation = Quaternion.Euler(0, 0, -30);

            yield return new WaitForSeconds(0.5f);

            transform.localRotation = Quaternion.Euler(0, 0, 0);

            ShowUI();
            _GameManager.DisableUIInteraction = false;
        }

        private void SetOwnerColor()
        {
            _OwnerFactionSprite.color = _GameManager.GetFactionColor(ModelUnit.Faction);
        }
    }
}