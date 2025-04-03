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

        public IEnumerable<(Vector3, bool)> Generate()
        {
            cell[,] maze = GenerateMap(Vector2Int.zero);
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    var info = maze[x, y];
                    for (int i = 0; i < 4; i++)
                    {
                        float height;
                        if (info.wallDown == null || !info.wallDown[i])
                        {
                            //Exterior walls
                            if ((x == 0 && i == 3) || (x == _size.x - 1 && i == 1) || (y == 0 && i == 2) ||
                                (y == _size.y - 1 && i == 0))
                                height = 1f;
                            else
                                height = Random.value > .5f
                                    ? 1f
                                    : .5f;
                        }
                        //If a wall should be up, it's either fully up or half up
                        else
                            height = Random.value > .5f
                                ? 0f
                                : .5f; //If a wall should be down, it's either fully down or half down

                        if (height >= .25f) //We only want at least half up walls to be concretely in
                            yield return (new Vector3(x + dirs[i].x / 2f, height, y + dirs[i].y / 2f), i % 2 == 0);
                    }
                }
            }
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