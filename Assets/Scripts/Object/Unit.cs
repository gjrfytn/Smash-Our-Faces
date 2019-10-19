using Sof.Model;
using System.Collections;
using System.Collections.Generic;
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

        public void Initialize(Model.Map map, int playerId)
        {
            if (map == null)
                throw new System.ArgumentNullException(nameof(map));

            ModelUnit = new Model.Unit(map, _Speed, _Health, _Damage, playerId);
        }

        public void Move(Position pos)
        {
            ModelUnit.SetTargetPosition(pos);

            var path = ModelUnit.GetPathToTarget();

            ModelUnit.Move();

            StartCoroutine(FollowPath(path));
        }

        public void ShowMoveArea()
        {
            var moveArea = ModelUnit.GetMoveRange();

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
            foreach (var point in path)
            {
                var pointVec = new Vector3(point.X, point.Y, 0);
                while (transform.position != pointVec)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pointVec, Time.deltaTime);

                    yield return null;
                }
            }
        }
    }
}