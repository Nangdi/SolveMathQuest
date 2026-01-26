using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Record : MonoBehaviour
{
    [SerializeField]
    private Sprite[] prizeSprites;
    public Image prizeImage;
    public TMP_Text ranking_text;
    public TMP_Text rankerName_text;
    public TMP_Text record_text;
    public int ranking;
    public string rankerName;
    public float record;

    void Start()
    {
        
    }

    public void SetRecord(int ranking , string rankerName , string recordTime)
    {
        UpdatePrizeImage(ranking);
        ranking_text.text = ranking.ToString();
        rankerName_text.text = rankerName;
        record_text.text = recordTime;
    }

    private void UpdatePrizeImage(int ranking)
    {
        if (ranking < 4)
        {
            prizeImage.gameObject.SetActive(true);
            prizeImage.sprite = prizeSprites[ranking - 1];
        }
        else
        {
            prizeImage.gameObject.SetActive(false);
        }
       
    }
}
