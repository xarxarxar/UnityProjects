using System.Collections;
using UnityEngine;

public class SunFlower : MonoBehaviour
{
    private int _bulletCount;
    private float _bulletDamage;
    private float _speed=1f;
    private Vector3 _shootDir;
    private float _currentAngle;

    public void Init(int count,float damage,Vector3 pos)
    {
        _bulletCount=count;
        _bulletDamage = damage;
        _currentAngle = 0f; // 0° = 正上方
        transform.position = pos;   
        StartCoroutine(Shoot());
        //存活周期为10秒
        TimerUtility.Instance.Timer(10, () =>
        {
            Destroy(gameObject);
        });
    }

    void Update()
    {
        // 每秒向上移动 speed 个单位
        transform.Translate(Vector2.up * _speed * Time.deltaTime* BattleManager.Instance.GameSpeed.Value);
    }

    private IEnumerator Shoot()
    {
        while (_bulletCount > 0)
        {
            //每次射击，角度都偏移30度
            _shootDir = Quaternion.Euler(0, 0, -_currentAngle) * Vector2.up;
            PierceBullet bullet = TowerManager.Instance.GetBullet<PierceBullet>();
            //bullet.gameObject.SetActive(false);
            //bullet.transform.position = transform.position;
            //bullet.OnGetFromPool();   //清空拖尾
            //bullet.gameObject.SetActive(true);
            bullet.Init((int)_bulletDamage, false, transform.position, _shootDir, false);
            _bulletCount--;
            _currentAngle += 30f; // 顺时针偏移 30°
            yield return TimerUtility.WaitForGameSeconds(0.1f);
        }
        yield break;
    }


}
