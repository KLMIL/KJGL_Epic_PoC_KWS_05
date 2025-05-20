/**********************************************************
 * Script Name: TurnManager
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - 각 턴에 따른 흐름 유도
 *********************************************************/

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] MapManager _mapManager;
    [SerializeField] CodeBlockManager _codeBlockManager;

    UnitController _playerUnitController;
    UnitController _enemyUnitController;

    bool _isExecuting = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Debug.LogWarning("Singleton made again: TurnManager");
        }
        Instance = this;    
    }

    private void Start()
    {
        _mapManager = FindFirstObjectByType<MapManager>();
        _codeBlockManager = FindFirstObjectByType<CodeBlockManager>();

        _mapManager.ResetMap();
        SetupUnits();
    }

    private void SetupUnits()
    {
        _playerUnitController = _mapManager.PlayerInstance.GetComponent<UnitController>();
        _playerUnitController.Initialize(_mapManager, new Vector2Int(0, 0), (int)UnitType.Player);

        _enemyUnitController = _mapManager.EnemyInstance.GetComponent<UnitController>();
        _enemyUnitController.Initialize(_mapManager, new Vector2Int(1, 1), (int)UnitType.Enemy);
    }

    public void StartTurnExecution()
    {
        if (!_isExecuting)
        {
            StartCoroutine(ExecuteTurns());
        }
    }

    private IEnumerator ExecuteTurns()
    {
        _isExecuting = true;
        LogManager.Instance.ClearLog();
        var codeBlocks = _codeBlockManager.GetCodeBlocks();

        for (int i = 0; i < codeBlocks.Count; i++)
        {
            // 플레이어 턴
            LogManager.Instance.IncrementTurn();
            ExecuteTurn(codeBlocks[i]);
            yield return new WaitForSeconds(1f);

            // 적 턴
            if (_enemyUnitController != null && _enemyUnitController.IsAlive())
            {
                LogManager.Instance.AddLog("Enemy's turn");
                _enemyUnitController.PerformEnemyAction(_playerUnitController);
                yield return new WaitForSeconds(1f);

                if (!_playerUnitController.IsAlive())
                {
                    LogManager.Instance.AddLog("Player defeated! Game Over.");
                    _isExecuting = false;
                    yield break;
                }
            }
        }
        LogManager.Instance.AddLog("Turn execution completed.\n");
        _isExecuting = false;
    }

    private void ExecuteTurn(string command)
    {
        Debug.Log($"Executing turn with command: {command}");
        if (command.StartsWith("Move"))
        {
            string direction = command.Split(' ')[1];
            _playerUnitController.Move(direction);
        }
        else if (command.ToLower() == "attack")
        {
            if (_enemyUnitController != null && _enemyUnitController.IsAlive())
            {
                _playerUnitController.Attack(_enemyUnitController);
                if (!_enemyUnitController.IsAlive())
                {
                    _mapManager.RemoveUnit(_enemyUnitController.Position);
                    _enemyUnitController = null;
                }
            }
            else
            {
                LogManager.Instance.AddLog("No enemy to attack!");
            }
        }
    }

    public void ResetGame()
    {
        LogManager.Instance.ClearLog();
        _codeBlockManager.ClearCodeBlocks();
        _mapManager.ResetMap();
        SetupUnits();
    }
}
