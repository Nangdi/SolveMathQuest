using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColSizeDownWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Transform[] chi = GetComponentsInChildren<Transform>(true);
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    Transform child = transform.GetChild(i);
            for (int j = 0; j < chi.Length; j++) 
            {
                Transform grandChild = chi[j];
                if(!grandChild.CompareTag("Wall")) 
                {
                    continue; // "Wall" 태그가 아닌 오브젝트는 건너뜁니다.
                }
                BoxCollider2D col = grandChild.GetComponent<BoxCollider2D>();
                if (col != null)
                {
                    Vector2 newSize = new Vector2(col.size.x * 0.6f, col.size.y * 0.6f);
                    col.size = newSize;
                }
            }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
