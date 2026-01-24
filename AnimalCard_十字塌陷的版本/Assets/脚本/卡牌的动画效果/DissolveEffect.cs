using UnityEngine;
using UnityEngine.Events;

public class DissolveGroupEffect : MonoBehaviour
{
    private float speed = 1.2f;

    private float value;
    private bool isPlaying;

    private SpriteRenderer[] renderers;
    private MaterialPropertyBlock mpb;

    private UnityAction _callback;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        mpb = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        ResetDissolve();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Play(() =>
            {
                Debug.Log("所有子物体溶解完毕");
            });
        }

        if (!isPlaying)
            return;

        value += Time.deltaTime * speed;
        value = Mathf.Clamp01(value);

        ApplyDissolve(value);

        if (value >= 1f)
        {
            isPlaying = false;
            _callback?.Invoke();
            _callback = null;
        }
    }

    /// <summary>
    /// 开始溶解（统一控制所有子 Sprite）
    /// </summary>
    public void Play(UnityAction callback = null)
    {
        _callback = callback;
        isPlaying = true;
    }
    /// <summary>
    /// 立即完成动画
    /// </summary>
    public void PlayImmediately()
    {
        value = 1;
    }

    /// <summary>
    /// 立即重置为完整状态
    /// </summary>
    public void ResetDissolve()
    {
        _callback = null;
        isPlaying = false;
        value = 0f;

        ApplyDissolve(value);
    }

    /// <summary>
    /// 将溶解值应用到所有子 SpriteRenderer
    /// </summary>
    private void ApplyDissolve(float dissolve)
    {
        foreach (var sr in renderers)
        {
            if (!sr) continue;

            sr.GetPropertyBlock(mpb);     // 关键：先取
            mpb.SetFloat("_Dissolve", dissolve);
            sr.SetPropertyBlock(mpb);
        }
    }
}
