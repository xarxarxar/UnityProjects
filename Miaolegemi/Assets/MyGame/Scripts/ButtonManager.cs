using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject startScene;//��Ϸ��ʼ�ĳ���
    [Header("��ʼ��ť")]
    public GameObject gameScene;//��Ϸ������
    public GameObject cutScenes;//����������UIԪ��
    public void StartButton()
    {
        startScene.SetActive(false);
        cutScenes.SetActive(true);
        gameScene.SetActive(true);
    }

    [Header("�ֿⰴť")]
    public GameObject collectScene;//�ֿ�ĳ���
    public void CollectButton()
    {
        startScene.SetActive(false);
        collectScene.SetActive(true);
    }

    [Header("���ð�ť")]
    public GameObject settingScene;//�������
    public void SettingButton()
    {
        settingScene.SetActive(true);
    }

    [Header("������ս��ť")]
    public GameObject tipPanel;//�����Ƿ��������ս
    public void GiveUpButton()//������ս��ť
    {
        tipPanel.SetActive(true);
    }

    public void ConfrimGiveUp()//ȷ�Ϸ�����ť
    {
        tipPanel.SetActive(false);
        cutScenes.SetActive(true);
        settingScene.SetActive(false);
        gameScene.SetActive(false);
        startScene.SetActive(true);
    }

    public void ContinueButton()//������ս��ť
    {
        tipPanel.SetActive(false);
    }

    public void BackToStart()//�Ӳֿ���淵�ؿ�ʼ����
    {
        collectScene.SetActive(false);
        startScene.SetActive(true) ;
    }
}
