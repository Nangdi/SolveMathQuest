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
        //방향판단을 뭘로할것인가
        //한 방향으로 이동했는가? 0,0 이면 패스 및 방향 업데이트

        //카운트가 0인가? pass
        //카운트가 0일시 현재자리의 카운트 업데이트, 방향 리셋 
        //if (movingDir == Vector2Int.zero && col.CompareTag("Start"))

        //이전위치 저장
        previousPos = currentPos;
        //현재위치 업데이트
        currentPos = MapManager.instance.colliderToIndex[col];
        //이동방향계산
        int dx = currentPos.x - previousPos.x;
        int dy = currentPos.y - previousPos.y;
        Vector2Int dir = new Vector2Int(dx, dy);
        moveCount--;
        Debug.Log($"무브 카운트 : {moveCount}");
        //초기화상태일시 
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
        Debug.Log($"정상진행");
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
