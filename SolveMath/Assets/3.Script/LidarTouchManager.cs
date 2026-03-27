using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[Serializable]
public class ClickUI
{
    public Button button;
    public RectTransform rect;
}

public class LidarTouchManager : MonoBehaviour
{
    [SerializeField]
    Camera gameCam;
    [SerializeField]
    SpriteRenderer mapRect;
    public GraphicRaycaster raycaster;    // 해당 Canvas에 붙은 것
    public EventSystem eventSystem;
    Vector3 debugVector2 = new Vector3 (0, 0);
    public GraphicRaycaster playerRaycaster;
    public GraphicRaycaster uiRaycaster;
    [Header("디버그용")]
    public playerMenu playerMenu; 
    Vector2 debugScreenPos;

    public RectTransform[] clickTargets;
    [SerializeField]
    private List<ClickUI> clickUIs = new List<ClickUI>();
    public RectTransform menuRect;
    private void Start()
    {
        //Button[] btnObjects = GameObject.FindObjectsOfType<Button>();
        //foreach (var target in btnObjects)
        //{
        //    ClickUI clickUI = new ClickUI();
        //    clickUI.rect = target.GetComponent<RectTransform>();
        //    clickUI.button = target.GetComponent<Button>();
        //    clickUIs.Add(clickUI);
        //}
        foreach (var target in clickTargets)
        {
            ClickUI clickUI = new ClickUI();
            clickUI.rect = target;
            clickUI.button = target.GetComponent<Button>();
            clickUIs.Add(clickUI);
        }

    }
    private void Update()
    {
        //디버그용

        //Vector3 screenPos = Input.mousePosition;
        //Click_RectTransformUtility(screenPos);
    }

    public bool IsPointerOverMenu(Vector2 Map01)
    {
        Vector2 screenPos = Map01ToScreenPos(Map01);
        bool inside = RectTransformUtility.RectangleContainsScreenPoint(menuRect, screenPos, gameCam);
        Debug.Log($"Menu 안에 들어왔는가: {inside}");
        return inside;
    }
    public bool Click_RectTransformUtility(Vector2 Map01)
    {
        Vector2 screenPos = Map01ToScreenPos(Map01);
        foreach (var clickUI in clickUIs)
        {

            if (RectTransformUtility.RectangleContainsScreenPoint(clickUI.rect, screenPos, gameCam) && clickUI.rect.gameObject.activeInHierarchy)
            {
                clickUI.button.onClick.Invoke();
                Debug.Log($"클릭된 UI : {clickUI.button.name}");
                return true;
            }
        }
            return false;
    }
    public Vector3 Map01ToScreenPos(Vector2 map01)
    {
        Vector3 world = Map01ToWorldPos(map01);

        // 3) 월드 → 스크린 픽셀
        Vector3 sp = gameCam.WorldToScreenPoint(world);
        //debugVector2 = sp;
        //Debug.Log($"변경된 월드좌표 : {sp}");



        //// 1368 x 768 기준으로 0~1로 정규화된 맵 좌표를 스크린 픽셀로 변환하는 과정
        //float x = map01.x * 1360f;
        //float y = map01.y * 768f;
        //return new Vector2(x, y);
        return sp;

    }
    public Vector3 Map01ToWorldPos(Vector2 map01)
    {
        Bounds b = mapRect.bounds;

        // 2) 0~1 → 월드 좌표 (XY 평면 기준)
        float wx = Mathf.Lerp(b.min.x, b.max.x, map01.x);
        float wy = Mathf.Lerp(b.min.y, b.max.y, map01.y);
        Vector3 world = new Vector3(wx, wy, mapRect.transform.position.z);
        //Debug.Log($"맵 좌표 {map01} → 월드 좌표 {world}");
        return world;
    }
    void OnDrawGizmos()
    {
        if (gameCam == null) return;

        // Screen → World 변환 
        Vector3 world = gameCam.ScreenToWorldPoint(
            new Vector3(debugVector2.x, debugVector2.y,
            Mathf.Abs(gameCam.transform.position.z))
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(world, 0.2f);

    }
  
   
}
