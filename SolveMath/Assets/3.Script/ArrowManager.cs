using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager instance;

    public GameObject[] Map_1;
    public GameObject[] Map_2;
    public GameObject[] Map_3;
    public Collider2D[,] colliderGrid;  // 실제 씬에 있는 타일 Collider 저장
    public Dictionary<Collider2D, Vector2Int> colliderToIndex; // 매핑 테이블
    public ArrowDir[,] arrowDirs1 = new ArrowDir[3, 3];
    public ArrowDir[,] arrowDirs2 = new ArrowDir[5, 5];
    public ArrowDir[,] arrowDirs3 ;


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
        SetArrow();

    }
    private void Start()
    {
        SetDic(Map_3);
        SetDic(Map_2);
        SetDic(Map_1);
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


    private void SetArrow()
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
    
}
