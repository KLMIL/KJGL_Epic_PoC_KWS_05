/**********************************************************
 * Script Name: BattleManager
 * Author: 김우성
 * Date Created: 2025-05-18
 * Last Modified: 2025-05-18
 * Description: 
 * - 전투에 관련된 로직을 관리
 *********************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.InputSystem.DefaultInputActions;

public class BattleManager : MonoBehaviour
{
    [SerializeField] Unit _player;
    [SerializeField] Unit _enemy;
    [SerializeField] List<ActionBlock> _playerActions = new List<ActionBlock>();
    [SerializeField] Text _logText;
    
    int _turn = 0;


    private void Start()
    {
        /* Find 말고 다른거로 해야함 ... 구조 바꾸기 */
        _player = new Unit(100, 0, 0, GameObject.Find("PlayerUnit"));
        _enemy = new Unit(100, 4, 4, GameObject.Find("EnemyUnit"));
        UpdateUnitPositions();
    }

    public void StartBattle()
    {
        _turn = 0;
        _logText.text = "";
        StartCoroutine(RunBattleCoroutine());
    }


    private IEnumerator RunBattleCoroutine()
    {
        /* 나중에는 while 조건 변경 해야할듯 */
        while (_player.hp > 0 && _enemy.hp > 0)
        {
            _turn++;
            _logText.text += $"턴: {_turn}\n";

            // 플레이어 턴
            foreach (var block in _playerActions)
            {
                if (EvaluateCondition(block.condition))
                {
                    ExecuteAction(block.type);
                }
            }

            // 적 턴 (간단한 AI 구현)
            if (GetDistance(_player, _enemy) <= 2)
            {
                _player.hp -= 15;
                _logText.text += $"적 공격! 플레이어 HP: {_player.hp}\n";
            }
            else
            {
                MoveToward(_enemy, _player);
                _logText.text += $"적 이동: ({_enemy.x}, {_enemy.y}\n";
            }

            yield return new WaitForSeconds(_turn); // 턴 딜레이
        }

        _logText.text += (_player.hp > 0 ? "승리!" : "패배..");
    }

    private bool EvaluateCondition(string condition)
    {
        if (string.IsNullOrEmpty(condition)) return true;

        // 조건 평가
        int distance = GetDistance(_player, _enemy);

        if (condition == "enemyDistance <= 2") return distance <= 2;

        return true; // 조건 없으면 항상 실행
    }

    private void ExecuteAction(string type)
    {
        switch (type)
        {
            case "Attack":
                if (GetDistance(_player, _enemy) <= 2)
                {
                    _enemy.hp -= 20;
                    _logText.text += $"플레이어 공격! 적 HP: {_enemy.hp}\n";
                } 
                break;
            case "Move":
                MoveToward(_player, _enemy);
                _logText.text += $"플레이어 이동: ({_player.x}, {_player.y})\n";
                break;
            case "Defend":
                /* 다음 턴 받는 데미지 감소 로직 추가 */
                _logText.text += "플레이어 방어!\n";
                break;
            case "Wait":
                _logText.text += "플레이어 대기\n";
                break;
        }
    }

    private int GetDistance(Unit a, Unit b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void MoveToward(Unit unit, Unit target)
    {
        if (unit.x < target.x) unit.x++;
        else if (unit.y < target.y) unit.y++;
    }

    private void UpdateUnitPositions()
    {
        _player.gameObject.transform.position = new Vector3(_player.x, _player.y, 0);
        _enemy.gameObject.transform.position = new Vector3(_enemy.x, _enemy.y, 0);
    }

    public void AddActionBlock(ActionBlock block)
    {
        _playerActions.Add(block);
    }
}

