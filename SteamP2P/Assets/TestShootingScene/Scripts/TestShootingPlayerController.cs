using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestShootingPlayerController : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public TextMesh healthBar;

    [Tooltip("�ӵ�Ԥ����")]
    public GameObject projectilePrefab;
    [Tooltip("�ӵ����ɵ�")]
    public Transform projectileMount;

    [Header("״̬")]
    [SyncVar] public int health = 10;

    void Update()
    {
        // (SyncVar hook ����ֻ���ڿͻ��˸��£������ڷ������˸���)
        healthBar.text = new string('-', health);

        //ֻ������Ϸ���ڴ��ڽ���ʱ�Ż�ִ��
        //if (!Application.isFocused) return;

        // ��ǰ�ͻ��˵ĸ���
        if (isLocalPlayer)
        {
            // ��ȡ���뷽��
            float moveDirectionX = Input.GetAxis("Horizontal");
            float moveDirectionZ = Input.GetAxis("Vertical");

            // �����ƶ�����
            Vector3 moveDirection = transform.right * moveDirectionX + transform.forward * moveDirectionZ;
            // ���� NavMeshAgent ���ٶ�
            agent.velocity = moveDirection * agent.speed;

            //��ȡ�������Ļ�ϵ�λ��
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                Debug.DrawLine(ray.origin, hit.point);
                Vector3 lookRotation = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.transform.LookAt(lookRotation);
            }

            // ���
            if (Input.GetMouseButtonDown(0))
            {
                CmdFire();
            }

        }
    }

    private void LateUpdate()
    {
        healthBar.transform.forward = Camera.main.transform.forward;//Ѫ��ʱ����������
    }

    //�÷����ڷ�������ִ��
    [Command]
    void CmdFire()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, projectileMount.rotation);
        /*
         * ����ʹ��NetworkServer.Spawn(projectile)�����ɺ�ͬ������ʱ��
         * ������������һ���Ѿ���NetworkManager�����Registered Spawnable Prefabs�б���ע�����Ԥ���塣
         * ֻ���������ͻ��˲�����ȷ�ؽ��յ���ʵ�����������
         */
        NetworkServer.Spawn(projectile);//���������ϵ�ĳЩ��������ҡ����ˡ��ӵ��ȣ�ͬ�������пͻ����ϣ�ʹÿ����Ҷ��ܿ�����ͬ����Ϸ״̬
        RpcOnFire();
    }

    //�����ܿ����ö���Ŀͻ��˶���ִ���������
    [ClientRpc]
    void RpcOnFire()
    {
        //animator.SetTrigger("Shoot");
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TestShootingBulletController>() != null)
        {
            --health;
            if (health == 0)
                NetworkServer.Destroy(gameObject);//������ͬ��״̬
        }
    }

}
