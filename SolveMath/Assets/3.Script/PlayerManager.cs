using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private CircleMaskManager circleMaskManager;


    // 게임 씬 크기 (월드 좌표계)
    Vector2 targetPos;
    public Vector2 startPos;
    public Vector2 circlePosition;
    private Vector2 mappingPos;
    private Vector2 mappingScale;
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
        Debug.Log(JsonManager.instance);
        mappingPos = JsonManager.instance.gameSettingData.mappingPos;
        mappingScale = JsonManager.instance.gameSettingData.mappingScale;
    }
    public void SetPlayerPosition(Vector2 lidarPos)
    {
        //if (!GameManager.instance.startGame) return;
        // (0~1) → (-worldWidth/2 ~ +worldWidth/2, 0 ~ worldHeight)
        float worldX = ((lidarPos.x - 0.5f)  * mappingScale.x) + mappingPos.x;
        float worldZ = ((lidarPos.y-0.5f)  * mappingScale.y )+ mappingPos.y;
        //플레이어 위치 매핑 크기 , 위치 조정
        // 위치 반영
        circleMaskManager.SetCirclePosition(lidarPos);
        targetPos = new Vector2(worldX, worldZ);

        Debug.Log($"플레이어 좌표 변환: {lidarPos} → {transform.position}");
    }
    
    private void Update()
    {
        //라이더사용시 다시 활성화 todo
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            SetPlayerPosition(new Vector2(0.5f,0.5f));
        }
        if (targetPos == Vector2.zero) return;
        transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
        if(Vector2.Distance(transform.position , targetPos) < 0.001f)
        {
            transform.position = targetPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"트리거ㄴ엔터 {collision.name}");
        //재시작
        if (collision.CompareTag("Start"))
        {
            if (!GameManager.instance.startGame)
            {
                //스타트문구띄우고 도착 시 카운트다운 시작 3초 후 -> 게임스타트
                //GameManager.instance.GameStart();
                Debug.Log("게임 시작");

            }
            if (GameManager.instance.Paused)
            {
                //GameManager.instance.Paused = false;
                GameManager.instance.gameRule.ResetData(collision);
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

    public void MoveStartPoint()
    {
        Debug.Log("플레이어 시작위치로이동");
        if (startPos == Vector2.zero)
        {

            Transform startTf = FindStartTF();
            startPos = startTf.position;
        }

        gameObject.transform.position = startPos;
        Vector2 startPos_m = ConvertPosToMappingPos(startPos);
        circleMaskManager.SetCirclePosition(startPos_m);


    }
    public Transform FindStartTF()
    {
        GameObject mapObject = GameManager.instance.uiFlowManager.GetCurrentMap();
        Transform mapTf = mapObject.transform;
        for (int i = 0; i < mapTf.childCount; i++)
        {

            if(mapTf.GetChild(i).childCount > 0)
            {
                for (int j = 0; j < mapTf.GetChild(i).childCount; j++)
                {
                    if (mapTf.GetChild(i).GetChild(j).gameObject.CompareTag("Start"))
                    {
                        return mapTf.GetChild(i).GetChild(j);
                    }

                }
            }
            else
            {
                if (mapTf.GetChild(i).gameObject.CompareTag("Start"))
                {
                    return mapTf.GetChild(i);
                }
            }
           
        }
        Debug.Log("시작위치찾지못함");
        return null;
    }
    public void ResetPlayerState()
    {
        clearTime = 0;
        isClear = false;
        targetPos = Vector2.zero;

    }
    public Vector2 ConvertPosToMappingPos(Vector2 pos)
    {
        float x = ((pos.x - mappingPos.x) / mappingScale.x) + 0.5f;
        float y = ((pos.y - mappingPos.y) / mappingScale.y) + 0.5f;


        Vector2 convertPos = new Vector2(x, y);
        return convertPos;
    }

}
