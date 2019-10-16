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

        private Model.Unit _Unit;

        public int Speed => _Speed;
        public int Health => _Health;
        public int Damage => _Damage;

        public Position Pos => _Unit.Pos;

        public void Initialize(Model.Unit unit)
        {
            _Unit = unit ?? throw new System.ArgumentNullException(nameof(unit));
        }

        public void Move(Position pos)
        {
            _Unit.SetTargetPosition(pos);

            var path = _Unit.GetPathToTarget();

            _Unit.Move();

            StartCoroutine(FollowPath(path));
        }

        public void ShowMoveArea()
        {
            var moveArea = _Unit.GetMoveRange();

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