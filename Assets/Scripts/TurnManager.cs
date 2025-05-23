using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class TurnManager : MonoBehaviour
{
    [SerializeField] MapManager _mapManager;
    [SerializeField] CodeBlockManager _codeBlockManager;
    [SerializeField] Button _startButton;
    [SerializeField] Button _resetButton;
    bool _isExecuting;
    UnitController _playerUnitController;

    private void Start()
    {
        _mapManager.ResetMap();
        SetupUnits();
        _startButton.onClick.AddListener(ToggleExecution);
        _resetButton.onClick.AddListener(ResetGame);
    }

    private void SetupUnits()
    {
        _playerUnitController = _mapManager.Players.FirstOrDefault();
        if (_playerUnitController == null || _mapManager.Enemies.Count != 3)
            Debug.LogError("Failed to setup units! Expected 1 player and 3 enemies.");
    }

    public void ToggleExecution()
    {
        if (_isExecuting)
        {
            StopAllCoroutines();
            _isExecuting = false;
            _startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        }
        else
        {
            StartCoroutine(ExecuteTurns());
        }
    }

    private IEnumerator ExecuteTurns()
    {
        _isExecuting = true;
        _startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
        float currentTime = 0f, maxTime = 10f;
        while (currentTime < maxTime)
        {
            if (_playerUnitController != null && _playerUnitController.IsAlive)
            {
                var sortedPairs = _codeBlockManager.GetConditionActions().OrderBy(p => p.Priority).ToList();
                foreach (var pair in sortedPairs)
                {
                    if (_playerUnitController.EvaluateCondition(pair.Condition, _mapManager.Enemies))
                    {
                        _playerUnitController.ExecuteAction(pair.Action, _mapManager.Enemies);
                        break;
                    }
                }
            }
            foreach (var enemy in _mapManager.Enemies)
            {
                if (enemy != null && enemy.IsAlive)
                    enemy.PerformEnemyAction(_mapManager.Players);
            }

            currentTime += Time.deltaTime;
            yield return null;
        }
        _isExecuting = false;
        _startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
    }

    public void ResetGame()
    {
        StopAllCoroutines();
        _isExecuting = false;
        _startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        _mapManager.ResetMap();
        SetupUnits();
        _codeBlockManager.ClearCodeBlocks();
    }
}