using UnityEngine;
using DG.Tweening;
using WeChatWASM;

public class CameraShaker : MonoBehaviour
{
    public float duration = 0.3f;      // 震动时长
    public float strength = 0.5f;      // 震动强度（幅度）
    public int vibrato = 10;           // 抖动频率
    public float randomness = 90f;     // 抖动方向的随机性

    private Vector3 originalPos;
    public Camera cam;

    private void Start()
    {
        originalPos = cam.transform.localPosition;
        //cam= GameObject.Find("UICamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Shake();
        }
    }

    public void Shake()
    {
        cam.transform.localPosition = originalPos;

        cam.transform.DOShakePosition(
            duration,         // 震动时间
            strength,         // 每次震动的强度
            vibrato,          // 震动频率
            randomness,       // 随机程度
            false,            // 不是基于本地坐标
            true              // 抖动回到起始位置
        ).OnComplete(() => {
            cam.transform.localPosition = originalPos;
        });
    }
}
