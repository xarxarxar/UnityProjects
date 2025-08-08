using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotating : MonoBehaviour
{
    public float rotateSpeed = 180f; // 每秒旋转角度（单位：度/秒）

    void Update()
    {
        // 以 Z 轴为中心顺时针旋转（适合 2D）
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}
