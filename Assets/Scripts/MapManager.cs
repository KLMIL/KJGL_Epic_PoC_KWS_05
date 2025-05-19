/**********************************************************
 * Script Name: MapController
 * Author: 김우성
 * Date Created: 2025-05-18
 * Last Modified: 2025-05-18
 * Description: 
 * - 맵 위에 유닛 위치 정보 저장 및 스프라이트 이동
 *********************************************************/

using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;

enum UnitType
{
    None = 0,
    Player = 1,
    Enemy = 2
}

public class MapManager : MonoBehaviour
{
    int[,] _map = new int[5, 5];
    int _mapSize = 5;
    float _cellSize = 1f;

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] GameObject _enemyPrefab;

    GameObject _playerInstance;
    GameObject _enemyInstance;
    public GameObject PlayerInstance => _playerInstance;
    public GameObject EnemyInstance => _enemyInstance;


    private void Start()
    {
        ResetMap();
    }

    public void ResetMap()
    {
        for (int i = 0; i < _mapSize; i++)
        {
            for (int j = 0; j < _mapSize; j++)
            {
                _map[i, j] = (int)UnitType.None;
            }
        }

        // 초기 위치 설정
        _map[2, 2] = (int)UnitType.Player; // (0, 0), 중앙
        _map[3, 3] = (int)UnitType.Enemy; // (1, 1), 플레이어 우측 상단 생성

        // 플레이어 유닛 스폰
        if (_playerInstance)
        {
            Destroy(_playerInstance);
        };
        _playerInstance = Instantiate(_playerPrefab, GetWorldPosition(2, 2), Quaternion.identity);


        // 적 유닛 스폰
        if (_enemyInstance)
        {
            Destroy(_enemyInstance);
        }
        _enemyInstance = Instantiate(_enemyPrefab, GetWorldPosition(3, 3), Quaternion.identity);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(
                x * _cellSize - (_mapSize * _cellSize / 2) + (_cellSize / 2),
                y * _cellSize - (_mapSize * _cellSize / 2) + (_cellSize / 2),
                0
            );
    }

    public bool IsValidMove(int x, int y)
    {
        int offset = _mapSize / 2;
        int mapX = x + offset;
        int mapY = y + offset;

        return x >= -offset && x <= offset && y >= -offset && y <= offset
            && mapX >= 0 && mapY >= 0 && mapX < _mapSize && mapY < _mapSize
            && _map[mapX, mapY] == (int)UnitType.None;
    }

    public void UpdateMap(int oldX, int oldY, int newX, int newY, int unitType)
    {
        int offset = _mapSize / 2;
        _map[oldX + offset, oldY + offset] = (int)UnitType.None;
        _map[newX + offset, newY + offset] = (int)unitType;

        Vector3 newPosition = GetWorldPosition(newX + offset, newY + offset);
        if (unitType == (int)UnitType.Player)
        {
            _playerInstance.transform.position = newPosition;
        }
        else if (unitType == (int)UnitType.Enemy)
        {
            _enemyInstance.transform.position = newPosition;
        }
    }

    public void RemoveUnit(Vector2Int position)
    {
        int offset = _mapSize / 2;
        _map[position.x + offset, position.y + offset] = (int)UnitType.None;
    }
}