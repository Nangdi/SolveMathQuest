using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
public class GameSettingData
{
    public bool useUnityOnTop;
    public int[] displayIndex = new int[] { 0, 1, 2 };
    public Vector2 mappingPos = new Vector2(-1.8133f, 0.8125f);
    public Vector2 mappingScale = new Vector2(4.3369f, 4.3369f);
    public bool useClickDebug = false;
    public float ResetScreenTime = 120;
    public int multiDetectCount = 3;
    public float menuHoldTime = 3;
    public float deadLine = 0.01f;

}
public class GameDynamicData
{
}
public class PortJson
{
    public string com = "COM4";
    public int baudLate = 19200;
}

public class JsonManager : MonoBehaviour
{

    public static JsonManager instance;
    public GameSettingData gameSettingData;
    public PortJson portJson;
    public GameDynamicData gameDynamicData;
    private string gameDataPath;
    private string gameDynamicDataPath;
    private string portPath;


    [Header("Custom")]
    [SerializeField]
    private playerMovementController playerMovementController;
    [SerializeField]
    private LidarUDPReceiver lidarUDPReceiver;

    public TMP_InputField screenReset_T;
    public TMP_InputField lidarStopCount_T;
    public TMP_InputField menuHoldTime_T;
    public TMP_InputField deadLine_T;

    [SerializeField]
    private GameObject settingPanel;
    [SerializeField]
    private Transform MappingSquare;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        portPath = Path.Combine(Application.streamingAssetsPath, "port.json");
        gameDynamicDataPath = Path.Combine(Application.streamingAssetsPath, "Setting.json");
        gameDataPath = Path.Combine(Application.persistentDataPath, "gameSettingData.json");
        //gameSettingData.displayIndex = new int[] { 0, 1, 2 };
        gameSettingData = LoadData(gameDataPath, gameSettingData);
        gameDynamicData = LoadData(gameDynamicDataPath, gameDynamicData);
        portJson = LoadData(portPath, portJson);
    }
    private void Start()
    {
        playerMovementController.gameObject.SetActive(gameSettingData.useClickDebug);
        screenReset_T.text = gameSettingData.ResetScreenTime.ToString();
        lidarStopCount_T.text = gameSettingData.multiDetectCount.ToString();
        menuHoldTime_T.text = gameSettingData.menuHoldTime.ToString();
        deadLine_T.text = gameSettingData.deadLine .ToString();
        MappingSquare.transform.position = gameSettingData.mappingPos;
        MappingSquare.transform.localScale = gameSettingData.mappingScale;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
        }
    }
    //저장할 json 객체 , 경로설정
    public static void SaveData<T>(T jsonObject, string path) where T : new()
    {
        if (jsonObject == null)
            jsonObject = new T();  // 기본 생성자로 객체 초기화
        string json = JsonUtility.ToJson(jsonObject, true);
        File.WriteAllText(path, json);
        Debug.Log($"저장됨: {path}");
    }

    public static T LoadData<T>(string path, T data) where T : new()
    {
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning("JSON 파일이 존재하지 않습니다.");
                SaveData(data, path);
            }
            Debug.Log("JSON로드");
            string json = File.ReadAllText(path);
            T jsonData = JsonUtility.FromJson<T>(json);
            return jsonData;
        }

        //예시 실행코드
        //JsonManager.LoadData(파일경로 , 데이터클래스);

    }
    public void SaveData()
    {
       gameSettingData.ResetScreenTime = float.Parse(screenReset_T.text);
       gameSettingData.multiDetectCount = int.Parse(lidarStopCount_T.text);
       gameSettingData.menuHoldTime = float.Parse(menuHoldTime_T.text);
       gameSettingData.deadLine = float.Parse(deadLine_T.text);
        gameSettingData.mappingPos = MappingSquare.position;
       gameSettingData.mappingScale = MappingSquare.localScale;
        lidarUDPReceiver.deadLine = gameSettingData.deadLine;
        SaveData(gameSettingData , gameDataPath);
    }
}
