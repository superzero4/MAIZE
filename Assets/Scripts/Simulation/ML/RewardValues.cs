using System;
using UnityEngine;

namespace Simulation.ML
{
    [Serializable]
    public struct RewardValues
    {
        [Header("Global")] [SerializeField, Range(0.001f, 1000f)]
        private float _scale;

        [Header("Positives")] [SerializeField, Range(0.001f, 10000f)]
        private float _goalReached;

        [SerializeField, Range(0f, 100f)] private float _goalSeen;

        [Header("Negatives")] [SerializeField, Range(-100f, 0f)]
        private float _timeOut;

        [SerializeField, Range(-100f, 0f)] private float _wallHit;

        [Header("Relatives")]
        [SerializeField, Range(0f, 1f),
         Tooltip(
             "When there is a timeout, we calculate how close the agent is and then reduce the negative reward proportinally to this value and the relative distance to goal considering the start position as max Distance,\n => A value of 1 would make a -~0.0001 reward if we are 1 pixel away from the objective\n => A value of 0.5 would make half up to half the full negativ reward the close we get\n => A value of 0 would make the same negative reward for any timeout, without taking the distance in consideration")]
        private float _distanceToGoalBonusProportion;

        public float GoalReached => _goalReached * _scale;

        public float GoalSeen => _goalSeen * _scale;

        public float WallHit => _wallHit;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="howFar">0 means we reached the objective, 1 or more means we are at same distance as on start of more far</param>
        /// <returns></returns>
        public float TimeOut(float howFar) =>
            (_timeOut - _timeOut * _distanceToGoalBonusProportion * (1 - howFar)) * _scale;
    }
}