using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class Map
{
    public GameObject[] maps;
}


public class UIFlowManager : MonoBehaviour
{
    public SpriteSwaper spriteSwaper;
    [SerializeField]
    private int currentFlowIndex = 0;
    private int currentModedIndex = 0;
    private int currentDifficultyIndex = 0;
    private bool ChangingScene = false;
    [SerializeField]
    private GameObject[] Screens;
    [SerializeField]
    private Map[] gameMaps;
    [SerializeField]
    private GameObject myRecord;
    [SerializeField]
    private GameObject[] FloorScreens;

    public Button hintYesBtn;
    private void Start()
    {
        AllDeactiveMap();
        JumpScene(currentFlowIndex);
    }
    public void SelectDifficulty(int index)
    {
        GameManager.instance.SetDifficulty(index);
        currentDifficultyIndex = index;
        //난이도 선택시 스프라이트 교체
        spriteSwaper.SwapDifficultySprite();
        //난이도, 모드에 맞는 맵 켜주기 
        ActiveMap(currentModedIndex, currentDifficultyIndex);
        GameManager.instance.ResetGame();

    }
    public void SelectGameMode(int index)
    {
        //모드데이터변경
        GameManager.instance.SetGameMode(index);
        currentModedIndex = index;
        spriteSwaper.SwapModeSprite();
        //모드 선택시 스프라이트 교체
    }
    //뒤로가기
    public void ChangeScene(int index = 1)
    {
        if (ChangingScene) return;
        GameManager.instance.startGame = false;
        StartCoroutine(ChangeScene_co(index));


    }
    public void JumpScene(int index)
    {
        if (ChangingScene) return;
        GameManager.instance.startGame = false;
        /*
         초기화면 = 0
         
         */
        StartCoroutine(JumpScene_co(index));
    }
    private IEnumerator ChangeScene_co(int index)
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
    private IEnumerator JumpScene_co(int index )
    {
        ChangingScene = true;
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < Screens.Length; i++)
        {
            Screens[i].SetActive(false);
        }

        currentFlowIndex = index;
        Screens[currentFlowIndex].SetActive(true);
        ChangingScene = false;
    }
    public void UpdateResultPanel(bool isClear)
    {
        spriteSwaper.SwapResultSprite(isClear);
        myRecord.SetActive(isClear);
    }
    private void ActiveMap(int modeIndex , int difficultyIndex)
    {
        AllDeactiveMap();
        gameMaps[modeIndex].maps[difficultyIndex].SetActive(true);
    }
    private void AllDeactiveMap()
    {
        for (int i = 0; i < gameMaps.Length; i++)
        {
            for (int j = 0; j < gameMaps[i].maps.Length; j++)
            {
                gameMaps[i].maps[j].SetActive(false);
            }
        }
    }
    public void ResetFloorScreen()
    {
        for (int i = 0; i < FloorScreens.Length; i++)
        {
            FloorScreens[i].SetActive(false);
        }
        FloorScreens[0].SetActive(true);
        FloorScreens[1].SetActive(true);
        JumpScene(0);
        AllDeactiveMap();
    }
    public GameObject GetCurrentMap()
    {
        return gameMaps[currentModedIndex].maps[currentDifficultyIndex];
    }
}
