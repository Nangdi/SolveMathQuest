using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameRule 
{
    public bool isRuleViolated(Collider2D col);
    public void ResetData(Collider2D col);
}
