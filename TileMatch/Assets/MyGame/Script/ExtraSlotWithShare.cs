using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Watermelon;
using UnityEngine.UI;

namespace Watermelon.IAPStore
{
    public class ExtraSlotWithShare : MonoBehaviour, IIAPStoreOffer
    {
        [Space]
        [SerializeField] string description = "����֮���ȡһ�����⿨�۵���"; // ��ʾ�������ı�
        [SerializeField] TMP_Text descriptionText; // ��ʾ�������ı����

        [Space]
        [SerializeField] TMP_Text timerText; // ��ʾ��ʱ�����ı����
        [SerializeField] int timerDurationInMinutes; // ��ʱ���ĳ���ʱ�䣨���ӣ�

        [Space]
        [SerializeField] Button button; // ��ȡ��ҵİ�ť

        public GameObject GameObject => gameObject; // ��ȡ��ǰ��Ϸ����

        private RectTransform rect; // ��ǰ����� RectTransform
        public float Height => rect.sizeDelta.y; // ��ȡ����ĸ߶�

        SimpleLongSave save; // �����ʱ��״̬�Ķ���
        DateTime timerStartTime; // ��ʱ����ʼ��ʱ��

        private void Awake()
        {
            // ��ȡ��ǰ����� RectTransform
            rect = GetComponent<RectTransform>();
        }

        public void Init()
        {
            // �ӱ����������ȡ��ʱ��״̬
            save = SaveController.GetSaveObject<SimpleLongSave>("Shuffle With Share");

            // �ӱ���Ķ����������лָ���ʱ����ʼ��ʱ��
            timerStartTime = DateTime.FromBinary(save.Value);

            // Ϊ��ť��ӵ���¼�������
            button.onClick.AddListener(OnAdButtonClicked);

            descriptionText.text = description.ToString();
        }

        private void Update()
        {
            // ���㵱ǰʱ�����ʱ����ʼʱ��Ĳ�ֵ
            var timer = DateTime.Now - timerStartTime;
            var duration = TimeSpan.FromMinutes(timerDurationInMinutes);

            if (timer > duration) // �����ʱ����
            {
                button.enabled = true; // ���ð�ť
                button.interactable = true;
                timerText.text = "����"; // ��ʾ�ı�
            }
            else // �����ʱδ����
            {
                button.enabled = false; // ���ð�ť
                button.interactable = false;

                var timeLeft = duration - timer; // ����ʣ��ʱ��

                // ��ʽ��ʣ��ʱ�䲢������ʾ
                if (timeLeft.Hours > 0)
                {
                    timerText.text = string.Format("{0:hh\\:mm\\:ss}", timeLeft);
                }
                else
                {
                    timerText.text = string.Format("{0:mm\\:ss}", timeLeft);
                }

                // ��̬������ʱ�ı��Ͱ�ť�Ŀ��
                var prefferedWidth = timerText.preferredWidth;
                if (prefferedWidth < 270) prefferedWidth = 270;

                timerText.rectTransform.sizeDelta = timerText.rectTransform.sizeDelta.SetX(prefferedWidth + 5);
                button.image.rectTransform.sizeDelta = button.image.rectTransform.sizeDelta.SetX(prefferedWidth + 10);
            }
        }

        // ��ť����¼�����
        private void OnAdButtonClicked()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
            CallWechat.ShareApp(() =>
            {
                // ���±���ļ�ʱ����ʼʱ��
                save.Value = DateTime.Now.ToBinary();
                timerStartTime = DateTime.Now;
                PUController.AddPowerUp(PUType.ExtraSlot, 1);//���һ������ʾ�����ߵ�����
                AudioController.PlaySound(AudioController.Sounds.buySuccess);
            });
        }
    }
}
