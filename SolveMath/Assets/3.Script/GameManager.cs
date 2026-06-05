using System.Collections;
using System.Collections.Generic;
using System.Timers;
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
    [SerializeField] private MazeSolutionLineAnimator hintAnimator;
    [SerializeField] public playerMenu playMenu;

    [SerializeField] private GameObject rePlayPanel;
    public UIFlowManager uiFlowManager;
    public GameType gameType;
    public IGameRule gameRule;
    public Difficulty difficultyMode;
    public GameModeUIData[] datas;
    public GameModeUIData currentData;
    public bool startGame;
    public bool Paused =true;
    private int _life = 3;
    public bool hintPlaying = false;

    public int life
    {
        get { return _life; }
        set
        {
            if (_life == value) return;
            _life = value;
            if (_life >= 0)
            {
                sessionManager.LoseLife(_life);
            }
            //if (_life < 1)
            //{
            //    uiFlowManager.hintYesBtn.interactable = false;
            //}
            //else
            //{
            //    uiFlowManager.hintYesBtn.interactable = true;
            //}
        }
    }
    public float time = 300;
    public int hintCount = 0;
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
        if (!startGame) return;
        //if (Paused) return;
        elapsedTime += Time.deltaTime;
        sessionManager.UpdateTimeText(time - elapsedTime);
        //ui타이머 업데이트
        //타이머 0초될시 게임오버
        if (elapsedTime> time)
        {
            ResultGame(false);
            Debug.Log("게임오버");
        }
        else
        {
            
        }
    }
    public void Fail()
    {
        life--;
        Paused = true;
        
        player.ResetPlayerState();
        Debug.Log($"라이프감소 남은라이프 : {life} , 게임 일시정지 시작위치로 돌아가세요");
        if (life < 0)
        {
            // player 정
            ResultGame(false);
            Debug.Log("게임오버");
            return;
        }
        sessionManager.pauseScreen.SetActive(true);
        player.MoveStartPoint();
    }
    public void ResetGame()
    {
        life = 3;
        hintCount = 0;
        elapsedTime = 0;
        Paused = true;
        startGame = false;
        sessionManager.ResetUIState();
        player.startPos = Vector2.zero;
        player.MoveStartPoint();
        player.ResetPlayerState();
        playMenu.CloseMenu();
        playMenu.gameStateTextController.SetGameStateText(GameState.None);
        if (gameRule != null)
        {
            gameRule.ResetData(player.FindStartTF().GetComponent<Collider2D>());
        }
        uiFlowManager.spriteSwaper.floorResultImage.gameObject.SetActive(false);
        //메뉴글씨 초기화
    }
    public void ReStart()
    {
        ResetGame();
        uiFlowManager.JumpScene(4);
        if (hintAnimator != null)
        {
            hintAnimator.Stop();

        }
        uiFlowManager.CloseCountDownPanel();

    }
    public void GameStart()
    {
        ResetGame();
        startGame = true;
        Paused = false;
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
    public void ResultGame(bool isClear)
    {
        startGame = false;
        uiFlowManager.JumpScene(5);
        uiFlowManager.UpdateResultPanel(isClear);
        playMenu.gameStateTextController.SetActiveText(false);
        uiFlowManager.CloseCountDownPanel();
    }
    //게임스타트 (출발위치에 이동후 3초지나서 시작) , 출발위치에 안올라올시 1분뒤 대기화면으로
    //게임시간 진행
    //게임퍼즈 ( 힌트, fail , 오류 등)
    //게임시간정지
    public void Hint()
    {
        if (!startGame || Paused || hintPlaying) return;
        if (life < 1) return;
        life--;
        hintCount++;
        Paused = true;
        float hintSpeed = 0;
        if (hintCount == 1)
        {
            hintSpeed = 0.1f;
        }
        else if (hintCount == 2)
        {
            hintSpeed = 0.5f;
        }
        else if (hintCount == 3)
        {
            hintSpeed = 1f;
        }
        hintAnimator.Play(hintSpeed);
        playMenu.gameStateTextController.gameObject.SetActive(true);
        playMenu.gameStateTextController.SetGameStateText(GameState.Hint);
    }
    public void GiveUp()
    {
        hintAnimator.Play(0.5f,true);
        startGame = false;
        playMenu.gameStateTextController.SetGameStateText(GameState.Correct);
    }
    public void ForcedGameStop()
    {
        startGame = false;
        uiFlowManager.AllDeactiveMap();
        playMenu.gameStateTextController.SetActiveText(false);
        if (hintAnimator != null)
        {
            hintAnimator.Stop();

        }
        uiFlowManager.CloseCountDownPanel();
    }
    public void OnEndHint(bool isGiveUp)
    {
        if (!isGiveUp)
        {
            rePlayPanel.SetActive(true);

        }
        else
        {
            ResultGame(false);
        }
        playMenu.gameStateTextController.SetActiveText(false);
        hintPlaying = false;
        //Paused = false;
    }
}