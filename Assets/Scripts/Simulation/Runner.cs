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

        //Vision ray is shooted from the center therefore it will remain strictly positive before this becomes true (is a large estimation but way enough for simulation
        public bool GoalReached => DistToGoal < _maze.Goal.Bounds.size.x;
        public double DistToGoal => Mathf.Sqrt(GoalChecker.SqrDistance(_maze.Goal.Bounds.center));
        public Bounds GoalChecker => _mazeAgent.Collider.Bounds;
        public Runner(Maze maze, IBrain brain, MazeAgent mazeAgent)
        {
            _brain = brain;
            _mazeAgent = mazeAgent;
            _maze = maze;
        }

        public void SetBrain(IBrain brain) => _brain = brain;

        public Maze Maze => _maze;

        public MazeAgent MazeAgent => _mazeAgent;
        public void Init()
        {
            time = 0f;
        }

        public void Tick(float deltaTime)
        {
            Time.deltaTime = deltaTime;
            Time.time += deltaTime;
            var dir = _brain.GetRotation(_mazeAgent, _maze);
            _mazeAgent.Rotate(dir * deltaTime);
            var imp = _brain.GetImpulsion(_mazeAgent, _maze);
            _mazeAgent.Move(imp * deltaTime);
            if (_maze.Depenetrate(_mazeAgent.Collider, out Vector3 force))
                _mazeAgent.MoveVector(force);
            var jump = _brain.GetJump(_mazeAgent, _maze);
            if (jump)
                _mazeAgent.Jump();
            _mazeAgent.Tick();
        }
    }
}