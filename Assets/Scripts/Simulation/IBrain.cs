using UnityEngine;
namespace Simulation
{
    public interface IBrain
    {
        public float GetRotation(MazeAgent a, Maze maze);
        public float GetImpulsion(MazeAgent a, Maze maze);
        public bool GetJump(MazeAgent a, Maze maze);
    }
}