using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hook : MonoBehaviour
{
    public event UnityAction<FlyingObject> OnCatchFlyObj;
    private HookController _controller;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("有物体进入");
        //if (!_controller._extending || _controller._caughtTarget != null)
        //    return;

        FlyingObject fo = other.GetComponent<FlyingObject>();
        OnCatchFlyObj?.Invoke(fo);
    }
}
