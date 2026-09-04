using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;
using UnityEngine.UI;
using TMPro;

public class TestWXFont : MonoBehaviour
{
    public TextMeshProUGUI[] allTexts;
    // Start is called before the first frame update
    void Start()
    {
        allTexts=Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        var feedback = "";
        WX.GetWXFont(feedback, (font) =>
        {
            for(int i = 0; i < allTexts.Length; i++)
            {
                allTexts[i].font = TMP_FontAsset.CreateFontAsset(font);    
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
