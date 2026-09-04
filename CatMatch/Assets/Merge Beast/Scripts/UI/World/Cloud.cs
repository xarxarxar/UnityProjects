using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public RectTransform rectTransform;
    public float speed = 50f;
    bool left;

    void Start()
    {

    }

    void OnEnable()
    {

    }

    void Update()
    {
        var tempPos = rectTransform.anchoredPosition;
        tempPos += new Vector2((left ? -1 : 1) * Time.deltaTime * speed, 0);
        rectTransform.anchoredPosition = tempPos;
        if (left)
        {
            if (tempPos.x <= -1823)
            {
                left = false;
            }
        }
        else
        {
            if (tempPos.x >= 1658)
            {
                left = true;
            }
        }
    }
}
