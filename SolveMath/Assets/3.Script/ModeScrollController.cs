using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModeScrollController : MonoBehaviour
{
    [Header("Scroll & Content")]
    public ScrollRect scrollRect;
    public RectTransform content;

    [Header("Cards (RectTransform Array)")]
    public RectTransform[] cards;

    [Header("Snap Settings")]
    public float snapSpeed = 10f;
    public float velocityThreshold = 200f; // 감속이 거의 끝났다고 판단할 속도 기준


    private HorizontalLayoutGroup layoutGroup;
    private int currentIndex = 0;
    private bool isSnapping = false;
    private bool isStopping = true;
    private Vector2 targetPosition;

    void Start()
    {
        layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
        UpdateTargetPosition();
    }

    void Update()
    {
        if (/*!isSnapping &&*/ scrollRect.velocity.magnitude < velocityThreshold && scrollRect.velocity.magnitude > 0f)
        {
            // 스크롤이 거의 멈췄을 때 한 번만 실행
            if (!isStopping)
            {
                isStopping = true;
                OnEndDrag();
            }
        }
        else if (scrollRect.velocity.magnitude > velocityThreshold)
        {
            isStopping = false; // 다시 움직이기 시작하면 리셋
        }


       

        if (isSnapping &&isStopping)
        {
            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPosition, Time.deltaTime * snapSpeed);
            if (Vector2.Distance(content.anchoredPosition, targetPosition) < 0.1f)
                isSnapping = false;
        }
    }

    // ────────────────────────────────────────────────
    // 드래그 종료 시 (EventTrigger의 EndDrag 연결 가능)
    // ────────────────────────────────────────────────
    public void OnEndDrag()
    {
        float scrollPos = Mathf.Abs(content.anchoredPosition.x);
        float cardWidth = cards[0].rect.width + layoutGroup.spacing;

        // padding 포함하여 가장 가까운 인덱스 계산
        int nearestIndex = Mathf.RoundToInt((scrollPos - (cards[0].rect.width/2)) / cardWidth);
        nearestIndex = Mathf.Clamp(nearestIndex, 0, cards.Length - 1);
        currentIndex = nearestIndex;
        UpdateTargetPosition();
        //isSnapping = true;
    }
    public void beginDrag()
    {
        isSnapping = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    // ────────────────────────────────────────────────
    // 타겟 위치 계산 (패딩 포함)
    // ────────────────────────────────────────────────
    private void UpdateTargetPosition()
    {
        float cardWidth = cards[0].rect.width ;
        float targetX = -(cardWidth/2 +layoutGroup.spacing/4 + currentIndex * (cardWidth+layoutGroup.spacing));
        //Debug.Log($"타겟위치 : {targetX}");
        //if(currentIndex != 0)
        //{
        //    targetX -= layoutGroup.spacing;
        //}
        targetPosition = new Vector2(targetX, content.anchoredPosition.y);
    }

    // ────────────────────────────────────────────────
    // 버튼으로 이전/다음 이동
    // ────────────────────────────────────────────────
    public void Next()
    {
        if (currentIndex < cards.Length - 1)
            currentIndex++;
        UpdateTargetPosition();
        isSnapping = true;
    }

    public void Prev()
    {
        if (currentIndex > 0)
            currentIndex--;
        UpdateTargetPosition();
        isSnapping = true;
    }
    public void test()
    {
        Debug.Log("버튼눌림");
    }
}
