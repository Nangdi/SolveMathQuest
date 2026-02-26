using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LidarTouchManager : MonoBehaviour
{
    [SerializeField]
    Camera uiCam;
    [SerializeField]
    SpriteRenderer mapRect;
    public GraphicRaycaster raycaster;    // 해당 Canvas에 붙은 것
    public EventSystem eventSystem;
    Vector3 debugVector2 = new Vector3 (0, 0);

    [Header("디버그용")]
    public playerMenu playerMenu; 
    private void Update()
    {
        //디버그용
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition;

            // z 값은 카메라와의 거리
            screenPos.z = Mathf.Abs(uiCam.transform.position.z);
            bool Touchmenu = CheckHitMouseDebug(screenPos);
            if (Touchmenu)
            {
                playerMenu.SetStopRotating(true);
            }
            else
            {
                playerMenu.SetStopRotating(false);
            }

            //Debug.Log("Clicked World Pos (2D): " + screenPos);
        }
    }
    public Vector2 Map01ToScreenPos(Vector2 map01)
    {

        Vector3 world = Map01ToWorldPos(map01);

        // 3) 월드 → 스크린 픽셀
        Vector3 sp = uiCam.WorldToScreenPoint(world);
        debugVector2 = sp;
        //Debug.Log($"변경된 월드좌표 : {sp}");
        return sp;
       
    }
    public Vector2 Map01ToWorldPos(Vector2 map01)
    {
        Bounds b = mapRect.bounds;

        // 2) 0~1 → 월드 좌표 (XY 평면 기준)
        float wx = Mathf.Lerp(b.min.x, b.max.x, map01.x);
        float wy = Mathf.Lerp(b.min.y, b.max.y, map01.y);
        Vector3 world = new Vector3(wx, wy, mapRect.transform.position.z);
        return world;
    }
    void OnDrawGizmos()
    {
        if (uiCam == null) return;

        // Screen → World 변환 (깊이 중요)
        Vector3 world = uiCam.ScreenToWorldPoint(
            new Vector3(debugVector2.x, debugVector2.y,
            Mathf.Abs(uiCam.transform.position.z))
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(world, 0.2f);
    }
    public bool CheckHit(Vector2 map01)
    {

        Vector2 screenPos = Map01ToScreenPos(map01);

        // 3️⃣ PointerEventData 생성
        PointerEventData ped = new PointerEventData(eventSystem);
        ped.position = screenPos;

        // 4️⃣ Raycast
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(ped, results);

        if (results.Count == 0)
            return false;

        // 5️⃣ 특정 UI인지 확인
        foreach (var r in results)
        {
            //if (r.gameObject.transform.IsChildOf(targetUiRoot))
            //{
            //    Debug.Log("✔ 플레이어 UI 위에 있음");
            //    return true;
            //}
            if (r.gameObject.CompareTag("Menu"))
            {
                Debug.Log(r.gameObject.name);
                return true;
            }
        }

        return false;
    }
    public bool CheckHitMouseDebug(Vector2 screenPos)
    {
        // 3️⃣ PointerEventData 생성
        PointerEventData ped = new PointerEventData(eventSystem);
        ped.position = screenPos;

        // 4️⃣ Raycast
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(ped, results);

        if (results.Count == 0)
            return false;

        // 5️⃣ 특정 UI인지 확인
        foreach (var r in results)
        {
            //if (r.gameObject.transform.IsChildOf(targetUiRoot))
            //{
            //    Debug.Log("✔ 플레이어 UI 위에 있음");
            //    return true;
            //}
            if (r.gameObject.CompareTag("Menu"))
            {
                Debug.Log(r.gameObject.name);
                return true;
            }
        }

        return false;
    }
    public void ClickScreenbylidar(Vector2 map01)
    {
        Vector2 screenPos = Map01ToScreenPos(map01);
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPos;
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            GameObject target = results[0].gameObject;

            //ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerDownHandler);
            //ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }
}
