using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    // 게임 씬 크기 (월드 좌표계)
    public readonly float worldWidth = 5.6f;   // X축 크기
    public readonly float worldHeight = 5.6f;  // Z축 크기
    Vector2 targetPos;

    [Header("PlayerInfo")]
    public string playerName;
    public float clearTime;
    public bool isClear = false;    

    private void Awake()
    {
     
        
    }

    // 라이다에서 받은 (0~1) 좌표값 입력
    private void Start()
    {
    }
    public void SetPlayerPosition(Vector2 lidarPos)
    {
        if (!GameManager.instance.startGame) return;
        // (0~1) → (-worldWidth/2 ~ +worldWidth/2, 0 ~ worldHeight)
        float worldX = (lidarPos.x - 0.5f) * worldWidth;
        float worldZ = (lidarPos.y-0.5f) * worldHeight;

        // 위치 반영
        targetPos = new Vector2(worldX, worldZ);
        

        //Debug.Log($"플레이어 좌표 변환: {lidarPos} → {transform.position}");
    }
    
    private void Update()
    {
        //라이더사용시 다시 활성화 todo
        //transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime*3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"트리거ㄴ엔터 {collision.name}");
        //재시작
        if (collision.CompareTag("Start"))
        {
            if (!GameManager.instance.startGame)
            {
                GameManager.instance.gameRule.ResetData(collision);
                //스타트문구띄우고 도착 시 카운트다운 시작 3초 후 -> 게임스타트
                
                GameManager.instance.GameStart();
            }
            if (GameManager.instance.Paused)
            {
                GameManager.instance.Paused = false;
                Debug.Log("게임 재시작");
                return;
            }
            //재시작은 현재위치에서 해야함 /todo
            //Debug.Log("아무일도없음");
            return;



        }
        if (!GameManager.instance.startGame || GameManager.instance.Paused) return;


        if (GameManager.instance.gameRule.isRuleViolated(collision))
        {
            GameManager.instance.Fail();
        }else if (GameManager.instance.gameRule.GameClear(collision))
        {
            //게임매니저 게임클리어 로직 todo
            //성공시 성공화면 전환
            
            Debug.Log("게임클리어!");
            isClear = true;
            clearTime = GameManager.instance.elapsedTime;
            GameManager.instance.Clear();

        }


    }

   
   
    
}
