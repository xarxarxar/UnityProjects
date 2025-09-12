using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyExplode : MonoBehaviour
{
    public Color flashColor = new Color(1f, 0.6f, 0.3f); // 暖橙色 // 闪红颜色
    private Color _defaultColor;
    private SpriteRenderer spriteRenderer;
    private float _changeScale = 1.5f;//变大的倍数，要乘以最初的倍数

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defalutColor"></param>
    /// <param name="scale"></param>
    public void Init(Vector3 pos,Color defalutColor,float scale,UnityAction callback)
    {
        transform.position = new Vector3(pos.x,pos.y,1);
        transform.localScale =Vector3.one* scale;
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        _defaultColor= defalutColor;
        _changeScale = scale * 1.2f;
        gameObject.SetActive(true);
        PlayExplosion(callback);
    }

    private void PlayExplosion(UnityAction callback)
    {
        float scaleDuration = 1.5f;

        Sequence seq = DOTween.Sequence();

        // 1. 缩放到1.5倍
        Tweener scaleTween = transform.DOScale(_changeScale, scaleDuration);
        scaleTween.timeScale = BattleManager.Instance.GameSpeed.Value;
        seq.Append(scaleTween);

        // 2. 闪红次数固定5次，频率逐渐加快
        int flashCount = 6;
        float singleFlashDuration = 0.5f;
        
        Sequence flashSeq = DOTween.Sequence();
        for (int i = 0; i < flashCount; i++)
        {
            if (singleFlashDuration > 0.1f)
            {
                singleFlashDuration *= Mathf.Pow(0.9f, i);
            }
            singleFlashDuration = Mathf.Max(0.1f, singleFlashDuration);
            
            flashSeq.Append(spriteRenderer.DOColor(flashColor, singleFlashDuration / 2));
            flashSeq.Append(spriteRenderer.DOColor(_defaultColor, singleFlashDuration / 2));
        }
        flashSeq.timeScale = BattleManager.Instance.GameSpeed.Value;
        // 3. 并行播放缩放和闪红
        seq.Join(flashSeq);

        // 4. 最后消失
        seq.AppendCallback(() =>
        {
            ParticleSystem particleSystem = EnemyManager.Instance.ExplosionEffectPool.Get();
            particleSystem.transform.position = transform.position;
            particleSystem.Play();
            EnemyManager.Instance.ExplosionAnimPool.Return(this);
            TimerUtility.Instance.Timer(0.5f, () => { EnemyManager.Instance.ExplosionEffectPool.Return(particleSystem); });
            callback?.Invoke();
        });

    }
}
