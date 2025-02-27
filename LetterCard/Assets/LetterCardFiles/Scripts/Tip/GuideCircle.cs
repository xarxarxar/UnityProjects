using DG.Tweening;
using UnityEngine;

public class GuideCircle : MonoBehaviour
{
    public Transform inCricle;
    public Transform middleCircle;
    public Transform outCircle;
    private Sequence circleSequence;

    private void OnEnable()
    {
        // 启动动画循环
        StartLoopingCircle();
    }

    private void OnDisable()
    {
        // 停止动画循环
        StopLoopingCircle();
    }

    private void StartLoopingCircle()
    {
        // 初始化动画序列
        circleSequence = DOTween.Sequence();

        // 设置初始状态
        inCricle.transform.localScale = Vector3.zero;
        middleCircle.transform.localScale = Vector3.zero;
        outCircle.transform.localScale = Vector3.zero;

        // 使用 Join() 让这些动画同时执行
        circleSequence.Append(inCricle.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuart));
        circleSequence.Join(middleCircle.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuart));
        circleSequence.Join(outCircle.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuart));

        // 添加停顿一秒
        circleSequence.AppendInterval(0.4f);  // 停顿

        // 添加回缩动画（也要并排执行）
        circleSequence.Append(inCricle.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InQuart));
        circleSequence.Join(middleCircle.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuart));
        circleSequence.Join(outCircle.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InQuart));


        // 设置循环
        circleSequence.SetLoops(-1, LoopType.Restart); // 无限循环，按顺序从头开始

        // 启动循环动画
        circleSequence.Play();
    }

    private void StopLoopingCircle()
    {
        // 停止并销毁动画
        if (circleSequence != null)
        {
            circleSequence.Kill(); // 停止动画并清除
        }
    }
}
