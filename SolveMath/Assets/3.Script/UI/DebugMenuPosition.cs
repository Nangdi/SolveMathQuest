using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenuPosition : MonoBehaviour
{
    public Camera Camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"메뉴위치 : {Camera.WorldToScreenPoint(transform.position)}");
    }
    public void DebugBtn()
    {
        
    }
}
