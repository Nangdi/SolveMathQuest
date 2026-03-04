using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTimer : MonoBehaviour
{
    public float lapseTime = 0;
    private float resetTime = 60;
    private bool _isRunning;
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
    }
    private void Update()
    {
        if (!_isRunning || isTrigger) return;
        lapseTime += Time.deltaTime;
        if(lapseTime >= resetTime)
        {
            _isRunning = false;
            isTrigger = true;
            uIFlowManager.ResetFloorScreen();
           

        }
    }
}
