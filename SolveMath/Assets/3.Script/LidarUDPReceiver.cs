using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using DG.Tweening.Core.Easing;

[Serializable]
public class LidarData
{
    public int touchId;
    public int stateId;
    public Vector2 pos;
}

public class LidarUDPReceiver : MonoBehaviour
{
    private UdpClient udp;
    private Thread receiveThread;
    private int port = 19872; // InteractiveEngine에서 지정한 포트 번호
    private string latestMessage;

    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private LidarTouchManager lidarTouchManager;
    [SerializeField] private SceneTimer sceneTimer;
    public LidarData[] lidarDatas;
    [SerializeField]
    private playerMenu playerMenu;
    public SpriteRenderer[] debugPoints;
    public GameObject multiDetectPanel;
    private int multiDetectCount = 3;



    void Start()
    {
        udp = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("UDP 수신 시작 (포트 " + port + ")");
        for (int i = 0; i < debugPoints.Length; i++)
        {
            debugPoints[i].enabled = false;
        }
        multiDetectCount = JsonManager.instance.gameSettingData.multiDetectCount;
    }




    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage) )
        {
          

            lidarDatas = Parseessage(latestMessage);
           
            if (lidarDatas.Length != 0)
            {
                //Debug.Log("받은 데이터: " + latestMessage);
                //Debug.Log(riderData[2]);
                //씬타이머 값 초기화 (사람 존재함)
                sceneTimer.isRunning = true;
                sceneTimer.lapseTime = 0;

                //감지되고있는 위치 포인트 오브젝트 업데이트
                DebugDetectPoint();
                if (!GameManager.instance.startGame) return;
                if (lidarDatas.Length >= multiDetectCount )
                {
                    //두명이상 인식시 게임스탑 
                    if (multiDetectPanel.activeSelf == false && !GameManager.instance.Paused )
                    {
                        Debug.Log($"2명이상 인식, 게임중지");
                        multiDetectPanel.SetActive(true);
                        GameManager.instance.Paused = true;
                        return;

                    }

                }
                else
                {
                    if (multiDetectPanel.activeSelf == true)
                    {
                        Debug.Log($"멀티감지 x , 게임재개");
                        multiDetectPanel.SetActive(false);

                    }
                }
                Vector2 sumPos = new Vector2();
                int sumCount = 0;
                foreach (var data in lidarDatas)
                {
                    bool Touchmenu;
                    switch (data.stateId)
                    {
                        case 1:
                            Debug.Log($"터치");
                            bool isClickMenu = lidarTouchManager.Click_RectTransformUtility(data.pos);
                            if (isClickMenu) return;

                            break;
                        case 2:
                            Touchmenu = lidarTouchManager.IsPointerOverMenu(data.pos);
                            if (Touchmenu) return;
                            sumPos += data.pos;
                            sumCount++;
                            //Debug.Log($"슬라이드, 위치 누적: {sumPos} , 더한 위치 : {data.pos}");
                            break;
                        case 3:
                            Touchmenu = lidarTouchManager.IsPointerOverMenu(data.pos);
                            if (Touchmenu) playerMenu.SetStopRotating(false);
                            //lidarTouchManager.ClickScreenbylidar(data.pos);

                            break;
                    }
                }
                if (sumCount == 0 && sumCount >2) return;
                Vector2 midPos = sumPos / sumCount;
                if (!GameManager.instance.Paused && midPos != Vector2.zero)
                {
                    playerManager.SetPlayerPosition(midPos);
                    Debug.Log($"플레이어 위치 업데이트: {midPos} , 나눈 갯수 {lidarDatas.Length}");
                }
              
            }
                latestMessage = null; // 한 번만 찍고 초기화
        }
    }
    private LidarData FindLidarDataByIndex(int index)
    {
        foreach (var lidarData in lidarDatas)
        {
            if(lidarData.touchId == index)
            {
                return lidarData;
            }
            
        }
        return null;
    }
    private void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            byte[] data = udp.Receive(ref remoteEP);
            latestMessage = Encoding.UTF8.GetString(data); // 변수에만 저장
        }
    }
    private void DebugDetectPoint()
    {
        for (int i = 0; i < debugPoints.Length; i++)
        {
            debugPoints[i].enabled = false;
        }
        for (int i = 0; i < lidarDatas.Length; i++)
        {
            debugPoints[i].enabled = true;
            debugPoints[i].transform.position = lidarTouchManager.Map01ToWorldPos(lidarDatas[i].pos);
            //lidarDatas 포지션에 해당하는 곳 디버그
            //위치 이동 , alpha = 0
        }

    }
    private LidarData[] Parseessage(string message)
    {
        // 1. '|' 기준으로 Split
        string[] parts = message.Split('|');

        List<LidarData> vectors = new List<LidarData>();

        foreach (string part in parts)
        {
            if (string.IsNullOrWhiteSpace(part)) continue;
            if (part == "NP2" || part == "End") continue;

            // 2. '&' 기준 Split
            string[] segments = part.Split('&');

            LidarData lidarData = new LidarData();
            string[] xy1 = segments[1].Split(',');
            if (xy1.Length == 2)
            {
                if (float.TryParse(xy1[0], out float x) &&
                    float.TryParse(xy1[1], out float y))
                {

                    lidarData.touchId = (int)x;
                    lidarData.stateId = (int)y;
                }
            }
            string[] xy2 = segments[2].Split(',');
            if (xy2.Length == 2)
            {
                if (float.TryParse(xy2[0], out float x) &&
                    float.TryParse(xy2[1], out float y))
                {

                    lidarData.pos = new Vector2(x, y);
                }
            }
            vectors.Add(lidarData);
        }

        // 3. Vector2[3] 리턴 (길이가 다르면 가능한 만큼만 채움)
        return vectors.ToArray();
    }
    private LidarData FindTargetPos(int id)
    {
        foreach (var data in lidarDatas)
        {
            if (data.touchId == id)
            {
                return data;
            }

        }
        return null;
    }
    private void OnApplicationQuit()
    {
        if (receiveThread != null) receiveThread.Abort();
        if (udp != null) udp.Close();
    }
}
