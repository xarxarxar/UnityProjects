using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingManager : MonoBehaviour
{
    public static FlyingManager instance;
    public Transform startPos;
    public Transform endPos;
    public float flyTime = 1.2f;
    public float waveHeight = 0.3f;   // 上下晃动幅度
    public float waveFrequency = 6f;  // 晃动频率

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(Repeat());
    }

    IEnumerator Repeat()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1.5f);
            PoolManager.Instance.GetRandomFlyingObject().Init();
        }
    }
}
