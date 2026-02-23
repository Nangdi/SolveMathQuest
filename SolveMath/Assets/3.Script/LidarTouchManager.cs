using System.Collections;
using System.Collections.Generic;
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
    private void Update()
    {
        //디버그용
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition;

            // z 값은 카메라와의 거리
            screenPos.z = Mathf.Abs(uiCam.transform.position.z);
            CheckHitMouseDebug(screenPos);


            Debug.Log("Clicked World Pos (2D): " + screenPos);
        }
    }
    public Vector2 Map01ToScreenPos(Vector2 map01)
    {
        Bounds b = mapRect.bounds;

        // 2) 0~1 → 월드 좌표 (XY 평면 기준)
        float wx = Mathf.Lerp(b.min.x, b.max.x, map01.x);
        float wy = Mathf.Lerp(b.min.y, b.max.y, map01.y);
        Vector3 world = new Vector3(wx, wy, mapRect.transform.position.z);

        // 3) 월드 → 스크린 픽셀
        Vector3 sp = uiCam.WorldToScreenPoint(world);
        debugVector2 = sp;
        Debug.Log($"변경된 월드좌표 : {sp}");
        return sp;
       
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
            Debug.Log(r.gameObject.name);
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
            Debug.Log(r.gameObject.name);
        }

        return false;
    }
}
