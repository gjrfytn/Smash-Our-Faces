using System.Collections.Generic;
using System.Linq;

namespace Sof.Model
{
    public class Unit
    {
        private readonly Map _Map;
        private readonly int _MovePoints;
        private readonly int _Health;
        private readonly int _Damage;

        private Position _TargetPos;
        private int _MovePointsLeft;
        private IEnumerable<Position> _CurrentPath;

        public int PlayerId { get; private set; }

        public Unit(Map map, int movePoints, int health, int damage, int playerId)
        {
            _Map = map;
            _MovePoints = movePoints;
            _Health = health;
            _Damage = damage;
            PlayerId = playerId;
        }

        public void EndTurn()
        {
            _MovePointsLeft = _MovePoints;
        }

        public void SetTargetPosition(Position pos)
        {
            _TargetPos = pos;

            _CurrentPath = _Map.GetBestPath(this, _TargetPos);
        }

        public IEnumerable<Position> GetPathToTarget() => _CurrentPath;

        public void Move()
        {
            _Map.MoveUnit(this, _CurrentPath.Last()); //TODO nullref

            _CurrentPath = null;
        }

        public IEnumerable<MovePoint> GetMoveRange() => _Map.GetMoveRange(this);
    }
}