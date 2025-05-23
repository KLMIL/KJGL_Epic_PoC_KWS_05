/**********************************************************
 * Script Name: CodeManager
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - 플레이어가 선택한 코드 블럭을 저장하고, 정보 전달
 *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ConditionType { MonsterNear, NoMonsterNear, HPLower30 }
public enum ActionType { AttackMonster, MoveToMonster, UsePotion }


public class CodeBlockManager : MonoBehaviour
{
    public struct ConditionActionPair
    {
        public ConditionType Condition;
        public ActionType Action;
        public int Priority;
    }

    [SerializeField] List<ConditionActionPair> _conditionActions = new List<ConditionActionPair>();
    [SerializeField] TMP_Dropdown _conditionDropdown;
    [SerializeField] TMP_Dropdown _actionDropdown;
    [SerializeField] TMP_Dropdown _priorityDropdown;
    [SerializeField] Button _addPairButton;

    [SerializeField] Button _startButton;
    [SerializeField] Button _resetButton;

    [SerializeField] Button _moveUpButton;
    [SerializeField] Button _moveLeftButton;
    [SerializeField] Button _moveRightButton;
    [SerializeField] Button _moveDownButton;

    [SerializeField] Button _attackButton;

    [SerializeField] GameObject _codeArea;
    [SerializeField] List<GameObject> _slots = new List<GameObject>();
    [SerializeField] GameObject _blockCodePrefab;
    List<string> _codeBlocks = new List<string>();

    int _maxSlots = 10;

    public List<string> GetCodeBlocks() => _codeBlocks;
    public List<ConditionActionPair> GetConditionActions() => _conditionActions;


    private void Start()
    {
        /* 각 버튼 Add Listener */
        _startButton.onClick.AddListener(() => TurnManager.Instance.ToggleExecution());
        _resetButton.onClick.AddListener(() => TurnManager.Instance.ResetGame());

        _moveUpButton.onClick.AddListener(() => AddMoveBlock("up"));
        _moveLeftButton.onClick.AddListener(() => AddMoveBlock("left"));
        _moveDownButton.onClick.AddListener(() => AddMoveBlock("down"));
        _moveRightButton.onClick.AddListener(() => AddMoveBlock("right"));

        _attackButton.onClick.AddListener(() => AddAttackBlock());

        _addPairButton.onClick.AddListener(AddConditionAction);
        _conditionDropdown.options = Enum.GetNames(typeof(ConditionType)).Select(name => new TMP_Dropdown.OptionData(name)).ToList();
        _actionDropdown.options = Enum.GetNames(typeof(ActionType)).Select(name => new TMP_Dropdown.OptionData(name)).ToList();
        _priorityDropdown.options = Enumerable.Range(1, 11).Select(i => new TMP_Dropdown.OptionData(i.ToString())).ToList();
    }

    private void AddConditionAction()
    {
        if (_conditionActions.Count < _maxSlots)
        {
            ConditionActionPair pair = new ConditionActionPair
            {
                Condition = (ConditionType)_conditionDropdown.value,
                Action = (ActionType)_actionDropdown.value,
                Priority = _priorityDropdown.value + 1
            };
            _conditionActions.Add(pair);
            UpdateCodeDisplay();
        }
    }

    private void AddMoveBlock(string direction)
    {
        if (_codeBlocks.Count < _maxSlots)
        {
            Debug.Log("AddMoveBLock called");
            _codeBlocks.Add($"Move {direction}");
            UpdateCodeDisplay();
        }
        else
        {
            Debug.LogWarning("Maximum number of command blocks reached!");
        }
    }

    private void AddAttackBlock()
    {
        if (_codeBlocks.Count < _maxSlots)
        {
            _codeBlocks.Add("Attack");
            UpdateCodeDisplay();
        }
        else
        {
            Debug.LogWarning("Maximum number of command blocks reached!");
        }
    }

    private void UpdateCodeDisplay()
    {
        // Vertical Layout 문제로 인해(Scale을 줄이는 방식으로 크기 조정한게 문제가 됐음)
        // 임시로, 객체를 만들어 두고 해당 객체의 자식으로 생성 후 삭제하는 방식으로
        // 블럭코딩 구현함. 추후 UI 재정립 작업에서 주의할 것.
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
                    case "attack":
                        commandTxt.text = "Attack";
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
