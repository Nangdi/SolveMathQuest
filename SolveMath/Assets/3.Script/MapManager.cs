using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CorrectPath
{
    public Transform[] path;
}


public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public GameObject[] arrowMap_1;
    public GameObject[] arrowMap_2;
    public GameObject[] arrowMap_3;
    public GameObject[] jumpMap_1;
    public GameObject[] jumpMap_2;
    public GameObject[] jumpMap_3;
    public GameObject[] numMap_1;
    public GameObject[] numMap_2;
    public GameObject[] numMap_3;
    public Collider2D[,] colliderGrid;  // 실제 씬에 있는 타일 Collider 저장
    public Dictionary<Collider2D, Vector2Int> colliderToIndex; // 매핑 테이블
    public Dictionary<GameType, List<CorrectPath>> typeToPathDic = new Dictionary<GameType, List<CorrectPath>>(); // 매핑 테이블
    public ArrowDir[,] arrowDirs1 = new ArrowDir[3, 3];
    public ArrowDir[,] arrowDirs2 = new ArrowDir[5, 5];
    public ArrowDir[,] arrowDirs3 ;
    public int[,] jumpCount1;
    public int[,] jumpCount2;
    public int[,] jumpCount3;
    public int[,] num1;
    public int[,] num2;
    public int[,] num3;

    public Dictionary<string, List<(string dest, int time)>> graph1;
    public Dictionary<string, List<(string dest, int time)>> graph2;
    public Dictionary<string, List<(string dest, int time)>> graph3;

    public List<CorrectPath> leftCorrectPath;
    public List<CorrectPath> numCorrectPath;
    public List<CorrectPath> jumpCorrectPath;
    public List<CorrectPath> arrowCorrectPath;
    public List<CorrectPath> shortestCorrectPath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        colliderToIndex = new Dictionary<Collider2D, Vector2Int>();
        InitArrow();
        InitJumpCount();
        InitNum();
        InitPath();

    }
    private void Start()
    {
        SetDic(arrowMap_1);
        SetDic(arrowMap_2);
        SetDic(arrowMap_3);
        SetDic(jumpMap_1);
        SetDic(jumpMap_2);
        SetDic(jumpMap_3);
        SetDic(numMap_1);
        SetDic(numMap_2);
        SetDic(numMap_3);
        typeToPathDic[GameType.Arrow] = arrowCorrectPath;
        typeToPathDic[GameType.Number] = numCorrectPath;
        typeToPathDic[GameType.ShortestPath] = shortestCorrectPath;
        typeToPathDic[GameType.NoLeftTurn] = leftCorrectPath;
        typeToPathDic[GameType.Jump] = jumpCorrectPath;
    }
    public void SetDic(GameObject[] map)
    {
        for (int i = 0; i < map.Length; i++)
        {
            Transform parent = map[i].transform;

            for (int k = 0; k < parent.childCount; k++)
            {
                Collider2D col = parent.GetChild(k).GetComponent<Collider2D>();
                if (col == null)
                {
                    Debug.LogWarning($"[{parent.GetChild(k).name}] Collider2D 없음 — 무시됨");
                    continue;
                }


                colliderToIndex.Add(col, new Vector2Int(k, i));
            }
        }

        Debug.Log($"Collider Dictionary 등록 완료 ({colliderToIndex.Count}개)");
    }
    public Transform[] GetCorrectPath(GameType type , Difficulty difficulty)
    {
        return typeToPathDic[type][(int)difficulty].path;
    }

    private void InitArrow()
    {
        arrowDirs1 = new ArrowDir[,] {
            { ArrowDir.UpRight, ArrowDir.Up, ArrowDir.Left},
            { ArrowDir.Up, ArrowDir.DownRight, ArrowDir.Left},
            { ArrowDir.None, ArrowDir.Right, ArrowDir.Down},
        };
        arrowDirs2 = new ArrowDir[,] {
            { ArrowDir.UpRight, ArrowDir.Up, ArrowDir.DownRight, ArrowDir.DownLeft, ArrowDir.Left },
            { ArrowDir.Up, ArrowDir.Left, ArrowDir.UpRight, ArrowDir.Left, ArrowDir.UpLeft },
            { ArrowDir.Down, ArrowDir.DownRight, ArrowDir.Right, ArrowDir.UpLeft, ArrowDir.DownLeft },
            { ArrowDir.UpRight, ArrowDir.DownRight, ArrowDir.Left, ArrowDir.Right, ArrowDir.UpLeft },
            { ArrowDir.None, ArrowDir.UpLeft, ArrowDir.Down, ArrowDir.Left, ArrowDir.Down },
        };

        arrowDirs3 = new ArrowDir[, ] {
            { ArrowDir.Right, ArrowDir.Left, ArrowDir.UpLeft, ArrowDir.Right, ArrowDir.UpLeft,ArrowDir.Right,ArrowDir.Left },
            { ArrowDir.Up, ArrowDir.DownLeft, ArrowDir.DownRight, ArrowDir.DownLeft, ArrowDir.Up,ArrowDir.Down,ArrowDir.UpLeft },
            { ArrowDir.UpRight, ArrowDir.Up, ArrowDir.DownRight, ArrowDir.UpLeft, ArrowDir.UpRight,ArrowDir.Left,ArrowDir.DownLeft },
            { ArrowDir.UpRight, ArrowDir.Left, ArrowDir.Down, ArrowDir.None, ArrowDir.DownLeft,ArrowDir.DownRight,ArrowDir.UpLeft },
            { ArrowDir.DownRight, ArrowDir.UpRight, ArrowDir.DownLeft, ArrowDir.DownLeft, ArrowDir.UpLeft,ArrowDir.DownLeft,ArrowDir.UpLeft },
            { ArrowDir.DownRight, ArrowDir.Right, ArrowDir.UpLeft, ArrowDir.DownRight, ArrowDir.Up,ArrowDir.Down,ArrowDir.Up },
            { ArrowDir.DownRight, ArrowDir.DownLeft, ArrowDir.DownRight, ArrowDir.DownRight, ArrowDir.DownLeft,ArrowDir.DownRight,ArrowDir.Down },
        };
    }
    private void InitJumpCount()
    {
        jumpCount1 = new int[,]
        {
            {1,2,1},
            {2,1,1},
            {2,2,0},
        };
        jumpCount2 = new int[,]
        {
            {2,3,2,1},
            {1,2,1,3},
            {3,2,1,1},
            {0,3,2,3},

        };
        jumpCount3 = new int[,]
        {
            {3,2,1,1,1},
            {2,1,3,2,3},
            {1,2,0,3,2},
            {3,1,2,2,1},
            {1,2,1,3,2},
        };
    }
    private void InitNum()
    {
        num1 = new int[,]
       {
            {0,0,0,2,2,2},
            {2,0,0,2,0,0},
            {0,0,2,2,2,2},
            {2,2,0,0,0,2},
            {0,2,0,0,0,2},
            {0,2,2,2,2,2},
       };
        num2 = new int[,]
       {
            {2 ,3 ,5 ,2 ,5 ,2 ,5 ,2},
            {3 ,10,2 ,2 ,3 ,2 ,2 ,5},
            {3 ,5 ,5 ,3 ,3 ,5 ,10,3},
            {3 ,2 ,2 ,5 ,2 ,5 ,2 ,5},
            {3 ,3 ,2 ,3 ,3 ,5 ,3 ,2},
            {2 ,3 ,10,3 ,3 ,5 ,3 ,5},
            {10,2 ,5 ,2 ,3 ,2 ,5 ,2},
            {10,10,5 ,5 ,2 ,5 ,3 ,3},
       };
        num3 = new int[,]
       {
            {0,0,0,0,2,2,2,2},
            {2,2,2,2,2,0,0,0},
            {2,0,0,0,0,0,0,0},
            {2,0,0,0,0,0,0,0},
            {2,2,2,2,2,2,0,0},
            {0,0,0,0,0,2,2,0},
            {0,0,0,0,0,0,2,0},
            {2,2,2,2,2,2,2,0},
       };
    }
    private void InitPath()
    {
        graph3 = new Dictionary<string, List<(string dest, int time)>>()
        {
            { "숲",     new List<(string,int)> { ("오두막",9), ("헬기장",10), ("천문대",6) } },
            { "오두막", new List<(string,int)> { ("숲",9), ("헬기장",7), ("수학마을",5), ("목장", 7) } },
            { "헬기장", new List<(string,int)> { ("숲",10), ("오두막",7), ("천문대",3), ("온천",3) } },
            { "천문대", new List<(string,int)> { ("숲",6), ("헬기장",3), ("온천",4) , ("목장", 12)} },
            { "온천",   new List<(string,int)> { ("천문대",4), ("목장",4), ("헬기장",3) } },
            { "목장",   new List<(string,int)> { ("오두막",7), ("온천",4), ("수학마을",2),("천문대",12) } },
            { "수학마을", new List<(string,int)> { ("오두막",5),  ("목장",2) } }
        };
        graph2 = new Dictionary<string, List<(string dest, int time)>>()
        {
            { "숲",       new List<(string,int)> { ("폭포",1), ("천문대",3) , ("온천",7) } },
            { "폭포",     new List<(string,int)> { ("숲",1), ("수학마을",4), ("천문대",3) } },
            { "천문대",   new List<(string,int)> { ("숲",3), ("폭포",3), ("온천",4) } },
            { "온천",     new List<(string,int)> { ("천문대",4), ("수학마을",5) ,("숲",7)} },
            { "수학마을", new List<(string,int)> { ("폭포",4), ("온천",5) } }
        };
        graph1 = new Dictionary<string, List<(string dest, int time)>>()
        {
             { "숲",       new List<(string,int)> { ("연못",3), ("목장",6), ("수학마을",7) } },
            { "연못",     new List<(string,int)> { ("숲",3), ("목장",1) } },
            { "목장",     new List<(string,int)> { ("숲",6), ("연못",1), ("수학마을",1) } },
            { "수학마을", new List<(string,int)> { ("숲",7), ("목장",1) } }
        };
    }
}
