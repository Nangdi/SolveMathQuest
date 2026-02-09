using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class GameSessionManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text timeText_Kiosk;
    [SerializeField]
    TMP_Text timeText_Floor;
    [SerializeField]
    Slider timeSlider;

    [SerializeField]
    GameObject[] floorLifes;
    [SerializeField]
    GameObject[] kioskLifes;
    public GameObject pauseScreen;


    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateTimeText(float currentTime)
    {
        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(currentTime));

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timeText_Kiosk.text = $"{minutes:00}:{seconds:00}";
        timeText_Floor.text = $"{minutes:00}:{seconds:00}";
        UpdateSliderValue(currentTime);
    }
    public void LoseLife(int life)
    {
        for (int i = 0; i < 3-life; i++)
        {
            floorLifes[i].SetActive(false);
            kioskLifes[i].SetActive(false);
        }
    }
    private void UpdateSliderValue(float currentTime)
    {
        float ratio = currentTime / 300;
        timeSlider.value = ratio;
    }
    public void ResetUIState()
    {
        for (int i = 0; i > 3; i++)
        {
            floorLifes[i].SetActive(true);
            kioskLifes[i].SetActive(true);
        }
        UpdateSliderValue(300);
        UpdateTimeText(300);
    }
}
