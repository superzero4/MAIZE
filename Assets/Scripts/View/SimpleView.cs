using Simulation;
using UnityEngine;

namespace View.Simple
{
    public class SimpleView : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] private GameObject _agentPrefab;

        [SerializeField] private GameObject _wallPrefab;
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
        }

        void Awake()
        {
            _runner = new Runner(GetComponent<IBrain>() ?? new RandomBrain(), new Vision(5, 30, 10),.5f , -1, 0);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _runner.Init();
            foreach (var wall in _runner.Maze.Walls)
            {
                var wallBounds = wall.Collider.Bounds;
                var go = Instantiate(_wallPrefab, new Vector3(wallBounds.center.x, 0, wallBounds.center.y),
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
            _runner.Tick(Time.deltaTime);
            SnapAgent();
            Debug.Log($"Vision : {string.Join(",", _runner.Agent.ComputeVision(_runner.Maze))}");
        }
    }
}