using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rule_ShortestPath : MonoBehaviour, IGameRule
{
    public List<string> visitLocations = new List<string>();
  
    private int sumTime = 0;
    private int correctTime = 0;
    private string previousLocation;
    private string currentLocation;
    private Dictionary<string, List<(string dest, int time)>> selectGraph = new Dictionary<string, List<(string dest, int time)>>();
    private void Awake()
    {
       
       
    }
    private void Start()
    {
      

    }
    public bool GameClear(Collider2D col)
    {
        if (col.CompareTag("Arrive"))
        {
            if (visitLocations.Count == selectGraph.Count - 1 && sumTime == correctTime)
            {
            return true;

            }
            //GameManager.instance.Fail();
        }
        return false;
    }

    public bool isRuleViolated(Collider2D col)
    {
        //연결되지않은 길로가면 룰위반 
        previousLocation = currentLocation;
        //현재위치 업데이트
        currentLocation = col.name;
       
        //이전위치에 저장
        int timeTaken =0;
        //갈수있는 위치 리스트 갱신
        //Debug.Log(selectGraph[previousLocation]);
        var placesAYouCanGoList = selectGraph[previousLocation];
        //갈수있는 위치 dest에 현재 위치의 장소가 있는지 확인해야함
        foreach (var place in placesAYouCanGoList)
        {
            if(place.dest == currentLocation)
            {
                timeTaken = place.time;
                sumTime += timeTaken;
                Debug.Log($"통과 ,누적시간 : {sumTime}");
                if (!visitLocations.Contains(place.dest))
                {
                    visitLocations.Add(place.dest);
                }

                return false;
            }
        }
        //도착한곳이 arrive일때 조건만족안할시 fail
        if(currentLocation == previousLocation)
        {
            return false;
        }
        Debug.Log("길이없음");
        return true;
    }

    public void ResetData(Collider2D col)
    {
        currentLocation = "숲";
        previousLocation = "";
        visitLocations.Clear();
        sumTime = 0;
    }
    public void SetDifficultMode(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.eazy:
                selectGraph = MapManager.instance.graph1;
                correctTime = 5;

                break;
            case Difficulty.Normal:
                selectGraph = MapManager.instance.graph2;
                correctTime = 13;
                break;
            case Difficulty.Hard:
                Debug.Log("난이도 하드설정");
                selectGraph = MapManager.instance.graph3;
                correctTime = 28;
                break;
        }

    }
    private void ee()
    {
    }

    public void VeiwHint()
    {
        throw new System.NotImplementedException();
    }
}
