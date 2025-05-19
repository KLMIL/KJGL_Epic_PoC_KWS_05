/**********************************************************
 * Script Name: TurnManager
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - 각 턴에 따른 흐름 유도
 *********************************************************/

using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] MapManager _mapManager;
    [SerializeField] CodeBlockManager _codeBlockManager;

    UnitController _playerUnitController;

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
            LogManager.Instance.IncrementTurn();
            ExecuteTurn(codeBlocks[i]);
            yield return new WaitForSeconds(1f);
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
    }

    public void ResetGame()
    {
        LogManager.Instance.ClearLog();
        _codeBlockManager.ClearCodeBlocks();
        _mapManager.ResetMap();
        SetupUnits();
    }
}
