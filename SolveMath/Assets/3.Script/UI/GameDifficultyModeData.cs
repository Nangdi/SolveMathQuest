using UnityEngine;

[CreateAssetMenu(fileName = "GameModeDifficultData", menuName = "GameMode/difficultData")]
public class GameDifficultyModeData : ScriptableObject
{
    public string modeName;
    //난이도선택시 나오는 맵 3개
    public Sprite[] mapSprites;
    //게임창, 결과창에 나오는 모드이름 sprite
    public Sprite titleSprite;
    public Sprite mapSprite;
    public Sprite[] descriptionSprites;
}
