/*
 *  UDPManager;
 *  Edited by Shenzhen Xinwanhudong Co., Ltd.
 *  http://www.sznplay.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class UDPManager {

    UDPPress udppress = new UDPPress();

    public List<XTouch> xtouch = new List<XTouch>();
    public List<XCircle> xcircle = new List<XCircle>();
    public List<XPolygon> xpolygon = new List<XPolygon>();
    public List<XFace> xface = new List<XFace>();
    public List<XLine> xline = new List<XLine>();
    public List<XImage> ximage = new List<XImage>();

    public void InitManager(string bindIP, int port)
    {
        udppress.SocketInit(bindIP, port);
    }

    public bool UpUDP()
    {
        string gd = "";

        if (udppress.GetStr(ref gd) == 0)
        {
            AnalysisText(gd);
            return true;
        }

        return false;
    }

    int AnalysisText(string srcstr)
    {
        int retval = 0;

        string[] strs = srcstr.Split('|');

        Debug.Log(srcstr);

        if (strs.Length > 2)
        {
            if (strs[0] == "NP2" && strs[strs.Length - 1] == "End")
            {
                xtouch.Clear();
                xpolygon.Clear();
                xcircle.Clear();
                xface.Clear();
                xline.Clear();
                ximage.Clear();

                for (int i = 0; i < strs.Length - 2; i++)
                {
                    string[] points_str = strs[i + 1].Split('&');
                    List<Vector2> points = new List<Vector2>();

                    try
                    {
                        for (int j = 0; j < points_str.Length; j++)
                        {
                            string[] xy_str = points_str[j].Split(',');
                            if (xy_str.Length == 2)
                            {
                                float x = float.Parse(xy_str[0]);
                                float y = float.Parse(xy_str[1]);
                                points.Add(new Vector2(x, y));
                            }
                        }
                    }
                    catch
                    {
                        retval = 1;
                    }

                    if (points.Count > 2 && points[0].y == points.Count - 1)
                    {
                        if (points[0].x == 1 && points.Count == 3)
                        {
                            XTouch xt = new XTouch();
                            if (points[1].y == 1)
                            {
                                xt.state = XTouchState.Down;
                            }
                            else if (points[1].y == 1)
                            {
                                xt.state = XTouchState.Move;
                            }
                            else
                            {
                                xt.state = XTouchState.Up;
                            }
                            xt.touchid = (int)points[1].x;
                            xt.position = points[2];
                            xtouch.Add(xt);
                        }
                        else if (points[0].x == 2 && points.Count == 3)
                        {
                            XCircle xc = new XCircle();
                            xc.centre = points[1];
                            xc.radius = points[2].x;
                            xcircle.Add(xc);
                        }
                        else if (points[0].x == 3)
                        {
                            XPolygon xp = new XPolygon();
                            xp.points = new List<Vector2>();
                            for (int m = 1; m < points.Count; m++ )
                            {
                                xp.points.Add(points[m]);
                            }
                            xpolygon.Add(xp);
                        }
                        else if (points[0].x == 4 && points.Count == 3)
                        {
                            XFace xf = new XFace();
                            xf.top_left_corner = points[1];
                            xf.bottom_right_corner = points[2];
                            xface.Add(xf);
                        }
                        else if (points[0].x == 5 && points.Count == 3)
                        {
                            XLine xl = new XLine();
                            xl.p1 = points[1];
                            xl.p2 = points[2];
                            xline.Add(xl);
                        }
                        else if (points[0].x == 6 && points.Count == 7)
                        {
                            XImage xi = new XImage();
                            xi.capindex = (int)points[1].x;
                            xi.imageid = (int)points[1].y;
                            xi.p1 = points[2];
                            xi.p2 = points[3];
                            xi.p3 = points[4];
                            xi.p4 = points[5];
                            xi.p_centre = points[6];
                            ximage.Add(xi);
                        }
                    }
                }
            }
            else
            {
                retval = 2;
            }
        }
        else if (strs.Length ==2)
        {
            if (strs[0] == "NP2" && strs[1] == "End")
            {
                xtouch.Clear();
                xpolygon.Clear();
                xcircle.Clear();
                xface.Clear();
                xline.Clear();
                ximage.Clear();
            }
            else
            {
                retval = 3;
            }
        }
        else
        {
            retval = 4;
        }

        return retval;
    }

    public void ReleaseManager()
    {
        udppress.SocketQuit();
    }
}

public enum XTouchState
{
    Down = 0,
    Move = 1,
    Up = 2
}
public struct XTouch
{
    public int touchid;
    public Vector2 position;
    public XTouchState state;
}
public struct XCircle
{
    public Vector2 centre;
    public float radius;
}
public struct XPolygon
{
    public List<Vector2> points;
}
public struct XFace
{
    public Vector2 top_left_corner;
    public Vector2 bottom_right_corner;
}
public struct XLine
{
    public Vector2 p1;
    public Vector2 p2;
}
public struct XImage
{
    public int capindex;
    public int imageid;
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 p3;
    public Vector2 p4;
    public Vector2 p_centre;
}

public class UDPPress
{
    Socket socket;
    EndPoint serverEnd;

    Thread connectThread;

    byte[] recvData = new byte[5000];
    int recvLen = 0;
    string recvStr;

    string dataStr;
    object dataLock = new object();
    bool hasDataReset = false;

    public void SocketInit(string bindIP, int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        serverEnd = new IPEndPoint(IPAddress.Parse(bindIP), port);
        socket.Bind(serverEnd);
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    public int GetStr(ref string srcstr)
    {
        int retval = 1;

        lock (dataLock)
        {
            if(hasDataReset)
            {
                retval = 0;
                srcstr = dataStr;
                hasDataReset = false;
            }
        }

        return retval;
    }

    void SocketReceive()
    {
        while (true)
        {
            try
            {
                recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);

                break;
            }

            if (recvLen > 0)
            {
                recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);

                Debug.Log(recvStr);

                lock (dataLock)
                {
                    dataStr = recvStr;

                    hasDataReset = true;
                }
            }
        }
    }

    public void SocketQuit()
    {
        if (socket != null)
        {
            socket.Close();
        }          
    }
}

