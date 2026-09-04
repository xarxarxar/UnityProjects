using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Critical : MonoBehaviour
{
    public Text text;
    public Image image;

    public void SetUp(string txt, Sprite sprite, Color color)
    {
        text.gameObject.SetActive(sprite == null);
        image.gameObject.SetActive(sprite != null);
        if (sprite != null) image.sprite = sprite;
        else
        {
            text.text = txt;
            text.color = color;
        }
    }

    public void Move(float delay)
    {
        StartCoroutine(MoveIE(delay));
    }

    private IEnumerator MoveIE(float delay)
    {
        yield return new WaitForSeconds(delay);

        var targetPos = transform.position + new Vector3(0, 1f);
        while (targetPos != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }

        ObjectPool.Instance.ReleaseObject(gameObject);
    }
}
