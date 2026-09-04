using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelLoading : MonoBehaviour
{
    public Image image;

    private IEnumerator startIE;
    private float rotatePerMin = 145;

    void Start()
    {
    }

    public void StartLoading()
    {
        gameObject.SetActive(true);
        startIE = StartLoadingIE();
        StartCoroutine(startIE);
    }

    private IEnumerator StartLoadingIE()
    {
        float totalTime = 0f;
        while (true)
        {
            yield return null;
            totalTime += Time.deltaTime;
            image.transform.Rotate(0, 0, rotatePerMin * Time.deltaTime);
            if(totalTime > 2f)
            {
                gameObject.SetActive(false);
                GameManager.instance.StartGame();
                break;
            }
        }
    }

    public void StopLoading()
    {
        if (startIE != null)
        {
            StopCoroutine(startIE);
            startIE = null;
        }
        gameObject.SetActive(false);
    }
}
