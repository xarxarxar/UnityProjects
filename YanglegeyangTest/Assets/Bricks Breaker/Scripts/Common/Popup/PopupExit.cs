using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupExit : PopupBase
{
    bool isQuit = false;

    public void Click_Okay()
    {
        if (isQuit) return;
        isQuit = true;
        Application.Quit();
        Debug.Log("Quit");
    }

#if UNITY_ANDROID
    private void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Close();
        }
    }
#endif

}
