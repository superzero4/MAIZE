using System;
using UnityEngine;
using UnityEngine.Serialization;

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
        private float _wallHit;

        [FormerlySerializedAs("_maxTime")] [Header("Time")] [SerializeField, Range(1f, 1000f)]
        private float _referenceTime;

        [SerializeField, Range(0f, 500),
         Tooltip(
             "Relative to reference time, will be relatively punished if above max time and relatively rewarded if below, 0 means it reached the reference time, it will get half this value if it performs in half the time")]
        private float _timeReward;

        public float FinishedReward(float elapsed)
        {
            return (_goalReached + _timeReward * (1 - elapsed / _referenceTime)) * _scale;
        }

        [Header("Distance")] [SerializeField] private bool _timeOutAfterRefTime;

        [SerializeField, Range(-1000f, 0),
         Tooltip(
             "Only used if timeout enabled, max reached when relative distance is maximized from to the goal from position when timeout")]
        private float _maxTimeoutMalus;

        public float TimeOut(float howFar) => (_maxTimeoutMalus * howFar * _scale);


        public float GoalSeen => _goalSeen * _scale;

        public float WallHit => _wallHit * _scale;

        public float GoalReached => _goalReached * _scale;

        public bool TimeOutAfterRefTime => _timeOutAfterRefTime;

        public float ReferenceTime => _referenceTime;
    }
}