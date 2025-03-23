using System.Linq;
using NUnit.Framework;
using Simulation;
using Simulation.Generation;
using UnityEngine;
using UnityEngine.Serialization;

namespace View.Simple
{
    public class RunSettings : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private Vector2Int _size;

        [SerializeField] private float _objetRadius = .5f;
        [SerializeField] private bool _goalIsOnMiddle;
        [FormerlySerializedAs("_goalPosition")] [SerializeField, Tooltip("Only if not in middle")]
        private Vector2Int _goalOverride;

        [SerializeField, UnityEngine.Range(0.1f, 3f)]
        private float _xzGlobalScale;

        [SerializeField] private Vector3 _wallScale;
        
        
        public Runner CreateRunner(IBrain brain)
        {
            var goal = _goalIsOnMiddle ? _size / 2 : _goalOverride;
            float cellRadius = _objetRadius;
            var generator = new MazeGenerator(_size, goal);
            var walls = generator.Generate().Select(w =>
            {
                return new Wall(new Vector2(w.Item1.x * _xzGlobalScale, w.Item1.z * _xzGlobalScale),
                    new Vector3(_wallScale.x * _xzGlobalScale, w.Item1.y * _wallScale.y, _wallScale.z), w.Item2);
            });
            var maze = new Maze(walls, new Vector2(goal.x, goal.y) * _xzGlobalScale, cellRadius * _xzGlobalScale);
            var ag = new MazeAgent(new Vector2(0, 0), cellRadius, 5f, 0, 300, new Vision(5, 30, 10 * _xzGlobalScale));
            var run = new Runner(maze, brain, ag);
            run.Init();
            return run;
        }
    }
}