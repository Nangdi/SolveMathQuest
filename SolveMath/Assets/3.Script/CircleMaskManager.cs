using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMaskManager : MonoBehaviour
{
    public Material material;


    public void SetCirclePosition(Vector2 target)
    {
        material.SetVector("_Pos", target);
    }
}
