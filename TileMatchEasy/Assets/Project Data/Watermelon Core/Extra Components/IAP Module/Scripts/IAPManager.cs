using System.Collections.Generic;
using UnityEngine;

#if MODULE_IAP
using UnityEngine.Purchasing; // 引入 Unity 的 IAP 库
#endif

namespace Watermelon
{
    [HelpURL("https://docs.google.com/document/d/1GlS55aF4z4Ddn4a1QCu5h0152PoOb29Iy4y9RKZ9Y9Y")] // 帮助文档链接
    public static class IAPManager
    {
        // 商品类型到商品链接的字典
        private static Dictionary<ProductKeyType, IAPItem> productsTypeToProductLink = new Dictionary<ProductKeyType, IAPItem>();
        // 商品 ID 到商品链接的字典
        private static Dictionary<string, IAPItem> productsKeyToProductLink = new Dictionary<string, IAPItem>();

        // 初始化状态
        private static bool isInitialised = false;
        public static bool IsInitialised => isInitialised; // 是否已经初始化

        // IAP Wrapper，用于与 IAP 系统交互
        private static BaseIAPWrapper wrapper;

        // 购买模块初始化完成事件
        public static event SimpleCallback OnPurchaseModuleInitted;
        // 购买完成事件
        public static event ProductCallback OnPurchaseComplete;
        // 购买失败事件
        public static event ProductFailCallback OnPurchaseFailded;

        // 初始化 IAP 管理器
        public static void Initialise(GameObject initObject, IAPSettings settings)
        {
            // 如果已经初始化，打印日志并返回
            if (isInitialised)
            {
                Debug.Log("[IAP Manager]: Module is already initialized!");
                return;
            }

            // 从设置中获取商店商品
            IAPItem[] items = settings.StoreItems;
            for (int i = 0; i < items.Length; i++)
            {
                // 将商品类型和 ID 添加到字典中
                productsTypeToProductLink.Add(items[i].ProductKeyType, items[i]);
                productsKeyToProductLink.Add(items[i].ID, items[i]);
            }

#if MODULE_IAP
            wrapper = new IAPWrapper(); // 初始化 IAP Wrapper
#else
            wrapper = new DummyIAPWrapper(); // 如果未启用 IAP，使用虚拟 Wrapper
#endif

            // 调用 Wrapper 的初始化方法
            wrapper.Initialise(settings);
        }

        // 根据商品类型获取 IAP 商品
        public static IAPItem GetIAPItem(ProductKeyType productKeyType)
        {
            if (productsTypeToProductLink.ContainsKey(productKeyType))
                return productsTypeToProductLink[productKeyType];

            return null; // 如果未找到，返回 null
        }

        // 根据商品 ID 获取 IAP 商品
        public static IAPItem GetIAPItem(string ID)
        {
            if (productsKeyToProductLink.ContainsKey(ID))
                return productsKeyToProductLink[ID];

            return null; // 如果未找到，返回 null
        }

#if MODULE_IAP
        // 根据商品类型获取产品对象
        public static Product GetProduct(ProductKeyType productKeyType)
        {
            IAPItem iapItem = GetIAPItem(productKeyType);
            if (iapItem != null)
            {
                return IAPWrapper.Controller.products.WithID(iapItem.ID); // 返回对应的 Product 对象
            }

            return null; // 如果未找到，返回 null
        }
#endif

        // 恢复购买
        public static void RestorePurchases()
        {
            wrapper.RestorePurchases(); // 调用 Wrapper 的恢复购买方法
        }

        // 订阅购买模块初始化事件
        public static void SubscribeOnPurchaseModuleInitted(SimpleCallback callback)
        {
            if (isInitialised)
                callback?.Invoke(); // 如果已经初始化，直接调用回调
            else
                OnPurchaseModuleInitted += callback; // 否则，将回调订阅到事件中
        }

        // 购买商品
        public static void BuyProduct(ProductKeyType productKeyType)
        {
            wrapper.BuyProduct(productKeyType); // 调用 Wrapper 的购买方法
        }

        // 获取商品数据
        public static ProductData GetProductData(ProductKeyType productKeyType)
        {
            return wrapper.GetProductData(productKeyType); // 调用 Wrapper 的获取商品数据方法
        }

        // 检查是否已订阅商品
        public static bool IsSubscribed(ProductKeyType productKeyType)
        {
            return wrapper.IsSubscribed(productKeyType); // 调用 Wrapper 的检查订阅状态方法
        }

