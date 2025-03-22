﻿using Simulation;
using UnityEngine;

namespace View.Simple
{
    public class KeyboardBrain : MonoBehaviour, IBrain
    {
        public float GetRotation(Agent a, Maze maze)
        {
            return Input.GetAxis("Horizontal");
        }

        public float GetImpulsion(Agent a, Maze maze)
        {
            return Input.GetAxis("Vertical");
        }

        public bool GetJump(Agent a, Maze maze)
        {
            return Input.GetKeyDown(KeyCode.Space);
        }
    }
}