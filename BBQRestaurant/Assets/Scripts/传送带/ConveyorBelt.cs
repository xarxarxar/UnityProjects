using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public Transform startPoint; // 食材出现的位置
    public Transform cachePoint; // 食材缓存的位置
    public Transform endPoint;   // 食材离开的位置
    public float speed = 2f;     // 移动速度

    private List<Ingredient> ingredientsOnBelt = new List<Ingredient>();
    private List<Ingredient> cacheIngredients = new List<Ingredient>();//食材缓存列表

    private void Update()
    {
        if (ingredientsOnBelt.Count == 0 && cacheIngredients.Count > 0)
        {
            MoveIngredientToBelt(cacheIngredients[0]);
        }
        for (int i = 0; i < ingredientsOnBelt.Count; i++)
        {
            var ing = ingredientsOnBelt[i];
            if (ing == null) continue;
            //if (ing.isOnHand) continue; // 食材在玩家手上

            // --- 前方检测 ---
            bool canMove = true;
            float safetyDistance = 1.5f; // 前方最小间距，你可以暴露为 public

            Vector3 forwardDir = (endPoint.position - ing.transform.position).normalized;

            foreach (var other in ingredientsOnBelt)
            {
                if (other == null || other == ing) continue;

                // 计算当前食材与其他食材的向量
                Vector3 toOther = other.transform.position - ing.transform.position;

                // 判断 other 是否在 ing 前方，并且距离小于 safetyDistance
                if (Vector3.Dot(forwardDir, toOther) > 0 && toOther.magnitude < safetyDistance)
                {
                    canMove = false;
                    break;
                }
            }

            if (!canMove) continue;

            if(i== ingredientsOnBelt.Count - 1)//如果是最后一个，则判断与起点的距离有没有超过安全距离
            {
                // 计算从起点到当前食材的向量
                Vector3 startToIng = ing.transform.position - startPoint.position;
                float distanceFromStart = startToIng.magnitude;

                // 可以暴露为 public 或常量的安全距离
                float startSafetyDistance = 1.5f;

                if (distanceFromStart >= startSafetyDistance && cacheIngredients.Count>0)
                {
                    // 离起点足够远，可以移动
                    MoveIngredientToBelt(cacheIngredients[0]);
                }
            }

            // 沿着传送带移动
            ing.transform.position = Vector3.MoveTowards(
                ing.transform.position, endPoint.position, speed * Time.deltaTime);

            // 到达终点后做处理
            if (Vector3.Distance(ing.transform.position, endPoint.position) < 0.01f)
            {
                ingredientsOnBelt.RemoveAt(i);
                i--;
                // 可触发到达逻辑或回收
                RemoveIngredient(ing);
                ing.OnArriveConeryBeltEnd();
            }
        }
    }


    /// <summary>
    /// 在传送带缓存位置放置一个物体
    /// </summary>
    /// <param name="prefab"></param>
    public void SpawnIngredient(Ingredient ingredient)
    {
        ingredient.transform.position = cachePoint.position;
        ingredient.gameObject.SetActive(false);
        cacheIngredients.Add(ingredient);
    }

    //将食材放到传送带上
    private void MoveIngredientToBelt(Ingredient ingredient)
    {
        cacheIngredients.Remove(ingredient);
        ingredient.transform.position = startPoint.position;
        ingredient.gameObject.SetActive(true);
        ingredientsOnBelt.Add(ingredient);
    }

    /// <summary>
    /// 从传送带移走一个食材
    /// </summary>
    /// <param name="ingredient"></param>
    public void RemoveIngredient(Ingredient ingredient)
    {
        if (ingredientsOnBelt.Contains(ingredient))
        {
            ingredientsOnBelt.Remove(ingredient);
        }
    }
}
