using System.Collections.Generic;
using System.Linq;

namespace Sof.Model
{
    internal class Pathfinder
    {
        private readonly Map _Map;

        public Pathfinder(Map map)
        {
            _Map = map;
        }

        public IEnumerable<Position> GetClosestPath(Position from, Position to)
        {
            var processedCells = new Dictionary<Position, Position>();
            var costs = new Dictionary<Position, int>();
            var queue = new List<(Position pos, int cost)>();

            processedCells[from] = null;
            costs[from] = 0;
            queue.Add((from, 0));
            while (queue.Any())
            {
                queue.Sort((a, b) => a.cost < b.cost ? -1 : (a.cost == b.cost ? 0 : 1));
                var node = queue.First();
                queue.Remove(node);
                if (Spread(queue, processedCells, costs, node, to))
                    return MakePath(from, to, processedCells);
            }

            return MakePath(from, GetClosestPositions(to, processedCells.Keys).OrderBy(p => costs[p]).First(), processedCells);
        }

        public IEnumerable<MovePoint> GetMoveRange(Position pos, int movePoints)
        {
            var processedCells = new Dictionary<Position, Position>();
            var costs = new Dictionary<Position, int>();
            var queue = new List<(Position pos, int cost)>();

            processedCells[pos] = null;
            costs[pos] = 0;
            queue.Add((pos, 0));
            while (queue.Any())
            {
                queue.Sort((a, b) => a.cost < b.cost ? -1 : (a.cost == b.cost ? 0 : 1));
                var node = queue.First();
                queue.Remove(node);
                Spread(queue, processedCells, costs, node, movePoints);
            }

            return processedCells.Keys.Select(c => new MovePoint(c, costs[c]));
        }

        private bool Spread(List<(Position pos, int cost)> queue, Dictionary<Position, Position> processedCells, Dictionary<Position, int> costs, (Position pos, int cost) node, Position target)
        {
            if (node.pos == target)
                return true;

            foreach (var neighbour in GetNeighbours(node.pos))
                if (!_Map[neighbour].Blocked)
                {
                    var neighbourCost = node.cost + _Map[neighbour].MoveCost;
                    if (!processedCells.ContainsKey(neighbour) || neighbourCost < costs[neighbour])
                    {
                        processedCells[neighbour] = node.pos;
                        costs[neighbour] = neighbourCost;
                        queue.Add((neighbour, neighbourCost));
                    }
                }

            return false;
        }

        private void Spread(List<(Position pos, int cost)> queue, Dictionary<Position, Position> processedCells, Dictionary<Position, int> costs, (Position pos, int cost) node, int movePoints)
        {
            foreach (var neighbour in GetNeighbours(node.pos))
                if (!_Map[neighbour].Blocked)
                {
                    var neighbourCost = node.cost + _Map[neighbour].MoveCost;
                    if (movePoints - neighbourCost >= 0 && (!processedCells.ContainsKey(neighbour) || neighbourCost < costs[neighbour]))
                    {
                        processedCells[neighbour] = node.pos;
                        costs[neighbour] = neighbourCost;
                        queue.Add((neighbour, neighbourCost));
                    }
                }
        }

        private IEnumerable<Position> MakePath(Position from, Position to, Dictionary<Position, Position> processedCells)
        {
            var current = to;
            var path = new List<Position>();
            while (current != from)
            {
                path.Add(current);
                current = processedCells[current];
            }

            path.Reverse();

            return path;
        }

        private IEnumerable<Position> GetNeighbours(Position pos)
        {
            var neighbours = new List<Position>();

            if (pos.X - 1 >= 0)
                neighbours.Add(new Position(pos.X - 1, pos.Y));

            if (pos.Y + 1 < _Map.Height)
                neighbours.Add(new Position(pos.X, pos.Y + 1));

            if (pos.X + 1 < _Map.Width)
                neighbours.Add(new Position(pos.X + 1, pos.Y));

            if (pos.Y - 1 >= 0)
                neighbours.Add(new Position(pos.X, pos.Y - 1));

            return neighbours;
        }

        private IEnumerable<Position> GetClosestPositions(Position pos, IEnumerable<Position> options)
        {
            var orderedPositions = options.Select(p => new { Pos = p, Dist = UnityEngine.Mathf.Abs(p.X - pos.X) + UnityEngine.Mathf.Abs(p.Y - pos.Y) })
                                          .OrderBy(p => p.Dist)
                                          .ToArray();

            return orderedPositions.Where(p => p.Dist == orderedPositions.First().Dist).Select(p => p.Pos);
        }
    }
}
