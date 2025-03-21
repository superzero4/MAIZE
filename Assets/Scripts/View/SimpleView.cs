using System.Linq;
using Simulation;
using Simulation.Generation;
using UnityEngine;
using UnityEngine.Serialization;
using Time = UnityEngine.Time;

namespace View.Simple
{
    public class SimpleView : MonoBehaviour
    {
        [SerializeField] private Vector2Int _size;
        [FormerlySerializedAs("_scale")] [SerializeField, Range(0.1f, 3f)] private float _xzScale;
        [Header("Prefabs")] [SerializeField] private GameObject _agentPrefab;

        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject _goalPrefab;
        [SerializeField] private GameObject agent;
        [Header("Settings")] [SerializeField] private Runner _runner;

        private class RandomBrain : IBrain
        {
            public float GetRotation(Agent a, Maze maze)
            {
                return UnityEngine.Random.Range(-1f, 1f);
            }

            public float GetImpulsion(Agent a, Maze maze)
            {
                return UnityEngine.Random.Range(-1f, 1f);
            }

            public bool GetJump(Agent a, Maze maze)
            {
                return UnityEngine.Random.value > .9f;
            }
        }

        void Awake()
        {
            var goal = _size / 2;
            float cellRadius = .5f;
            var generator = new MazeGenerator(_size, goal);
            var walls = generator.Generate().Select(w =>
            {
                return new Wall(new Vector2(w.Item1.x * _xzScale, w.Item1.z *_xzScale), new Vector3(1*_xzScale, w.Item1.y * 1.5f, .1f), w.Item2);
            });
            var maze = new Maze(walls, new Vector2(goal.x, goal.y) * _xzScale, cellRadius);
            var ag = new Agent(new Vector2(0, 0), cellRadius, 5f, 0, 300, new Vision(5, 30, 10));
            _runner = new Runner(maze, GetComponent<IBrain>() ?? new RandomBrain(), ag);
            Instantiate(_goalPrefab, maze.Goal.Bounds.center, Quaternion.identity);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _runner.Init();
            foreach (var wall in _runner.Maze.Walls)
            {
                var wallBounds = wall.Collider.Bounds;
                var go = Instantiate(_wallPrefab,
                    new Vector3(wallBounds.center.x, wallBounds.center.y, wallBounds.center.z),
                    Quaternion.identity);
                go.transform.localScale = wallBounds.size;
            }

            agent = Instantiate(_agentPrefab, transform);
            SnapAgent();
        }

        private void SnapAgent()
        {
            var agentBounds = _runner.Agent.Collider.Bounds;
            agent.transform.localScale = agentBounds.size;
            agent.transform.localPosition = agentBounds.center;
            agent.transform.eulerAngles = new(0, _runner.Agent.Orientation, 0);
        }

        // Update is called once per frame
        void Update()
        {
            _runner.Tick(UnityEngine.Time.deltaTime);
            SnapAgent();
            Debug.Log($"Vision : {string.Join(",", _runner.Agent.ComputeVision(_runner.Maze))}");
        }
    }
}