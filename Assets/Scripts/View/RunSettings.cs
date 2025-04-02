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
        [FormerlySerializedAs("_newLayoutOnInit")]
        [Header("Random")]
        [SerializeField,Tooltip("Decide either we use the seed or we start with a new random labyrinth on every new call of maze creation => new episode")] private bool _useFixedLabyrinth;

        [SerializeField,Tooltip("The fixed see used to generate the same labyrinth if it's not randomized on each run")] private int _fixedSeed;

        [SerializeField, UnityEngine.Range(1, 180)]
        private int _visionRangeDegree = 5;

        [SerializeField, UnityEngine.Range(1, 300)]
        private int _visionResolution = 10;

        [SerializeField, UnityEngine.Range(1, 10f)]
        private float _visionRange = 10;

        [SerializeField] private float _agentRadius = .5f;

        [SerializeField, Tooltip("Scales with the maze global XZ size")]
        private float _goalRadiusBase = .5f;

        [SerializeField] private bool _goalIsOnMiddle;

        [FormerlySerializedAs("_goalPosition")] [SerializeField, Tooltip("Only if not in middle")]
        private Vector2Int _goalOverride;

        [SerializeField, UnityEngine.Range(0.1f, 3f)]
        private float _xzGlobalScale;

        [SerializeField] private Vector3 _wallScale;
        public float MasSize => Mathf.Max(_size.x*_xzGlobalScale, _size.y*_xzGlobalScale);

        public Runner CreateRunner(IBrain brain)
        {
            Vector2Int offset = Vector2Int.zero;//Not used
            if (_useFixedLabyrinth)
                Random.InitState(_fixedSeed);
            var goal = offset + (_goalIsOnMiddle ? _size / 2 : _goalOverride);
            var generator = new MazeGenerator(_size, goal);
            var walls = generator.Generate().Select(w =>
            {
                var pos = w.Item1;
                pos += new Vector3(offset.x, 0, offset.y);
                return new Wall(new Vector2(pos.x * _xzGlobalScale, pos.z * _xzGlobalScale),
                    new Vector3(_wallScale.x * _xzGlobalScale, pos.y * _wallScale.y, _wallScale.z), w.Item2);
            });
            var goalBounds =
                new Bounds(new Vector3(goal.x * _xzGlobalScale, _wallScale.y * .5f, goal.y * _xzGlobalScale),
                    new Vector3(_goalRadiusBase * _xzGlobalScale, _wallScale.y, _goalRadiusBase * _xzGlobalScale));
            var maze = new Maze(walls, goalBounds, new Bounds(goalBounds.center, new Vector3(_size.x * _xzGlobalScale, _wallScale.y*10, _size.y * _xzGlobalScale)));
            var ag = new MazeAgent(offset, _agentRadius, 5f, 0, 300,
                new Vision(_visionRangeDegree, _visionResolution, _visionRange * _xzGlobalScale));
            var run = new Runner(maze, brain, ag);
            run.Init();
            return run;
        }
    }
}