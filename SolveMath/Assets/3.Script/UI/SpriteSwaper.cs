using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwaper : MonoBehaviour
{
    [SerializeField]
    private Image[] targetImages;
    [SerializeField]
    private Sprite[] difficultUIs;
    [Header("결과창")]
    [SerializeField]
    private Image resultImage;
    [SerializeField]
    private Image frameImage;
    [SerializeField]
    private Sprite[] resultSprites;
    [SerializeField]
    private Sprite[] frameSprites;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwapModeSprite()
    {
        GameModeUIData uiData = GameManager.instance.currentData;
        for (int i = 0; i < uiData.mapSprites.Length; i++)
        {
            targetImages[i].sprite = uiData.mapSprites[i];
        }
        targetImages[3].sprite = uiData.titleSprite;
        targetImages[4].sprite = uiData.titleSprite;
        targetImages[5].sprite = uiData.titleSprite;
        


    }
    public void SwapDifficultySprite()
    {
        GameModeUIData uiData = GameManager.instance.currentData;
        int difficultyIndex = (int)GameManager.instance.difficultyMode;
        //숫자미로 설명이 난이도별로 다르기때문에 이때 sprite를 바꿔줘야함
        if (uiData.descriptionSprites.Length == 1)
        {
            targetImages[6].sprite = uiData.descriptionSprites[0];
            targetImages[6].SetNativeSize();
            targetImages[10].sprite = uiData.descriptionSprites[0];
            targetImages[10].SetNativeSize();
        }
        else
        {
            targetImages[6].sprite = uiData.descriptionSprites[difficultyIndex];
            targetImages[6].SetNativeSize();
            targetImages[10].sprite = uiData.descriptionSprites[difficultyIndex];
            targetImages[10].SetNativeSize();
        }
        targetImages[7].sprite = difficultUIs[difficultyIndex];
        targetImages[8].sprite = difficultUIs[difficultyIndex];
        targetImages[9].sprite = uiData.mapSprites[difficultyIndex];
    }
    public void SwapResultSprite(bool isClear)
    {
        if (isClear)
        {
            resultImage.sprite = resultSprites[0];
            frameImage.sprite = frameSprites[0];
        }
        else
        {
            resultImage.sprite = resultSprites[1];
            frameImage.sprite = frameSprites[1];

        }
    }
}
