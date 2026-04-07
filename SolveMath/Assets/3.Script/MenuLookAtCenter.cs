using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuLookAtCenter : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform center;
    public Transform myPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateLocation();

    }
    private void updateLocation()
    {
        Vector3 dir = center.position -myPosition.position; 
        transform.up = dir;
    }
}
