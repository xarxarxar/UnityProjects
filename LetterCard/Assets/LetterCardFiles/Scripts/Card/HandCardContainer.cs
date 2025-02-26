using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

/// <summary>
/// 管理手牌的摆放
/// </summary>
public class HandCardContainer : MonoBehaviour
{
    public bool isSorted=false;//是否需要排序
    // 排序规则
    public enum SortOrder
    {
        ByLetterFirst,  // 先按字母排序
        ByColorFirst    // 先按颜色排序
    }
    public SortOrder currentSortOrder = SortOrder.ByLetterFirst;

    [SerializeField] public int sortCount = 5; // 卡牌单行数量
    [SerializeField] private float horizontalSpacing = 0.1f; // 卡牌横向间距
    [SerializeField] private float verticalSpacing = 1.2f;    // 卡牌纵向间距
    private readonly List<Transform> cards = new List<Transform>();

    private void OnTransformChildrenChanged()
    {
        cards.Clear();
        foreach (Transform t in transform)
        {
            cards.Add(t);
        }
        // 添加卡牌
        ArrangeCards();//排列卡牌
    }

    /// <summary>
    /// 对卡牌排序
    /// </summary>
    private void SortCards()
    {
        List<Transform> childrenList = new List<Transform>();
        foreach (Transform child in transform)
        {
            childrenList.Add(child);
        }
        // 根据不同的排序规则排列子物体
        switch (currentSortOrder)
        {
            case SortOrder.ByLetterFirst:
                // 按字母排序，先大写字母后小写字母，并且按字母顺序和颜色排序
                childrenList = childrenList.OrderBy(card => card.GetComponent<LetterCard>().Letter.ToString().ToLower())
                                           .ThenBy(card => card.GetComponent<LetterCard>().Color)
                                           .ToList();

                // 对大写字母和小写字母分别排序
                var upperCards = childrenList.Where(card => char.IsUpper(card.GetComponent<LetterCard>().Letter)).ToList();
                var lowerCards = childrenList.Where(card => char.IsLower(card.GetComponent<LetterCard>().Letter)).ToList();

                upperCards = upperCards.OrderBy(card => card.GetComponent<LetterCard>().Letter).ThenBy(card => card.GetComponent<LetterCard>().Color).ToList();
                lowerCards = lowerCards.OrderBy(card => card.GetComponent<LetterCard>().Letter).ThenBy(card => card.GetComponent<LetterCard>().Color).ToList();

                // 合并大写字母和小写字母
                childrenList = upperCards.Concat(lowerCards).ToList();
                break;

            case SortOrder.ByColorFirst:
                // 先按颜色排序，红、黄、蓝、绿排序
                var sortedByColor = childrenList.OrderBy(card => card.GetComponent<LetterCard>().Color)
                                                .ThenBy(card => card.GetComponent<LetterCard>().Letter.ToString().ToLower())
                                                .ToList();

                // 对每种颜色的字母（大写和小写）分别排序
                var redCards = sortedByColor.Where(card => card.GetComponent<LetterCard>().Color == 'R').ToList();
                var yellowCards = sortedByColor.Where(card => card.GetComponent<LetterCard>().Color == 'Y').ToList();
                var blueCards = sortedByColor.Where(card => card.GetComponent<LetterCard>().Color == 'B').ToList();
                var greenCards = sortedByColor.Where(card => card.GetComponent<LetterCard>().Color == 'G').ToList();

                // 对每种颜色下的字母进行字母排序
                redCards = redCards.OrderBy(card => card.GetComponent<LetterCard>().Letter).ThenBy(card => card.GetComponent<LetterCard>().Color).ToList();
                yellowCards = yellowCards.OrderBy(card => card.GetComponent<LetterCard>().Letter).ThenBy(card => card.GetComponent<LetterCard>().Color).ToList();
                blueCards = blueCards.OrderBy(card => card.GetComponent<LetterCard>().Letter).ThenBy(card => card.GetComponent<LetterCard>().Color).ToList();
                greenCards = greenCards.OrderBy(card => card.GetComponent<LetterCard>().Letter).ThenBy(card => card.GetComponent<LetterCard>().Color).ToList();

                // 合并所有颜色的卡牌
                childrenList = redCards.Concat(yellowCards).Concat(blueCards).Concat(greenCards).ToList();
                break;
        }

        // 根据排序后的顺序排列子物体
        for (int i = 0; i < childrenList.Count; i++)
        {
            childrenList[i].SetSiblingIndex(i);
        }
    }

