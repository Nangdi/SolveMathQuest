using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

[Serializable]
class LoadingCircle 
{
    public Transform transform;
    public Image image;
}


public class LoadingAction : MonoBehaviour
{
    [SerializeField]
    private GameObject[] circleObjects ;
    private LoadingCircle[] loadingCircles = new LoadingCircle[12];
    Coroutine loadingCoroutine;
    Tween tween1;
    
    private void Awake()
    {
        for (int i = 0; i < circleObjects.Length; i++)
        {
            LoadingCircle tempLC = new LoadingCircle();
            tempLC.transform = circleObjects[i].GetComponent<Transform>();
            tempLC.image = circleObjects[i].GetComponent<Image>();

            loadingCircles[i] = tempLC;
        }
        StartCoroutine(UpDown_co());

    }
    private void OnEnable()
    {
        loadingCoroutine= StartCoroutine(UpDown_co());
    }




    private void UpMoveCircle(LoadingCircle circle)
    {

        Vector3 startPos = circle.transform.localPosition;
        Color startColor = circle.image.color;
        Vector3 startScale = circle.transform.localScale;

        Vector3 targetVector = circle.transform.localPosition + circle.transform.up * 10;
        //Debug.Log($"현재포지션 : {circle.transform.localPosition}");
        //Debug.Log($"추가할포지션 : {circle.transform.up * 10}");
        //Debug.Log($"최종 포지션 : {targetVector}");
        circle.transform.DOLocalMove(targetVector, 0.6f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutSine).OnComplete(() => circle.transform.localPosition = startPos); ;

        circle.transform.DOScale(1.2f, 0.6f).SetEase(Ease.OutSine).SetLoops(2, LoopType.Yoyo).OnComplete(() => circle.transform.localScale = startScale); ;
        circle.image.DOColor(Color.white, 0.6f).SetEase(Ease.OutSine).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => circle.image.color = startColor); 


    }
    private IEnumerator UpDown_co()
    {
        while (true)
        {
            for (int i = 0; i < loadingCircles.Length; i++)
            {
                UpMoveCircle(loadingCircles[i]);
                yield return new WaitForSeconds(0.2f);
            }
        }

        //StartCoroutine(UpDown_co());
    }
    private IEnumerator LoadingDuration_co()
    {
        while (true)
        {

        }
    }
    

    void OnDisable()
    {
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }
    }

}
