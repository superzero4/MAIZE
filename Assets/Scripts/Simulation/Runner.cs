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
        private Agent _agent;
        private Maze _maze;
        private IBrain _brain;
        private float time = 0f;

        public Runner(Maze maze, IBrain brain, Agent agent)
        {
            _brain = brain;
            _agent = agent;
            _maze = maze;
        }

        public Maze Maze => _maze;

        public Agent Agent => _agent;

        public void Init()
        {
            time = 0f;
        }

        public void Tick(float deltaTime)
        {
            Time.deltaTime = deltaTime;
            Time.time += deltaTime;
            var dir = _brain.GetRotation(_agent, _maze);
            _agent.Rotate(dir * deltaTime);
            var imp = _brain.GetImpulsion(_agent, _maze);
            _agent.Move(imp * deltaTime);
            if (_maze.Depenetrate(_agent.Collider, out Vector3 force))
                _agent.MoveVector(force);
            var jump = _brain.GetJump(_agent, _maze);
            if (jump)
                _agent.Jump();
            _agent.Tick();
        }
    }
}