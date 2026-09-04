using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    // 管理游戏内货币的控制器
    public class CurrenciesController : MonoBehaviour
    {
        // 单例模式的实例
        private static CurrenciesController currenciesController;

        // 货币数据库，存储所有可用货币的信息
        [SerializeField] CurrenciesDatabase currenciesDatabase;
        public CurrenciesDatabase CurrenciesDatabase => currenciesDatabase;

        // 存储所有货币的数组
        private static Currency[] currencies;
        public static Currency[] Currencies => currencies;

        // 货币类型与其索引的映射
        private static Dictionary<CurrencyType, int> currenciesLink;

        // 初始化标志
        private static bool isInitialised;

        // 模块初始化完成的回调事件
        private static event SimpleCallback onModuleInitialised;

        // 初始化函数
        public virtual void Initialise()
        {
            // 如果已经初始化过，则返回
            if (isInitialised) return;

            // 设置单例实例
            currenciesController = this;

            // 初始化货币数据库
            currenciesDatabase.Initialise();

            // 存储活动货币
            currencies = currenciesDatabase.Currencies;

            // 根据货币类型链接货币与其索引
            currenciesLink = new Dictionary<CurrencyType, int>();
            for (int i = 0; i < currencies.Length; i++)
            {
                // 如果该类型货币尚未添加，进行添加
                if (!currenciesLink.ContainsKey(currencies[i].CurrencyType))
                {
                    currenciesLink.Add(currencies[i].CurrencyType, i);
                }
                else
                {
                    Debug.LogError(string.Format("[货币系统]: 类型为 {0} 的货币在数据库中重复添加!", currencies[i].CurrencyType));
                }

                // 获取保存的货币状态
                Currency.Save save = SaveController.GetSaveObject<Currency.Save>("currency" + ":" + (int)currencies[i].CurrencyType);
                if (save.Amount == -1)
                    save.Amount = currencies[i].DefaultAmount;

                // 设置货币的保存状态
                currencies[i].SetSave(save);
            }

            // 标记初始化完成
            isInitialised = true;

            // 调用初始化完成的回调事件
            onModuleInitialised?.Invoke();
            onModuleInitialised = null;
        }

        // 检查指定类型货币的数量是否足够
        public static bool HasAmount(CurrencyType currencyType, int amount)
        {
            return currencies[currenciesLink[currencyType]].Amount >= amount;
        }

        // 获取指定类型的货币数量
        public static int Get(CurrencyType currencyType)
        {
            return currencies[currenciesLink[currencyType]].Amount;
        }

        // 获取指定类型的货币对象
        public static Currency GetCurrency(CurrencyType currencyType)
        {
            return currencies[currenciesLink[currencyType]];
        }

        // 设置指定类型的货币数量
        public static void Set(CurrencyType currencyType, int amount)
        {
            Currency currency = currencies[currenciesLink[currencyType]];

            currency.Amount = amount;

            // 标记保存状态为需要保存
            SaveController.MarkAsSaveIsRequired();

            // 调用货币变更事件
            currency.InvokeChangeEvent(0);
        }

        // 增加指定类型的货币数量
        public static void Add(CurrencyType currencyType, int amount)
        {
            Currency currency = currencies[currenciesLink[currencyType]];

            currency.Amount += amount;

            // 标记保存状态为需要保存
            SaveController.MarkAsSaveIsRequired();

            // 调用货币变更事件;
            currency.InvokeChangeEvent(amount);
        }

        // 减少指定类型的货币数量
        public static void Substract(CurrencyType currencyType, int amount)
        {
            Currency currency = currencies[currenciesLink[currencyType]];

            currency.Amount -= amount;

            // 标记保存状态为需要保存
            SaveController.MarkAsSaveIsRequired();

            // 调用货币变更事件
            currency.InvokeChangeEvent(-amount);
        }

        // 订阅全局货币变更回调
        public static void SubscribeGlobalCallback(CurrencyChangeDelegate currencyChange)
        {
            for (int i = 0; i < currencies.Length; i++)
            {
                currencies[i].OnCurrencyChanged += currencyChange;
            }
        }

        // 取消订阅全局货币变更回调
        public static void UnsubscribeGlobalCallback(CurrencyChangeDelegate currencyChange)
        {
            for (int i = 0; i < currencies.Length; i++)
            {
                currencies[i].OnCurrencyChanged -= currencyChange;
            }
        }

        // 根据模块初始化状态调用或订阅回调
        public static void InvokeOrSubcrtibe(SimpleCallback callback)
        {
            if (isInitialised)
            {
                callback?.Invoke();
            }
            else
            {
                onModuleInitialised += callback;
            }
        }
    }

    // 货币变更的委托定义
    public delegate void CurrencyChangeDelegate(Currency currency, int difference);
}

// -----------------
// Currencies v1.1
// -----------------
// 更新日志
// v 1.1
// ? 增加可管理的默认数量
// v 1.0
// ? 基本逻辑
