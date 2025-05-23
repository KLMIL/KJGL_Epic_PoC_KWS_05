/**********************************************************
 * Script Name: MapController
 * Author: 김우성
 * Date Created: 2025-05-18
 * Last Modified: 2025-05-18
 * Description: 
 * - 맵 위에 유닛 위치 정보 저장 및 스프라이트 이동
 *********************************************************/

using NUnit.Framework;
using System.Collections.Generic;
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

    [SerializeField] List<UnitController> _players = new List<UnitController>();
    [SerializeField] List<UnitController> _enemies = new List<UnitController>();

    public List<UnitController> Players => _players;
    public List<UnitController> Enemies => _enemies;


    private void Start()
    {
        //ResetMap();
    }

    public void DEP_ResetMap()
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

    public void ResetMap()
    {
        _players.Clear();
        _enemies.Clear();

        foreach (var p in _players)
        {
            if (p != null && p.gameObject != null)
            {
                Destroy(p.gameObject);
            }
        }

        foreach (var e in _enemies)
        {
            if (e != null && e.gameObject != null)
            {
                Destroy(e.gameObject);
            }
        }

        var player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<UnitController>().Initialize(this, Vector2Int.zero, (int)UnitType.Player);
        _players.Add(player.GetComponent<UnitController>());

        var enemy = Instantiate(_enemyPrefab, Vector3.zero, Quaternion.identity);
        enemy.GetComponent<UnitController>().Initialize(this, new Vector2Int(2, 2), (int)UnitType.Enemy);
        _enemies.Add(enemy.GetComponent<UnitController>());
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(
                x * _cellSize - (_mapSize * _cellSize / 2) + (_cellSize / 2),
                y * _cellSize - (_mapSize * _cellSize / 2) + (_cellSize / 2),
                0
            );
    }

    public bool DEP_IsValidMove(int x, int y)
    {
        int offset = _mapSize / 2;
        int mapX = x + offset;
        int mapY = y + offset;

        return x >= -offset && x <= offset && y >= -offset && y <= offset
            && mapX >= 0 && mapY >= 0 && mapX < _mapSize && mapY < _mapSize
            && _map[mapX, mapY] == (int)UnitType.None;
    }

    public bool IsValidMove(Vector2 position)
    {
        float boundary = _mapSize * _cellSize / 2;
        return position.x >= -boundary && position.x <= boundary && position.y >= -boundary && position.y <= boundary;
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