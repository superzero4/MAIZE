using UnityEngine;

namespace Simulation
{
    public struct Vision
    {
        private float _radius;
        private float _range;
        private float[] _visionBuffer;

        public Vision(float radius, int resolution, float range)
        {
            _range = range;
            _radius = radius;
            if (resolution % 2 == 0)
                resolution++; //To ensure we have a center aligned point
            _visionBuffer = new float[resolution];
        }

        public float[] ComputeVision(Agent agent, Maze maze)
        {
            for (int i = 0; i < _visionBuffer.Length; i++)
            {
                float offset = ((2f * i) / _visionBuffer.Length - 1) * _radius;
                var angle = agent.Orientation + offset;
                angle += 90;//So we remap 0 to forward and not right (-90 as we want)
                angle *= Mathf.Deg2Rad;
                var direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                float distance = _range;
                Ray ray = new Ray(agent.Collider.Bounds.center, direction);
                Debug.Log($"Ray {ray} from angle {angle * Mathf.Rad2Deg} for offset {offset}");
                foreach (var wall in maze.Walls)
                {
                    Debug.Log($"Wall {wall} hit at {_range}");
                    if (wall.Collider.Intersects(ray, out float result))
                    {
                        if (result < distance)
                        {
                            distance = result;
                            //We find the closest wall
                        }
                    }
                }

                _visionBuffer[i] = distance;
            }

            return _visionBuffer;
        }
    }
}