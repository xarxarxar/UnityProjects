using System;
using TMPro; // ���ڴ���TextMeshPro�ı�
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // ���ڴ���UI��ť��ͼ��

namespace Watermelon
{
    public class LivesIndicator : MonoBehaviour
    {
        [Space] // ��Inspector�зָ��ֶ�
        [SerializeField] TextMeshProUGUI livesCountText; // ��ʾ����������TextMeshPro�ı����
        [SerializeField] Image infinityImage; // ��ʾ��������ʱ��ͼ��
        [SerializeField] TextMeshProUGUI durationText; // ��ʾ����ʱʱ���������״̬���ı����

        [Space]
        [SerializeField] Button addButton; // ��������İ�ť
        [SerializeField] AddLivesPanel addLivesPanel; // ������ʾ���������������

        // ���ڴ洢��ǰ����������
        private LivesData Data { get; set; }

        // ��־�Ƿ��ѳ�ʼ������ֹ�ظ���ʼ��
        private bool isInitialised;

        /// <summary>
        /// ��ʼ������ָʾ��������LivesData���ݡ�
        /// ����Ѿ���ʼ�����������ظ���ʼ����
        /// </summary>
        public void Init(LivesData data)
        {
            if (isInitialised) return; // ����Ѿ���ʼ������ֱ�ӷ���

            Data = data;

            // ���addLivesPanel���ڣ���ʾ��Ӱ�ť�����õ���¼�
            if (addLivesPanel != null)
            {
                addButton.gameObject.SetActive(true); // ���ť
                addButton.onClick.AddListener(() => addLivesPanel.Show()); // ���õ��ʱ��ʾ����������
            }
            else
            {
                addButton.gameObject.SetActive(false); // �������ذ�ť
            }

            isInitialised = true; // ���Ϊ�ѳ�ʼ��
        }

        /// <summary>
        /// ����������������ʾ״̬��
        /// ��������������ʱ����ʾ��������ͼ�꣬�������������ı���
        /// </summary>
        public void SetInfinite(bool isInfinite)
        {
            infinityImage.gameObject.SetActive(isInfinite); // ��ʾ��������������ͼ��
            livesCountText.gameObject.SetActive(!isInfinite); // ��Ӧ�����ػ���ʾ���������ı�
        }

        /// <summary>
        /// ���õ�ǰ��������������UI��
        /// ����������ﵽ���ֵ��������ť����ʾ�ı���
        /// </summary>
        public void SetLivesCount(int count)
        {
            if (!isInitialised) return; // �����δ��ʼ����ֱ�ӷ���

            livesCountText.text = count.ToString(); // �������������ı�

            // ���������С�����ֵ����addLivesPanel���ڣ�����ʾ��Ӱ�ť
            addButton.gameObject.SetActive(count != Data.maxLivesCount && addLivesPanel != null);

            // ����������ﵽ���ֵ����ʾ����������״̬
            if (count == Data.maxLivesCount)
            {
                FullText(); // ����FullText����������ʾ�ı�
            }
        }

        /// <summary>
        /// ���õ���ʱʱ�䣬�����ݵ���ʱʱ����ʽ���ı���
        /// </summary>
        public void SetDuration(TimeSpan duration)
        {
            if (!isInitialised) return; // �����δ��ʼ����ֱ�ӷ���

            // ����ʱ��ѡ��ͬ��ʱ���ʽ������ʾ
            if (duration >= TimeSpan.FromHours(1))
            {
                durationText.text = string.Format(Data.longTimespanFormat, duration);
            }
            else
            {
                durationText.text = string.Format(Data.timespanFormat, duration);
            }

            // �����ı����С�����ⰴť�ڵ�
            SetTextSize(!addButton.gameObject.activeSelf);
        }

        /// <summary>
        /// ��ʾ������ʱ���ı���Ϣ��
        /// </summary>
        public void FullText()
        {
            if (!isInitialised) return; // �����δ��ʼ����ֱ�ӷ���

            durationText.text = Data.fullText; // ����Ϊ�����������ı�

            SetTextSize(true); // �����ı����СΪȫ��״̬
        }

        /// <summary>
        /// �����Ƿ���ȫ��״̬�����ı���Ĵ�С��
        /// </summary>
        private void SetTextSize(bool fullPanel)
        {
            if (fullPanel)
            {
                durationText.rectTransform.offsetMin = new Vector2(70, 0); // ������߾�
                durationText.rectTransform.offsetMax = new Vector2(-38, 0); // �����ұ߾�
            }
            else
            {
                durationText.rectTransform.offsetMin = new Vector2(95, 0);
                durationText.rectTransform.offsetMax = new Vector2(-100, 0);
            }
        }

        /// <summary>
        /// �����������ʱ��ע�ᵽLivesManager�С�
        /// </summary>
        private void OnEnable()
        {
            LivesManager.AddIndicator(this); // ע�ᵱǰ����ָʾ��
        }

        /// <summary>
        /// �����������ʱ����LivesManager���Ƴ���
        /// </summary>
        private void OnDisable()
        {
            LivesManager.RemoveIndicator(this); // �Ƴ���ǰ����ָʾ��
        }
    }
}
