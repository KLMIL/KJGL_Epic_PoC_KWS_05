/**********************************************************
 * Script Name: UnitController
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - CodeBlockManager에 따른 Unit의 기능 수행
 *********************************************************/

using Unity.Hierarchy;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] MapManager _mapManager;
    [SerializeField] Vector2Int _position;
    [SerializeField] int _unitType;
    [SerializeField] int _hp = 100;

    public bool IsAlive() => _hp > 0;
    public int UnitType => _unitType;
    public Vector2Int Position => _position;

    public void Initialize(MapManager manager, Vector2Int initialPosition, int type)
    {
        _mapManager = manager;
        _position = initialPosition;
        _unitType = type;
        transform.position = _mapManager.GetWorldPosition(_position.x + 2, _position.y + 2);
    }

    public void Move(string direction)
    {
        Vector2Int moveDirection = Vector2Int.zero;
        switch (direction.ToLower())
        {
            case "up": moveDirection = new Vector2Int(0, 1); break;
            case "down": moveDirection = new Vector2Int(0, -1); break;
            case "left": moveDirection = new Vector2Int(-1, 0); break;
            case "right": moveDirection = new Vector2Int(1, 0); break;
        }

        int newX = _position.x + moveDirection.x;
        int newY = _position.y + moveDirection.y;

        if (_mapManager.IsValidMove(newX, newY))
        {
            _mapManager.UpdateMap(_position.x, _position.y, newX, newY, _unitType);
            _position = new Vector2Int(newX, newY);
            LogManager.Instance.AddLog($"{_unitType} moved {direction} to ({newX}, {newY})");
        }
        else
        {
            LogManager.Instance.AddLog($"{_unitType} failed to move {direction} to ({newX}, {newY})");
        }
    }

    public bool Attack(UnitController target)
    {
        if (target == null || !target.IsAlive()) return false;

        int distance = Mathf.Abs(_position.x - target._position.x) + Mathf.Abs(_position.y - target._position.y);
        if (distance == 1) // 맨해튼 거리 1
        {
            target.TakeDamage(10); // 10 대미지 부여
            LogManager.Instance.AddLog($"{_unitType} attacked {target.UnitType} at ({target._position.x}, {target._position.y})");
            return true;
        }
        else
        {
            LogManager.Instance.AddLog($"{_unitType} failed to attack {target.UnitType}: Out of range");
            return false;
        }
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            _hp = 0;
            LogManager.Instance.AddLog($"{_unitType} at ({_position.x}, {_position}) has been defeated");
            gameObject.SetActive(false);
        }
        else
        {
            LogManager.Instance.AddLog($"{_unitType} at ({_position.x}, {_position.y}) took {damage} damage, HP: {_hp}");
        }
    }
}
