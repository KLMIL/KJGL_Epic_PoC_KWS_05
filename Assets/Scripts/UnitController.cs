/**********************************************************
 * Script Name: UnitController
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - CodeBlockManager에 따른 Unit의 기능 수행
 *********************************************************/

using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] MapManager _mapManager;
    [SerializeField] Vector2Int _position;
    [SerializeField] int _unitType;

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
}
