using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 如果启用了 IAP 模块，则引入 Unity Purchasing 命名空间
#if MODULE_IAP
using UnityEngine.Purchasing;
#endif

namespace Watermelon.IAPStore
{
    // 定义一个抽象类，继承自 MonoBehaviour，并实现 IIAPStoreOffer 接口
    public abstract class IAPStoreOffer : MonoBehaviour, IIAPStoreOffer
    {
        [SerializeField] ProductKeyType productKey; // 商品的键类型，用于唯一标识每个商品

        [Space]
        [SerializeField] IAPButton purchaseButton; // 购买按钮的引用

        public GameObject GameObject => gameObject; // 获取当前游戏对象

        private RectTransform rect; // RectTransform 用于获取该对象的高度
        public float Height => rect.sizeDelta.y; // 获取该对象的高度

        private SimpleBoolSave save; // 保存购买状态的变量
        protected bool Bought => !save.Value; // 判断商品是否已经购买

        private ProductData product; // 商品数据对象

        // 在对象初始化时执行
        protected virtual void Awake()
        {
            rect = GetComponent<RectTransform>(); // 获取当前物体的 RectTransform 组件
        }

        // 初始化商品
        public void Init()
        {
            // 根据 productKey 获取对应的保存数据对象
            save = SaveController.GetSaveObject<SimpleBoolSave>($"Product_{productKey}");

            // 获取商品数据
            product = IAPManager.GetProductData(productKey);

            // 检查商品是否已购买或是非消耗品且已被保存
            if (product.IsPurchased || product.ProductType == ProductType.NonConsumable && save.Value)
            {
                ReapplyOffer(); // 重新应用商品效果
                if (product.ProductType == ProductType.NonConsumable)
                    Disable(); // 禁用商品，如果是非消耗品
                else
                    purchaseButton.Init(productKey); // 初始化购买按钮
            }
            else
            {
                purchaseButton.Init(productKey); // 初始化购买按钮
                IAPManager.OnPurchaseComplete += OnPurchaseComplete; // 订阅购买完成事件
            }
        }

        // 禁用商品，停止监听购买完成事件
        public void Disable()
        {
            IAPManager.OnPurchaseComplete -= OnPurchaseComplete; // 取消订阅事件
            gameObject.SetActive(false); // 隐藏当前游戏对象
        }

        // 购买完成后调用
        private void OnPurchaseComplete(ProductKeyType key)
        {
            if (productKey == key) // 检查当前商品是否与购买完成的商品匹配
            {
                ApplyOffer(); // 应用购买后的效果

                if (product.ProductType == ProductType.NonConsumable) Disable(); // 如果是非消耗品则禁用商品

                save.Value = true; // 更新保存状态为已购买
            }
        }

        // 抽象方法，应用购买后的效果
        protected abstract void ApplyOffer();

        // 抽象方法，重新应用商品效果
        protected abstract void ReapplyOffer();
    }
}
