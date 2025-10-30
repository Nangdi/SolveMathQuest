using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour
{
    public TMP_InputField targetInput;
    private HangulComposer composer = new HangulComposer();

    public void OnKeyPress(string key)
    {
        switch (key)
        {
            case "°Á":
                composer.Backspace();
                break;
            case "√ ±‚»≠":
                composer.Clear();
                break;
            default:
                composer.AddKey(key);
                break;
        }
        targetInput.text = composer.GetText();
    }
}
