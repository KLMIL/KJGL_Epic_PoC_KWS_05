using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class CodeBlockManager : MonoBehaviour
{
    public struct ConditionActionPair
    {
        public ConditionType Condition;
        public ActionType Action;
        public int Priority;
    }

    [SerializeField] TMP_Dropdown _conditionDropdown;
    [SerializeField] TMP_Dropdown _actionDropdown;
    [SerializeField] TMP_Dropdown _priorityDropdown;
    [SerializeField] Button _addPairButton;
    [SerializeField] GameObject _codeArea;
    [SerializeField] List<GameObject> _slots = new List<GameObject>();
    [SerializeField] GameObject _blockCodePrefab;
    [SerializeField] List<ConditionActionPair> _conditionActions = new List<ConditionActionPair>();
    int _maxSlots = 5;

    public List<ConditionActionPair> GetConditionActions() => _conditionActions;

    private void Start()
    {
        _addPairButton.onClick.AddListener(AddConditionAction);
        _conditionDropdown.options = System.Enum.GetNames(typeof(ConditionType)).Select(name => new TMP_Dropdown.OptionData(name)).ToList();
        _actionDropdown.options = System.Enum.GetNames(typeof(ActionType)).Select(name => new TMP_Dropdown.OptionData(name)).ToList();
        _priorityDropdown.options = Enumerable.Range(1, 6).Select(i => new TMP_Dropdown.OptionData(i.ToString())).ToList();
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
        else
        {
            Debug.Log("Max condition-action pairs reached!");
        }
    }

    private void UpdateCodeDisplay()
    {
        foreach (Transform child in _codeArea.transform)
            foreach (Transform grandChild in child)
                Destroy(grandChild.gameObject);

        var sortedPairs = _conditionActions.OrderBy(p => p.Priority).ToList();
        for (int i = 0; i < sortedPairs.Count; i++)
        {
            var pair = sortedPairs[i];
            GameObject commandObj = Instantiate(_blockCodePrefab, _slots[i].transform);
            TextMeshProUGUI commandTxt = commandObj.GetComponentInChildren<TextMeshProUGUI>();
            commandTxt.text = $"{pair.Condition} -> {pair.Action} (P:{pair.Priority})";
        }
    }

    public void ClearCodeBlocks()
    {
        _conditionActions.Clear();
        UpdateCodeDisplay();
    }
}