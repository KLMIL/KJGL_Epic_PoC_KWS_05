using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] MapManager _mapManager;

    List<UnitController> _playerUnits;
    List<UnitController> _enemyUnits;
    bool _battleActive = false;

    private void Start()
    {
        _playerUnits = _mapManager.GetPlayerUnits();
        _enemyUnits = _mapManager.GetEnemyUnits();
    }

    public void StartBattle()
    {
        _battleActive = true;
        foreach (UnitController unit in _playerUnits)
        {
            StartCoroutine(AttackRoutine(unit));
        }
        foreach (UnitController unit in _enemyUnits)
        {
            StartCoroutine(AttackRoutine(unit));
        }
    }

    private System.Collections.IEnumerator AttackRoutine(UnitController unit)
    {
        while (_battleActive && unit.isAlive)
        {
            UnitController target = FindTarget(unit);
            if (target != null)
            {
                unit.Attack(target);
                if (!target.isAlive)
                {
                    if (target.isEnemy)
                    {
                        _enemyUnits.Remove(target);
                    }
                    else
                    {
                        _playerUnits.Remove(target);
                    }
                }
            }

            // 전투 종료 조건 확인
            if (_playerUnits.Count == 0 || _enemyUnits.Count == 0)
            {
                _battleActive = false;
                Debug.Log(_playerUnits.Count == 0 ? "Enemy wins" : "Player wins");
                yield break;
            }

            yield return new WaitForSeconds(1f / unit.attackSpeed);
        }
    }

    private UnitController FindTarget(UnitController attacker)
    {
        List<UnitController> targets = attacker.isEnemy ? _playerUnits : _enemyUnits;
        UnitController closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (UnitController target in targets)
        {
            if (!target.isAlive) continue;

            Vector2Int attackerPos = _mapManager.GetUnitPosition(attacker.gameObject);
            Vector2Int targetPos = _mapManager.GetUnitPosition(target.gameObject);
            Vector2 attackerWorldPos = _mapManager.GridToWorld(attackerPos);
            Vector2 targetWorldPos = _mapManager.GridToWorld(targetPos);
            float distance = Vector2.Distance(attackerWorldPos, targetWorldPos);

            if (distance <= attacker.attackRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }
}
