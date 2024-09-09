using UnityEngine;
using DG.Tweening;

public class PaperExplosionEffect : MonoBehaviour
{
    public GameObject paperPrefab; // 纸片的预制体
    public int paperCount = 20; // 纸片数量
    public float explosionRadius = 300f; // 爆炸半径
    public float explosionDuration = 1f; // 爆炸动画持续时间
    public float maxSpawnDelay = 0.3f; // 每个纸片生成的最大随机延迟

    private void OnEnable()
    {
        CreateExplosion();
    }

    void CreateExplosion()
    {
        for (int i = 0; i < paperCount; i++)
        {
            // 生成纸片实例
            GameObject paper = Instantiate(paperPrefab, transform);
            RectTransform paperRect = paper.GetComponent<RectTransform>();

            // 设置纸片的初始位置和旋转
            paperRect.anchoredPosition = Vector2.zero; // 起始于中心
            paperRect.localScale = Vector3.one * 0.5f; // 初始缩放
            paperRect.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360)); // 随机初始旋转

            // 随机散射方向
            Vector2 randomDirection = Random.insideUnitCircle.normalized * explosionRadius;

            // 创建动画链
            Sequence explosionSequence = DOTween.Sequence();

            // 为每个纸片添加一个随机的生成延迟
            float spawnDelay = Random.Range(0f, maxSpawnDelay);
            explosionSequence.AppendInterval(spawnDelay);

            // 添加位置动画：向外移动
            explosionSequence.Append(paperRect.DOAnchorPos(randomDirection, explosionDuration).SetEase(Ease.OutQuad));

            // 添加缩放动画：缩小纸片
            explosionSequence.Join(paperRect.DOScale(0, explosionDuration).SetEase(Ease.InQuad));

            // 添加旋转动画：随机旋转
            explosionSequence.Join(paperRect.DORotate(new Vector3(0, 0, Random.Range(-180f, 180f)), explosionDuration, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));

            // 添加渐变动画：淡出纸片
            explosionSequence.Join(paper.GetComponent<CanvasGroup>().DOFade(0, explosionDuration).SetEase(Ease.OutQuad));

            // 在动画结束时删除纸片对象
            explosionSequence.OnComplete(() => Destroy(paper));
        }

        // 在所有动画结束后进行场景切换
        DOVirtual.DelayedCall(explosionDuration + maxSpawnDelay, () =>
        {
            // 切换到新界面，或者调用场景切换函数
            gameObject.SetActive(false);
        });
    }
}
