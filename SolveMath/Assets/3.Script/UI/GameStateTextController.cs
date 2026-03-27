using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState
{
    None,
    Menu,
    Correct,
    Hint
}
public class GameStateTextController : MonoBehaviour
{
    public GameState gameState = GameState.None;
    string menuString = "메뉴를 선택 중입니다.. ";
    string correctString = "정답을 확인 중입니다.. ";
    string HintString = "힌트를 확인 중입니다.. ";

    [SerializeField]
    private TMP_Text gameStateText;

    private void Start()
    {
        SetActiveText(false);
    }

    public void SetActiveText(bool active)
    {
        gameStateText.gameObject.SetActive(active);
    }
    public void SetGameStateText(GameState state)
    {
        gameState = state;
        switch (gameState)
        {
            //메뉴창 오픈시
            case GameState.Menu:
                gameStateText.text = menuString;
                break;
            //정답 확인시
            case GameState.Correct:
                gameStateText.text = correctString;
                break;
            //힌트 확인시
            case GameState.Hint:
                gameStateText.text = HintString;
                break;
            default:
                break;
        }
    }
}
