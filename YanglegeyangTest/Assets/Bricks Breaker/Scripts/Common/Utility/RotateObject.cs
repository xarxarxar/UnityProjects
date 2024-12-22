using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateObject : MonoBehaviour
{

    private Vector3 rot = new Vector3(0f, 0f, 1f);
    public float time = 2f;
    public bool isRight = false;

    private void OnEnable()
    {
        transform.DOKill();
        transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        if (isRight)
        {
            transform.DORotate(new Vector3(0, 0, -360), time, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                .SetLoops(-1);
        }
        else
        {
            transform.DORotate(new Vector3(0, 0, 360), time, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                .SetLoops(-1);
        }

    }
}
