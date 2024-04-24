using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public static class JumpPointSearch
    {
        public static List<Coordinate> FindPath(MapGenerator mapGenerator, Coordinate start, Coordinate end)
        {
            List<Coordinate> path = new List<Coordinate>();

            HashSet<Coordinate> openSet = new HashSet<Coordinate>();
            HashSet<Coordinate> closedSet = new HashSet<Coordinate>();

            Dictionary<Coordinate, Coordinate> cameFrom = new Dictionary<Coordinate, Coordinate>();

            openSet.Add(start);

            while (openSet.Count > 0)
            {
                Coordinate current = GetNodeWithLowestFCost(openSet, start, end);

                if (current.Equals(end))
                {
                    path = RetracePath(cameFrom, start, end);
                    break;
                }

                openSet.Remove(current);
                closedSet.Add(current);

                List<Coordinate> neighbors = GetNeighbors(mapGenerator, current);
                foreach (Coordinate neighbor in neighbors)
                {
                    if (closedSet.Contains(neighbor) || !IsWalkable(mapGenerator, neighbor))
                    {
                        continue;
                    }

                    int newCost = GetGCost(start, current) + GetDistance(current, neighbor);

                    if (!openSet.Contains(neighbor) || newCost < GetGCost(start, neighbor))
                    {
                        cameFrom[neighbor] = current;
                        openSet.Add(neighbor);
                    }
                }
            }

            return path;
        }

        private static Coordinate GetNodeWithLowestFCost(HashSet<Coordinate> openSet, Coordinate start, Coordinate end)
        {
            Coordinate lowestCostNode = Default.Coordinate;
            int lowestCost = int.MaxValue;

            foreach (Coordinate node in openSet)
            {
                int fCost = GetGCost(start, node) + GetHCost(node, end);
                if (fCost < lowestCost)
                {
                    lowestCost = fCost;
                    lowestCostNode = node;
                }
            }

            return lowestCostNode;
        }

        private static List<Coordinate> RetracePath(Dictionary<Coordinate, Coordinate> cameFrom, Coordinate start, Coordinate end)
        {
            List<Coordinate> path = new List<Coordinate>();
            Coordinate currentNode = end;

            while (!currentNode.Equals(start))
            {
                path.Add(currentNode);
                currentNode = cameFrom[currentNode];
            }

            path.Reverse();
            return path;
        }

        private static int GetGCost(Coordinate start, Coordinate node)
        {
            int dx = Mathf.Abs(start.x - node.x);
            int dy = Mathf.Abs(start.y - node.y);
            return dx + dy;
        }

        private static int GetHCost(Coordinate node, Coordinate end)
        {
            int dx = Mathf.Abs(end.x - node.x);
            int dy = Mathf.Abs(end.y - node.y);
            return dx + dy;
        }

        private static int GetDistance(Coordinate current, Coordinate neighbor)
        {
            int dx = Mathf.Abs(current.x - neighbor.x);
            int dy = Mathf.Abs(current.y - neighbor.y);
            return Mathf.Max(dx, dy);
        }

        private static List<Coordinate> GetNeighbors(MapGenerator mapGenerator, Coordinate node)
        {
            List<Coordinate> neighbors = new List<Coordinate>();

            // Assuming a 4-directional grid
            int[] dx = { 1, 0, -1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = node.x + dx[i];
                int ny = node.y + dy[i];
                neighbors.Add(new Coordinate(nx, ny));
            }

            return neighbors;
        }

        private static bool IsWalkable(MapGenerator mapGenerator, Coordinate node)
        {

            return mapGenerator.IsTileWalkable(node);
        }
    }
}