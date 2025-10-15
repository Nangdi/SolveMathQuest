using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Difficulty
{ 
    eazy,
    Normal,
    Hard
}

public interface IGameRule 
{
    public bool isRuleViolated(Collider2D col);
    public void ResetData(Collider2D col);
    public bool GameClear(Collider2D col);
    public void SetDifficultMode(Difficulty difficulty);
}
