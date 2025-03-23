using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
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
        [SerializeField] private RunSettings _view;
        [SerializeField] private BehaviorParameters _parameters;
        private CachedBrain _brain;
        [SerializeField] private Runner _main;

        [SerializeField,Range(0.5f,100f)] private float _maxTime;
        [Header("Readonly")]
        [SerializeField] private float currentTime;
        [SerializeField] private float lastStartTime;
        [SerializeField] private float lastStepTime;

        public Runner Runner => _main;

        public override void Initialize()
        {
            InitBrain();
            currentTime = 0f;
            lastStartTime = 0f;
            lastStepTime = 0f;
            _parameters.BrainParameters.VectorObservationSize = 2 * _main.MazeAgent.Vision.Resolution;
            _parameters.BrainParameters.NumStackedVectorObservations = 1; // _main.MazeAgent.Vision.Resolution;
            _parameters.BrainParameters.ActionSpec = new ActionSpec(2, new[] { 1 });
            base.Initialize();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            base.Heuristic(in actionsOut);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            currentTime = UnityEngine.Time.time;
            base.OnActionReceived(actions);
            _brain.rotation = actions.ContinuousActions[0];
            _brain.impulsion = actions.ContinuousActions[1];
            _brain.jump = actions.DiscreteActions[0] == 1;
            _main.Tick(currentTime - lastStepTime);
            lastStepTime = currentTime;
            if (_main.GoalReached)
            {
                SetReward(100.0f);
                EndEpisode();
            }
            else if ((currentTime - lastStartTime) > _maxTime)
            {
                SetReward(-100.0f);
                EndEpisode();
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            base.CollectObservations(sensor);
            foreach (var v in _main.MazeAgent.ComputeVision(_main.Maze))
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
        }

        private void InitBrain()
        {
            _brain = new CachedBrain();
            _main = _view.CreateRunner(_brain);
        }
    }
}