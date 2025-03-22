using UnityEngine;
namespace Simulation
{
    public interface IBrain
    {
        public float GetRotation(Agent a, Maze maze);
        public float GetImpulsion(Agent a, Maze maze);
        public bool GetJump(Agent a, Maze maze);
    }
}