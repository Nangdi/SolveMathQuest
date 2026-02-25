using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class pulseDebugPoint : MonoBehaviour
{ 
    [SerializeField]
    SpriteRenderer spriteRenderer;
    WaitForSeconds waitSeconds =  new WaitForSeconds(0.1f);
    // Start is called before the first frame update
    
    void OnEnable()
    {
       
    }
    void Start()
    {
        StartCoroutine(PulseAlpha_co());
    }
    IEnumerator PulseAlpha_co()
    {
        while (true) 
        {
            spriteRenderer.DOFade(1, 0);
            yield return waitSeconds;
            spriteRenderer.DOFade(0, 0);
            yield return waitSeconds;
        }
    }

   
}
