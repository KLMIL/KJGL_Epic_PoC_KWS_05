using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] int _gridWidth = 12;
    [SerializeField] int _gridHeight = 8;
    [SerializeField] float _tileSize = 1f;

    GameObject[,] _grid; // 유닛 저장용 그리드
    Vector2 _gridOffset; // 좌표 계산용 오프셋
    Dictionary<GameObject, Vector2Int> _unitPositions = new Dictionary<GameObject, Vector2Int>();
    List<string> _battleLog = new List<string>();

    List<UnitController> _playerUnits = new List<UnitController>();
    List<UnitController> _enemyUnits = new List<UnitController>();

    [SerializeField] List<GameObject> _playerUnitPrefabs;
    [SerializeField] List<GameObject> _enemyUnitPrefabs;

    public List<UnitController> GetPlayerUnits() => _playerUnits;
    public List<UnitController> GetEnemyUnits() => _enemyUnits;

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        _grid = new GameObject[_gridWidth, _gridHeight];
        //_gridOffset = new Vector2(-_gridWidth / 2 * _tileSize, -_gridHeight / 2 * _tileSize);
        _gridOffset = new Vector2(-6f, -4f); // 일단 조정해봄
    }


    // 유닛 배치
    public void PlaceUnits(List<GameObject> playerUnitsPrefabs, List<GameObject> enemyUnitsPrefabs)
    {

    }


    // 유닛 이동 처리 및 로그 기록
    public bool MoveUnit(GameObject unit, Vector2Int newGridPos)
    {
        // 배열 인덱스 범위 확인
        if (newGridPos.x < 0 || newGridPos.x > _gridWidth || newGridPos.y < 0 || newGridPos.y >= _gridHeight)
        {
            Debug.Log("Invalid position: Out of bounds");
            return false;
        }

        // 타일에 이미 유닛이 있는지 확인
        if (_grid[newGridPos.x, newGridPos.y] != null)
        {
            Debug.Log("Cannot move: Position is occupied");
            return false;
        }

        // 기존 위치에서 유닛 제거
        if (_unitPositions.ContainsKey(unit))
        {
            Vector2Int oldPos = _unitPositions[unit];
            _grid[oldPos.x, oldPos.y] = null;
        }

        // 새 위치로 유닛 이동
        _grid[newGridPos.x, newGridPos.y] = unit;
        _unitPositions[unit] = newGridPos;

        // 월드 좌표로 이동
        Vector2 worldPosition = GridToWorld(newGridPos);
        unit.transform.position = worldPosition;

        string log = $"Unit {unit.name} moved to ({newGridPos.x - _gridWidth / 2}, {newGridPos.y - _gridHeight / 2} at {System.DateTime.Now}";
        _battleLog.Add(log);
        Debug.Log(log);

        return true;
    }

    // 그리드 인덱스를 월드 좌표로 변환
    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        float x = gridPos.x * _tileSize + _gridOffset.x + _tileSize / 2;
        float y = gridPos.y * _tileSize + _gridOffset.y + _tileSize / 2;
        return new Vector2(x, y);
    }

    // 월드 좌표를 그리드 인덱스로 변환
    public Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - _gridOffset.x) / _tileSize);
        int y = Mathf.FloorToInt((worldPosition.y - _gridOffset.y) / _tileSize);

        x = Mathf.Clamp(x, 0, _gridWidth - 1);
        y = Mathf.Clamp(y, 0, _gridHeight - 1);

        return new Vector2Int(x, y);
    }

    // 유닛의 현재 위치 반환
    public Vector2Int GetUnitPosition(GameObject unit)
    {
        if (_unitPositions.ContainsKey(unit))
        {
            return _unitPositions[unit];
        }
        return new Vector2Int(-1, -1);
    }

    // 특정 위치의 유닛 반환
    public GameObject GetUnitAt(Vector2Int position)
    {
        if (position.x >= 0 && position.x < _gridWidth && position.y >= 0 && position.y < _gridHeight)
        {
            return _grid[position.x, position.y];
        }
        return null;
    }

    // 전투 로그 반환
    public List<string> GetBattleLog()
    {
        return new List<string>(_battleLog);
    }

    // 실제 좌표로 변환(-6부터 6, -4부터 4 등)
    public Vector2Int GridToMapCoordinates(Vector2Int gridPos)
    {
        return new Vector2Int(gridPos.x - _gridWidth / 2, gridPos.y - _gridHeight / 2);
    }

    // 맵 좌표를 그리드 인덱스로 변환
    public Vector2Int MapToGridCoordinates(Vector2Int mapPos)
    {
        return new Vector2Int(mapPos.x + _gridWidth / 2, mapPos.y + _gridHeight / 2);
    }
}