        // 获取商品的本地价格字符串
        public static string GetProductLocalPriceString(ProductKeyType productKeyType)
        {
            ProductData product = GetProductData(productKeyType);

            if (product == null)
                return string.Empty; // 如果未找到，返回空字符串

            return string.Format("{0} {1}", product.ISOCurrencyCode, product.Price); // 返回格式化后的价格字符串
        }

        // 模块初始化完成
        public static void OnModuleInitialised()
        {
            isInitialised = true; // 设置初始化状态为 true

            OnPurchaseModuleInitted?.Invoke(); // 调用初始化完成事件

            Debug.Log("[IAPManager]: Module is initialized!"); // 打印日志
        }

        // 购买完成事件处理
        public static void OnPurchaseCompled(ProductKeyType productKey)
        {
            OnPurchaseComplete?.Invoke(productKey); // 调用购买完成事件
        }

        // 购买失败事件处理
        public static void OnPurchaseFailed(ProductKeyType productKey, Watermelon.PurchaseFailureReason failureReason)
        {
            OnPurchaseFailded?.Invoke(productKey, failureReason); // 调用购买失败事件
        }

        // 商品回调委托
        public delegate void ProductCallback(ProductKeyType productKeyType);
        // 购买失败回调委托
        public delegate void ProductFailCallback(ProductKeyType productKeyType, Watermelon.PurchaseFailureReason failureReason);
    }

    // 商品数据类
    public class ProductData
    {
        public ProductType ProductType { get; } // 商品类型
        public bool IsPurchased { get; } // 是否已购买

        public decimal Price { get; } // 商品价格
        public string ISOCurrencyCode { get; } // ISO 货币代码

        public bool IsSubscribed { get; } // 是否已订阅

#if MODULE_IAP
        public Product Product { get; } // 商品对象
#endif

        // 商品数据构造函数（仅商品类型）
        public ProductData(ProductType productType)
        {
            ProductType = productType;
            Price = 0.00m; // 默认价格为 0
            ISOCurrencyCode = "USD"; // 默认货币为美元
            IsPurchased = false; // 默认未购买
            IsSubscribed = false; // 默认未订阅
        }

#if MODULE_IAP
        // 商品数据构造函数（根据 Product 对象）
        public ProductData(Product product)
        {
            Product = product;

            ProductType = (ProductType)product.definition.type; // 获取商品类型

            IsPurchased = product.hasReceipt; // 根据是否有收据确定是否已购买

            Price = product.metadata.localizedPrice; // 获取本地化价格
            ISOCurrencyCode = product.metadata.isoCurrencyCode; // 获取 ISO 货币代码
        }
#endif
    }

    // 购买失败原因枚举
    public enum PurchaseFailureReason
    {
        PurchasingUnavailable = 0, // 购买不可用
        ExistingPurchasePending = 1, // 现有购买待处理
        ProductUnavailable = 2, // 商品不可用
        SignatureInvalid = 3, // 签名无效
        UserCancelled = 4, // 用户取消
        PaymentDeclined = 5, // 支付被拒绝
        DuplicateTransaction = 6, // 重复交易
        Unknown = 7 // 未知错误
    }

    // 商品类型枚举
    public enum ProductType
    {
        Consumable = 0, // 可消耗商品
        NonConsumable = 1, // 非消耗商品
        Subscription = 2 // 订阅商品
    }
}

// -----------------
// IAP Manager v 1.2.2
// -----------------
// 更新日志
// v 1.2.2
// • 修复序列化错误
// v 1.2.1
// • 添加测试模式
// v 1.2
// • 支持 IAP 版本 4.11.0
// • 添加编辑器购买包装器
// v 1.1
// • 支持 IAP 版本 4.9.3
// v 1.0.3
// • 支持 IAP 版本 4.7.0
// v 1.0.2
// • 通过 GetProductLocalPriceString 方法快速访问 IAP 的本地价格
// v 1.0.1
// • 添加恢复状态消息
// v 1.0.0
// • 添加文档
// v 0.4
// • IAPStoreListener 继承自 MonoBehaviour
// v 0.3
// • 编辑器样式更新
// v 0.2
// • IAPManager 结构更改
// • 防止序列化问题的枚举复制
// v 0.1
// • 添加基本版本
