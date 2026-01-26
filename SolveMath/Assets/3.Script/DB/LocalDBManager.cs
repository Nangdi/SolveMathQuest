using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.IO;
public struct RecordEntry
{
    public int Rank;
    public string Name;
    public float Time;

    public RecordEntry(int rank, string name, float time)
    {
        Rank = rank;
        Name = name;
        Time = time;
    }
}

public class LocalDBManager : MonoBehaviour
{
    private string dbPath;

    [SerializeField] private Transform recordRoot;

    public struct RecordEntry
    {
        public int Rank;
        public string Name;
        public float Time;

        public RecordEntry(int rank, string name, float time)
        {
            Rank = rank;
            Name = name;
            Time = time;
        }
    }

    void Awake()
    {
        dbPath = "URI=file:" + Path.Combine(Application.persistentDataPath, "records.db");
        CreateTableIfNotExists();
    }

    private void CreateTableIfNotExists()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS records (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    time REAL
                )";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void AddRecord(string name, float time)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO records (name, time) VALUES (@name, @time)";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@time", time);
                cmd.ExecuteNonQuery();
            }
        }
        Debug.Log($"✅ 기록 저장 완료: {name} / {time:F2}");
    }

    /// <summary>
    /// Top N 기록을 시간 오름차순으로 가져오며 (순위, 이름, 기록)을 모두 반환.
    /// 동점은 같은 순위(DENSE RANK).
    /// </summary>
    public List<RecordEntry> GetTopRecordsDetailed(int limit = 20)
    {
        var result = new List<RecordEntry>(limit);

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT name, time FROM records ORDER BY time ASC LIMIT @limit";
                cmd.Parameters.AddWithValue("@limit", limit);

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    int rank = 0;
                    float? prevTime = null;

                    while (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        float t = Convert.ToSingle(reader["time"]);

                        // DENSE RANK: 시간이 바뀔 때만 rank 증가
                        if (!prevTime.HasValue || !Mathf.Approximately(prevTime.Value, t))
                        {
                            rank++;
                            prevTime = t;
                        }

                        result.Add(new RecordEntry(rank, name, t));
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Top N을 UI에 적용 (예: recordRoot 자식에 세팅)
    /// </summary>
    public void ApplyTopRecordsToUI(int limit = 20)
    {
        var top = GetTopRecordsDetailed(limit);

        for (int i = 0; i < top.Count && i < recordRoot.childCount; i++)
        {
            var e = top[i];
            recordRoot.GetChild(i).GetComponent<Record>()
                .SetRecord(e.Rank, e.Name, e.Time.ToString("F2"));
        }
    }

    /// <summary>
    /// "내 순위" 뿐 아니라 "내 기록(time)"도 DB에서 같이 가져오기.
    /// (playerName의 최고 기록 MIN(time) 기준)
    /// </summary>
    public bool TryGetMyRankAndTime(string playerName, out int rank, float bestTime)
    {
        rank = -1;
        //bestTime = -1f;

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();

            // 1) 내 최고 기록 가져오기
            //using (var cmd = conn.CreateCommand())
            //{
            //    cmd.CommandText = "SELECT MIN(time) FROM records WHERE name = @name";
            //    cmd.Parameters.AddWithValue("@name", playerName);

            //    object scalar = cmd.ExecuteScalar();
            //    if (scalar == null || scalar == DBNull.Value)
            //        return false;

            //    bestTime = Convert.ToSingle(scalar);
            //}

            // 2) 그 기록으로 순위 계산 (나보다 빠른 기록 수 + 1)
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM records WHERE time < @myTime";
                cmd.Parameters.AddWithValue("@myTime", bestTime);

                rank = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            }
        }

        Debug.Log($"🏁 {playerName}의 최고기록: {bestTime:F2}, 순위: {rank}위");
        return true;
    }
}
