using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    // ���� �� ũ�� (���� ��ǥ��)
    public float worldWidth = 5.6f;   // X�� ũ��
    public float worldHeight = 5.6f;  // Z�� ũ��
    Vector2 targetPos;
    public Vector2 currentPos;
    public Vector2 previousPos;
    public Vector2 backPos;

    public int headDir;
    // ���̴ٿ��� ���� (0~1) ��ǥ�� �Է�
    private void Start()
    {
    }
    public void SetPlayerPosition(Vector2 lidarPos)
    {
        // (0~1) �� (-worldWidth/2 ~ +worldWidth/2, 0 ~ worldHeight)
        float worldX = (lidarPos.x - 0.5f) * worldWidth;
        float worldZ = (lidarPos.y-0.5f) * worldHeight;

        // ��ġ �ݿ�
        targetPos = new Vector2(worldX, worldZ);
        

        //Debug.Log($"�÷��̾� ��ǥ ��ȯ: {lidarPos} �� {transform.position}");
    }
    private void Update()
    {
        //transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime*3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"Ʈ���ſ��� {collision.name}");
        if (collision.CompareTag("Start"))
        {
            if (!GameManager.instance.startGame)
            {
                ResetData(collision);
                GameManager.instance.startGame = true;
                Debug.Log("���ӽ���");
            }else if (GameManager.instance.Paused) 
            {
                
                GameManager.instance.Paused = false;
                ResetData(collision);
                Debug.Log("���� �����");
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
                //���ӿ���
            }
            //���Ӷ����� -1
        }
        //�Դ��� ����
        //�����Ѱ� ����
        Vector2 newPos = collision.transform.position;




        Vector2 prevDir = (currentPos - previousPos).normalized;  // ���� ����
        Vector2 currDir = (newPos - currentPos).normalized;       // ���� ����

        float cross = prevDir.x * currDir.y - prevDir.y * currDir.x; // ����
        float dot = Vector2.Dot(prevDir, currDir);                  // ����

        if (dot < -0.7f)  // ���� �ݴ� �� ����
        {
            Fail();
            Debug.Log("����");
        }
        else if (cross > 0.3f) // ��ȸ��
        {
            Fail();
            Debug.Log("��ȸ��");
        }
        else if (cross < -0.3f) // ��ȸ��
        {
            Debug.Log("��ȸ�� - ���");
            headDir += 1;
            headDir %= 4;
        }
        else // ����
        {
            Debug.Log("���� - ���");
        }

        // ��ġ ����
        previousPos = currentPos;
        currentPos = newPos;

    }

    private void Fail()
    {
        GameManager.instance.life--;
        GameManager.instance.Paused = true;
        Debug.Log($"���������� ���������� : {GameManager.instance.life} , ���� �Ͻ����� ������ġ�� ���ư�����");

        if (GameManager.instance.life < 1)
        {
            // TODO: ���ӿ��� ó��
            Debug.Log("���ӿ���");
        }
        //���������. ��ȸ��,���� -���
        //            ��ȸ��,���� -Life -1


    }
    private void ResetData(Collider2D collision)
    {
        {
            currentPos = collision.transform.position;
            Debug.Log($"������ġ {currentPos}");
            previousPos = currentPos;
            Debug.Log($"������ġ {previousPos}");
            headDir = 0;
        }
    }
}
