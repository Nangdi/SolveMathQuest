using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    // 게임 씬 크기 (월드 좌표계)
    public float worldWidth = 5.6f;   // X축 크기
    public float worldHeight = 5.6f;  // Z축 크기
    Vector2 targetPos;
    // 라이다에서 받은 (0~1) 좌표값 입력
    public void SetPlayerPosition(Vector2 lidarPos)
    {
        // (0~1) → (-worldWidth/2 ~ +worldWidth/2, 0 ~ worldHeight)
        float worldX = (lidarPos.x - 0.5f) * worldWidth;
        float worldZ = (lidarPos.y-0.5f) * worldHeight;

        // 위치 반영
        targetPos = new Vector2(worldX, worldZ);
        

        Debug.Log($"플레이어 좌표 변환: {lidarPos} → {transform.position}");
    }
    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime*3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            if (GameManager.instance.Paused) return;



            GameManager.instance.life--;
            GameManager.instance.Paused = true;
            if (GameManager.instance.life < 1)
            {
                //게임오버
            }
            //게임라이프 -1
        }
        if (collision.CompareTag("Start"))
        {
            if (!GameManager.instance.startGame)
            {
                GameManager.instance.startGame = true;
            }else if (GameManager.instance.Paused)
            {
                GameManager.instance.Paused = false;
            }
            
        }

    }
}
