using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void Hit()
    {
        AudioManager.instance.PlaySFX("»÷´ò");
        Camera.main.GetComponent<CameraShaker>().Shake();
    }
}
