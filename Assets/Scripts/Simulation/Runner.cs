using System.Linq;
using UnityEngine;

namespace Simulation
{
    public class Runner
    {
        private Agent _agent;
        private Maze _maze;
        private IBrain _brain;

        public Runner(IBrain brain, Vision vision, float x = 0, float y = 0, float orientation = 0)
        {
            _brain = brain;
            _agent = new Agent(new Vector2(x, y), .5f, 5f, orientation, 300, vision);
            _maze = new Maze(
                Enumerable.Range(0, 10)
                    .Select(i => new Wall(new Vector2(i, i), new Vector3(1.5f, .5f, .1f), i % 2 == 0))
            );
        }

        public Maze Maze => _maze;

        public Agent Agent => _agent;

        public void Init()
        {
        }

        public void Tick(float deltaTime)
        {
            var dir = _brain.GetRotation(_agent, _maze);
            _agent.Rotate(dir * deltaTime);
            var imp = _brain.GetImpulsion(_agent, _maze);
            _agent.Move(imp * deltaTime);
        }
    }
}