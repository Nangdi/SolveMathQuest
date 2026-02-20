using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovementController : MonoBehaviour
{
    public Camera cam;
    public SpriteRenderer spriteRenderer;
    public PlayerManager player;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

            if (hit.collider != null)
            {
                Transform t = spriteRenderer.transform;

                // 월드 → 로컬
                Vector2 localPos = t.InverseTransformPoint(mouseWorld);

                // 스프라이트 실제 월드 크기
                //Vector2 size = spriteRenderer.bounds.size;

                float u = localPos.x + 0.5f;
                float v = localPos.y + 0.5f;
                Vector2 normalizationPos = new Vector2(u, v);
                Debug.Log($"정규화 좌표 : {u :F2}, {v:F2}");
                player.SetPlayerPosition(normalizationPos);
            }
        }
    }
}
