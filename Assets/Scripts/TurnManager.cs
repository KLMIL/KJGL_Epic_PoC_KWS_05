/**********************************************************
 * Script Name: TurnManager
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - 각 턴에 따른 흐름 유도
 *********************************************************/

using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] MapManager _mapManager;
    [SerializeField] CodeBlockManager _codeBlockManager;

    UnitController _playerUnitController;
    UnitController _enemyUnitController;

    bool _isExecuting = false;
    bool _isPaused = false;

    [Header("TimeLine")]
    [SerializeField] GameObject _timeLinePanel;
    [SerializeField] List<Button> _timeLineSlots = new List<Button>();
    [SerializeField] int _maxTurn = 10;
    int _pauseAtTurn = -1; // 멈출 턴 인덱스


    // 임시로, 버튼 할당해서 텍스트 변경
    [SerializeField] Button _startButton;



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

        // 타임라인 버튼 이벤트 추가. 10턴만 쓸거니 하드코딩
        for (int i = 0; i < 10; i++)
        {
            int turnIndex = i;
            _timeLineSlots[i].onClick.AddListener(() => OnTimeLineButtonClicked(turnIndex));
        }
    }

    private void SetupUnits()
    {
        _playerUnitController = _mapManager.PlayerInstance.GetComponent<UnitController>();
        _playerUnitController.Initialize(_mapManager, new Vector2Int(0, 0), (int)UnitType.Player);

        _enemyUnitController = _mapManager.EnemyInstance.GetComponent<UnitController>();
        _enemyUnitController.Initialize(_mapManager, new Vector2Int(1, 1), (int)UnitType.Enemy);
    }

    private void OnTimeLineButtonClicked(int turnIndex)
    {
        if (_isExecuting) return; // 실행 중일때는 무시

        // 목표 턴만 빨간 점 표시
        for (int i = 0; i < 10; i++)
        {
            var redDot = _timeLineSlots[i].transform.GetChild(0).gameObject;
            redDot.SetActive(i == turnIndex);
        }

        _pauseAtTurn = turnIndex;
        _isPaused = true;
        LogManager.Instance.AddLog($"Paused at turn {turnIndex + 1}");
        UpdateStartButtonText();
    }

    public void ToggleExecution()
    {
        // 실행중 -> 일시정지
        if (_isExecuting && !_isPaused)
        {
            _isPaused = true;
            LogManager.Instance.AddLog("Execution puased");
        }
        else // 정지중 -> 실행
        {
            _isPaused = false;
            if (!_isExecuting)
            {
                StartCoroutine(ExecuteTurns());
            }
        }
        UpdateStartButtonText();
    }

    private void UpdateStartButtonText()
    {
        TextMeshProUGUI buttonText = _startButton.GetComponentInChildren<TextMeshProUGUI>();
        if (!_isExecuting && !_isPaused)
        {
            buttonText.text = "Stop";
        }
        else
        {
            buttonText.text = "Start";
        }
    }

    private IEnumerator ExecuteTurns()
    {
        _isExecuting = true;
        _isPaused = false;
        LogManager.Instance.ClearLog();
        var codeBlocks = _codeBlockManager.GetCodeBlocks();
        int currentCodeBlock = 0;

        for (int turn = 0; turn < _maxTurn; turn++)
        {
            if (_pauseAtTurn == turn)
            {
                _isPaused = true;
                while (_isPaused)
                {
                    yield return null;
                }
            }

            LogManager.Instance.IncrementTurn();
            int turnIndex = LogManager.Instance.GetTurnCount() - 1;
            UpdateTimeLine(turnIndex);

            // 플레이어 명령 실행
            if (codeBlocks.Count > 0)
            {
                string command = codeBlocks[currentCodeBlock];
                bool shouldEndTurn = ExecuteTurn(command);
                currentCodeBlock = (currentCodeBlock + 1) % codeBlocks.Count;
                if (shouldEndTurn)
                {
                    LogManager.Instance.AddLog("Enemy defeated! Ending turn execution");
                    _isExecuting = false;
                    UpdateStartButtonText();
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.5f);

            // 적 턴
            if (_enemyUnitController != null && _enemyUnitController.IsAlive())
            {
                LogManager.Instance.AddLog("Enemy's turn");
                _enemyUnitController.PerformEnemyAction(_playerUnitController);
                yield return new WaitForSeconds(1f);

                if (!_playerUnitController.IsAlive())
                {
                    LogManager.Instance.AddLog("Player defeated! Game Over");
                    _isExecuting = false;
                    UpdateStartButtonText();
                    yield break;
                }
            }

            yield return new WaitForSeconds(1f);
        }

        LogManager.Instance.AddLog("Turn execution completed");
        _isExecuting = false;
    }

    private bool ExecuteTurn(string command)
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
                    return true;
                }
            }
            else
            {
                LogManager.Instance.AddLog("No enemy to attack!");
            }
            return false;
        }
        return false;
    }

    public void ResetGame()
    {
        LogManager.Instance.ClearLog();
        _codeBlockManager.ClearCodeBlocks();
        _mapManager.ResetMap();
        SetupUnits();
        ResetTimeLine();

        _isExecuting = false;
        _isPaused = false;
        _pauseAtTurn = -1;
        UpdateStartButtonText();
    }

    private void UpdateTimeLine(int turnIndex)
    {
        if (turnIndex >= 0 && turnIndex < _timeLineSlots.Count)
        {
            _timeLineSlots[turnIndex].GetComponent<Image>().color = Color.blue;
        }
    }

    private void ResetTimeLine()
    {
        foreach (var slot in _timeLineSlots)
        {
            slot.GetComponent<Image>().color = Color.black;
            slot.transform.GetChild(0).gameObject.SetActive(false); // 빨간 점 비활성화
        }
    }
}
