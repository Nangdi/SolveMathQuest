using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.IO;
using Unity.VisualScripting.Dependencies.Sqlite;

public class LocalDBManager : MonoBehaviour
{
    private string dbPath;

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
    public int GetMyRank(string playerName, float clearTime)
    {
        int rank = 1;

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // 나보다 빠른 기록 수 계산 → 그 수 + 1 = 내 순위
                cmd.CommandText = "SELECT COUNT(*) FROM records WHERE time < @myTime";
                cmd.Parameters.Add(new SqliteParameter("@myTime", clearTime));
                rank = int.Parse(cmd.ExecuteScalar().ToString()) + 1;
            }
        }

        Debug.Log($"🏁 {playerName}의 현재 순위: {rank}위");
        return rank;
    }

    public void GetTopRecords(int limit = 100)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT name, time FROM records ORDER BY time ASC LIMIT {limit}";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log($"{reader["name"]} : {reader["time"]}");
                    }
                }
            }
        }
    }
}
