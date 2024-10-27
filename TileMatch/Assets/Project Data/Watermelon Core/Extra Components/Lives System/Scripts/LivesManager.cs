using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class LivesManager : MonoBehaviour
    {
        private static LivesManager instance; // ����ģʽ������ȫ�ַ��ʸù�����

        [SerializeField] LivesData data; // �������ݵ������࣬���ڴ洢����������������ָ�ʱ������Ϣ

        private static LivesSave save; // ��������������ݵ��࣬������ǰ����������������״̬��

        // ���λָ�ʱ����
        public static int OneLifeInterval;
        // ʹ�����Թ�����������������������ʱ�����SetLifes����
        public static int Lives { get => save.livesCount; private set => SetLifes(value); }
        private static DateTime LivesDate { get => save.date; set => save.date = value; } // ��¼�����ָ�����ʼʱ��

        private static Coroutine livesCoroutine; // �����ָ���Э��

        private static List<LivesIndicator> indicators = new List<LivesIndicator>(); // ����������ʾ������ָʾ���б�
        private static List<AddLivesPanel> addLivesPanels = new List<AddLivesPanel>(); // �����������������б�

        // �жϵ�ǰ�������Ƿ�ﵽ�������ֵ
        public static bool IsMaxLives => Lives == instance.data.customedMaxLivesCount;

        private void Awake()
        {
            // ��Awake�����п��Խ���һЩ��ʼ����������ǰδʵ�֣�
        }

        private void Start()
        {
            instance = this; // ��ʼ������
            OneLifeInterval = instance.data.oneLifeRestorationDuration;//��ȡ���λָ�ʱ����
            save = SaveController.GetSaveObject<LivesSave>("Lives"); // ��ȡ����ʼ���������������
            save.Init(data); // ʹ���������ݳ�ʼ���������

            // ��ʼ��������ʾ
            SetLifes(Lives);

            if (save.infiniteLives)
            {
                // �������������������������������ĵ���ʱ
                Tween.InvokeCoroutine(InfiniteLivesCoroutine());
            }
            else if (Lives < data.customedMaxLivesCount)
            {
                // �������ֵС���������ֵ�������ָ������ĵ���ʱ
                livesCoroutine = Tween.InvokeCoroutine(LivesCoroutine());
            }
        }

        /// <summary>
        /// ���һ������������嵽�б��С�
        /// </summary>
        public static void AddPanel(AddLivesPanel panel)
        {
            if (!addLivesPanels.Contains(panel)) addLivesPanels.Add(panel); // ����б���û�и���壬��ӵ��б�

            if (instance == null)
            {
                Tween.NextFrame(() => {
                    panel.SetLivesCount(Lives); // ����һ֡���������ʾ����������
                });
            }
            else
            {
                panel.SetLivesCount(Lives); // �������������ʾ����������
            }
        }

        /// <summary>
        /// ���б����Ƴ�����������塣
        /// </summary>
        public static void RemovePanel(AddLivesPanel panel)
        {
            addLivesPanels.Remove(panel); // �Ƴ�ָ�����
        }

        /// <summary>
        /// ���һ������ָʾ�����б��С�
        /// </summary>
        public static void AddIndicator(LivesIndicator indicator)
        {
            if (!indicators.Contains(indicator)) indicators.Add(indicator); // ����б���û�и�ָʾ������ӵ��б�

            if (instance == null)
            {
                Tween.NextFrame(() => {
                    indicator.Init(instance.data); // ��ʼ��ָʾ��

                    indicator.SetLivesCount(Lives); // ���õ�ǰ������
                    indicator.SetInfinite(save.infiniteLives); // �����Ƿ�Ϊ��������
                });
            }
            else
            {
                indicator.Init(instance.data); // ��ʼ��ָʾ��

                indicator.SetLivesCount(Lives); // ���õ�ǰ������
                indicator.SetInfinite(save.infiniteLives); // �����Ƿ�Ϊ��������
            }
        }

        /// <summary>
        /// ���б����Ƴ�����ָʾ����
        /// </summary>
        public static void RemoveIndicator(LivesIndicator indicator)
        {
            indicators.Remove(indicator); // �Ƴ�ָ��ָʾ��
        }

        /// <summary>
        /// ���õ�ǰ������������֪ͨ����ָʾ������������ʾ��
        /// </summary>
        private static void SetLifes(int value)
        {
            save.livesCount = value; // ���±����������

            // ֪ͨ����ָʾ����������������������״̬
            foreach (var indicator in indicators)
            {
                indicator.SetLivesCount(Lives);
                indicator.SetInfinite(save.infiniteLives);
            }

            // ֪ͨ����������������
            foreach (var panel in addLivesPanels)
            {
                panel.SetLivesCount(value);
            }
        }

        /// <summary>
        /// ����һ������������������������߲��ɼ����������򲻻���١�
        /// </summary>
        public static void RemoveLife()
        {
            if (save.infiniteLives || !DoNotSpendLivesMenu.CanLivesBeSpent()) return; // ����������������߲��������������ֱ�ӷ���

            Lives--; // ��������һ

            if (Lives < 0)
                Lives = 0; // ��������С��0

            if (livesCoroutine == null)
            {
                LivesDate = DateTime.Now; // ��¼����������ʱ��
                livesCoroutine = Tween.InvokeCoroutine(instance.LivesCoroutine()); // �����ָ�������Э��
            }
        }

        /// <summary>
        /// ����һ�����������û�дﵽ�����������
        /// </summary>
        public static void AddLife()
        {
            if (Lives < instance.data.customedMaxLivesCount)
                Lives++; // ��������һ
        }

        /// <summary>
        /// ����������Э�̣�ֱ����������ʱ�������
        /// </summary>
        private IEnumerator InfiniteLivesCoroutine()
        {
            var wait = new WaitForSeconds(0.25f); // ÿ0.25����һ��
            while (DateTime.Now < save.date) // ����������δ����
            {
                var span = save.date - DateTime.Now; // ����ʣ��ʱ��

                // ֪ͨ����ָʾ������ʣ��ʱ��
                foreach (var indicator in indicators)
                {
                    indicator.SetDuration(span);
                }

                yield return wait; // �ȴ�
            }

            save.infiniteLives = false; // ������������

            SetLifes(data.customedMaxLivesCount); // ����������Ϊ���ֵ

            // ��������ָʾ��������������״̬
            foreach (var indicator in indicators)
            {
                indicator.FullText();
            }

            foreach (var panel in addLivesPanels)
            {
                panel.SetTime(data.fullText);
            }

            yield return LivesCoroutine(); // �������������ָ�Э��
        }

        /// <summary>
        /// �ָ�������Э�̣�ÿ��һ��ʱ��ָ�һ��������
        /// </summary>
        private IEnumerator LivesCoroutine()
        {
            var wait = new WaitForSeconds(0.25f); // ÿ0.25����һ��
            while (Lives < data.customedMaxLivesCount) // ��������δ�ﵽ���ֵ
            {
                var oneLifeSpan = TimeSpan.FromSeconds(data.oneLifeRestorationDuration);
                var timespan = DateTime.Now - LivesDate; // �����ϴ��������ٵ����ڵ�ʱ��

                if (timespan >= TimeSpan.FromSeconds(OneLifeInterval))// �ָ�һ��������Ҫ��ʱ��
                {
                    Lives++; // ����һ������

                    LivesDate = DateTime.Now; // ���»ָ�ʱ��
                }

                // ֪ͨ����ָʾ���������µ���ʱ
                foreach (var indicator in indicators)
                {
                    indicator.SetDuration(oneLifeSpan - timespan);
                }

                foreach (var panel in addLivesPanels)
                {
                    panel.SetTime(string.Format(data.timespanFormat, oneLifeSpan - timespan));
                }

                yield return wait; // �ȴ�0.25��
            }

            // �ָ���������ʱ��֪ͨ����ָʾ����������������״̬
            foreach (var indicator in indicators)
            {
                indicator.FullText();
            }

            foreach (var panel in addLivesPanels)
            {
                panel.SetTime(data.fullText);
            }

            livesCoroutine = null; // ֹͣЭ��
        }

        /// <summary>
        /// ���ü��ٵ��λָ����ĵ�ʱ������20241024���
        /// </summary>
        /// <param name="seconds"></param>
        public static  void RemoveOneLifeTime(int seconds)
        {
            OneLifeInterval -= seconds;
            instance.data.oneLifeRestorationDuration-=seconds;
        }

        /// <summary>
        /// ��������һ���������ֵ��20241024���
        /// </summary>
        public static  void AddMaxLife()
        {
            instance.data.customedMaxLivesCount++;
        }

        /// <summary>
        /// ��ȡ��ǰ��ʱ��ָ������20241026���
        /// </summary>
        public static int CurrentLiveInterval()
        {
            return instance.data.oneLifeRestorationDuration;
        }

        /// <summary>
        /// ��ȡ��ǰ���������ֵ��20241026���
        /// </summary>
        public static int CurrentMaxLiveCount()
        {
            return instance.data.customedMaxLivesCount;
        }

        /// <summary>
        /// ������������������ָ����������
        /// </summary>
        public static void StartInfiniteLives(float duration)
        {
            instance.InfiniteLives(duration);
        }

        /// <summary>
        /// �ڲ��������������������������ý���ʱ�䡣
        /// </summary>
        private void InfiniteLives(float duration)
        {
            save.infiniteLives = true; // ����Ϊ��������
            save.date = DateTime.Now + TimeSpan.FromSeconds(duration); // �������������Ľ���ʱ��

            SetLifes(data.customedMaxLivesCount); // ����������Ϊ���ֵ

            if (livesCoroutine != null)
            {
                Tween.StopCustomCoroutine(livesCoroutine); // ֹ֮ͣǰ�������ָ�Э��
                livesCoroutine = null;
            }
            Tween.InvokeCoroutine(InfiniteLivesCoroutine()); // ������������Э��
        }

        // �����࣬�������л�����������Ϣ
        private class LivesSave : ISaveObject
        {
            public int livesCount; // ��¼��ǰ��������
            public bool infiniteLives; // �Ƿ�����������״̬

            public long dateBinary; // ��������ʽ�����ʱ���
            public DateTime date; // �ָ�ʱ��

            [SerializeField] bool firstTime = true; // �Ƿ��ǵ�һ�ν�����Ϸ

            public void Init(LivesData data)
            {
                if (firstTime)
                {
                    firstTime = false;

                    livesCount = data.maxLivesCount; // ��ʼ��Ϊ���������
                    data.customedMaxLivesCount = data.maxLivesCount;//��ʼ��Ĭ�ϵ��������ֵ
                    data.oneLifeRestorationDuration = data.defaultLifeRestorationDuration;//��ʼ��Ĭ�ϵ������ָ����
                    date = DateTime.Now; // ��¼��ǰʱ��
                }
                else
                {
                    date = DateTime.FromBinary(dateBinary); // �Ӷ����ƻָ�ʱ��

                    if (infiniteLives)
                    {
                        livesCount = data.customedMaxLivesCount; // �������������״̬������������Ϊ���ֵ

                        if (DateTime.Now >= date) infiniteLives = false; // ������������Ƿ��ѹ���
                    }

                    if (livesCount < data.customedMaxLivesCount)
                    {
                        var timeDif = DateTime.Now - date; // ����ָ�ʱ���

                        var oneLifeSpan = TimeSpan.FromSeconds(data.oneLifeRestorationDuration); // ÿ�������ָ���Ҫ��ʱ��

                        // �����ָ�������ֱ���ָ�������û���㹻��ʱ���
                        while (timeDif >= oneLifeSpan && livesCount < data.customedMaxLivesCount)
                        {
                            timeDif -= oneLifeSpan;
                            date += oneLifeSpan;

                            livesCount++;
                        }
                    }
                }
            }

            public void Flush()
            {
                dateBinary = date.ToBinary(); // ����ǰʱ�䱣��Ϊ������
            }
        }



        #region Development

        // �������԰�ť���ֶ����һ������
        [Button("Add Life")]
        private void AddLifeDev()
        {
            AddLife();
        }

        // �������԰�ť���ֶ�����һ������
        [Button("Remove Life")]
        private void RemoveLifeDev()
        {
            RemoveLife();
        }

        #endregion
    }
}
