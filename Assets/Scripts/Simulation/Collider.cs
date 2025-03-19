using UnityEngine;

namespace Simulation
{
    public struct Collider
    {
        private Bounds _bounds;
        public Collider(Bounds bounds)
        {
            _bounds = bounds;
        }
        public Collider(Vector3 center, Vector3 size) : this (new Bounds(center, size))
        {
        }
        public Bounds Bounds
        {
            get { return _bounds; }
        }

        public void Move(Vector3 displacement)
        {
            _bounds.center += displacement;
        }

        public bool Intersects(Collider other)
        {
            return _bounds.Intersects(other.Bounds);
        }

        public bool Intersects(Ray ray, out float distance)
        {
            return _bounds.IntersectRay(ray, out distance);
        }
    }
}