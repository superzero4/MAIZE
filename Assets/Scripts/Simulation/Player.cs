using UnityEngine;
namespace Simulation
{
    public interface IBrain
    {
        public float GetRotation(Agent a, Maze maze);
        public float IBrain(Agent a, Maze maze);
    }
}