using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;
using View.Simple;


namespace Simulation.ML
{
    public class CachedBrain : IBrain
    {
        public float rotation;
        public float impulsion;
        public bool jump;

        public float GetRotation(MazeAgent a, Maze maze)
        {
            return rotation;
        }

        public float GetImpulsion(MazeAgent a, Maze maze)
        {
            return impulsion;
        }

        public bool GetJump(MazeAgent a, Maze maze)
        {
            return jump;
        }
    }

    public class TrainedMazeAgent : Agent
    {
        public Timer timer;
        [SerializeField] private RunSettings _view;
        [SerializeField] private BehaviorParameters _parameters;
        [SerializeField] private KeyboardBrain _keyboardBrain;
        private CachedBrain _brain;
        [SerializeField] private Runner _main;

        [FormerlySerializedAs("_reward")] [SerializeField, Header("Reward")]
        private RewardValues _rewardValues;


        [Header("Readonly")] [Header("Time")] [SerializeField]
        private float currentTime;

        [SerializeField] private float lastStartTime;
        [SerializeField] private float lastStepTime;

        [Header("Agent vs maze info")] [SerializeField]
        private bool _wallHit;

        private bool _wentOut;

        [SerializeField] private bool _reachedObjective;
        [Header("ML Infos")] [SerializeField] private int _lastEpisodeReward;

        public Runner Runner => _main;

        public int LastEpisodeReward => _lastEpisodeReward;

        public override void Initialize()
        {
            InitBrain();
            currentTime = 0f;
            lastStartTime = 0f;
            lastStepTime = 0f;
            _parameters.BrainParameters.VectorObservationSize = /* one bool and one float*/
                2 * _main.MazeAgent.Vision.Resolution;
            _parameters.BrainParameters.NumStackedVectorObservations = 1; // _main.MazeAgent.Vision.Resolution;
            _parameters.BrainParameters.ActionSpec = new ActionSpec(3, new[] { 1 });
            base.Initialize();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = _keyboardBrain.GetRotation(default, default);
            continuousActionsOut[1] = _keyboardBrain.GetImpulsion(default, default);
            continuousActionsOut[2] = _keyboardBrain.GetJump(default, default) ? 1.0f : 0.0f;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            currentTime = UnityEngine.Time.time;
            base.OnActionReceived(actions);
            _brain.rotation = actions.ContinuousActions[0];
            _brain.impulsion = actions.ContinuousActions[1];
            //_brain.jump = actions.DiscreteActions[0] == 1;
            //if (actions.ContinuousActions[2] > .5f)
            //    Debug.Log(actions.ContinuousActions[2]);
            _brain.jump = actions.ContinuousActions[2] > .5f;
            _main.Tick(currentTime - lastStepTime, out _wallHit, out _reachedObjective, out _wentOut);
            lastStepTime = currentTime;
            SetRewards();
        }

        private void SetRewards()
        {
            if (_wallHit)
            {
                AddReward(_rewardValues.WallHit);
            }

            if (_main.MazeAgent.Vision.hasGoalInSight)
            {
                AddReward(_rewardValues.GoalSeen);
            }

            if (_reachedObjective)
            {
                AddReward(_rewardValues.FinishedReward(currentTime - lastStartTime));
                _lastEpisodeReward = (int)GetCumulativeReward();
                EndEpisode();
            }

            if (_wentOut)
            {
                AddReward(-_rewardValues.GoalReached);
                _lastEpisodeReward = (int)GetCumulativeReward();
                EndEpisode();
            }
            if (_rewardValues.TimeOutAfterRefTime)
            {
                if (currentTime - lastStartTime > _rewardValues.ReferenceTime)
                {
                    AddReward(_rewardValues.TimeOut(_main.RelativeAgentDistToGoal));
                    _lastEpisodeReward = (int)GetCumulativeReward();
                    EndEpisode();
                }
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            base.CollectObservations(sensor);
            var array = _main.MazeAgent.ComputeVision(_main.Maze);
            foreach (var v in array)
            {
                //TODO create a new observation vector before putting distance and isGoal
                sensor.AddObservation(v.isGoal);
                sensor.AddObservation(v.distance);
            }
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            InitBrain();
            lastStartTime = UnityEngine.Time.time;
            _main.MazeAgent.Reset();
            timer.reset();
        }

        private void InitBrain()
        {
            _brain = new CachedBrain();
            _main = _view.CreateRunner(_brain);
        }
    }
}