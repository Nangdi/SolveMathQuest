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
    private void Start()
    {
    }
    private void Update()
    {
        //디버그용
        Vector3 rel = Display.RelativeMouseAt(Input.mousePosition);
        //Debug.Log($"Input.mousePosition = {Input.mousePosition}");
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition;
         
            //// z 값은 카메라와의 거리
            //screenPos.z = Mathf.Abs(uiCam.transform.position.z);
            //bool Touchmenu = CheckHitMouseDebug(screenPos);
            //if (Touchmenu )
            //{
            //    playerMenu.SetStopRotating(true);
            //}
            //else
            //{
            //    playerMenu.SetStopRotating(false);
            //}

            //Debug.Log("Clicked World Pos (2D): " + screenPos);
            CheckHitMouseDebug(screenPos);
        }
    }
    public RectTransform menuRect;

    bool IsPointerOverMenu(Vector2 screenPos)
    {
        bool inside = RectTransformUtility.RectangleContainsScreenPoint(menuRect, screenPos, gameCam);
        Debug.Log($"Menu 안에 들어왔는가: {inside}");
        return inside;
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
        return world;
    }
    void OnDrawGizmos()
    {
        if (gameCam == null) return;

        // Screen → World 변환 (깊이 중요)
        Vector3 world = gameCam.ScreenToWorldPoint(
            new Vector3(debugVector2.x, debugVector2.y,
            Mathf.Abs(gameCam.transform.position.z))
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(world, 0.2f);

    }
    public bool CheckHit(Vector2 map01)
    {

        Vector3 screenPos = Map01ToScreenPos(map01);

        // 3️⃣ PointerEventData 생성
        PointerEventData ped = new PointerEventData(eventSystem);
        ped.position = screenPos;
        Debug.Log($"isPointerOverMenu : {IsPointerOverMenu(screenPos)}");
        return IsPointerOverMenu(screenPos);
       
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
            
        }

        return false;
    }
    public void CheckHitMouseDebug(Vector2 screenPos)
    {
        // 3️⃣ PointerEventData 생성
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = screenPos;
        debugVector2 = pointerData.position;

        List<RaycastResult> results = new List<RaycastResult>();

        playerRaycaster.Raycast(pointerData, results);
        uiRaycaster.Raycast(pointerData, results);
        if (results.Count > 0)
        {
            foreach (var r in results)
            {
                Debug.Log($"레이캐스트된 오브젝트 : {r.gameObject.name}");
            }
            GameObject target = results[0].gameObject;

            //ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerDownHandler);
            //ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerClickHandler);
            Debug.Log($"클릭된 오브젝트 : {target.name}");
            if (target.name == "Menu")
            {

            }
        }
    }
    public void ClickScreenbylidar(Vector2 map01)
    {

        Vector3 screenPos = Map01ToScreenPos(map01);
        //Vector2 screenPos = Map01ToWorldPos(map01);
        PointerEventData pointerData = new PointerEventData(eventSystem);

        //pointerData.displayIndex = 1;
        pointerData.position = screenPos;
        debugVector2 = pointerData.position;
        //Debug.Log($"레이캐스트 시작 좌표 : {pointerData.position}");
        List<RaycastResult> results = new List<RaycastResult>();

        //Vector2 menuPos = uiCam.WorldToScreenPoint(playerMenu.transform.position);
        //IsPointerOverMenu(screenPos);
        //EventSystem.current.RaycastAll(pointerData, results);
        playerRaycaster.Raycast(pointerData, results);
        //uiRaycaster.Raycast(pointerData, results);
        Debug.Log($"레이캐스트된 오브젝트 수 : {results.Count}");
        if (results.Count > 0)
        {
            foreach (var r in results)
            {
                Debug.Log($"레이캐스트된 오브젝트 : {r.gameObject.name}");
                ExecuteEvents.Execute(r.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
                if (r.gameObject.CompareTag("Menu"))
                {
                    Debug.Log("Menu 클릭됨");
                    playerMenu.SetStopRotating(true);
                    return;
                }
            }
        
        }
        playerMenu.SetStopRotating(false);
    }
   
}