    public void ToggleSortOrder()
    {
        if(!isSorted) { return; }
        // 切换排序规则
        currentSortOrder = (SortOrder)(((int)currentSortOrder + 1) % 2);
        SortCards();
    }


    private void ArrangeCards()
    {
        if (isSorted) SortCards();
        List<List<Transform>> rows = new List<List<Transform>>();
        List<Transform> currentRow = new List<Transform>();

        // 将卡牌按每行最多5个分组
        foreach (var card in cards)
        {
            currentRow.Add(card);
            if (currentRow.Count == 5)
            {
                rows.Add(currentRow);
                currentRow = new List<Transform>();
            }
        }
        if (currentRow.Count > 0) rows.Add(currentRow);

        // 计算每行位置
        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var row = rows[rowIndex];
            int cardsInRow = row.Count;
            float yPosition = -rowIndex * verticalSpacing;
            

            // 计算行内卡牌位置
            for (int i = 0; i < cardsInRow; i++)
            {
                float totalWidth = (cardsInRow - 1) * (1 + horizontalSpacing);
                float xPosition = (i - (cardsInRow - 1) / 2f) * (1 + horizontalSpacing);
                //row[i].localPosition = new Vector3(xPosition, yPosition, 0);
                row[i].DOLocalMove(new Vector3(xPosition, yPosition, 0), 0.5f).SetEase(Ease.OutQuad);
            }
        }
    }

    // 水平排列手牌
    private void ArrangeCardsHorizontally(float containerWidth)
    {
        // 获取容器的 RectTransform 并计算左端位置
        RectTransform containerRect = GetComponent<RectTransform>();
        float containerLeftEdge = containerRect.anchoredPosition.x - containerWidth / 2;

        float currentX = containerLeftEdge; // 从容器左端开始
        
        // 将卡牌水平排列
        foreach (Transform card in transform)
        {
            // 设置卡牌的位置
            //card.GetComponent<RectTransform>().DOAnchorPosX(currentX + 0.5f * singleWidth, 0.5f).SetEase(Ease.OutQuad);// 设置缓动效果
            
            //card.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentX + 0.5f * singleWidth, 0);

            //currentX += singleWidth;
            //Debug.Log($"card位置为{card.GetComponent<RectTransform>().anchoredPosition}");
        }
    }

    // 叠放手牌
    private void ArrangeCardsOverlapping(float containerWidth)
    {
        // 获取容器的 RectTransform 并计算左端位置
        RectTransform containerRect = GetComponent<RectTransform>();
        float containerLeftEdge = containerRect.anchoredPosition.x - containerWidth / 2;
        
        float overlapAmount;
        // 如果只有一个卡牌，不需要重叠
        if (transform.childCount == 1) overlapAmount = 0;
        //else overlapAmount = (containerWidth - singleWidth) / (transform.childCount - 1);// 计算每张卡牌的重叠偏移量


        // 从容器的左端开始叠放卡牌
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform card = transform.GetChild(i).GetComponent<RectTransform>();

            // 计算每张卡牌的偏移量，并应用到卡牌位置
            //float offsetX = containerLeftEdge + 0.5f * singleWidth + i* overlapAmount;

            // 只在 Y 轴上移动 UI 元素
           // card.GetComponent<RectTransform>().DOAnchorPosX(offsetX, 0.5f).SetEase(Ease.OutQuad);// 设置缓动效果
            // 设置卡牌的位置
            //card.anchoredPosition = new Vector2(offsetX, 0);
            
            //Debug.Log($"card位置为{card.anchoredPosition}");
        }
    }


   
}
