using System.Linq;
using NUnit.Framework;
using Simulation;
using Simulation.Generation;
using UnityEngine;
using UnityEngine.Serialization;
using Time = Simulation.Time;

namespace View.Simple
{
    public class RunSettings : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private Vector2Int _size;
        [SerializeField] private bool _newLayoutOnInit;

        [SerializeField]
        private float _agentRadius = .5f;

        [SerializeField, Tooltip("Scales with the maze global XZ size")]
        private float _goalRadiusBase = .5f;

        [SerializeField] private bool _goalIsOnMiddle;

        [FormerlySerializedAs("_goalPosition")] [SerializeField, Tooltip("Only if not in middle")]
        private Vector2Int _goalOverride;

        [SerializeField, UnityEngine.Range(0.1f, 3f)]
        private float _xzGlobalScale;

        [SerializeField] private Vector3 _wallScale;


        public Runner CreateRunner(IBrain brain)
        {
            if (!_newLayoutOnInit)
                Random.InitState(42);
            var goal = _goalIsOnMiddle ? _size / 2 : _goalOverride;
            var generator = new MazeGenerator(_size, goal);
            var walls = generator.Generate().Select(w =>
            {
                return new Wall(new Vector2(w.Item1.x * _xzGlobalScale, w.Item1.z * _xzGlobalScale),
                    new Vector3(_wallScale.x * _xzGlobalScale, w.Item1.y * _wallScale.y, _wallScale.z), w.Item2);
            });
            var goalBounds = new Bounds(new Vector3(goal.x * _xzGlobalScale, _wallScale.y * .5f, goal.y * _xzGlobalScale), new Vector3(_goalRadiusBase * _xzGlobalScale, _wallScale.y, _goalRadiusBase * _xzGlobalScale));
            var maze = new Maze(walls, goalBounds);
            var ag = new MazeAgent(new Vector2(0, 0), _agentRadius, 5f, 0, 300, new Vision(5, 30, 10 * _xzGlobalScale));
            var run = new Runner(maze, brain, ag);
            run.Init();
            return run;
        }
    }
}