/**********************************************************
 * Script Name: ActionBlock
 * Author: 김우성
 * Date Created: 2025-05-18
 * Last Modified: 2025-05-18
 * Description: 
 * - 플레이어가 AI를 제어할 수 있는 행동 블럭 정의
 *********************************************************/

[System.Serializable]
public class ActionBlock
{
    public string type; // "Move", "Attack", "Defend", "Wait"
    public string condition; // ex: "enemyDistance <= 2"
}
