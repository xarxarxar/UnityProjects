using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Camera _cachedMainCamera;
    private Camera MainCamera
    {
        get
        {
            if (_cachedMainCamera == null)
                _cachedMainCamera = Camera.main;
            return _cachedMainCamera;
        }
    }

    /// <summary>
    /// 对敌人造成伤害后，显示伤害文字的UI
    /// </summary>
    private void OnEnemyDamaged(Transform enemy, bool isCritical, int damage)
    {
        // 世界坐标 → 屏幕坐标
        Vector3 worldPos = enemy.transform.position + Vector3.up * 1.2f;  // 头顶偏移
        Vector3 screenPos = MainCamera.WorldToScreenPoint(worldPos);

        // 创建伤害Text（属于 screen-space canvas）
        //DamageText dmgText = _damageTextPool.Get();  // 用对象池
        //dmgText.Init(screenPos, isCritical, damage);
    }
}
