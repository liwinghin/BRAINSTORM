using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class Node
    {
        public Coordinate position;
        public Node parent;
        public int gCost;
        public int hCost;

        public Node(Coordinate position)
        {
            this.position = position;
            this.parent = null;
            this.gCost = 0;
            this.hCost = 0;
        }

        public int FCost
        {
            get { return gCost + hCost; }
        }
    }

    public static class AStar
    {
        public static List<Coordinate> FindPath(MapGenerator mapGenerator, Coordinate start, Coordinate end)
        {
            List<Coordinate> path = new List<Coordinate>();

            HashSet<Coordinate> openSet = new HashSet<Coordinate>();
            HashSet<Coordinate> closedSet = new HashSet<Coordinate>();
            Dictionary<Coordinate, Node> nodeMap = new Dictionary<Coordinate, Node>();

            openSet.Add(start);

            while (openSet.Count > 0)
            {
                Coordinate current = Default.Coordinate;
                foreach (Coordinate node in openSet)
                {
                    if (current.Equals(Default.Coordinate) || GetFCost(node, start, end) < GetFCost(current, start, end))
                    {
                        current = node;
                    }
                }

                if (current.Equals(end))
                {
                    path = RetracePath(nodeMap, start, end);
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

                    int tentativeGCost = GetGCost(start, current) + 1;

                    if (!openSet.Contains(neighbor) || tentativeGCost < GetGCost(start, neighbor))
                    {
                        Node neighborNode = new Node(neighbor);
                        if (nodeMap.TryGetValue(current, out Node currentNode))
                        {
                            neighborNode.parent = currentNode;
                            neighborNode.gCost = tentativeGCost;
                            neighborNode.hCost = GetHCost(neighbor, end);
                        }

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }

                        if (nodeMap.ContainsKey(neighbor))
                        {
                            nodeMap[neighbor] = neighborNode;
                        }
                        else
                        {
                            nodeMap.Add(neighbor, neighborNode);
                        }
                    }
                }
            }

            return path;
        }
        private static List<Coordinate> RetracePath(Dictionary<Coordinate, Node> nodeMap, Coordinate start, Coordinate end)
        {
            List<Coordinate> path = new List<Coordinate>();

            if (nodeMap == null || !nodeMap.ContainsKey(end))
            {
                Debug.LogError("Invalid nodeMap or end coordinate not found in nodeMap.");
                return path;
            }

            Node currentNode = nodeMap[end];

            while (currentNode != null && !currentNode.position.Equals(start))
            {
                path.Add(currentNode.position);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        private static int GetFCost(Coordinate node, Coordinate start, Coordinate end)
        {
            return GetGCost(start, node) + GetHCost(node, end);
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

        private static List<Coordinate> GetNeighbors(MapGenerator mapGenerator, Coordinate node)
        {
            List<Coordinate> neighbors = new List<Coordinate>();
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