using System;
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

        public IEnumerable<Position> GetBestPath(Position from, Position to)
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

            throw new Exception("Pathfinding error.");
        }

        private bool Spread(List<(Position pos, int cost)> queue, Dictionary<Position, Position> processedCells, Dictionary<Position, int> costs, (Position pos, int cost) node, Position target)
        {
            if (node.pos == target)
                return true;

            foreach (var neighbour in GetNeighbours(node.pos))
            {
                var newCost = node.cost + _Map[neighbour.X, neighbour.Y].MoveCost;
                if (!processedCells.ContainsKey(neighbour) || newCost < costs[neighbour])
                {
                    processedCells[neighbour] = node.pos;
                    costs[neighbour] = newCost;
                    queue.Add((neighbour, newCost));
                }
            }

            return false;
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
    }
}
