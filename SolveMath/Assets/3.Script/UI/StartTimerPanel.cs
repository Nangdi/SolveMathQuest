using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartTimerPanel : MonoBehaviour
{
    
    public TMP_Text timerText;
    private float exposedTime =0;
    int targetTime= 7;

    private void OnEnable()
    {
        exposedTime = 0;
        timerText.text = $"{targetTime}";
    }
    private void Update()
    {
        exposedTime += Time.deltaTime;
        timerText.text = $"{(int)(targetTime - exposedTime)}";
        if (exposedTime > targetTime)
        {
            if (!GameManager.instance.startGame)
            {
                GameManager.instance.GameStart();
            }
            else if (GameManager.instance.Paused)
            {

                GameManager.instance.Paused = false;
                GameManager.instance.playMenu.SetStopRotating(false);
            }
                gameObject.SetActive(false);
        }
    }
}
