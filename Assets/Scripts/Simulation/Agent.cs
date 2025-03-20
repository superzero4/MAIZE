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

        public Agent(Vector2 XZposition, float radius, float speed, float orientation, float _rotationSpeed,
            Vision vision)
        {
            _jumpStart = -2 * _jumpDuration;
            _vision = vision;
            this._rotationSpeed = _rotationSpeed;
            _orientation = orientation;
            _collider = new Collider(new Vector3(XZposition.x, radius / 2f, XZposition.y),
                new Vector3(radius, radius, radius));
            _speed = speed;
        }

        public void MoveVector(Vector3 displacement)
        {
            _collider.Move(displacement);
        }

        public void Move(float displacementIntensity)
        {
            (float x, float y) = (Mathf.Cos((_orientation) * Mathf.Deg2Rad), Mathf.Sin((_orientation) * Mathf.Deg2Rad));
            Assert.IsTrue(displacementIntensity >= -1 && displacementIntensity <= 1);
            _collider.Move(new Vector3(y, 0, x) *
                           (displacementIntensity * _speed));
        }

        public Collider Collider => _collider;
        public float Orientation => _orientation;

        public float[] ComputeVision(Maze maze)
        {
            return _vision.ComputeVision(this, maze);
        }

        public void Rotate(float rotation)
        {
            Assert.IsTrue(rotation >= -1 && rotation <= 1);
            _orientation += rotation * _rotationSpeed;
        }

        private float _jumpStart;
        private const float _jumpHeight = .1f;
        private const float _jumpDuration = 1f;

        public void Jump()
        {
            if (_jumpStart + _jumpDuration < Time.time)
                _jumpStart = Time.time;
        }

        private float Radius => _collider.Bounds.size.x;

        public void Tick()
        {
            float timeJump = Time.time - _jumpStart;
            float gravity = 9.81f;
            if (timeJump < _jumpDuration)
            {
                gravity *= -_jumpHeight;
                //float h = Mathf.Sin(timeJump * Mathf.PI/ _jumpDuration) * _jumpHeight * Radius * Time.deltaTime;
                //Debug.Log($"Jumping with h={h}, timeJump={timeJump}, _jumpStart={_jumpStart}, Time.time={Time.time}");
                //_collider.Move(Vector3.up * h);
            }
            if(_collider.Bounds.center.y > Radius || gravity < 0)
                _collider.Move(Vector3.down * (gravity * Time.deltaTime));
            //else
            //    _collider.Bounds.center = new Vector3(_collider.Bounds.center.x, Radius, _collider.Bounds.center.z);
        }
    }
}