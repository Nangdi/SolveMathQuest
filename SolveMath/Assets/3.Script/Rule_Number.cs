using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule_Number : MonoBehaviour, IGameRule
{
    int[,] map;
    Vector2Int currentPos;
    Vector2Int previousPos;
    int currentNum = 2;
    int[] nomalInt = { 5, 2 };
    int moveCount = 1;

    public bool isRuleViolated(Collider2D col)
    {
        //다음발판 숫자 업데이트
        //현재숫자업데이트
        previousPos = currentPos;
        ResetData(col);
        int nextNum = map[currentPos.y, currentPos.x];
        switch (GameManager.instance.difficultyMode)
        {
            case Difficulty.eazy:
            case Difficulty.Hard:
                if (eazyHardRule(nextNum))
                {
                    return true;
                }
                break;
            case Difficulty.Normal:
                if (NomalRule(nextNum))
                {
                    return true;
                }
                break;
        }
        Debug.Log("정답.정상진행");
        return false;
    }

    
    public bool GameClear(Collider2D col)
    {
        if (col.CompareTag("Arrive"))
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
                map = new int[MapManager.instance.num1.Length, MapManager.instance.num1.Length];
                map = MapManager.instance.num1;
                break;
            case Difficulty.Normal:
                map = new int[MapManager.instance.num2.Length, MapManager.instance.num2.Length];
                map = MapManager.instance.num2;
                break;
            case Difficulty.Hard:
                map = new int[MapManager.instance.num3.Length, MapManager.instance.num3.Length];
                map = MapManager.instance.num3;
                break;
        }
    }
    private bool eazyHardRule(int nextNum)
    {
        if (currentNum != nextNum)
        {
            return true;
        }
        return false;
    }
    private bool NomalRule(int nextNum)
    {
        moveCount++;
        moveCount %= 2;
            Debug.Log($"현재발판 숫자 : {nextNum}, 다음밟아야하는 배수 : {nomalInt[moveCount]} ");
        if (nextNum % nomalInt[moveCount] !=0)
        {
            Debug.Log($"잘못된길");
            moveCount = 1;
            return true;
        }

        return false;
    }
    public void ResetData(Collider2D col)
    {
        currentPos = MapManager.instance.colliderToIndex[col];
    }

    public void VeiwHint()
    {
        throw new System.NotImplementedException();
    }
}
