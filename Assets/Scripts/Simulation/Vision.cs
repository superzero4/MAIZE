using System.Linq;
using UnityEngine;

namespace Simulation
{
    public struct Vision
    {
        public struct VisionData
        {
            public bool isGoal;
            public float distance;
            override public string ToString()
            {
                return $"({distance},{isGoal})";
            }
        }

        private float _radius;
        private float _range;
        private VisionData[] _visionBuffer;

        public Vision(float radius, int resolution, float range)
        {
            _range = range;
            _radius = radius;
            if (resolution % 2 == 0)
                resolution++; //To ensure we have a center aligned point
            _visionBuffer = new VisionData[resolution];
        }

        public VisionData[] ComputeVision(Agent agent, Maze maze)
        {
            for (int i = 0; i < _visionBuffer.Length; i++)
            {
                float offset = ((2f * i) / _visionBuffer.Length - 1) * _radius;
                var angle = agent.Orientation + offset;
                angle += 90; //So we remap 0 to forward and not right (-90 as we want)
                angle *= Mathf.Deg2Rad;
                var direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                float distance = _range;
                bool goal = false;
                Ray ray = new Ray(agent.Collider.Bounds.center, direction);
                //Debug.Log($"Ray {ray} from angle {angle * Mathf.Rad2Deg} for offset {offset}");
                foreach ((Collider wall, bool isGoal) in maze.Walls.Select(w => (w.Collider, false))
                             .Append((maze.Goal, true)))
                {
                    //Debug.Log($"Wall {wall} hit at {_range}");
                    if (wall.Intersects(ray, out float result))
                    {
                        if (result < distance)
                        {
                            distance = result;
                            goal = isGoal;
                        }
                    }
                }

                _visionBuffer[i].distance = distance;
                _visionBuffer[i].isGoal = goal;
            }

            return _visionBuffer;
        }
    }
}