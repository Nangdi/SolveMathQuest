using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Rule_NoLeftTurn : MonoBehaviour, IGameRule
{
    public Vector2 currentPos;
    public Vector2 previousPos;
    public int headDir;
    public bool isRuleViolated(Collider2D col)
    {


        if (col.CompareTag("Wall"))
        {
            Debug.Log(col.name);
            return false;
          
        }
        //왔던길 저장
        //도착한곳 갱신


        Vector2 newPos = col.transform.position;


        Vector2 prevDir = (currentPos - previousPos).normalized;  // 이전 방향
        Vector2 currDir = (newPos - currentPos).normalized;       // 현재 방향

        float cross = prevDir.x * currDir.y - prevDir.y * currDir.x; // 외적
        float dot = Vector2.Dot(prevDir, currDir);                  // 내적

        if (dot < -0.7f)  // 거의 반대 → 후진
        {
            Debug.Log("후진");
            return false;

        }
        else if (cross > 0.3f) // 좌회전
        {
            Debug.Log("좌회전");
            return false;
        }
        else if (cross < -0.3f) // 우회전
        {
            Debug.Log("우회전 - 통과");
            headDir += 1;
            headDir %= 4;
        }
        else // 직진
        {
            Debug.Log("직진 - 통과");
        }

        // 위치 갱신
        previousPos = currentPos;
        currentPos = newPos;
        return true;
    }

    public void ResetData(Collider2D col)
    {
        currentPos = col.transform.position;
        Debug.Log($"지금위치 {currentPos}");
        previousPos = currentPos;
        Debug.Log($"이전위치 {previousPos}");
        headDir = 0;
    }
}
