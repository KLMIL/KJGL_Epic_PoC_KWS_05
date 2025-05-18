/**********************************************************
 * Script Name: BlockCodingUI
 * Author: 김우성
 * Date Created: 2025-05-18
 * Last Modified: 2025-05-18
 * Description: 
 * - 드래그 앤 드롭으로 블록을 설정하고 BattleManager에 정보 전달
 *********************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class BlockCodingUI : MonoBehaviour
{
    [SerializeField] BattleManager _battleManager;
    [SerializeField] Transform _actionSlotPanel;

    public void AddBlock(string blockType, string conditon = "")
    {
        ActionBlock block = new ActionBlock { type = blockType, condition = conditon };
        _battleManager.AddActionBlock(block);
        Debug.Log($"블록 추가: {blockType}, 조건: {conditon}");
    }

    public void OnBlockButtonClicked(string blockType)
    {
        string condition = (blockType == "Attack") ? "enemyDistance <= 2" : "";
        AddBlock(blockType, condition);
    }
}