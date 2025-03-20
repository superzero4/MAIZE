using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simulation.Generation
{
    public class MazeGenerator
    {
        private Vector2Int _size;
        private Vector2Int _end;

        readonly Vector2Int[] dirs = new Vector2Int[]
            { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        public MazeGenerator(Vector2Int size, Vector2Int end)
        {
            _size = size;
            _end = end;
        }

        struct cell
        {
            public bool[] wallDown;

            public bool this[int nextI]
            {
                get => (wallDown ??= new bool[4])[nextI];
                set => (wallDown ??= new bool[4])[nextI] = value;
            }
        }

        public IEnumerable<(Vector2, bool)> Generate()
        {
            cell[,] maze = GenerateMap(Vector2Int.zero);
            var list = new List<(Vector2, bool)>();
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    var info = maze[x, y];
                    for (int i = 0; i < 4; i++)
                    {
                        if (info.wallDown == null || !info.wallDown[i])
                            list.Add((new Vector2(x + dirs[i].x / 2f, y + dirs[i].y / 2f), i % 2 == 0));
                    }
                }
            }

            return list;
        }

        private cell[,] GenerateMap(Vector2Int start)
        {
            int width = _size.x;
            int length = _size.y;
            cell[,] maze = new cell[width, length];
            HashSet<Vector2Int> visited = new();
            Stack<Vector2Int> toVisit = new();
            toVisit.Push(start);
            visited.Add(start);
            while (toVisit.Count > 0)
            {
                Vector2Int current = toVisit.Pop();
                var valid = dirs
                    .Select((d, i) => (current + d, i))
                    .Where(v => v.Item1.x >= 0 && v.Item1.x < width && v.Item1.y >= 0 && v.Item1.y < length &&
                                !visited.Contains(v.Item1)).ToList();
                if (valid.Count == 0)
                {
                    continue;
                }

                toVisit.Push(current);
                var next = valid[Random.Range(0, valid.Count)];
                toVisit.Push(next.Item1);
                visited.Add(next.Item1);
                maze[current.x, current.y][next.i] = true;
                maze[next.Item1.x, next.Item1.y][(next.i + 2) % 4] = true;
            }

            return maze;
        }
    }
}