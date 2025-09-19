using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;

public class pc : MonoBehaviour {

    UDPManager um = new UDPManager();

    public GameObject lr_pre;
    public GameObject circle_pre;

    public List<LineRenderer> line_list;
    public List<GameObject> ircle_list;

	void Start () {
        List<string> srciplist = new List<string>();
        IPAddress[] ips = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());  //绑定本机所有IP地址
        for (int i = 0; i < ips.Length; i++)                                              //设定域名
        {
            if (ips[i].AddressFamily == AddressFamily.InterNetwork) //IPv4地址 
            {
                srciplist.Add(ips[i].ToString());
            }
        }

        if (srciplist.Count > 0)
        {
            um.InitManager(srciplist[0], 1984);//本机可以用127.0.0.1
        }

        for (int i = 0; i < 10; i++ )
        {
            line_list.Add(Instantiate(lr_pre).GetComponent<LineRenderer>());
            ircle_list.Add(Instantiate(circle_pre));
        }
        for (int i = 0; i < 10; i++)
        {
            line_list[i].material.color = Color.red;
            line_list[i].startColor = Color.red;
            line_list[i].endColor = Color.red;
            line_list[i].startWidth = 0.2f;
            line_list[i].endWidth = 0.2f;
        }
	}
	
	void Update () {
        if (um.UpUDP())
        {
            Debug.Log("touch num:" + um.xtouch.Count + "...circle num:" + um.xcircle.Count + "...polygon num:" + um.xpolygon.Count + "...face num:" + um.xface.Count);

            for (int i = 0; i < 10; i++)
            {
                ircle_list[i].SetActive(false);
            }
            if (um.xcircle.Count > 0)
            {
                //point_rect[point_color.Count - 1].anchoredPosition = new Vector2(um.xcircle[0].centre.x * Screen.width, Screen.height - um.xcircle[0].centre.y * Screen.height);
                //point_color[point_color.Count - 1].color = new Color32(0, 255, 0, 255);
                //point_rect[point_color.Count - 1].gameObject.SetActive(true);
                for (int i = 0; i < um.xcircle.Count; i++)
                {
                    if (i >= 10)
                    {
                        break;
                    }
                    ircle_list[i].SetActive(true);
                    ircle_list[i].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(um.xcircle[i].centre.x * Screen.width, Screen.height - um.xcircle[i].centre.y * Screen.height, 5));
                }
            }

            for (int i = 0; i < 10; i++)
            {
                line_list[i].positionCount = 0;
            }

            if (um.xpolygon.Count > 0)
            {
                for(int i =0; i < um.xpolygon.Count; i++)
                {
                    if(i >= 10)
                    {
                        break;
                    }
                    line_list[i].positionCount = um.xpolygon[i].points.Count + 1;
                    for (int m = 0; m < um.xpolygon[i].points.Count; m++)
                    {
                        line_list[i].SetPosition(m, Camera.main.ScreenToWorldPoint(new Vector3(um.xpolygon[i].points[m].x * Screen.width, Screen.height - um.xpolygon[i].points[m].y * Screen.height, 5)));
                    }
                    line_list[i].SetPosition(um.xpolygon[i].points.Count, Camera.main.ScreenToWorldPoint(new Vector3(um.xpolygon[i].points[0].x * Screen.width, Screen.height - um.xpolygon[i].points[0].y * Screen.height, 5)));
                }
            }
        }
	}


    void OnApplicationQuit()
    {
        um.ReleaseManager();
    }
}
