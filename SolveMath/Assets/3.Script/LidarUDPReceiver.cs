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
    private int port = 19872; // InteractiveEngine���� ������ ��Ʈ ��ȣ
    private string latestMessage;

    public Vector2[] riderData;
    [SerializeField] private PlayerManager playerManager;
    void Start()
    {
        udp = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("UDP ���� ���� (��Ʈ " + port + ")");
    }


  

    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage))
        {
            Debug.Log("���� ������: " + latestMessage);
            riderData = Parseessage(latestMessage);
            if(riderData.Length != 0)
            {
                Debug.Log(riderData[2]);
                playerManager.SetPlayerPosition(riderData[2]);
                //�θ��̻� �νĽ� ���ӽ�ž

            }
            latestMessage = null; // �� ���� ��� �ʱ�ȭ
        }
    }
    private void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            byte[] data = udp.Receive(ref remoteEP);
            latestMessage = Encoding.UTF8.GetString(data); // �������� ����
        }
    }

    private Vector2[] Parseessage(string message)
    {
        // 1. '|' �������� Split
        string[] parts = message.Split('|');

        List<Vector2> vectors = new List<Vector2>();

        foreach (string part in parts)
        {
            if (string.IsNullOrWhiteSpace(part)) continue;
            if (part == "NP2" || part == "End") continue;

            // 2. '&' ���� Split
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

        // 3. Vector2[3] ���� (���̰� �ٸ��� ������ ��ŭ�� ä��)
        return vectors.ToArray();
    }

    private void OnApplicationQuit()
    {
        if (receiveThread != null) receiveThread.Abort();
        if (udp != null) udp.Close();
    }
}
