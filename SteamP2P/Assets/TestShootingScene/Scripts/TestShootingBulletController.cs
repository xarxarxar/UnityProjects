using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShootingBulletController : NetworkBehaviour
{
    public float destroyAfter = 2;//2�������
    public Rigidbody rigidBody;
    public float force = 10;//���Ĵ�С

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    // Ϊ�ӵ������
    void Start()
    {
        rigidBody.AddForce(transform.forward * force);
    }

    // �ڷ�������Ϊÿ���ͻ��������������
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    // �÷�������ڿͻ�����ִ�У����ܻᵼ�°�ȫ�������Ϸ״̬�Ĳ�һ�£�
    // ���ʹ��ServerCallback���߷������ڷ�������������
    //�� [Server] ���Բ�ͬ��[ServerCallback] �������� Mirror �Զ��ܾ����õ���Ϊ�����Ǿ�Ĭ�غ��Կͻ��˵ĵ���
    [ServerCallback]
    void OnTriggerEnter(Collider co) 
    {
        Debug.Log("�ӵ����ӵ���ײ������");
        DestroySelf();
    } 
}
