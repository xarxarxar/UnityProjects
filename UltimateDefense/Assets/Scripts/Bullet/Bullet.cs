
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    private Enemy _targetEnemy;//目标
    private bool _isCritical;//是否暴击
    private int _damage;//伤害
    [SerializeField]private float _moveSpeed = 50f;//运动速度
    private TrailRenderer _trailRenderer;//拖尾
    private float _baseTrailTime = 0.2f; // 一倍速下的 trail 时间

    /// <summary>
    /// 子弹命中敌人事件，哪个敌人，是否暴击，伤害是多少
    /// </summary>
    //public static event UnityAction<Enemy, bool, int> OnBulletHit;

    private void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        if (_trailRenderer != null)
        {
            _baseTrailTime = _trailRenderer.time;
        }
        BattleManager.OnEndBattle += OnEndBattle;
    }

    private void OnEnable()
    {
        // 重置拖尾
        //if (_trailRenderer != null)
        //{
        //    _trailRenderer.Clear();
        //    _trailRenderer.emitting = true;
        //}
    }

    public void Init(Vector3 position, Enemy enemy,bool isCritical, int damage)
    {
        transform.position = position;
        _targetEnemy = enemy;
        _isCritical = isCritical;
        _damage = damage;
    }

    void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (BattleManager.Instance.IsPaused)
        {
            if (_trailRenderer != null)
                _trailRenderer.emitting = false;
            return;
        }

        if (_trailRenderer != null)
        {
            _trailRenderer.emitting = true;
            _trailRenderer.time = _baseTrailTime / BattleManager.Instance.GameSpeed;
        }

        if (_targetEnemy == null)
        {
            TowerManager.Instance.BulletPool.Return(this);//返回对象池
            return; 
        }
        transform.position = Vector3.MoveTowards(transform.position, _targetEnemy.transform.position, _moveSpeed * BattleManager.Instance.GameSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetEnemy.transform.position) < 0.1f)
        {
            ReachTarget();//到达目标
        }
    }

    private void ReachTarget()
    {
        _targetEnemy.TakeDamage(_isCritical,_damage);
        TowerManager.Instance.BulletPool.Return(this);//返回对象池
    }

    //挑战结束
    private void OnEndBattle(bool success)
    {
        TowerManager.Instance.BulletPool.Return(this);//返回对象池
    }
}
