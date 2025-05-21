/**********************************************************
 * Script Name: LogManager
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - 각 턴당 행동을 기록하는 로그 기록
 *********************************************************/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance { get; private set; }
    [SerializeField] TextMeshProUGUI _logText;
    string _logContent = "";
    int _turnCount = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            Debug.LogWarning("Singleton made again on LogManager");
        }
        Instance = this;
    }

    public void AddLog(string message)
    {
        _logContent += $"Turn {_turnCount}: {message}\n";
        if (_logText != null)
        {
            _logText.text = _logContent;
            Canvas.ForceUpdateCanvases();
            var scrollRect = _logText.GetComponentInParent<ScrollRect>();
            if (scrollRect) scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    public void IncrementTurn() => _turnCount++;
    public int GetTurnCount() => _turnCount;

    public void ClearLog()
    {
        _turnCount = 0;
        _logContent = "";
        if (_logText != null) _logText.text = "";
    }
}
