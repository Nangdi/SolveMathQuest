using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MazeSolutionLineAnimator : MonoBehaviour
{
    public static MazeSolutionLineAnimator instance;

    [Header("Animation")]
    [Min(0.05f)] public float duration = 1.5f;   // 전체 그려지는 시간
    [Min(4)] public int samplesPerUnit = 12;     // 1 유닛(월드거리)당 샘플 개수 (부드러움)
    public float pointLifetime = 1.5f;   // 유지 시간 (그려진 후)
    public bool playOnStart = true;

    [Header("Line Look")]
    public float width = 0.15f;
    public int capVertices = 8;
    public int cornerVertices = 8;
    public int sortingOrder = 10;
    public float lineSpeed = 0.5f;

    List<Vector3> path = new List<Vector3>();

   LineRenderer line;
    Coroutine co;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        line = GetComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.positionCount = 0;
        line.startWidth = width;
        line.endWidth = width;
        line.numCapVertices = capVertices;
        line.numCornerVertices = cornerVertices;
        line.startColor = Color.white;
        line.endColor = Color.white;
        // 2D에서 확실히 보이도록 기본 머티리얼
        if (line.material == null)
            line.material = new Material(Shader.Find("Sprites/Default"));

        line.sortingOrder = sortingOrder;
    }

    void Start()
    {
        CorectLineSetting(MapManager.instance.shortestCorrectPath[2].path);
        if (!playOnStart) return;
        // 예시 경로(월드 좌표). 너는 실제 정답 경로로 교체.
        //var path = new List<Vector3>()
        //{
        //    new Vector3(0, 0, 0),
        //    new Vector3(3, 0, 0),
        //    new Vector3(3, 2, 0),
        //    new Vector3(6, 2, 0),
        //    new Vector3(6, 5, 0)
        //};

        //Play();
    }

    /// <summary>정답 라인을 서서히 그리기 시작</summary>
    public void Play(float speed)
    {
        Transform[] correctPath = MapManager.instance.GetCorrectPath(GameManager.instance.gameType , GameManager.instance.difficultyMode);
        CorectLineSetting(correctPath);

        if (path == null || path.Count < 2) return;

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(AnimateDraw(path, speed * path.Count));
    }

    IEnumerator AnimateDraw(IReadOnlyList<Vector3> path, float drawDuration)
    {
        var sampled = BuildSampledPoints(path, samplesPerUnit);
        int total = sampled.Count;

        // 포인트 등장 속도(시간 기준): total개를 drawDuration 동안 순차 등장
        float pointsPerSecond = total / drawDuration;

        float t = 0f;
        float endTime = drawDuration + lineSpeed * 3; // 마지막 포인트가 사라질 때까지

        while (t < endTime)
        {
            t += Time.deltaTime;

            // head: 지금까지 등장한 마지막 포인트 index
            int head = Mathf.FloorToInt(t * pointsPerSecond);

            // tail: (t - pointLifetime) 이전에 등장했던 포인트들은 제거
            int tail = Mathf.FloorToInt((t - lineSpeed*3) * pointsPerSecond);

            head = Mathf.Clamp(head, 0, total - 1);
            tail = Mathf.Clamp(tail, 0, total - 1);

            // 아직 그려질 포인트가 거의 없을 때
            if (head <= 0)
            {
                line.positionCount = 0;
                yield return null;
                continue;
            }

            // tail이 head를 넘어가면(끝나고 다 사라진 상태)
            if (tail >= head)
            {
                line.positionCount = 0;
                yield return null;
                continue;
            }

            int count = (head - tail) + 1;
            line.positionCount = count;

            // sampled[tail..head] 구간만 라인으로 표시
            for (int i = 0; i < count; i++)
                line.SetPosition(i, sampled[tail + i]);

            yield return null;
        }

        line.positionCount = 0;
        co = null;
        GameManager.instance.WorkAfterHint();
    }

    /// <summary>
    /// 원래 경로(꺾인 polyline)를 거리 기준으로 샘플링해서 부드럽게 늘려 그릴 수 있게 만든다.
    /// </summary>
    static List<Vector3> BuildSampledPoints(IReadOnlyList<Vector3> path, int samplesPerUnit)
    {
        var result = new List<Vector3>();
        result.Add(path[0]);

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 a = path[i];
            Vector3 b = path[i + 1];

            float dist = Vector3.Distance(a, b);
            int steps = Mathf.Max(1, Mathf.CeilToInt(dist * samplesPerUnit));

            // a는 이미 들어가 있으니 1부터
            for (int s = 1; s <= steps; s++)
            {
                float t = (float)s / steps;
                result.Add(Vector3.Lerp(a, b, t));
            }
        }

        // 중복 제거(아주 드물게 동일 좌표가 연속으로 들어갈 수 있음)
        for (int i = result.Count - 2; i >= 0; i--)
        {
            if ((result[i + 1] - result[i]).sqrMagnitude < 1e-10f)
                result.RemoveAt(i + 1);
        }

        return result;
    }
    public void CorectLineSetting(Transform[] corectpath)
    {
        path.Clear();

        for (int i = 0;i < corectpath.Length; i++)
        {
            path.Add(corectpath[i].position);
        }
    }
}
