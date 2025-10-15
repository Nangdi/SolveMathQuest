using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameType
{
    NoLeftTurn,
    Number,
    Jump,
    Arrow,
    ShortestPath
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameType gameType;
    public IGameRule gameRule;
    public Difficulty difficultyMode;
    public bool startGame;
    public bool Paused;
    public int life = 3;
    public float time;
    public float elapsedTime;

    

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }
    private void Start()
    {
       
    }
    public void Fail()
    {
        life--;
        Paused = true;
        Debug.Log($"라이프감소 남은라이프 : {life} , 게임 일시정지 시작위치로 돌아가세요");

        if (life < 1)
        {
            // TODO: 게임오버 처리
            Debug.Log("게임오버");
        }
    }
    public void SetGameRule(GameType newGame)
    {
        this.gameType = newGame;

        switch (newGame)
        {
            case GameType.NoLeftTurn:
                gameRule = new Rule_NoLeftTurn();
                break;
            case GameType.Number:
                gameRule = new Rule_Number();
                break;
            case GameType.Jump:
                gameRule = new Rule_Jump();
                break;
            case GameType.Arrow:
                gameRule = new Rule_Arrow();
                break;
            case GameType.ShortestPath:
                gameRule = new Rule_ShortestPath();
                break;
        }
    }
}