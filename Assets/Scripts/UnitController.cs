using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum UnitType { Player, Enemy }
public enum ConditionType { MonsterNear3, NoMonsterNear, HPLower30 }
public enum ActionType { AttackMonster, MoveToMonster, UsePosition }

public class UnitController : MonoBehaviour
{
    [SerializeField] UnitType _unitType;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] int maxHP = 100;
    int _hp;
    float lastAttackTime = 0f;
    MapManager _mapManager;

    public UnitType UnitType => _unitType;
    public bool IsAlive => _hp > 0;

    public void Initialize(MapManager manager, Vector2 position)
    {
        _mapManager = manager;
        transform.position = position;
        _hp = maxHP;
    }

    public void TakeDamage(int damage)
    {
        _hp = Mathf.Max(0, _hp - damage);
        if (!IsAlive) gameObject.SetActive(false);
        Debug.Log($"{_unitType} took {damage} damage, HP: {_hp}");
    }

    public bool CanAttack() => Time.time >= lastAttackTime + attackCooldown;

    public void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;
        if (_mapManager.IsValidMove(newPosition))
        {
            transform.position = newPosition;
            Debug.Log($"{_unitType} moved towards {target}");
        }
    }

    public bool Attack(UnitController target)
    {
        if (!CanAttack()) return false;
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= attackRange)
        {
            target.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
            Debug.Log($"{_unitType} attacked {target.UnitType} for {attackDamage} damage");
            return true;
        }
        return false;
    }

    public bool EvaluateCondition(ConditionType condition, List<UnitController> enemies)
    {
        switch (condition)
        {
            case ConditionType.MonsterNear3:
                return enemies.Any(e => Vector2.Distance(transform.position, e.transform.position) <= 3f);
            case ConditionType.NoMonsterNear:
                return !enemies.Any(e => Vector2.Distance(transform.position, e.transform.position) <= 3f);
            case ConditionType.HPLower30:
                return _hp < 30;
            default:
                return false;
        }
    }

    public void ExecuteAction(ActionType action, List<UnitController> enemies)
    {
        switch (action)
        {
            case ActionType.AttackMonster:
                var nearest = enemies.OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).FirstOrDefault();
                if (nearest != null) Attack(nearest);
                break;
            case ActionType.MoveToMonster:
                var target = enemies.OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).FirstOrDefault();
                if (target != null) MoveTowards(target.transform.position);
                break;
            case ActionType.UsePosition:
                MoveTowards(Vector2.zero);
                _hp += 50;
                break;
        }
    }

    public void PerformEnemyAction(List<UnitController> players)
    {
        var nearest = players.OrderBy(p => Vector2.Distance(transform.position, p.transform.position)).FirstOrDefault();
        if (nearest == null) return;
        float distance = Vector2.Distance(transform.position, nearest.transform.position);
        if (distance <= attackRange && CanAttack())
            Attack(nearest);
        else
            MoveTowards(nearest.transform.position);
    }
}