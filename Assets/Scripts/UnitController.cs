/**********************************************************
 * Script Name: UnitController
 * Author: 김우성
 * Date Created: 2025-05-19
 * Last Modified: 2025-05-19
 * Description: 
 * - CodeBlockManager에 따른 Unit의 기능 수행
 *********************************************************/

using System.Collections;
using Unity.Hierarchy;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] MapManager _mapManager;
    [SerializeField] Vector2Int _position;
    [SerializeField] int _unitType;
    [SerializeField] int _hp = 100;

    SpriteRenderer _spriteRenderer;
    Vector3 _originalScale;

    public Vector2Int Position => _position;
    public int UnitType => _unitType;

    public bool IsAlive() => _hp > 0;
    


    public void Initialize(MapManager manager, Vector2Int initialPosition, int type)
    {
        _mapManager = manager;
        _position = initialPosition;
        _unitType = type;
        transform.position = _mapManager.GetWorldPosition(_position.x + 2, _position.y + 2);

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
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
        StartCoroutine(PlayHitEffect());
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

    private IEnumerator PlayHitEffect()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        Color _originalColor = _spriteRenderer.color;

        Vector3 targetScale = _originalScale * 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, t);
            _spriteRenderer.color = Color.Lerp(_originalColor, Color.red, t); // 빨간빛 전환
            yield return null;
        }

        elapsed = 0f;
        // 스케일 및 색상 복원
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, _originalScale, t);
            _spriteRenderer.color = Color.Lerp(Color.red, _originalColor, t); // 원래 색상 복원
            yield return null;
        }

        // 정확히 원래 상태로 복원
        transform.localScale = _originalScale;
        _spriteRenderer.color = _originalColor;
    }


    // 적 AI 관련
    public void PerformEnemyAction(UnitController player)
    {
        if (!IsAlive() || player == null || !player.IsAlive()) return;

        int distance = Mathf.Abs(_position.x - player.Position.x) + Mathf.Abs(_position.y - player.Position.y);
        if (distance == 1)
        {
            Attack(player);
        }
        else
        {
            MoveTowardsPlayer(player);
        }
    }

    private void MoveTowardsPlayer(UnitController player)
    {
        int dx = player.Position.x - _position.x;
        int dy = player.Position.y - _position.y;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
        {
            if (dx > 0) Move("right");
            else if (dx < 0) Move("left");
        }
        else
        {
            if (dy > 0) Move("up");
            else if (dy < 0) Move("down");
        }
    }
}
