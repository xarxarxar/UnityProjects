using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    // ������Ϸ�ڻ��ҵĿ�����
    public class CurrenciesController : MonoBehaviour
    {
        // ����ģʽ��ʵ��
        private static CurrenciesController currenciesController;

        // �������ݿ⣬�洢���п��û��ҵ���Ϣ
        [SerializeField] CurrenciesDatabase currenciesDatabase;
        public CurrenciesDatabase CurrenciesDatabase => currenciesDatabase;

        // �洢���л��ҵ�����
        private static Currency[] currencies;
        public static Currency[] Currencies => currencies;

        // ������������������ӳ��
        private static Dictionary<CurrencyType, int> currenciesLink;

        // ��ʼ����־
        private static bool isInitialised;

        // ģ���ʼ����ɵĻص��¼�
        private static event SimpleCallback onModuleInitialised;

        // ��ʼ������
        public virtual void Initialise()
        {
            // ����Ѿ���ʼ�������򷵻�
            if (isInitialised) return;

            // ���õ���ʵ��
            currenciesController = this;

            // ��ʼ���������ݿ�
            currenciesDatabase.Initialise();

            // �洢�����
            currencies = currenciesDatabase.Currencies;

            // ���ݻ����������ӻ�����������
            currenciesLink = new Dictionary<CurrencyType, int>();
            for (int i = 0; i < currencies.Length; i++)
            {
                // ��������ͻ�����δ��ӣ��������
                if (!currenciesLink.ContainsKey(currencies[i].CurrencyType))
                {
                    currenciesLink.Add(currencies[i].CurrencyType, i);
                }
                else
                {
                    Debug.LogError(string.Format("[����ϵͳ]: ����Ϊ {0} �Ļ��������ݿ����ظ����!", currencies[i].CurrencyType));
                }

                // ��ȡ����Ļ���״̬
                Currency.Save save = SaveController.GetSaveObject<Currency.Save>("currency" + ":" + (int)currencies[i].CurrencyType);
                if (save.Amount == -1)
                    save.Amount = currencies[i].DefaultAmount;

                // ���û��ҵı���״̬
                currencies[i].SetSave(save);
            }

            // ��ǳ�ʼ�����
            isInitialised = true;

            // ���ó�ʼ����ɵĻص��¼�
            onModuleInitialised?.Invoke();
            onModuleInitialised = null;
        }

        // ���ָ�����ͻ��ҵ������Ƿ��㹻
        public static bool HasAmount(CurrencyType currencyType, int amount)
        {
            return currencies[currenciesLink[currencyType]].Amount >= amount;
        }

        // ��ȡָ�����͵Ļ�������
        public static int Get(CurrencyType currencyType)
        {
            return currencies[currenciesLink[currencyType]].Amount;
        }

        // ��ȡָ�����͵Ļ��Ҷ���
        public static Currency GetCurrency(CurrencyType currencyType)
        {
            return currencies[currenciesLink[currencyType]];
        }

        // ����ָ�����͵Ļ�������
        public static void Set(CurrencyType currencyType, int amount)
        {
            Currency currency = currencies[currenciesLink[currencyType]];

            currency.Amount = amount;

            // ��Ǳ���״̬Ϊ��Ҫ����
            SaveController.MarkAsSaveIsRequired();

            // ���û��ұ���¼�
            currency.InvokeChangeEvent(0);
        }

        // ����ָ�����͵Ļ�������
        public static void Add(CurrencyType currencyType, int amount)
        {
            Currency currency = currencies[currenciesLink[currencyType]];

            currency.Amount += amount;

            // ��Ǳ���״̬Ϊ��Ҫ����
            SaveController.MarkAsSaveIsRequired();

            // ���û��ұ���¼�;
            currency.InvokeChangeEvent(amount);
        }

        // ����ָ�����͵Ļ�������
        public static void Substract(CurrencyType currencyType, int amount)
        {
            Currency currency = currencies[currenciesLink[currencyType]];

            currency.Amount -= amount;

            // ��Ǳ���״̬Ϊ��Ҫ����
            SaveController.MarkAsSaveIsRequired();

            // ���û��ұ���¼�
            currency.InvokeChangeEvent(-amount);
        }

        // ����ȫ�ֻ��ұ���ص�
        public static void SubscribeGlobalCallback(CurrencyChangeDelegate currencyChange)
        {
            for (int i = 0; i < currencies.Length; i++)
            {
                currencies[i].OnCurrencyChanged += currencyChange;
            }
        }

        // ȡ������ȫ�ֻ��ұ���ص�
        public static void UnsubscribeGlobalCallback(CurrencyChangeDelegate currencyChange)
        {
            for (int i = 0; i < currencies.Length; i++)
            {
                currencies[i].OnCurrencyChanged -= currencyChange;
            }
        }

        // ����ģ���ʼ��״̬���û��Ļص�
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

    // ���ұ����ί�ж���
    public delegate void CurrencyChangeDelegate(Currency currency, int difference);
}

// -----------------
// Currencies v1.1
// -----------------
// ������־
// v 1.1
// ? ���ӿɹ����Ĭ������
// v 1.0
// ? �����߼�
