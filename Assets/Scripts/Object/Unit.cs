using Sof.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class Unit : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _MoveTile;

        [SerializeField]
        private int _Speed;
        [SerializeField]
        private int _Health;
        [SerializeField]
        private int _Damage;

        public Model.Unit ModelUnit { get; private set; }

        public int Speed => _Speed;
        public int Health => _Health;
        public int Damage => _Damage;

        private GameManager _GameManager;

        public void Initialize(GameManager gameManager, Model.Map map, int playerId)
        {
            if (gameManager == null)
                throw new System.ArgumentNullException(nameof(gameManager));

            if (map == null)
                throw new System.ArgumentNullException(nameof(map));

            _GameManager = gameManager;

            ModelUnit = new Model.Unit(map, _Speed, _Health, _Damage, playerId);
            ModelUnit.UnitMovedAlongPath += ModelUnit_UnitMovedAlongPath; ;
        }

        public void Move(Position pos) => ModelUnit.Move(pos);

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

        private IEnumerator FollowPath(IEnumerable<Position> path)
        {
            _GameManager.DisableUIInteraction = true;
            HideMoveArea();

            foreach (var point in path)
            {
                var pointVec = new Vector3(point.X, point.Y, 0);
                while (transform.position != pointVec)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pointVec, Time.deltaTime);

                    yield return null;
                }
            }

            ShowMoveArea();
            _GameManager.DisableUIInteraction = false;
        }

        private void ModelUnit_UnitMovedAlongPath(IEnumerable<Position> path)
        {
            StartCoroutine(FollowPath(path));
        }
    }
}