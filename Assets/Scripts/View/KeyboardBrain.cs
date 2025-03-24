using Simulation;
using UnityEngine;

namespace View.Simple
{
    public class KeyboardBrain : MonoBehaviour, IBrain
    {
        public float GetRotation(MazeAgent a, Maze maze)
        {
            return Input.GetAxis("Horizontal");
        }

        public float GetImpulsion(MazeAgent a, Maze maze)
        {
            return Input.GetAxis("Vertical");
        }

        public bool GetJump(MazeAgent a, Maze maze)
        {
            return Input.GetKey(KeyCode.Space);
        }
    }
}