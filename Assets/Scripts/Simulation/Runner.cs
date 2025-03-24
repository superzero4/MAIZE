using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simulation
{
    public static class Time
    {
        public static float time;
        public static float deltaTime;
    }

    public class Runner
    {
        private MazeAgent _mazeAgent;
        private Maze _maze;
        private IBrain _brain;

        private float time = 0f;

        public bool GoalReached =>
            AgentDistToGoal < Mathf.Min(_maze.Goal.Bounds.extents.x, _mazeAgent.Collider.Bounds.extents.x);

        public double AgentDistToGoal => DistToGoal(_mazeAgent.Collider.Bounds.center);
        public double DistToGoal(Vector3 pos) => Mathf.Sqrt(GoalChecker.SqrDistance(pos));
        public float RelativeAgentDistToGoal => (float)(AgentDistToGoal / DistToGoal(startPos));
        public Bounds GoalChecker => _maze.Goal.Bounds;

        public Runner(Maze maze, IBrain brain, MazeAgent mazeAgent)
        {
            _brain = brain;
            _mazeAgent = mazeAgent;
            _maze = maze;
        }

        public void SetBrain(IBrain brain) => _brain = brain;

        public Maze Maze => _maze;

        public MazeAgent MazeAgent => _mazeAgent;
        private Vector3 startPos;

        public void Init()
        {
            time = 0f;
            startPos = _mazeAgent.Collider.Bounds.center;
        }

        public void Tick(float deltaTime, out bool hitWall, out bool reachedObjective)
        {
            Time.deltaTime = deltaTime;
            Time.time += deltaTime;
            var dir = _brain.GetRotation(_mazeAgent, _maze);
            _mazeAgent.Rotate(dir * deltaTime);
            var imp = _brain.GetImpulsion(_mazeAgent, _maze);
            _mazeAgent.Move(imp * deltaTime);
            var jump = _brain.GetJump(_mazeAgent, _maze);
            if (jump)
                _mazeAgent.Jump();
            _mazeAgent.Tick();
            hitWall = false;
            if (_maze.Depenetrate(_mazeAgent.Collider, out Vector3 force))
            {
                hitWall = true;
                _mazeAgent.MoveVector(force);
            }

            reachedObjective = GoalReached;
        }
    }
}