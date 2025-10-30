using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIFlowManager : MonoBehaviour
{
    [SerializeField]
    private SpriteSwaper spriteSwaper;

    private int currentFlowIndex =2;
    private bool ChangingScene = false;
    [SerializeField]
    private GameObject[] Screens;
    public void SelectDifficulty(int index)
    {
        GameManager.instance.SetDifficulty(index);
        //난이도 선택시 스프라이트 교체
        spriteSwaper.SwapDifficultySprite();
        //난이도, 모드에 맞는 맵 켜주기 

    }
    public void SelectGameMode(int index)
    {
        //모드데이터변경
        GameManager.instance.SetGameMode(index);
        spriteSwaper.SwapModeSprite();
        //모드 선택시 스프라이트 교체
    }
    //뒤로가기
    public void ChangeScene(int index = 1)
    {
        if (ChangingScene) return;
        
        StartCoroutine(DelayTime(index));
        

    }
    private IEnumerator DelayTime(int index)
    {
        ChangingScene = true;
        yield return new WaitForSeconds(0.5f);
        
        for (int i = 0; i < Screens.Length; i++)
        {
            Screens[i].SetActive(false);
        }
        currentFlowIndex += index;
        Screens[currentFlowIndex].SetActive(true);
        ChangingScene = false;
    }
}
