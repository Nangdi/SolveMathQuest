using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEditor.VersionControl;

public class LidarUDPReceiver : MonoBehaviour
{
    private UdpClient udp;
    private Thread receiveThread;
    private int port = 19872; // InteractiveEngine에서 지정한 포트 번호
    private string latestMessage;

    public Vector2[] riderData;
    [SerializeField] private PlayerManager playerManager;
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
            Debug.Log("받은 데이터: " + latestMessage);
            riderData = Parseessage(latestMessage);
            if(riderData.Length != 0)
            {
                Debug.Log(riderData[2]);
                playerManager.SetPlayerPosition(riderData[2]);
                //두명이상 인식시 게임스탑

            }
            latestMessage = null; // 한 번만 찍고 초기화
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

    private Vector2[] Parseessage(string message)
    {
        // 1. '|' 기준으로 Split
        string[] parts = message.Split('|');

        List<Vector2> vectors = new List<Vector2>();

        foreach (string part in parts)
        {
            if (string.IsNullOrWhiteSpace(part)) continue;
            if (part == "NP2" || part == "End") continue;

            // 2. '&' 기준 Split
            string[] segments = part.Split('&');

            foreach (string seg in segments)
            {
                string[] xy = seg.Split(',');
                if (xy.Length == 2)
                {
                    if (float.TryParse(xy[0], out float x) &&
                        float.TryParse(xy[1], out float y))
                    {
                        vectors.Add(new Vector2(x, y));
                    }
                }
            }
        }

        // 3. Vector2[3] 리턴 (길이가 다르면 가능한 만큼만 채움)
        return vectors.ToArray();
    }

    private void OnApplicationQuit()
    {
        if (receiveThread != null) receiveThread.Abort();
        if (udp != null) udp.Close();
    }
}
