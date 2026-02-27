using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonSettingPanelManager : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer mappingSqureRenderer;
    [SerializeField]
    private Transform MappingTF;

    // Start is called before the first frame update
    [Header("Move")]
    public float moveSpeed = 0.5f;          // 기본 이동 속도 (unit/sec)
    public float fineMoveMultiplier = 0.1f; // Ctrl 누르면 0.1배 (초정밀)
    public bool useLocalMove = false;

    [Header("Scale")]
    public float scaleSpeed = 0.5f;
    public float fineScaleMultiplier = 0.1f;
    public bool uniformScale = true;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        mappingSqureRenderer.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        HandleMove();
        HandleScale();
    }

    void HandleMove()
    {
        Vector3 input = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow)) input.x -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) input.x += 1f;
        if (Input.GetKey(KeyCode.UpArrow)) input.y += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) input.y -= 1f;

        if (input == Vector3.zero) return;

        float multiplier = 1f;

        // 정밀 모드 (Ctrl)
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            multiplier *= fineMoveMultiplier;

        Vector3 delta = input.normalized * moveSpeed * multiplier * Time.deltaTime;

        if (useLocalMove)
            MappingTF.Translate(delta, Space.Self);
        else
            MappingTF.position += delta;
    }

    void HandleScale()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
            input += 1f;

        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            input -= 1f;

        if (Mathf.Approximately(input, 0f)) return;

        float multiplier = 1f;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            multiplier *= fineScaleMultiplier;

        float amount = scaleSpeed * multiplier * Time.deltaTime * input;

        if (uniformScale)
            MappingTF.localScale += Vector3.one * amount;
        else
            MappingTF.localScale += new Vector3(amount, amount, amount);
    }
    private void OnDisable()
    {
        mappingSqureRenderer.enabled = false;
    }
}
