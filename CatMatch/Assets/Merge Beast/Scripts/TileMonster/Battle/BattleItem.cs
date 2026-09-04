using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleItem : MonoBehaviour
{
    // Start is called before the first frame update
    public int currentIndex;
    private int originIndex;
    private RectTransform rectTransform;
    private List<Vector2> listPos = new List<Vector2>();

    private bool canMove = true;
    private DragBattle dragBattle;
    public string text;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        listPos.Clear();
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            listPos.Add(transform.parent.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
        }

        dragBattle = GetComponentInParent<DragBattle>();
    }

    private void OnEnable()
    {

        originIndex = rectTransform.GetSiblingIndex();
        currentIndex = originIndex;
        Debug.Log("BattleItem:" + originIndex + " text:" + text);

    }

    public void ChangeIndex(int value, UnityAction<int> callback)
    {
        // if(!canMove) return;
        canMove = false;
        currentIndex += value;
        if (currentIndex < 0)
        {
            currentIndex = listPos.Count - 1;
        }
        else if (currentIndex > listPos.Count - 1)
        {
            currentIndex = 0;
        }
        Debug.Log("currentIndex:" + currentIndex + " text:" + text);

        if (callback != null && currentIndex == 2)
        {
            callback.Invoke(originIndex);
        }
        rectTransform.DOAnchorPos(listPos[currentIndex], 0.3f).OnComplete(() => canMove = true);
        rectTransform.DOScale(currentIndex == 2 ? new Vector3(1.5f, 1.5f) : new Vector3(0.5f, 0.5f), 0.3f);
    }


}
