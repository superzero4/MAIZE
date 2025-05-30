﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simulation
{
    public struct Wall
    {
        private Collider collider;

        public Collider Collider => collider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size">Lenght, Height, Width (flipped with the horizontal bool)</param>
        /// <param name="horizontal"></param>
        public Wall(Vector2 center, Vector3 size, bool horizontal)
        {
            if (!horizontal)
                size = new Vector3(size.z, size.y, size.x);
            collider = new Collider(new Bounds(new Vector3(center.x, size.y / 2f, center.y), size));
        }

        override public string ToString()
        {
            return collider.Bounds.ToString();
        }
    }

    public struct Maze
    {
        private Wall[] _walls;
        private Collider _goal;
        public IEnumerable<Wall> Walls => _walls;
        private Bounds _global;

        public Collider Goal => _goal;

        public Bounds Global => _global; 

        public Maze(IEnumerable<Wall> walls, Bounds goal, Bounds global)
        {
            _walls = walls.ToArray();
            _goal = new Collider(goal);
            _global = global;
        }

        public bool Depenetrate(Collider collider, out Vector3 force)
        {
            force = Vector3.zero;
            foreach (var wall in _walls)
            {
                if (collider.Intersects(wall.Collider))
                {
                    Vector3 closest = wall.Collider.Bounds.ClosestPoint(collider.Bounds.center);
                    force += -(closest - collider.Bounds.center);
                }
            }

            return force != Vector3.zero;
        }
    }
}