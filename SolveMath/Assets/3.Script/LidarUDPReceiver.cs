using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using System;

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

    public Vector2[] riderData;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private LidarTouchManager lidarTouchManager;
    public LidarData[] lidarDatas;
    [SerializeField]
    private playerMenu playerMenu;
    void Start()
    {
        udp = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("UDP 수신 시작 (포트 " + port + ")");
    }




    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage))
        {
            lidarDatas = Parseessage(latestMessage);
            if (lidarDatas.Length != 0)
            {
                //Debug.Log("받은 데이터: " + latestMessage);
                //Debug.Log(riderData[2]);
                LidarData firstData = FindTargetPos(0);
                LidarData secondData = FindTargetPos(1);
                if (firstData != null && (!GameManager.instance.startGame || GameManager.instance.Paused))
                {
                    playerManager.SetPlayerPosition(firstData.pos);
                }
                if (secondData != null)
                {
                    if (secondData.stateId == 2)
                    {
                        bool Touchmenu = lidarTouchManager.CheckHit(secondData.pos);
                        if (Touchmenu)
                        {
                            playerMenu.SetRotating(false);
                        }
                        else
                        {
                            playerMenu.SetRotating(true);
                        }
                        

                    }
                }
                if (lidarDatas.Length > 3)
                {
                    //두명이상 인식시 게임스탑 todo
                }

            }
            latestMessage = null; // 한 번만 찍고 초기화
        }
        else
        {
            //Debug.Log($"아무도 안들어온 상태");
            //여기서 타이머 작동 일정시간 경과 시 -> 대기화면
            //
        }
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
            if (data.touchId == id )
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
