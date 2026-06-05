using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;

public class Rule_Jump : MonoBehaviour, IGameRule
{
    int[,] map;
    int moveCount = 0;
    Vector2Int currentPos;
    Vector2Int movingDir;
    Vector2Int previousPos;


    public bool isRuleViolated(Collider2D col)
    {
        previousPos = currentPos;

        if (!MapManager.instance.colliderToIndex.ContainsKey(col))
        {
            Debug.LogWarning($"등록되지 않은 콜라이더: {col.name}");
            return false;
        }

        currentPos = MapManager.instance.colliderToIndex[col];

        Vector2Int dir = currentPos - previousPos;

        if (dir == Vector2Int.zero)
        {
            Debug.Log("같은 칸 재진입 - 무시");
            return false;
        }

        if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) != 1)
        {
            Debug.Log($"비정상 이동 감지: {previousPos} -> {currentPos}");
            return true;
        }

        moveCount--;

        Debug.Log($"무브 카운트 : {moveCount}");

        if (movingDir == Vector2Int.zero)
        {
            movingDir = dir;
        }
        else if (movingDir != dir)
        {
            Debug.Log($"진행방향틀림 {movingDir} , {dir}");
            return true;
        }

        if (col.CompareTag("Arrive") && moveCount != 0)
        {
            Debug.Log("도착지점에 왔지만 count가 0이아님 클리어 X");
        }

        if (moveCount == 0)
        {
            Debug.Log("정상진행완료 , 초기화");
            ResetData(col);
        }

        return false;
    }

    public void ResetData(Collider2D col)
    {
        // 좌표찾기
        currentPos = MapManager.instance.colliderToIndex[col];
        
        //이동해야하는 Count 초기화
        if (map != null)
        {
            moveCount = map[currentPos.y, currentPos.x];
        }
        else
        {
            Debug.Log("Map이 null입니다");

        }
        //이동중인방향 초기화
        movingDir = Vector2Int.zero;

        Debug.Log($"현재바닥 숫자 {moveCount}, 현재좌표 : {currentPos} ");

    }
    public bool GameClear(Collider2D col)
    {
        if (col.CompareTag("Arrive") && moveCount == 0)
        {
            return true;

        }
        return false;
    }
    public void SetDifficultMode(Difficulty difficulty)
    {

        switch (difficulty)
        {
            case Difficulty.eazy:
                map = new int[MapManager.instance.jumpCount1.Length, MapManager.instance.jumpCount1.Length];
                map = MapManager.instance.jumpCount1;
                break;
            case Difficulty.Normal:
                map = new int[MapManager.instance.jumpCount2.Length, MapManager.instance.jumpCount2.Length];
                map = MapManager.instance.jumpCount2;
                break;
            case Difficulty.Hard:
                map = new int[MapManager.instance.jumpCount3.Length, MapManager.instance.jumpCount3.Length];
                map = MapManager.instance.jumpCount3;
                break;
        }
    }

    public void VeiwHint()
    {
        throw new System.NotImplementedException();
    }
}
