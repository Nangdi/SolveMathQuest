using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class playerMenu : MonoBehaviour
{
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


    void Start()
    {
        openMenu = false;   
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.startGame || GameManager.instance.Paused) return;
        if (stopRotating )
        {
            lapseTime += Time.deltaTime;
            fillMenu.fillAmount = Mathf.Clamp01(lapseTime/3);
            if (lapseTime > targetTime && !openMenu)
            {
                openMenu = true;
                //메뉴 열림 메소드 todo
                menuOptions[0].SetActive(true);
                menu.SetActive(false);
                blackBorad.SetActive(true);
                //게임 퍼즈
                GameManager.instance.Paused = true;
                //검은화면 On
            }

        }
        else
        {
            rotationValue += Time.deltaTime * rotationSpeed;
            if(rotationValue >= 360f)
            {
                rotationValue -= 360f;
            }
            transform.rotation = Quaternion.Euler(0f, 0f, rotationValue);
        }

        //회전 메소드
    }
    public void MenuReset()
    {
        stopRotating = false;
        openMenu = false;
        lapseTime = 0;
        fillMenu.fillAmount = 0;
    }
    public void SetStopRotating(bool rotating)
    {
        stopRotating = rotating;
    }
    public void CloseMenu()
    {
        //메뉴창 모두 닫기
        //메뉴설정초기화
        MenuReset();
        foreach (var item in menuOptions)
        {
            item.SetActive(false);
        }
        menu.SetActive(true);
        blackBorad.SetActive(false);
        restartPanel.SetActive(true);
    }
}
