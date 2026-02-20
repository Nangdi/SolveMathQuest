using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.IO;

public class LocalDBManager : MonoBehaviour
{
    private string dbPath;

    [SerializeField] private Transform recordRoot;

    [Serializable]
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

    // ------------------------------
    // 초기화
    // ------------------------------
    void Awake()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, "records.db");


        dbPath = "URI=file:" + fullPath;

        CreateTable();
    }

    private void CreateTable()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                CREATE TABLE records (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    time REAL NOT NULL,
                    gameType INTEGER NOT NULL,
                    difficulty INTEGER NOT NULL
                );";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    "CREATE INDEX idx_records_gd_time ON records(gameType, difficulty, time);";
                cmd.ExecuteNonQuery();
            }
        }

        Debug.Log("DB 생성 완료");
    }

    // ------------------------------
    // 범위 보정
    // ------------------------------
    private int ClampGameType(int v) => Mathf.Clamp(v, 0, 4);
    private int ClampDifficulty(int v) => Mathf.Clamp(v, 0, 2);

    // ------------------------------
    // 기록 추가
    // ------------------------------
    public void AddRecord(string name, float time, int gameType, int difficulty)
    {
        gameType = ClampGameType(gameType);
        difficulty = ClampDifficulty(difficulty);

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO records (name, time, gameType, difficulty) VALUES (@name, @time, @gt, @df)";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@time", time);
                cmd.Parameters.AddWithValue("@gt", gameType);
                cmd.Parameters.AddWithValue("@df", difficulty);
                cmd.ExecuteNonQuery();
            }
        }

        Debug.Log($"기록 저장: {name} / {time:F2} / GT:{gameType} / DF:{difficulty}");
    }

    // ------------------------------
    // Top N 조회 (DENSE RANK)
    // ------------------------------
    public List<RecordEntry> GetTopRecords(int gameType, int difficulty, int limit = 20)
    {
        gameType = ClampGameType(gameType);
        difficulty = ClampDifficulty(difficulty);

        var result = new List<RecordEntry>();

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                SELECT name, time
                FROM records
                WHERE gameType = @gt AND difficulty = @df
                ORDER BY time ASC
                LIMIT @limit;";

                cmd.Parameters.AddWithValue("@gt", gameType);
                cmd.Parameters.AddWithValue("@df", difficulty);
                cmd.Parameters.AddWithValue("@limit", limit);

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    int rank = 0;
                    int position = 0;      // 순서 카운트
                    float? prevTime = null;

                    while (reader.Read())
                    {
                        position++;

                        string name = reader["name"].ToString();
                        float time = Convert.ToSingle(reader["time"]);

                        // 첫 항목이거나 시간이 달라지면 rank 증가
                        if (!prevTime.HasValue || !Mathf.Approximately(prevTime.Value, time))
                        {
                            rank++;
                            prevTime = time;
                        }

                        result.Add(new RecordEntry(rank, name, time));
                    }
                }
            }
        }

        return result;
    }
    // ------------------------------
    // UI 적용
    // ------------------------------
    public void ApplyTopRecordsToUI(int gameType, int difficulty, int limit = 20)
    {
        var top = GetTopRecords(gameType, difficulty, limit);

        for (int i = 0; i < recordRoot.childCount; i++)
        {
            var record = recordRoot.GetChild(i).GetComponent<Record>();

            if (i < top.Count)
            {
                var e = top[i];
                record.SetRecord(e.Rank, e.Name, e.Time.ToString("F2"));
            }
            else
            {
                record.SetRecord(0, "",  "");
            }
        }
        recordRoot.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }

    // ------------------------------
    // 내 최고 기록 + 순위
    // ------------------------------
    /// <summary>
    /// 방금 나온 clearTime으로 현재 모드(gameType/difficulty)에서 순위를 계산.
    /// 공동순위(Competition Rank): 1,2,2,2,5...
    /// precisionDigits: 동점 판정 자릿수(기본 2 => 0.01초 단위)
    /// </summary>
    public bool TryGetRankByClearTime(float clearTime, int gameType, int difficulty, out int rank, int precisionDigits = 2)
    {
        rank = -1;

        gameType = ClampGameType(gameType);
        difficulty = ClampDifficulty(difficulty);

        try
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // Dense Rank: 나보다 빠른 '서로 다른' 기록 값 개수 + 1
                    cmd.CommandText = @"
                    SELECT COUNT(DISTINCT ROUND(time, @digits))
                    FROM records
                    WHERE gameType = @gt
                      AND difficulty = @df
                      AND ROUND(time, @digits) < ROUND(@myTime, @digits);";

                    cmd.Parameters.AddWithValue("@gt", gameType);
                    cmd.Parameters.AddWithValue("@df", difficulty);
                    cmd.Parameters.AddWithValue("@digits", precisionDigits);
                    cmd.Parameters.AddWithValue("@myTime", clearTime);

                    int distinctFasterTimes = Convert.ToInt32(cmd.ExecuteScalar());
                    rank = distinctFasterTimes + 1;
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"순위 계산 실패: {e.Message}");
            rank = 0;
            return false;
        }
    }

    // ------------------------------
    // 전체 초기화
    // ------------------------------
    public void ResetAllRecords()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM records;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM sqlite_sequence WHERE name='records';";
                cmd.ExecuteNonQuery();
            }
        }

        Debug.Log("전체 기록 초기화 완료");
    }

    // ------------------------------
    // 특정 모드 초기화
    // ------------------------------
    public void ResetRecords(int gameType, int difficulty)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "DELETE FROM records WHERE gameType=@gt AND difficulty=@df;";

                cmd.Parameters.AddWithValue("@gt", gameType);
                cmd.Parameters.AddWithValue("@df", difficulty);
                cmd.ExecuteNonQuery();
            }
        }

        Debug.Log($"기록 초기화: GT {gameType}, DF {difficulty}");
    }
}
