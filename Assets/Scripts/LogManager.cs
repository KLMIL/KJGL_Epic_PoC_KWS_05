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
    [SerializeField] RectTransform _logTextRect;
    [SerializeField] RectTransform _textMaskRect;
    [SerializeField] Scrollbar _scrollbar;

    float _contentHeight;
    float _viewHeight;

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

    private void Start()
    {
        _scrollbar.onValueChanged.AddListener(OnScrollValueChanged);
        UpdateTextHeight();
    }

    public void AddLog(string message)
    {
        _logText.text += $"Turn {_turnCount}: {message}\n";
        Canvas.ForceUpdateCanvases();
        UpdateTextHeight();
        _scrollbar.value = 0f; // 맨 아래로
    }

    private void UpdateTextHeight()
    {
        _logText.ForceMeshUpdate();
        _contentHeight = _logText.preferredHeight;
        _viewHeight = _textMaskRect.rect.height;

        // LogText 높이 조정
        _logTextRect.sizeDelta = new Vector2(_logTextRect.sizeDelta.x, _contentHeight);

        // Scrollbar 값 설정
        float scrollable = Mathf.Max(0, _contentHeight - _viewHeight);
        _scrollbar.size = _viewHeight / _contentHeight;
        _scrollbar.interactable = scrollable > 0;
    }

    private void OnScrollValueChanged(float value)
    {
        float scrollRange = _contentHeight - _viewHeight;
        float y = value * scrollRange;
        _logTextRect.anchoredPosition = new Vector2(0, y);
    }

    public void IncrementTurn() => _turnCount++;
    public int GetTurnCount() => _turnCount;

    public void ClearLog()
    {
        _turnCount = 0;
        if (_logText != null) _logText.text = "";
    }
}
