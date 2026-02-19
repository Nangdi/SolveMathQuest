using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor.Searcher;
using UnityEngine;
public enum GameType
{
    NoLeftTurn,
    Jump,
    Number,
    Arrow,
    ShortestPath
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameSessionManager sessionManager;
    public UIFlowManager uiFlowManager;
    public GameType gameType;
    public IGameRule gameRule;
    public Difficulty difficultyMode;
    public GameModeUIData[] datas;
    public GameModeUIData currentData;
    public bool startGame;
    public bool Paused;
    public int life = 3;
    public float time= 300;
    public float elapsedTime;
    public int useHintNum =0;

    public PlayerManager player;


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

        GameObject.FindGameObjectWithTag("Player").TryGetComponent(out player);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameStart();
        }
        if (!startGame) return;
        if (Paused) return;
        elapsedTime += Time.deltaTime;
        sessionManager.UpdateTimeText(time - elapsedTime);
        //ui타이머 업데이트
        //타이머 0초될시 게임오버
        if (elapsedTime> time)
        {
            ResultGame(false);
            Debug.Log("게임오버");
        }
    }
    public void Fail()
    {
        life--;
        Paused = true;
        sessionManager.pauseScreen.SetActive(true);
        sessionManager.LoseLife(life);
        player.MoveStartPoint();
        Debug.Log($"라이프감소 남은라이프 : {life} , 게임 일시정지 시작위치로 돌아가세요");
        if (life < 1)
        {
            // TODO: 게임오버 처리
            // player 정
            ResultGame(false);
            Debug.Log("게임오버");
        }
    }
    public void ResetGame()
    {
        life = 3;
        elapsedTime = 0;
        Paused = false;
        startGame = false;
        player.startPos = Vector2.zero;
        sessionManager.ResetUIState();
    }
    public void ReStart()
    {
        ResetGame();
        uiFlowManager.ChangeScene(-1);
    }
    public void GameStart()
    {
        ResetGame();
        startGame = true;
        Debug.Log("게임스타트");
    }
    public void Clear()
    {
        ResultGame(true);
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
    public void SetDifficulty(int index)
    {
        //0 : eazy , 1 : nomal , 2 : hard
        difficultyMode = (Difficulty)index;
        gameRule.SetDifficultMode(difficultyMode);
    }
    public void SetGameMode(int index)
    {
        gameType = (GameType)index;
        SetGameRule(gameType);


        currentData = datas[index];
        //모드 선택시 스프라이트 교체
    }
    private void ResultGame(bool isClear)
    {
        startGame = false;
        uiFlowManager.ChangeScene();
        uiFlowManager.UpdateResultPanel(isClear);
    }
    //게임스타트 (출발위치에 이동후 3초지나서 시작) , 출발위치에 안올라올시 1분뒤 대기화면으로
    //게임시간 진행
    //게임퍼즈 ( 힌트, fail , 오류 등)
    //게임시간정지

}