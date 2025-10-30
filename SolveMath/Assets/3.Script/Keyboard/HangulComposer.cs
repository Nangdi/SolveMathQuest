using System;
using System.Collections.Generic;
using System.Text;

public class HangulComposer
{
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ÇÑ±Û ÀÚ¸ð ±âº» Å×ÀÌºí
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private static readonly List<string> ÃÊ¼º¸®½ºÆ® = new()
    {
        "¤¡","¤¢","¤¤","¤§","¤¨","¤©","¤±","¤²","¤³","¤µ","¤¶","¤·","¤¸","¤¹","¤º","¤»","¤¼","¤½","¤¾"
    };

    private static readonly List<string> Áß¼º¸®½ºÆ® = new()
    {
        "¤¿","¤À","¤Á","¤Â","¤Ã","¤Ä","¤Å","¤Æ","¤Ç","¤È","¤É","¤Ê","¤Ë","¤Ì","¤Í","¤Î","¤Ï","¤Ð","¤Ñ","¤Ò","¤Ó"
    };

    private static readonly List<string> Á¾¼º¸®½ºÆ® = new()
    {
        "","¤¡","¤¢","¤£","¤¤","¤¥","¤¦","¤§","¤©","¤ª","¤«","¤¬","¤­","¤®","¤¯","¤°",
        "¤±","¤²","¤´","¤µ","¤¶","¤·","¤¸","¤º","¤»","¤¼","¤½","¤¾"
    };

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // °ã¸ðÀ½ / °ã¹ÞÄ§ ¸ÅÇÎ
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private static readonly Dictionary<string, string> °ã¸ðÀ½ = new()
    {
        {"¤Ç¤¿", "¤È"}, {"¤Ç¤À", "¤É"}, {"¤Ç¤Ó", "¤Ê"},
        {"¤Ì¤Ã", "¤Í"}, {"¤Ì¤Ä", "¤Î"}, {"¤Ì¤Ó", "¤Ï"},
        {"¤Ñ¤Ó", "¤Ò"}
    };

