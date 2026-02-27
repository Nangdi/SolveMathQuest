using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class multiDetectTimer : MonoBehaviour
{

    public float lapseTime = 0;
    private float resetTime = 10;
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
    [SerializeField]
    private TMP_Text timer;

    private void OnEnable()
    {
        lapseTime = 0;
        _isRunning = true;
        timer.text = "10";
    }
    private void Update()
    {
        if (!_isRunning) return;
        lapseTime += Time.deltaTime;
        int countDown = 10 - (int)lapseTime;
        timer.text = countDown.ToString();
        if (lapseTime >= resetTime)
        {
            _isRunning = false;
            gameObject.SetActive(false);
            uIFlowManager.ResetFloorScreen();


        }
    }
    private void OnDisable()
    {
        GameManager.instance.Paused = false;
    }
}


