using System;
using UnityEngine;
using View.Simple;

public class MultiSimulation : MonoBehaviour
{
    [SerializeField] private RunSettings _runnerToCopy;
    [SerializeField] private Vector2Int _amount;
    [SerializeField] private Vector2Int _amountToView;

    private void Awake()
    {
        for (int i = 0; i < _amount.x; i++)
        {
            for (int j = 0; j < _amount.y; j++)
            {
                var runner = i == 0 && j == 0 ? _runnerToCopy : Instantiate(_runnerToCopy, transform);
                runner.transform.position =
                    new Vector3(i * _runnerToCopy.MasSize * 1.1f, 0, j * _runnerToCopy.MasSize * 1.1f);
                runner.gameObject.name = $"Runner {i} {j}";
                if(runner.TryGetComponent<SimpleView>(out SimpleView view))
                    view.enabled = i < _amountToView.x && j < _amountToView.y;
            }
        }
    }
}