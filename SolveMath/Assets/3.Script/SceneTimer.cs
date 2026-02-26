using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTimer : MonoBehaviour
{
    public float lapseTime = 0;
    private float resetTime = 60;
    private bool _isRunning;
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
    private void Update()
    {
        if (!_isRunning) return;
        lapseTime += Time.deltaTime;
        if(lapseTime >= resetTime)
        {
            _isRunning = false;
            uIFlowManager.ResetFloorScreen();
           

        }
    }
}
