using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private RectTransform safeArea;
    private Vector3 saAnchorMin;
    private Vector3 saAnchorMax;


#if UNITY_IOS
    void Awake () {

        float screenRatio = (1.0f * Screen.width) / (1.0f * Screen.height);


        if (screenRatio > 0.6f && screenRatio < 0.7f) {
            //Debug.Log("3:2 iPhones models 4 and earlier");
        } else if (screenRatio > 0.5f && screenRatio < 0.6f) {
            //Debug.Log("16:9 iPhones models 5, SE, 8+");
        } else if (screenRatio > 0.4f && screenRatio < 0.5f) {
            //Debug.Log("19.5:9 iPhones - models X, Xs, Xr, Xsmas");
            safeArea = GetComponent<RectTransform>();
            if (safeArea != null) {
                UpdateSafeArea();
            }
        } else {
            Debug.Log("Find Not iPhones Size");
        }
    }
#endif


    private void UpdateSafeArea()
    {
#if UNITY_EDITOR
        float value = Screen.safeArea.height * (44f / 812f);
        Rect _safeArea = new Rect(0f, 44f, Screen.safeArea.width, Screen.safeArea.height - value);
#else
        Rect _safeArea = new Rect(0f, 66f, Screen.safeArea.width, Screen.safeArea.height + 88f);
#endif
        Vector2 _screen = new Vector2(Screen.width, Screen.height);
        this.saAnchorMin.x = _safeArea.x / _screen.x;
        this.saAnchorMin.y = _safeArea.y / _screen.y;
        this.saAnchorMax.x = (_safeArea.x + _safeArea.width) / _screen.x;
        this.saAnchorMax.y = (_safeArea.y + _safeArea.height) / _screen.y;

        safeArea.anchorMin = saAnchorMin;
        safeArea.anchorMax = saAnchorMax;
        return;
    }

}
