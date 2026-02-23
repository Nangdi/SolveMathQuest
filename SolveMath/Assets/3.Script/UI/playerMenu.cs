using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMenu : MonoBehaviour
{
    private bool stopRotating;
    private bool openMenu = false;
    private float targetTime = 3;
    private float lapseTime;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stopRotating )
        {
            lapseTime += Time.deltaTime;
            if (lapseTime > targetTime && !openMenu)
            {
                openMenu = true;
                //메뉴 열림 메소드 todo
            }

        }
        //회전 메소드
    }
    public void MenuReset()
    {
        stopRotating = false;
        openMenu = false;
        lapseTime = 0;
    }
    public void SetRotating(bool rotating)
    {
        stopRotating = rotating;
    }
}
