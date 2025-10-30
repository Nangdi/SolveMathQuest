using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private LocalDBManager db;

    void Start()
    {
        db = FindObjectOfType<LocalDBManager>();

        // 1️⃣ 내 기록 추가
        db.AddRecord("ㅀㅂㅈ", 40.23f);

        // 2️⃣ 내 순위 가져오기
        int myRank = db.GetMyRank("ㅀㅂㅈ", 40.23f);

        // 3️⃣ Top100 출력
        db.GetTopRecords(100);
    }
}
