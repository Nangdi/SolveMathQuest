using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultDBController : MonoBehaviour
{
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private LocalDBManager db;
    [SerializeField]
    private Record myRecord;
    void Start()
    {

        //    // 1️⃣ 내 기록 추가
        //    db.AddRecord("ㅀㅂㅈ", 40.23f);

        // 2️⃣ 내 순위 가져오기
        //    int myRank = db.GetMyRank("ㅀㅂㅈ", 40.23f);

        //    // 3️⃣ Top100 출력
        //    db.GetTopRecords(100);
    }
    private void OnEnable()
    {
        if (playerManager.isClear)
        {
            db.AddRecord(playerManager.playerName, playerManager.clearTime, (int)GameManager.instance.gameType, (int)GameManager.instance.difficultyMode);

            // 2️⃣ 내 순위 가져오기
            if (db.TryGetRankByClearTime(playerManager.clearTime, (int)GameManager.instance.gameType, (int)GameManager.instance.difficultyMode, out int r))
            {
                Debug.Log($"내 순위={r}, 내 기록={playerManager.clearTime:F2}");
                myRecord.SetRecord(r, playerManager.playerName, $"{playerManager.clearTime:F2}");
            }
        }

       
        // 3️⃣ Top100 출력
        db.ApplyTopRecordsToUI((int)GameManager.instance.gameType, (int)GameManager.instance.difficultyMode , 20);
        //bool d = db.TryGetMyRankAndTime(playerManager.name);


    }

    private void OnDisable()
    {
        //생성한 기록 초기화
    }
}
