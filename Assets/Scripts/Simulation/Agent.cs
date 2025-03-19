using NUnit.Framework;
using UnityEngine;

namespace Simulation
{
    public struct Agent
    {
        private Collider _collider;
        private float _orientation;
        private float _speed;
        private float _rotationSpeed;
        private Vision _vision;

        public Agent(Vector2 XZposition, float radius, float speed, float orientation, float _rotationSpeed, Vision vision)
        {
            _vision = vision;
            this._rotationSpeed = _rotationSpeed;
            _orientation = orientation;
            _collider = new Collider(new Vector3(XZposition.x, 0, XZposition.y), new Vector3(radius, radius, radius));
            _speed = speed;
        }
        public void MoveVector(Vector3 displacement)
        {
            _collider.Move(displacement);
        }
        public void Move(float displacementIntensity)
        {
            Assert.IsTrue(displacementIntensity>=-1 && displacementIntensity<=1);
            _collider.Move(new Vector3(Mathf.Cos(_orientation+90),0,Mathf.Sin(_orientation+90)) * (displacementIntensity * _speed));
        }

        public Collider Collider => _collider;
        public float Orientation => _orientation;
        public float[] ComputeVision(Maze maze)
        {
            return _vision.ComputeVision(this, maze);
        }
        public void Rotate(float rotation)
        {
            Assert.IsTrue(rotation>=-1 && rotation<=1);
            _orientation += rotation * _rotationSpeed;
        }
    }
}