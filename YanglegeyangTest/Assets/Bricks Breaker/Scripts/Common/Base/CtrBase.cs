using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrBase : MonoBehaviour
{
    private void Awake()
    {
        PlayManager.Instance.currentBase = this;
    }

    protected virtual void Start()
    {
        GameManager.Instance.Initialized();
    }
}
