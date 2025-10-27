using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
public enum ArrowDir { None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }
public class Rule_Arrow : MonoBehaviour, IGameRule
{

    //이동하고있는방향
    //바꿀수있는방향
    /*dir 구해서 이동하고있는방향 , 바꿀수있는방향과 동일하면 ture
    if 이동하고있는방향과 동일하면
     바꿀수있는방향 업데이트
    if 바꿀수있는방향과 동일하면
    이동하고있는방향 = 바꿀수있는방향
    바꿀수있는방향 = new 방향

     
     */
    public int movingDir;
    public int pendingDir;
    public ArrowDir[,] mazeGrid;     // 미로 배열
    public int rows = 5;
    public int cols = 5;

    public Vector2Int currentGridPos; //서있었던 위치
    public Vector2Int previousGridPos; // 이동한위치
    public Vector2Int targetDir;       // 현재 이동 방향
    public Vector2Int optionDir;       // 다음 변경될 방향
    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 타일이 가진 Grid 좌표 가져오기
        ArrowDir arrow = mazeGrid[currentGridPos.y, currentGridPos.x];  // y,x 순서 주의

        Vector2Int tileDir = ArrowDirToVector(arrow);

        Debug.Log($"▶ Enter Tile ({currentGridPos.x},{currentGridPos.y}) / TileDir: {tileDir}");

        // 1️⃣ 현재 방향과 타일 방향 비교
        bool sameAsMoveDir = tileDir == targetDir;
        bool sameAsNextDir = tileDir == optionDir;

        // 2️⃣ 규칙 처리
        if (sameAsMoveDir)
        {
            // 지금 진행방향과 같으면 → 바꿀 수 있는 방향 갱신
            optionDir = tileDir;
            Debug.Log("현재 진행방향과 동일 → nextDir 갱신");
        }
        else if (sameAsNextDir)
        {
            // nextDir과 같으면 → 이동방향 갱신
            targetDir = optionDir;
            optionDir = tileDir; // 다음 방향 미리 준비
            Debug.Log("nextDir 방향으로 회전 → moveDir 업데이트");
        }
        else
        {
            // 새로운 방향 등장 → 다음 변경 후보로 설정
            optionDir = tileDir;
            Debug.Log("새로운 방향 발견 → nextDir 후보로 설정");
        }

        // 디버그 출력
        Debug.Log($"현재 moveDir={targetDir}, nextDir={optionDir}");
    }

    private Vector2Int ArrowDirToVector(ArrowDir dir)
    {
        switch (dir)
        {
            case ArrowDir.Up: return Vector2Int.up;
            case ArrowDir.Down: return Vector2Int.down;
            case ArrowDir.Left: return Vector2Int.left;
            case ArrowDir.Right: return Vector2Int.right;
            case ArrowDir.UpLeft: return new Vector2Int(-1, 1);
            case ArrowDir.UpRight: return new Vector2Int(1, 1);
            case ArrowDir.DownLeft: return new Vector2Int(-1, -1);
            case ArrowDir.DownRight: return new Vector2Int(1, -1);
            default: return Vector2Int.zero;
        }
    }
    public bool isRuleViolated(Collider2D col)
    {
        //밟았던길 저장
        previousGridPos = currentGridPos;
        //밟은위치 좌표구하기/업데이트
        currentGridPos = MapManager.instance.colliderToIndex[col];
        //밟은위치 화살표 확인
        ArrowDir currentDir = mazeGrid[currentGridPos.y, currentGridPos.x];
        //화살표의 vector값
        Vector2Int currentArrowDir = ArrowDirToVector(currentDir);
        //내가이동한 방향구하기
        int dx = currentGridPos.x - previousGridPos.x;
        int dy = currentGridPos.y - previousGridPos.y;
        Vector2Int movingDir = new Vector2Int(dx, dy);
        Debug.Log($"내가밟은위치 화살표 {currentDir}");
        //내가 가야하는방향과 내가이동항방향 비교
        if(targetDir == movingDir)
        {
            //대체방향만 업데이트 후 계속진행
            Debug.Log("계속진행중");
            optionDir = currentArrowDir;
        }
        else if(optionDir == movingDir)
        {
            //이동방향 변경 
            //타겟업데이트
            Debug.Log("진로변경");
            targetDir = optionDir;
            optionDir = currentArrowDir;
        }
        else
        {
            //가야하는방향과 다른곳으로 이동함
            Debug.Log("틀린방향이동");
            return true;
        }
        Debug.Log($"targetDir : {targetDir} , optionDir : {optionDir} , movingDir : {movingDir} , currentPos : {currentGridPos} , previousPos : {previousGridPos}");



        return false;
    }

    public void ResetData(Collider2D col)
    {
        //스타트에 들어올시점
        //가야하는 방향업데이트 , 바꿀수있는 방향 초기화 , 위치 초기화
        currentGridPos = MapManager.instance.colliderToIndex[col];
        ArrowDir currentDir = mazeGrid[currentGridPos.y, currentGridPos.x];
        targetDir = ArrowDirToVector(currentDir);
        optionDir = Vector2Int.zero;
    }

    public bool GameClear(Collider2D col)
    {
        if (col.CompareTag("Arrive"))
        {
            return true;

        }
        return false;
    }
    public void SetDifficultMode(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.eazy:
                mazeGrid = new ArrowDir[MapManager.instance.arrowDirs1.Length, MapManager.instance.arrowDirs1.Length];
                mazeGrid = MapManager.instance.arrowDirs1;
                break;
            case Difficulty.Normal:
                mazeGrid = new ArrowDir[MapManager.instance.arrowDirs2.Length, MapManager.instance.arrowDirs2.Length];
                mazeGrid = MapManager.instance.arrowDirs2;
                break;
            case Difficulty.Hard:
                mazeGrid = new ArrowDir[MapManager.instance.arrowDirs3.Length, MapManager.instance.arrowDirs3.Length];
                mazeGrid = MapManager.instance.arrowDirs3;
                break;
        }
    }
}
