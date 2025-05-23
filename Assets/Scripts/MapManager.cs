using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [SerializeField] float _mapSize = 10f;
    [SerializeField] GameObject _playerInstance; // 씬에 배치
    [SerializeField] GameObject[] _enemyInstances; // 3개의 적, 씬에 배치
    public List<UnitController> Players = new List<UnitController>();
    public List<UnitController> Enemies = new List<UnitController>();

    public void ResetMap()
    {
        foreach (var player in Players)
            if (player != null && player.gameObject != null)
                player.gameObject.SetActive(true);
        Players.Clear();

        foreach (var enemy in Enemies)
            if (enemy != null && enemy.gameObject != null)
                enemy.gameObject.SetActive(true);
        Enemies.Clear();

        if (_playerInstance)
        {
            var player = _playerInstance.GetComponent<UnitController>();
            if (player)
            {
                player.Initialize(this, Vector2.zero);
                Players.Add(player);
            }
        }

        // 3개의 적 초기화
        if (_enemyInstances != null && _enemyInstances.Length == 3)
        {
            Vector2[] enemyPositions = new Vector2[] { new Vector2(3, 3), new Vector2(3, -3), new Vector2(-3, 3) };
            for (int i = 0; i < _enemyInstances.Length; i++)
            {
                if (_enemyInstances[i])
                {
                    var enemy = _enemyInstances[i].GetComponent<UnitController>();
                    if (enemy)
                    {
                        enemy.Initialize(this, enemyPositions[i]);
                        Enemies.Add(enemy);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("EnemyInstances array is not properly set! Expected 3 enemies.");
        }
    }

    public bool IsValidMove(Vector2 position)
    {
        float boundary = _mapSize / 2;
        return position.x >= -boundary && position.x <= boundary && position.y >= -boundary && position.y <= boundary;
    }
}