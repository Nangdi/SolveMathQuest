using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class playerTracingOB : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer tracingOb;
    WaitForSeconds waitSeconds = new WaitForSeconds(1.5f);
    void Start()
    {
        StartCoroutine(PulseAlpha_co());
    }
    IEnumerator PulseAlpha_co()
    {
        while (true)
        {
            tracingOb.DOFade(1, 1.5f);
            yield return waitSeconds;
            tracingOb.DOFade(0.3f, 1.5f);
            yield return waitSeconds;
        }
    }

}
