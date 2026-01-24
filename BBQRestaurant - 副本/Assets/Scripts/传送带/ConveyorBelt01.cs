using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ConveyorBelt01 : MonoBehaviour
{
    [Header("传送带生成点")]
    public Transform bornPoint;
    [Header("传送带起点")]
    public Transform startPoint;
    
    [Header("传送带终点")]
    public Transform endPoint;

    [Header("传送带移动速度")]
    public float speed = 2f;

    [Header("食材前方最小间距")]
    public float minDistance = 0.5f;

    private List<Ingredient> ingredientsOnBelt = new List<Ingredient>();

    private float startToEndDis = 0;//起点到终点的总距离
    private float IngredientTotalLength = 0;//传送带上的食材总长度

    public bool IsFull { get; private set; } = false;

    private void Awake()
    {
        // 计算从起点到终点的向量
        Vector3 startToEnd = endPoint.position - startPoint.position;
        startToEndDis = startToEnd.magnitude;
        Debug.Log($"传送带的长度为{startToEndDis}");
    }

    private void Update()
    {
        UpdateMovementAndFullStatus();
    }

    private void UpdateMovementAndFullStatus()
    {
        for (int i = 0; i < ingredientsOnBelt.Count; i++)
        {
            var ing = ingredientsOnBelt[i];
            if (ing == null) continue;
            //if (ing.isOnHand) continue; // 拿在手上就不动

            //判断该食材是否能移动
            if (i != 0 &&
                BoundsAbout.DistanceBetweenColliders(ing.Collider, ingredientsOnBelt[i-1].Collider)< minDistance)
            {
                continue;
            }
            //已到达边界
            if(BoundsAbout.DistanceBetweenColliderAndPoint(ing.Collider, endPoint.position)< 0.02f)
            {
                continue;
            }

            // --- 移动 ---
            ing.transform.position = Vector3.MoveTowards(
                ing.transform.position,
                endPoint.position,
                speed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// 在传送带放置食材
    /// </summary>
    public void SpawnIngredient(Ingredient ingredient)
    {
        IngredientTotalLength += BoundsAbout.GetLengthAlongComplex(ingredient.Collider, endPoint.position - startPoint.position)+ minDistance;
        if (IngredientTotalLength- minDistance > startToEndDis)
        {
            IsFull = true;
        }
        else
        {
            IsFull = false;
        }
        ingredient.transform.position = bornPoint.position;
        ingredient.gameObject.SetActive(true);
        ingredientsOnBelt.Add(ingredient);
    }

    /// <summary>
    /// 移除传送带上的食材
    /// </summary>
    public void RemoveIngredient(Ingredient ingredient)
    {
        if (ingredientsOnBelt.Contains(ingredient))
        {
            //IngredientManager.Instance.ReturnIngredient(ingredient);
            IngredientTotalLength -= BoundsAbout.GetLengthAlongComplex(ingredient.Collider, endPoint.position - startPoint.position) + minDistance;
            if (IngredientTotalLength - minDistance > startToEndDis)
            {
                IsFull = true;
            }
            else
            {
                IsFull = false;
            }
            ingredientsOnBelt.Remove(ingredient);
        }
            
    }
}
