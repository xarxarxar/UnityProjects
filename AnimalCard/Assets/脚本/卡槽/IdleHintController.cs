using UnityEngine;

/// <summary>
/// 玩家空闲提示控制器
/// </summary>
public class IdleHintController : MonoBehaviour
{
    [Header("Refs")]
    public BoardFlowController boardFlow;
    public HintController hintController;

    [Header("Settings")]
    public float idleTime = 5f;

    private float timer;

    void Update()
    {
        // 棋盘忙 → 不计时
        if (boardFlow.IsBusy)
        {
            ResetTimer();
            return;
        }

        // 有任何输入 → 重置
        if (HasPlayerInput())
        {
            ResetTimer();
            return;
        }

        timer += Time.deltaTime;

        if (timer >= idleTime)
        {
            bool showed = hintController.ShowHint();
            ResetTimer();

            // 如果你愿意，这里还能做降级逻辑（比如多次没提示就洗牌）
        }
    }

    private void ResetTimer()
    {
        timer = 0f;
    }

    private bool HasPlayerInput()
    {
        return Input.GetMouseButtonDown(0)
            || Input.touchCount > 0;
    }
}
