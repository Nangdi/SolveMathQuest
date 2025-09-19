/*
 *  UDPManager;
 *  Edited by Shenzhen Xinwanhudong Co., Ltd.
 *  http://www.sznplay.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net.Sockets;
using System.Net;

public class UDPTest : MonoBehaviour {

    UDPManager um = new UDPManager();

    public GameObject point_parent;
    public GameObject point_pre;
    List<RectTransform> point_rect = new List<RectTransform>();
    List<RawImage> point_color = new List<RawImage>();
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
            um.InitManager(srciplist[0], 19872);//本机可以用127.0.0.1
        }

        //新建几个物体来显示坐标点
        for(int i = 0; i < 30; i++)
        {
            GameObject gg = Instantiate(point_pre, point_parent.transform);
            gg.SetActive(false);
            point_rect.Add(gg.GetComponent<RectTransform>());
            point_color.Add(gg.GetComponent<RawImage>());
        }
	}
	
	void Update () {

        if(um.UpUDP())
        {
            Debug.Log("touch num:" + um.xtouch.Count + "...circle num:" + um.xcircle.Count + "...polygon num:" + um.xpolygon.Count + "...face num:" + um.xface.Count);

            //for (int i = 0; i < point_rect.Count; i++)
            //{
            //    if (i < um.xtouch.Count)
            //    {
            //        point_rect[i].anchoredPosition = new Vector2(um.xtouch[i].position.x * Screen.width, Screen.height - um.xtouch[i].position.y * Screen.height);
            //        point_rect[i].gameObject.SetActive(true);
            //        point_color[i].color = new Color32(255, 0, 0, 255);
            //    }
            //    else
            //    {
            //        point_rect[i].gameObject.SetActive(false);
            //    }
            //}
            for (int i = 0; i < um.xtouch.Count; i++)
            {
                point_rect[i].anchoredPosition =
                    new Vector2(um.xtouch[i].position.x * 500,
                                um.xtouch[i].position.y * 500);
                point_rect[i].gameObject.SetActive(true);
            }

            if (um.xcircle.Count > 0)
            {
                point_rect[point_color.Count - 1].anchoredPosition = new Vector2(um.xcircle[0].centre.x * Screen.width, Screen.height - um.xcircle[0].centre.y * Screen.height);
                point_color[point_color.Count - 1].color = new Color32(0, 255, 0, 255);
                point_rect[point_color.Count - 1].gameObject.SetActive(true);
            }

            if (um.xface.Count > 0)
            {
                point_rect[point_color.Count - 1].anchoredPosition = new Vector2(um.xface[0].top_left_corner.x * Screen.width, Screen.height - um.xface[0].top_left_corner.y * Screen.height);
                point_color[point_color.Count - 1].color = new Color32(0, 0, 255, 255);
                point_rect[point_color.Count - 1].gameObject.SetActive(true);

                point_rect[point_color.Count - 2].anchoredPosition = new Vector2(um.xface[0].bottom_right_corner.x * Screen.width, Screen.height - um.xface[0].bottom_right_corner.y * Screen.height);
                point_color[point_color.Count - 2].color = new Color32(0, 0, 255, 255);
                point_rect[point_color.Count - 2].gameObject.SetActive(true);
            }

            if (um.xpolygon.Count > 0)
            {
                for (int m = 0; m < um.xpolygon[0].points.Count; m++ )
                {
                    point_rect[point_color.Count - 1 - m].anchoredPosition = new Vector2(um.xpolygon[0].points[m].x * Screen.width, Screen.height - um.xpolygon[0].points[m].y * Screen.height);
                    point_color[point_color.Count - 1 - m].color = new Color32(0, 255, 255, 255);
                    point_rect[point_color.Count - 1 - m].gameObject.SetActive(true);
                }
            }

            if (um.xline.Count > 0)
            {
                point_rect[point_color.Count - 1].anchoredPosition = new Vector2(um.xline[0].p1.x * Screen.width, Screen.height - um.xline[0].p1.y * Screen.height);
                point_color[point_color.Count - 1].color = new Color32(0, 0, 255, 255);
                point_rect[point_color.Count - 1].gameObject.SetActive(true);

                point_rect[point_color.Count - 2].anchoredPosition = new Vector2(um.xline[0].p2.x * Screen.width, Screen.height - um.xline[0].p2.y * Screen.height);
                point_color[point_color.Count - 2].color = new Color32(0, 0, 255, 255);
                point_rect[point_color.Count - 2].gameObject.SetActive(true);
            }

            if(um.ximage.Count > 0)
            {
                Debug.Log(um.ximage[0].capindex + ",,," + um.ximage[0].imageid);

                for (int i = 0; i < point_rect.Count; i++ )
                {
                    point_rect[i].gameObject.SetActive(false);
                }

                point_rect[0].anchoredPosition = new Vector2(um.ximage[0].p1.x * Screen.width, Screen.height - um.ximage[0].p1.y * Screen.height);
                point_rect[0].gameObject.SetActive(true);
                point_color[0].color = new Color32(255, 0, 0, 255);

                point_rect[1].anchoredPosition = new Vector2(um.ximage[0].p2.x * Screen.width, Screen.height - um.ximage[0].p2.y * Screen.height);
                point_rect[1].gameObject.SetActive(true);
                point_color[1].color = new Color32(255, 0, 0, 255);

                point_rect[2].anchoredPosition = new Vector2(um.ximage[0].p3.x * Screen.width, Screen.height - um.ximage[0].p3.y * Screen.height);
                point_rect[2].gameObject.SetActive(true);
                point_color[2].color = new Color32(255, 0, 0, 255);

                point_rect[3].anchoredPosition = new Vector2(um.ximage[0].p4.x * Screen.width, Screen.height - um.ximage[0].p4.y * Screen.height);
                point_rect[3].gameObject.SetActive(true);
                point_color[3].color = new Color32(255, 0, 0, 255);
            }
        }
	}



    void OnApplicationQuit()
    {
        um.ReleaseManager();
    }
}
