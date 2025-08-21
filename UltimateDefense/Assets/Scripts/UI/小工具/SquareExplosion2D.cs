using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简单 2D 方块爆炸：从原点向四周飞溅并渐隐
/// </summary>
public class SquareExplosion2D : MonoBehaviour
{
    [Header("数量/尺寸/速度")]
    public int count = 30;
    public Vector2 sizeRange = new Vector2(0.06f, 0.14f);   // 每个方块的缩放范围
    public Vector2 speedRange = new Vector2(3f, 8f);        // 初速度范围
    public Vector2 angularSpeedRange = new Vector2(-360f, 360f); // 角速度（度/秒）
    public float lifetime = 0.8f;                           // 生存时间（秒）
    public float drag = 2.0f;                               // 简单阻尼（越大越快减速）
    public Gradient colorOverLife;                          // 颜色随时间（0→1）

    [Header("可选：使用自定义 Sprite")]
    public Sprite overrideSprite;                           // 留空则使用 runtime 生成的 1×1 白贴图

    static Sprite _runtimeWhiteSprite;                      // 复用的 1×1 白精灵

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Explode();
        }
    }

    /// <summary>在 transform.position 触发爆炸</summary>
    public void Explode() => Explode(transform.position);

    /// <summary>在指定位置触发爆炸</summary>
    public void Explode(Vector2 origin)
    {
        if (_runtimeWhiteSprite == null) _runtimeWhiteSprite = CreateWhiteSprite();

        for (int i = 0; i < count; i++)
        {
            // 生成方块对象
            var go = new GameObject("SqPiece");
            go.transform.SetPositionAndRotation(origin, Quaternion.identity);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = overrideSprite != null ? overrideSprite : _runtimeWhiteSprite;
            sr.sortingOrder = 1000; // 保证在前（按需调整）

            // 随机大小/方向/速度/角速度
            float size = Random.Range(sizeRange.x, sizeRange.y);
            go.transform.localScale = new Vector3(size, size, 1f);

            Vector2 dir = Random.insideUnitCircle.normalized;
            float speed = Random.Range(speedRange.x, speedRange.y);
            float angularSpeed = Random.Range(angularSpeedRange.x, angularSpeedRange.y);

            StartCoroutine(PieceRoutine(go, sr, dir * speed, angularSpeed));
        }
    }

    IEnumerator PieceRoutine(GameObject go, SpriteRenderer sr, Vector2 velocity, float angularSpeed)
    {
        float t = 0f;
        Color c0 = Color.white;

        while (t < lifetime)
        {
            float dt = Time.deltaTime;
            t += dt;

            // 简单运动：速度衰减（阻尼），位置积分
            velocity *= Mathf.Clamp01(1f - drag * dt);
            go.transform.position += (Vector3)(velocity * dt);

            // 自转
            go.transform.Rotate(0f, 0f, angularSpeed * dt);

            // 颜色/透明度随时间
            float u = Mathf.Clamp01(t / lifetime);
            if (colorOverLife != null && colorOverLife.colorKeys.Length > 0)
                sr.color = colorOverLife.Evaluate(u);
            else
            {
                // 默认：由白到透明
                c0.a = 1f - u;
                sr.color = c0;
            }

            yield return null;
        }

        Destroy(go);
    }

    // 生成 1x1 白色精灵，避免依赖资源
    static Sprite CreateWhiteSprite()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        var rect = new Rect(0, 0, 1, 1);
        return Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), 100f);
    }
}
