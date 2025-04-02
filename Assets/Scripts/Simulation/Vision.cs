using System.Linq;
using UnityEngine;

namespace Simulation
{
    public class Vision
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
        public bool hasGoalInSight;
        public int Resolution => _visionBuffer.Length;

        public Vision(float radius, int resolution, float range)
        {
            _range = range;
            _radius = radius;
            if (resolution % 2 == 0)
                resolution++; //To ensure we have a center aligned point
            _visionBuffer = new VisionData[resolution];
            hasGoalInSight = false;
        }

        public VisionData[] ComputeVision(MazeAgent mazeAgent, Maze maze)
        {
            hasGoalInSight = false;
            for (int i = 0; i < Resolution; i++)
            {
                float offset = ((2f * i) / Resolution - 1) * _radius;
                var angle = mazeAgent.Orientation + offset;
                angle += 0; //So we remap 0 to forward and not right (-90 as we want)
                angle *= Mathf.Deg2Rad;
                var direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                float distance = _range;
                bool goal = false;
                float result;
                _visionBuffer[i].isGoal = false;
                Ray ray = new Ray(mazeAgent.Collider.Bounds.center, direction);
                //Debug.Log($"Ray {ray} from angle {angle * Mathf.Rad2Deg} for offset {offset}");
                foreach (Wall wall in maze.Walls)
                    if (wall.Collider.Intersects(ray, out result) && result < distance)
                        distance = result;

                if (maze.Goal.Intersects(ray, out result) && result < distance)
                {
                    distance = result;
                    _visionBuffer[i].isGoal = true;
                    if (!hasGoalInSight)
                        hasGoalInSight = true;
                }

                _visionBuffer[i].distance = distance;
            }

            return _visionBuffer;
        }
    }
}