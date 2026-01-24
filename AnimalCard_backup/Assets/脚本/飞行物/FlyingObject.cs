using DG.Tweening;
using UnityEngine;

public class FlyingObject : MonoBehaviour
{
    private Transform startPos => FlyingManager.instance.startPos;
    private Transform endPos => FlyingManager.instance.endPos;
    private float flyTime => FlyingManager.instance.flyTime;
    private float waveHeight => FlyingManager.instance.waveHeight;
    private float waveFrequency => FlyingManager.instance.waveFrequency;

    private ObjectPool<FlyingObject> _pool;

    // Tween 引用（关键）
    private Tween _moveTween;
    private Tween _waveTween;

    private bool _isCaught;

    /// <summary>
    /// 一开始就在飞行
    /// </summary>
    public void Init()
    {
        transform.localScale = Vector3.one*0.7f;
        _isCaught = false;
        Play();
    }

    /// <summary>
    /// 设置对象池
    /// </summary>
    public void SetPool(ObjectPool<FlyingObject> pool)
    {
        _pool = pool;
    }

    public void ReturnPool()
    {
        _pool.Return(this);
    }

    #region Flying

    public void Play()
    {
        transform.position = startPos.position;

        // 主位移（匀速）
        _moveTween = transform.DOMove(endPos.position, flyTime)
            .SetEase(Ease.Linear);

        // 上下晃动（本地 Y）
        _waveTween = transform
            .DOLocalMoveY(waveHeight, flyTime / waveFrequency)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // 飞行结束（未被抓住）
        _moveTween.OnComplete(() =>
        {
            if (_isCaught) return;

            StopAllTweens();
            ReturnPool();
        });
    }

    #endregion

    #region Catch by Hook（核心）

    /// <summary>
    /// 被抓钩抓到
    /// </summary>
    public void OnCaught()
    {
        if (_isCaught)
            return;

        _isCaught = true;

        // 停止所有飞行动画
        StopAllTweens();

        // 后续位置由 HookController 控制
        // HookController.Update() 里：
        // _caughtTarget.transform.position = hook.position;
    }

    private void StopAllTweens()
    {
        if (_moveTween != null && _moveTween.IsActive())
            _moveTween.Kill();

        if (_waveTween != null && _waveTween.IsActive())
            _waveTween.Kill();

        _moveTween = null;
        _waveTween = null;
    }

    #endregion

    #region Pool

    private void OnDisable()
    {
        // 防止池回收后 Tween 泄漏
        StopAllTweens();
        _isCaught = false;
    }

    #endregion
}
