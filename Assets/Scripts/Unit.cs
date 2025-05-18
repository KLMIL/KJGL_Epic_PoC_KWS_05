/**********************************************************
 *Script Name: Unit
 * Author: 김우성
 * Date Created: 2025-05-18
 * Last Modified: 2025-05-18
 * Description: 
 * - 플레이어와 적 유닛의 상태 관리
 *********************************************************/

using UnityEngine;

[System.Serializable]
public class Unit
{
    public int hp;
    public int x, y;
    public GameObject gameObject; // Unity 오브젝트 참조

    public Unit(int hp, int x, int y, GameObject obj)
    {
        this.hp = hp;
        this.x = x;
        this.y = y;
        this.gameObject = obj;
    }
}