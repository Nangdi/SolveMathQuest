using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTimer : MonoBehaviour
{
    public float lapseTime = 0;
    private float resetTime = 60;
    private bool _isRunning= false;
    public bool isTrigger;
    public bool isRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning == value) return;
            _isRunning = value;
            lapseTime = 0;
        }
    }
    [SerializeField]
    private UIFlowManager uIFlowManager;
    private void Start()
    {
        resetTime = JsonManager.instance.gameSettingData.ResetScreenTime;
        isRunning = false;
    }
    private void Update()
    {
        if (!isRunning) return;
        lapseTime += Time.deltaTime;
            Debug.Log($"현재 값 : {isRunning}");
        if(lapseTime >= resetTime)
        {
            Debug.Log("들어옴");
            isRunning = false;
            Debug.Log($"현재 값 : {isRunning}");
            uIFlowManager.ResetFloorScreen();
           

        }
    }
}
