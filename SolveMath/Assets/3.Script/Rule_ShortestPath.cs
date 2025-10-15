using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule_ShortestPath : MonoBehaviour, IGameRule
{ 
    public bool GameClear(Collider2D col)
    {
        if (col.CompareTag("Arrive"))
        {
            return true;

        }
        return false;
    }

    public bool isRuleViolated(Collider2D col)
    {
        throw new System.NotImplementedException();
    }

    public void ResetData(Collider2D col)
    {
        throw new System.NotImplementedException();
    }
    public void SetDifficultMode(Difficulty difficulty)
    {

      
    }
}
