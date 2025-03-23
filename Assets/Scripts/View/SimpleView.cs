using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Simulation;
using Simulation.Generation;
using Simulation.ML;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Time = UnityEngine.Time;

namespace View.Simple
{
    public class SimpleView : MonoBehaviour
    {
        [SerializeField] private bool _useKB;
        [SerializeField] private bool _tickOnUpdate;
        [SerializeField] private TrainedMazeAgent _trainer;
        [Header("Prefabs")] [SerializeField] private GameObject _agentPrefab;

        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject _goalPrefab;
        [SerializeField] private GameObject _brain;
        private List<GameObject> _walls;
        private GameObject _goal;
        private GameObject agent;
        private Runner _runner;

        private class RandomBrain : IBrain
        {
            public float GetRotation(MazeAgent a, Maze maze)
            {
                return UnityEngine.Random.Range(-1f, 1f);
            }

            public float GetImpulsion(MazeAgent a, Maze maze)
            {
                return UnityEngine.Random.Range(-1f, 1f);
            }

            public bool GetJump(MazeAgent a, Maze maze)
            {
                return UnityEngine.Random.value > .9f;
            }
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            yield return new WaitWhile(() => _trainer.Runner == null);
            _walls = new List<GameObject>();
            while (true)
            {
                _runner = _trainer.Runner;
                if (_useKB)
                    _runner.SetBrain(new RandomBrain());
                int i = 0;
                foreach (var wall in _runner.Maze.Walls)
                {
                    var wallBounds = wall.Collider.Bounds;
                    GameObject go;
                    if (i < _walls.Count)
                        go = _walls[i];
                    else
                    {
                        go = Instantiate(_wallPrefab);
                        _walls.Add(go);
                    }

                    go.transform.localPosition = wallBounds.center;
                    go.transform.localScale = wallBounds.size;
                    i++;
                }

                for (; i < _walls.Count; i++)
                {
                    Destroy(_walls[i]);
                    _walls.RemoveAt(i);
                }
                _goal ??= Instantiate(_goalPrefab, _runner.Maze.Goal.Bounds.center, Quaternion.identity);
                _goal.transform.localScale = _runner.Maze.Goal.Bounds.size;
                agent = Instantiate(_agentPrefab, transform);
                SnapAgent();
                //Refresh view on change
                yield return new WaitUntil(() => _trainer.Runner != _runner);
            }
        }


        private void SnapAgent()
        {
            var agentBounds = _runner.MazeAgent.Collider.Bounds;
            agent.transform.localScale = agentBounds.size;
            agent.transform.localPosition = agentBounds.center;
            agent.transform.eulerAngles = new(0, _runner.MazeAgent.Orientation, 0);
        }

        // Update is called once per frame
        void Update()
        {
            if (_runner == null || agent == null)
                return;
            if (_tickOnUpdate)
                _runner.Tick(UnityEngine.Time.deltaTime);
            SnapAgent();
            //Debug.Log($"Vision : {string.Join(";", _runner.MazeAgent.ComputeVision(_runner.Maze))}");
            if (_runner.GoalReached)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}