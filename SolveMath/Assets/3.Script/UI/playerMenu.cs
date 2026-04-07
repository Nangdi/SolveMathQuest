using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class playerMenu : MonoBehaviour
{
    public GameStateTextController gameStateTextController;

    private bool stopRotating;
    public bool openMenu  { get; private set; }
    private float targetTime = 3;
    private float lapseTime;
    private float rotationValue;
    private float rotationSpeed = 23;
    [SerializeField]
    private GameObject blackBorad;
    [SerializeField]
    private GameObject restartPanel;
    [SerializeField]
    private GameObject[] menuOptions;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private Image fillMenu;
    [SerializeField] 
    private GameObject HintOptionOB;
    public GameObject optionRoot;

    void Start()
    {
        
        openMenu = false;
        CloseMenu();
        targetTime = JsonManager.instance.gameSettingData.menuHoldTime;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(stopRotating);
        if (!GameManager.instance.startGame || GameManager.instance.Paused) return;
        if (stopRotating )
        {
            lapseTime += Time.deltaTime;
            fillMenu.fillAmount = Mathf.Clamp01(lapseTime/ targetTime);
            if (lapseTime > targetTime && !openMenu)
            {
                OpenMenu();
            }

        }
        else if(!openMenu && !stopRotating)
        {
            rotationValue += Time.deltaTime * rotationSpeed;
            if (rotationValue >= 360f)
            {
                rotationValue -= 360f;
            }
            transform.rotation = Quaternion.Euler(0f, 0f, rotationValue);
        }

        //회전 메소드
    }
    public void MenuValueReset()
    {
        stopRotating = false;
        openMenu = false;
        lapseTime = 0;
        fillMenu.fillAmount = 0;
        HintOptionOB.SetActive(true);
    }
    public void SetStopRotating(bool touchMenu)
    {
        Debug.Log($"상태바꿈 : {touchMenu}");
        stopRotating = touchMenu;
        if (!openMenu && !touchMenu)
        {

            MenuValueReset();
        }
    }
    public void CloseMenu()
    {
        //메뉴창 모두 닫기
        //메뉴설정초기화
        MenuValueReset();
        foreach (var item in menuOptions)
        {
            item.SetActive(false);
        }
        menu.SetActive(true);
        blackBorad.SetActive(false);
    }
    public void OpenMenu()
    {
        openMenu = true;
        //메뉴 열림 메소드
        menuOptions[0].SetActive(true);
        if (GameManager.instance.life < 1)
        {
            HintOptionOB.SetActive(false);
        }
        menu.SetActive(false);
        //검은화면 On
        blackBorad.SetActive(true);
        gameStateTextController.SetActiveText(true);
        gameStateTextController.SetGameStateText(GameState.Menu);
        //게임 퍼즈
        GameManager.instance.Paused = true;
    }
}
