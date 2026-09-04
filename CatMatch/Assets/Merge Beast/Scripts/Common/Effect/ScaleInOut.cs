using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleInOut : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(FadeIE());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    IEnumerator FadeIE()
    {
        int i = 0;
        while (true)
        {
            i++;
            if (i == 1)
            {
                transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            }
            else
            {
                transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
                i = 0;
            }
            yield return new WaitForSeconds(0.4f);
        }
    }
}
