using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class RoleDataUIManager : MonoBehaviour
{
    public static RoleDataUIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    class RoleDataUI
    {
        public Transform roleTransform;
        public RectTransform container;
        public Slider healthSlider;
        public Text healthText;
        public Vector3 worldOffset;
    }
    [SerializeField] private List<RoleDataUI> uiList = new List<RoleDataUI>();

    /// <summary>
    /// 注册角色信息 UI
    /// </summary>
    public void RegisterRoleUI(Role role, Vector3 offset)
    {
        if (uiList.Any(d => d.roleTransform == role.transform))
        {
            Debug.LogWarning($"重复注册UI，忽略");
            return;
        }
        RectTransform rectTransform = PoolManager.Instance.roleDataUIPool.Get();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(role.transform.position);
        rectTransform.position = screenPos;

        GameObject uiObj = rectTransform.gameObject;
        var text = uiObj.GetComponentInChildren<Text>(true);
        Slider hpSlider = uiObj.GetComponentInChildren<Slider>(true);

        RoleDataUI newData = new RoleDataUI
        {
            roleTransform = role.transform,
            container = uiObj.GetComponent<RectTransform>(),
            healthSlider = hpSlider,
            worldOffset = offset,
            healthText= text
        };

        // 初始化血条和血量文本
        if (hpSlider != null)
        {
            //hpSlider.UpdateBar(enemy.CurrentHP.Value, enemy.maxHP.Value,
            //    enemy.CurrentShield.Value, enemy.maxShield.Value);
        }
            

        if (text != null)
        {
            int hp = role.currentHp;
            text.text = $"{hp}";
        }
        
        uiList.Add(newData);
    }

    /// <summary>
    /// 更新角色信息UI
    /// </summary>
    public void UpdateRoleHealth(Role role)
    {
        var data = uiList.Find(d => d.roleTransform == role.transform);
        if (data != null)
        {
            // 更新血条
            if (data.healthSlider != null)
            {
                data.healthSlider.value=(float)(role.currentHp)/role.maxHp;
                data.healthText.text = role.currentHp.ToString();
            }
        }

    }
    /// <summary>
    /// 移除角色信息UI
    /// </summary>
    /// <param name="role"></param>
    public void RemoveRoleUI(Role role)
    {
        var data = uiList.Find(d => d.roleTransform == role.transform);
        if (data != null)
        {
            PoolManager.Instance.roleDataUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
            uiList.Remove(data);
        }

    }

    /// <summary>
    /// 移除所有角色信息UI
    /// </summary>
    /// <param name="role"></param>
    public void RemoveAllRoleUI()
    {
        for (int i = 0;i< uiList.Count; i++)
        {
            var data = uiList[i];
            PoolManager.Instance.roleDataUIPool.Return(data.container.gameObject.GetComponent<RectTransform>());
            uiList.Remove(data);
        }
    }
}
