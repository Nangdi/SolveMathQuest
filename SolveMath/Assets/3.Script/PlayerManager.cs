using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    // ���� �� ũ�� (���� ��ǥ��)
    public float worldWidth = 5.6f;   // X�� ũ��
    public float worldHeight = 5.6f;  // Z�� ũ��
    Vector2 targetPos;
    // ���̴ٿ��� ���� (0~1) ��ǥ�� �Է�
    public void SetPlayerPosition(Vector2 lidarPos)
    {
        // (0~1) �� (-worldWidth/2 ~ +worldWidth/2, 0 ~ worldHeight)
        float worldX = (lidarPos.x - 0.5f) * worldWidth;
        float worldZ = (lidarPos.y-0.5f) * worldHeight;

        // ��ġ �ݿ�
        targetPos = new Vector2(worldX, worldZ);
        

        Debug.Log($"�÷��̾� ��ǥ ��ȯ: {lidarPos} �� {transform.position}");
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
                //���ӿ���
            }
            //���Ӷ����� -1
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
