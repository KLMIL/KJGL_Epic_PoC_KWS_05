/**********************************************************
 * Script Name: MapController
 * Author: 김우성
 * Date Created: 2025-05-18
 * Last Modified: 2025-05-18
 * Description: 
 * - 맵 위에 유닛 위치 정보 저장 및 스프라이트 이동
 *********************************************************/

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

    GameObject _playerInstance;
    public GameObject PlayerInstance => _playerInstance;


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

        // 유닛 스폰
        if (_playerInstance)
        {
            Destroy(_playerInstance);
        };
        _playerInstance = Instantiate(_playerPrefab, GetWorldPosition(2, 2), Quaternion.identity);
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
    }
}