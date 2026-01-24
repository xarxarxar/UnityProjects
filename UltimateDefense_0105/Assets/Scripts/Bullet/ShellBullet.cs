using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 炮弹型子弹，飞到预定地点爆炸
/// </summary>
public class ShellBullet : Bullet
{
    [HideInInspector]public Vector3 targetPosition;//目标位置
    public override float Speed => 6.0f;
    // 可配置
    private readonly float maxScale = 1.7f;
    private Vector3 startPosition;//起始位置
    private float totalDistance;//到目标点的总距离
    private float explosionRadius = 2.5f;//爆炸半径

    // 初始化（传入起点和目标位置）
    public virtual void Init(int damage, bool isCritical, Vector3 startPos, Vector3 targetPos)
    {
        gameObject.SetActive(false);
        base.Init(damage, isCritical, startPos);
        OnGetFromPool();
        gameObject.SetActive(true);
        targetPosition = targetPos;
        startPosition = startPos;
        totalDistance = Vector3.Distance(transform.position, targetPosition);
    }

    protected override void OnUpdate()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition,
            Speed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime);

        //当前离起点的距离
        // 计算进度 p（从 0 到 1）
        float distFromStart = Vector3.Distance(startPosition, transform.position);
        float p = Mathf.Clamp01(distFromStart / totalDistance);

        // 抛物线形缩放：peak 在 p=0.5
        //float t = 4f * p * (1f - p); // 0..1..0, peak=1 at p=0.5
        float t = Mathf.Pow(4f * p * (1f - p), 2f);  // 强化中点，拉开两端的差异
        float scale = Mathf.Lerp(1.0f, maxScale, t);
        transform.localScale = new Vector3(scale, scale, 1);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ReachTarget();
        }
    }


    private void ReachTarget()
    {
        Enemy[] enemiesInRange = GetEnemiesInRange(transform.position, explosionRadius);
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(IsCritical, Damage);
            }
        }
        ReturnToPool();
    }

    public Enemy[] GetEnemiesInRange(Vector2 center, float radius)
    {
        LayerMask enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] results = Physics2D.OverlapCircleAll(center, radius, enemyLayerMask);
        List<Enemy> enemies = new List<Enemy>();

        foreach (var col in results)
        {
            Enemy e = col.GetComponent<Enemy>();
            if (e != null)
                enemies.Add(e);
        }

        return enemies.ToArray();
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.4f); // 绿色半透明
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

}
