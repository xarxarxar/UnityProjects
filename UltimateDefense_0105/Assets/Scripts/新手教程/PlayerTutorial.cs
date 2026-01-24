using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NINESOFT.TUTORIAL_SYSTEM;
using WeChatWASM;
using UnityEngine.Events;
using Unity.VisualScripting;

/// <summary>
/// Target Type Enum (Ensures compatibility)
/// </summary>
public enum TargetType
{
    World2DObject, // 2D World Object (SpriteRenderer)
    UIElement      // UI Element (RectTransform)
}

/// <summary>
/// 新手教程
/// </summary>
public class PlayerTutorial : MonoBehaviour
{
    public static PlayerTutorial Instance;

    public TutorialMask tutorialMask;//镂空
    public TutorialDialog tutorialDialog;//对话
    public TutorialArrow tutorialArrow;//箭头

    //新手教程需要用到的物体
    public Transform tower;//炮塔
    public Transform crystal;//城墙
    public RectTransform upgradeRect;//升级区域
    public RectTransform updateButtonRect;//刷新按钮区域
    public RectTransform blessRect;//祝福区域

    //第二步新手指引
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BattleManager.OnStartBattle -= OnStartBattle;
        BattleManager.OnStartBattle += OnStartBattle;
        //Enemy.OnMoveInRange += OnMoveInRange;
    }

    /// <summary>
    /// 开始对战
    /// </summary>
    private void OnStartBattle()
    {
        Debug.Log("执行新手教程");
        //判断玩家是不是新手
        bool isNewPlayer = IsNewPlayer();
        if (isNewPlayer)
        {
            StartTutorial_01();
        }
    }

    private void OnMoveInRange(Enemy enemy)
    {

    }


    //开始新手教程
    private void StartTutorial_01()
    {
        BattleManager.Instance.PauseGame();
        ShowMask(crystal, new Vector2(50,50));
        ShowArrow(crystal, ArrowDirection.Down,200);
        ShowDialog("这是你必须守护的城墙。\r\n它一旦被摧毁，战斗就会结束。", DialogPositionType.Bottom, () =>
        {
            ShowMask(tower, new Vector2(100, 100));
            ShowArrow(tower, ArrowDirection.Left, 100);
            ShowDialog("这是你的炮塔。\r\n现在它还很弱，但别担心，它能变得更强。", DialogPositionType.Bottom, () =>
            {
                ShowMask(upgradeRect, new Vector2(50, 50));
                ShowArrow(upgradeRect, ArrowDirection.Down, 200);
                ShowDialog("在这里，你可以选择不同的增强Buff。\r\n有时候还能遇到折扣惊喜。\r\n如果两个Buff相同，必定打折。", DialogPositionType.Middle, () =>
                {
                    ShowMask(updateButtonRect, new Vector2(50, 50));
                    ShowArrow(updateButtonRect, ArrowDirection.Left, 200);
                    ShowDialog("当前Buff若都已购买，\r\n并且你有足够的金币时，\r\n可以刷新一批新的选项。", DialogPositionType.Middle, () =>
                    {

                        ShowMask(blessRect, new Vector2(50, 150));
                        ShowArrow(blessRect, ArrowDirection.Left, 200);
                        ShowDialog("击败一定数量的敌人时，\r\n可以开启宝箱，\r\n里面会刷新出强力商品", DialogPositionType.Middle, () =>
                        {
                            ShowDialog("敌人马上就要来了。\r\n总共 20 波……\r\n可能很艰难，但我相信你能撑住。", DialogPositionType.Middle, () =>
                            {
                                HideMask();
                                HideArrow();
                                HideDialog();
                                DataManager.Instance.PlayerInfo.SetConfig("NewPlayer", 0);
                                BattleManager.Instance.ResumeGame();
                            });
                        });
                        
                    });
                });
            });
        });
    }

    //判断玩家是否是新手
    private bool IsNewPlayer()
    {
        //判断是否有这个字段
        if (!DataManager.Instance.PlayerInfo.Config.TryGetValue("NewPlayer", out var NewPlayer))
        {
            if (DataManager.Instance.PlayerInfo.TotalPassCount > 0)//已经通关过
            {
                DataManager.Instance.PlayerInfo.SetConfig("NewPlayer", 0);//不是新手
            }
            else
            {
                DataManager.Instance.PlayerInfo.SetConfig("NewPlayer", 1);//是新手
            }
        }

        return DataManager.Instance.PlayerInfo.Config["NewPlayer"] == 1;

    }

    /// <summary>
    /// 显示镂空
    /// </summary>
    public void ShowMask(Transform target, Vector2 sizeOffset)
    {
        tutorialMask.ShowMask(target, sizeOffset);
    }

    /// <summary>
    /// 显示镂空
    /// </summary>
    public void ShowMask(RectTransform target, Vector2 sizeOffset)
    {
        tutorialMask.ShowMask(target, sizeOffset);
    }

    /// <summary>
    /// 隐藏镂空
    /// </summary>
    public void HideMask()
    {
        tutorialMask.Hide();
    }

    /// <summary>
    /// 显示箭头
    /// </summary>
    public void ShowArrow(Transform target, ArrowDirection arrowDirection,float distance)
    {
        tutorialArrow.SetNewTarget(target, arrowDirection, distance);
    }

    /// <summary>
    /// 显示箭头
    /// </summary>
    public void ShowArrow(RectTransform target,ArrowDirection arrowDirection, float distance)
    {
        tutorialArrow.SetNewTarget(target, arrowDirection, distance);
    }

    /// <summary>
    /// 隐藏箭头
    /// </summary>
    public void HideArrow()
    {
        tutorialArrow.HideArrow();
    }

    /// <summary>
    /// 显示对话框
    /// </summary>
    public void ShowDialog(string content,DialogPositionType dialogPositionType,UnityAction callback=null)
    {
        tutorialDialog.ShowDialog(content, dialogPositionType,callback);
    }

    /// <summary>
    /// 隐藏对话框
    /// </summary>
    public void HideDialog()
    {
        tutorialDialog.HideDialog();
    }
}
