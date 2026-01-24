using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance;

    [Header("烤盘区域")]
    public List<Collider2D> grillAreas = new List<Collider2D>();

    [Header("垃圾桶区域")]
    public Collider2D trashAreas;

    [Header("顾客区域")]
    public List<Collider2D> customerAreas = new List<Collider2D>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 添加一个顾客的区域
    /// </summary>
    /// <param name="customer"></param>
    public void AddCustomer(Customer customer)
    {
        Collider2D collider = customer.GetComponent<Collider2D>();
        if (customer != null && collider!=null&&!customerAreas.Contains(collider))
        {
            customerAreas.Add(customer.GetComponent<Collider2D>());
        }
    }

    /// <summary>
    /// 移除一个顾客区域
    /// </summary>
    /// <param name="customer"></param>
    public void RemoveCustomer(Customer customer)
    {
        if (customerAreas.Contains(customer.GetComponent<Collider2D>()))
        {
            customerAreas.Remove(customer.GetComponent<Collider2D>());
        }
    }
}
