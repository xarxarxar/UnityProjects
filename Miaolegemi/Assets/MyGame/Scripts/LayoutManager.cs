using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = transform.parent. GetComponent<RectTransform>();
        GridLayoutGroup gridLayoutGroup= GetComponent<GridLayoutGroup>();   
        float xWidth = 0.495f * rectTransform.rect.width;
        float yWidth = 0.481f * xWidth;
        gridLayoutGroup.cellSize = new Vector2(xWidth, yWidth);

        float xSpace= 0.01f* rectTransform.rect.width;
        gridLayoutGroup.spacing = new Vector2(xSpace,0);
    }

}
