using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public enum IngredientState
{
    OnBelt,         //在传送带上
    Raw,            // 生的
    Cooked,         // 熟的
    Burned          // 烤焦
}

public enum DropResult
{
    None,
    OnGrill,
    OnTrash,
    OnCustomer
}

[RequireComponent(typeof(Draggable2D))]
public class Ingredient : MonoBehaviour
{
    private Draggable2D _draggable;
    private IngredientAppearance _appearance;

    //食材的状态
    public IngredientData data;//该食材的数据
    public IngredientState state = IngredientState.OnBelt;
    public float currentCook=0;//当前制作食材的进度，0-100表示在制作中，100-200表示已熟，>=200表示已烧焦
    private Coroutine CookCoro;//制作食材的协程
    [HideInInspector]
    public Ingredient sourceIngredient;  // 产生它的源食材（只有克隆出来的才有值）

    [HideInInspector]
    public bool canDrag=false;//是否能被拖动
    [HideInInspector]
    public Collider2D Collider;

    private void Awake()
    {
        _draggable=GetComponent<Draggable2D>();
        _appearance = GetComponent<IngredientAppearance>();
        Collider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        currentCook = 0;
        CookCoro = null;
    }

    

    /// <summary>
    /// 在传送带上被拖拽时
    /// </summary>
    /// <param name="dragStartPos"></param>
    public void OnBeltDrag(Vector3 dragStartPos)
    {
        //当前食材半透明
        _appearance.SetAlpha(0.5f);
        // 根据当前食材的数据克隆一个新的
        Ingredient newIng = IngredientManager.Instance.GetIngredient(data.ingredientName);
        
        // 设置新食材位置（基于拖拽起点 WorldPos）
        Vector3 p = dragStartPos;
        p.z = 0;
        newIng.transform.position = p;

        // 新食材可以被拖动
        newIng.canDrag = true;
        // 4. 建立联动（非常关键！）
        newIng.sourceIngredient = this;

        // 让它立刻开始拖拽
        Draggable2D drag = newIng.GetComponent<Draggable2D>();
        drag.BeginDragImmediate(dragStartPos);
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    public void OnStartDrop()
    {
        if (canDrag == false)
        {
            Ingredient ingredient = IngredientManager.Instance.GetIngredient(data.ingredientName);//获取一个相同的食材
            ingredient.canDrag = true;
            return;
        }
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    public void OnDropping()
    {

    }

    /// <summary>
    /// 新统一接口：Draggable2D 会调用这个方法（与 Draggable2D 匹配）
    /// result: 哪个区域
    /// targetArea: 具体的碰撞体（哪个烤盘/垃圾桶/顾客）
    /// overlapping: 是否与其他食材重叠
    /// </summary>
    public void OnEndDrop(DropResult result, Collider2D targetArea, bool overlapping)
    {
        // 如果没有状态逻辑类（回退实现），使用内部简单分发（便于你快速测试）
        switch (result)
        {
            case DropResult.None:
                if(state== IngredientState.OnBelt)
                {
                    _draggable.MoveToPos(sourceIngredient.transform.position);
                }
                else
                {
                    _draggable.ReturnToOriginal();//食材回到原位
                }
                
                break;

            case DropResult.OnGrill:
                if (!overlapping)//没有与其他食物重叠
                {
                    OnGrillState();
                }
                else
                {
                    if (state == IngredientState.OnBelt)
                    {
                        _draggable.MoveToPos(sourceIngredient.transform.position);
                    }
                    else
                    {
                        _draggable.ReturnToOriginal();//食材回到原位
                    }
                }
                break;

            case DropResult.OnTrash:
                DestroyIngredient();
                break;

            case DropResult.OnCustomer:
                if (state == IngredientState.Cooked)
                {
                    FeedCustomer(targetArea.GetComponent<Customer>());
                }
                else
                {
                    _draggable.ReturnToOriginal();//食材回到原位
                }
                break;
        }
    }

    /// <summary>
    /// 食材移动回到传送带之后
    /// </summary>
    public void OnReturnBelt()
    {
        //食材还没生成，还在传送带上
        if (state == IngredientState.OnBelt)
        {
            sourceIngredient._appearance.SetAlpha(1f);
            DestroyIngredient();//清除这个食材
        }
    }

    /// <summary>
    /// 双击该食材
    /// </summary>
    public void OnDoubleClick()
    {
        //食材熟了，给客人
        if (state == IngredientState.Cooked)
        {
            List<Customer> customers = CustomerManager.Instance.AllCustomer;
            if (customers == null || customers.Count == 0)
                return;
            //找到耐心值最低的顾客
            Customer lowestCustomer = null;
            float lowestPatience = float.MaxValue;
            // 找到饱食度最低的顾客
            foreach (var c in customers)
            {
                if (c == null || c.seat == null) continue;

                if (c.currentPatience < lowestPatience)
                {
                    lowestPatience = c.currentPatience;
                    lowestCustomer = c;
                }
            }
            if (lowestCustomer == null)
                return;
            // ----------- DOTween 移动到顾客那里后再喂 -----------
            if(CookCoro != null)
            {
                StopCoroutine(CookCoro);
                CookCoro=null;
            }
            IngredientUIManager.Instance.RemoveIngredientUI(this);//注销UI

            // 禁止拖拽
            _draggable.isLocked = true;

            // 食材飞向顾客（使用顾客中心位置，也可以替换成 lowestCustomer.seatPoint.position）
            Vector3 targetPos = lowestCustomer.transform.position;

            transform.DOMove(targetPos, 0.35f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // 动画结束再喂
                    FeedCustomer(lowestCustomer);
                });
        }

        //食材烧焦了，给垃圾桶
        if (state == IngredientState.Burned)
        {
            if (CookCoro != null)
            {
                StopCoroutine(CookCoro);
                CookCoro = null;
            }
            IngredientUIManager.Instance.RemoveIngredientUI(this);//注销UI
            // 禁止拖拽
            _draggable.isLocked = true;

            // 食材飞向垃圾桶
            Vector3 targetPos = AreaManager.Instance.trashAreas.transform.position;

            transform.DOMove(targetPos, 0.35f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // 动画结束再消失
                    DestroyIngredient();
                });
        }
    }

