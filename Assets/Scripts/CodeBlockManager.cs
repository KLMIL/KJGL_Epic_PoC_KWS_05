/**********************************************************
 * Script Name: CodeManager
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - 플레이어가 선택한 코드 블럭을 저장하고, 정보 전달
 *********************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeBlockManager : MonoBehaviour
{
    [SerializeField] Button _startButton;
    [SerializeField] Button _resetButton;
    [SerializeField] Button _moveUpButton;
    [SerializeField] Button _moveLeftButton;
    [SerializeField] Button _moveRightButton;
    [SerializeField] Button _moveDownButton;
    [SerializeField] GameObject _codeArea;
    [SerializeField] List<GameObject> _slots = new List<GameObject>();
    [SerializeField] GameObject _blockCodePrefab;
    List<string> _codeBlocks = new List<string>();

    public List<string> GetCodeBlocks() => _codeBlocks;
    


    private void Start()
    {
        /* 각 버튼 Add Listener */
        _startButton.onClick.AddListener(() => TurnManager.Instance.StartTurnExecution());
        _resetButton.onClick.AddListener(() => TurnManager.Instance.ResetGame());

        _moveUpButton.onClick.AddListener(() => AddMoveBlock("up"));
        _moveLeftButton.onClick.AddListener(() => AddMoveBlock("left"));
        _moveDownButton.onClick.AddListener(() => AddMoveBlock("down"));
        _moveRightButton.onClick.AddListener(() => AddMoveBlock("right"));
    }

    private void AddMoveBlock(string direction)
    {
        Debug.Log("AddMoveBLock called");
        _codeBlocks.Add($"Move {direction}");
        UpdateCodeDisplay();
    }

    private void UpdateCodeDisplay()
    {
        foreach (Transform child in _codeArea.transform)
        {
            foreach (Transform grandChild in child)
            {
                Destroy(grandChild.gameObject);
            }
            
        }

        for (int i = 0; i < _codeBlocks.Count; i++) 
        { 
            string block = _codeBlocks[i];
            GameObject commandObj = Instantiate(_blockCodePrefab, _slots[i].transform);
            Image commandImg = commandObj.GetComponent<Image>();
            TextMeshProUGUI commandTxt = commandImg.GetComponentInChildren<TextMeshProUGUI>();


            if (commandImg != null)
            {
                switch (block.ToLower())
                {
                    case "move up":
                        commandTxt.text = "Move\nUp";
                        break;
                    case "move down":
                        commandTxt.text = "Move\nDown";
                        break;
                    case "move left":
                        commandTxt.text = "Move\nLeft";
                        break;
                    case "move right":
                        commandTxt.text = "Move\nRight";
                        break;
                }
            }
        }
    }

    public void ClearCodeBlocks()
    {
        _codeBlocks.Clear();
        foreach (Transform child in _codeArea.transform)
        {
            foreach (Transform grandChild in child)
            {
                Destroy(grandChild.gameObject);
            }

        }
    }
}
