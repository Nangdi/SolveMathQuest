using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    // 게임 씬 크기 (월드 좌표계)
    public float worldWidth = 5.6f;   // X축 크기
    public float worldHeight = 5.6f;  // Z축 크기
    Vector2 targetPos;
    public Vector2 currentPos;
    public Vector2 previousPos;
    public Vector2 backPos;

    public int headDir;
    // 라이다에서 받은 (0~1) 좌표값 입력
    private void Start()
    {
    }
    public void SetPlayerPosition(Vector2 lidarPos)
    {
        // (0~1) → (-worldWidth/2 ~ +worldWidth/2, 0 ~ worldHeight)
        float worldX = (lidarPos.x - 0.5f) * worldWidth;
        float worldZ = (lidarPos.y-0.5f) * worldHeight;

        // 위치 반영
        targetPos = new Vector2(worldX, worldZ);
        

        //Debug.Log($"플레이어 좌표 변환: {lidarPos} → {transform.position}");
    }
    private void Update()
    {
        //transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime*3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"트리거엔터 {collision.name}");
        if (collision.CompareTag("Start"))
        {
            if (!GameManager.instance.startGame)
            {
                ResetData(collision);
                GameManager.instance.startGame = true;
                Debug.Log("게임시작");
            }else if (GameManager.instance.Paused) 
            {
                
                GameManager.instance.Paused = false;
                ResetData(collision);
                Debug.Log("게임 재시작");
            }
            return;
        }
        if (!GameManager.instance.startGame || GameManager.instance.Paused) return;

        if (collision.CompareTag("Wall"))
        {
            Debug.Log(collision.name);
            Fail();
            if (GameManager.instance.life < 1)
            {
                //게임오버
            }
            //게임라이프 -1
        }
        //왔던길 저장
        //도착한곳 갱신
        Vector2 newPos = collision.transform.position;




        Vector2 prevDir = (currentPos - previousPos).normalized;  // 이전 방향
        Vector2 currDir = (newPos - currentPos).normalized;       // 현재 방향

        float cross = prevDir.x * currDir.y - prevDir.y * currDir.x; // 외적
        float dot = Vector2.Dot(prevDir, currDir);                  // 내적

        if (dot < -0.7f)  // 거의 반대 → 후진
        {
            Fail();
            Debug.Log("후진");
        }
        else if (cross > 0.3f) // 좌회전
        {
            Fail();
            Debug.Log("좌회전");
        }
        else if (cross < -0.3f) // 우회전
        {
            Debug.Log("우회전 - 통과");
            headDir += 1;
            headDir %= 4;
        }
        else // 직진
        {
            Debug.Log("직진 - 통과");
        }

        // 위치 갱신
        previousPos = currentPos;
        currentPos = newPos;

    }

    private void Fail()
    {
        GameManager.instance.life--;
        GameManager.instance.Paused = true;
        Debug.Log($"라이프감소 남은라이프 : {GameManager.instance.life} , 게임 일시정지 시작위치로 돌아가세요");

        if (GameManager.instance.life < 1)
        {
            // TODO: 게임오버 처리
            Debug.Log("게임오버");
        }
        //진행방향계산. 우회전,직진 -통과
        //            좌회전,후진 -Life -1


    }
    private void ResetData(Collider2D collision)
    {
        {
            currentPos = collision.transform.position;
            Debug.Log($"지금위치 {currentPos}");
            previousPos = currentPos;
            Debug.Log($"이전위치 {previousPos}");
            headDir = 0;
        }
    }
}