    /// <summary>
    /// 到达传送带的终点
    /// </summary>
    public void OnArriveConeryBeltEnd()
    {
        DestroyIngredient();
    }

    //食材放在烤盘上
    private void OnGrillState()
    {
        //将其源食材从传送带上移除
        if (state == IngredientState.OnBelt)
        {
            sourceIngredient.DestroyIngredient();
            IngredientUIManager.Instance.RegisterIngredientUI(this, new Vector3(0, 0.0f, 0));//注册食材的UI
            state = IngredientState.Raw;//现在食材是生的
        }
        if (CookCoro == null) 
        {
            CookCoro = StartCoroutine(CookIe());
        } 
        
    }

    //让这个食材消失，顾客食用之后消失，或者没放到烤盘上消失
    public void DestroyIngredient()
    {
        IngredientUIManager.Instance.RemoveIngredientUI(this);//注销UI
        _draggable.Init();
        IngredientManager.Instance.ConveyorBelt.RemoveIngredient(this);//从传送带上移除
        state = IngredientState.OnBelt;
        currentCook = 0;
        canDrag = false;
        CookCoro = null;
        sourceIngredient = null;
        _appearance.ResetAppearance();
        if (CookCoro != null)
        {
            StopCoroutine(CookCoro);
        }
        IngredientManager.Instance.ReturnIngredient(this);//返回对象池
    }

    private IEnumerator CookIe()
    {
        state = IngredientState.Raw;
        float cookTime = data.cookTime;   // 生->熟 时间
        float burnTime = data.burnTime;   // 熟->焦 时间
        currentCook = 0f;
        // 阶段 1：生 -> 熟 （0 - 100）
        while (currentCook < 100f)
        {
            // 暂停时不烹饪、不刷新UI
            while (LevelManager.Instance.isPaused)
                yield return null;

            currentCook += (100f / cookTime) * Time.deltaTime;
            if (currentCook > 100f) currentCook = 100f;

            state = IngredientState.Raw;
            IngredientUIManager.Instance.UpdateIngredientUI(this);

            yield return null;
        }
        _appearance.SetCooked(); // 已熟
        // 阶段 2：熟 -> 焦 （100 - 200）
        while (currentCook < 200f)
        {
            // 暂停时等待
            while (LevelManager.Instance.isPaused)
                yield return null;

            currentCook += (100f / burnTime) * Time.deltaTime;
            if (currentCook > 200f) currentCook = 200f;

            state = IngredientState.Cooked;
            IngredientUIManager.Instance.UpdateIngredientUI(this);

            yield return null;
        }
        _appearance.SetBurned();                // 已焦
        IngredientUIManager.Instance.RemoveIngredientUI(this); // 注销UI
        state = IngredientState.Burned;
    }


    //喂顾客
    private void FeedCustomer(Customer customer)
    {
        if(customer == null) return;
        customer.BeFeeded(this);
        //食材消失
        DestroyIngredient();

    }
}
