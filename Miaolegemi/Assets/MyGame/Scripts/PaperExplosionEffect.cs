using UnityEngine;
using DG.Tweening;

public class PaperExplosionEffect : MonoBehaviour
{
    public GameObject paperPrefab; // ֽƬ��Ԥ����
    public int paperCount = 20; // ֽƬ����
    public float explosionRadius = 300f; // ��ը�뾶
    public float explosionDuration = 1f; // ��ը��������ʱ��
    public float maxSpawnDelay = 0.3f; // ÿ��ֽƬ���ɵ��������ӳ�

    private void OnEnable()
    {
        CreateExplosion();
    }

    void CreateExplosion()
    {
        for (int i = 0; i < paperCount; i++)
        {
            // ����ֽƬʵ��
            GameObject paper = Instantiate(paperPrefab, transform);
            RectTransform paperRect = paper.GetComponent<RectTransform>();

            // ����ֽƬ�ĳ�ʼλ�ú���ת
            paperRect.anchoredPosition = Vector2.zero; // ��ʼ������
            paperRect.localScale = Vector3.one * 0.5f; // ��ʼ����
            paperRect.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360)); // �����ʼ��ת

            // ���ɢ�䷽��
            Vector2 randomDirection = Random.insideUnitCircle.normalized * explosionRadius;

            // ����������
            Sequence explosionSequence = DOTween.Sequence();

            // Ϊÿ��ֽƬ���һ������������ӳ�
            float spawnDelay = Random.Range(0f, maxSpawnDelay);
            explosionSequence.AppendInterval(spawnDelay);

            // ���λ�ö����������ƶ�
            explosionSequence.Append(paperRect.DOAnchorPos(randomDirection, explosionDuration).SetEase(Ease.OutQuad));

            // ������Ŷ�������СֽƬ
            explosionSequence.Join(paperRect.DOScale(0, explosionDuration).SetEase(Ease.InQuad));

            // �����ת�����������ת
            explosionSequence.Join(paperRect.DORotate(new Vector3(0, 0, Random.Range(-180f, 180f)), explosionDuration, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));

            // ��ӽ��䶯��������ֽƬ
            explosionSequence.Join(paper.GetComponent<CanvasGroup>().DOFade(0, explosionDuration).SetEase(Ease.OutQuad));

            // �ڶ�������ʱɾ��ֽƬ����
            explosionSequence.OnComplete(() => Destroy(paper));
        }

        // �����ж�����������г����л�
        DOVirtual.DelayedCall(explosionDuration + maxSpawnDelay, () =>
        {
            // �л����½��棬���ߵ��ó����л�����
            gameObject.SetActive(false);
        });
    }
}