    private static readonly Dictionary<string, string> °ã¹ÞÄ§ = new()
    {
        {"¤¡¤µ","¤£"}, {"¤¤¤¸","¤¥"}, {"¤¤¤¾","¤¦"}, {"¤©¤¡","¤ª"},
        {"¤©¤±","¤«"}, {"¤©¤²","¤¬"}, {"¤©¤µ","¤­"}, {"¤©¤¼","¤®"},
        {"¤©¤½","¤¯"}, {"¤©¤¾","¤°"}, {"¤²¤µ","¤´"}
    };

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ³»ºÎ ¹öÆÛ »óÅÂ
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private string ÃÊ = "";
    private string Áß = "";
    private string Á¾ = "";
    private readonly StringBuilder °á°ú = new();

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ÀÔ·Â
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void AddKey(string key)
    {
        // ÇÑ±Û ÀÚ¸ð ¿ÜÀÇ ÀÔ·ÂÀº ¹Ù·Î Ä¿¹Ô
        if (!IsHangulJamo(key))
        {
            Commit();
            °á°ú.Append(key);
            return;
        }

        // ÀÚÀ½ÀÎÁö ¸ðÀ½ÀÎÁö ÆÇº°
        bool isÀÚÀ½ = ÃÊ¼º¸®½ºÆ®.Contains(key) || Á¾¼º¸®½ºÆ®.Contains(key);
        bool is¸ðÀ½ = Áß¼º¸®½ºÆ®.Contains(key);

        // ÀÚÀ½ ÀÔ·Â
        if (isÀÚÀ½)
        {
            if (Áß == "")
            {
                // Áß¼ºÀÌ ºñ¾îÀÖÀ¸¸é ÃÊ¼ºÀ¸·Î
                if (ÃÊ == "")
                    ÃÊ = key;
                else
                {
                    // ÀÌ¹Ì ÃÊ¼ºÀÌ ÀÖÀ¸¸é, °ãÀÚÀ½ °¡´ÉÇÑÁö È®ÀÎ
                    string º¹ÇÕ = ÃÊ + key;
                    if (ÃÊ¼º¸®½ºÆ®.Contains(º¹ÇÕ)) ÃÊ = º¹ÇÕ;
                    else
                    {
                        Commit();
                        ÃÊ = key;
                    }
                }
            }
            else
            {
                // Áß¼ºÀÌ ÀÖ´Ù = Á¾¼º ¶Ç´Â ´ÙÀ½ ÃÊ¼º °¡´É¼º
                if (Á¾ == "")
                {
                    Á¾ = key;
                }
                else
                {
                    // °ã¹ÞÄ§ È®ÀÎ
                    string º¹ÇÕ = Á¾ + key;
                    if (°ã¹ÞÄ§.ContainsKey(º¹ÇÕ))
                    {
                        Á¾ = °ã¹ÞÄ§[º¹ÇÕ];
                    }
                    else
                    {
                        Commit();
                        ÃÊ = key;
                    }
                }
            }
        }
        // ¸ðÀ½ ÀÔ·Â
        else if (is¸ðÀ½)
        {
            if (Áß == "")
            {
                Áß = key;
            }
            else
            {
                // °ã¸ðÀ½ ½Ãµµ
                string º¹ÇÕ = Áß + key;
                if (°ã¸ðÀ½.ContainsKey(º¹ÇÕ))
                    Áß = °ã¸ðÀ½[º¹ÇÕ];
                else
                {
                    Commit();
                    Áß = key;
                }
            }
        }
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Á¶ÇÕ °á°ú °¡Á®¿À±â
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public string GetText()
    {
        if (ÃÊ != "" && Áß != "")
        {
            int ÃÊi = ÃÊ¼º¸®½ºÆ®.IndexOf(ÃÊ);
            int Áßi = Áß¼º¸®½ºÆ®.IndexOf(Áß);
            int Á¾i = Á¾¼º¸®½ºÆ®.IndexOf(Á¾);
            int code = 0xAC00 + (ÃÊi * 21 + Áßi) * 28 + Á¾i;
            return °á°ú.ToString() + char.ConvertFromUtf32(code);
        }
        else if (ÃÊ != "") return °á°ú.ToString() + ÃÊ;
        else return °á°ú.ToString();
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ¹é½ºÆäÀÌ½º
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void Backspace()
    {
        if (Á¾ != "") Á¾ = "";
        else if (Áß != "") Áß = "";
        else if (ÃÊ != "") ÃÊ = "";
        else if (°á°ú.Length > 0) °á°ú.Remove(°á°ú.Length - 1, 1);
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // Ä¿¹Ô (¿Ï¼ºµÈ ±ÛÀÚ °á°ú¿¡ Ãß°¡)
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private void Commit()
    {
        if (ÃÊ != "" && Áß != "")
        {
            int ÃÊi = ÃÊ¼º¸®½ºÆ®.IndexOf(ÃÊ);
            int Áßi = Áß¼º¸®½ºÆ®.IndexOf(Áß);
            int Á¾i = Á¾¼º¸®½ºÆ®.IndexOf(Á¾);
            int code = 0xAC00 + (ÃÊi * 21 + Áßi) * 28 + Á¾i;
            °á°ú.Append(char.ConvertFromUtf32(code));
        }
        ÃÊ = Áß = Á¾ = "";
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // ÃÊ±âÈ­
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    public void Clear()
    {
        ÃÊ = Áß = Á¾ = "";
        °á°ú.Clear();
    }

    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    // À¯Æ¿¸®Æ¼
    // ¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡
    private bool IsHangulJamo(string s)
    {
        return ÃÊ¼º¸®½ºÆ®.Contains(s) || Áß¼º¸®½ºÆ®.Contains(s) || Á¾¼º¸®½ºÆ®.Contains(s);
    }
}
