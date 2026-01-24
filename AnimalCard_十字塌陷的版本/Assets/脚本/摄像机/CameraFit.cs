using UnityEngine;

public class CameraFit : MonoBehaviour
{
    public float designWidth = 1080f;
    public float designHeight = 1920f;

    void Start()
    {
        Camera cam = Camera.main;

        float targetAspect = designWidth / designHeight;
        float currentAspect = (float)Screen.width / Screen.height;

        if (currentAspect < targetAspect)
        {
            // 屏幕更窄（微信常见）
            float scale = targetAspect / currentAspect;
            cam.orthographicSize *= scale;
        }
    }
}
