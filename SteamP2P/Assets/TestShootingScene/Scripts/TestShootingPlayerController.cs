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

    [Tooltip("子弹预制体")]
    public GameObject projectilePrefab;
    [Tooltip("子弹生成点")]
    public Transform projectileMount;

    [Header("状态")]
    [SyncVar] public int health = 10;

    void Update()
    {
        // (SyncVar hook 方法只会在客户端更新，不会在服务器端更新)
        healthBar.text = new string('-', health);

        //只有在游戏窗口处于焦点时才会执行
        //if (!Application.isFocused) return;

        // 当前客户端的更新
        if (isLocalPlayer)
        {
            // 获取输入方向
            float moveDirectionX = Input.GetAxis("Horizontal");
            float moveDirectionZ = Input.GetAxis("Vertical");

            // 计算移动向量
            Vector3 moveDirection = transform.right * moveDirectionX + transform.forward * moveDirectionZ;
            // 设置 NavMeshAgent 的速度
            agent.velocity = moveDirection * agent.speed;

            //获取鼠标在屏幕上的位置
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                Debug.DrawLine(ray.origin, hit.point);
                Vector3 lookRotation = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.transform.LookAt(lookRotation);
            }

            // 射击
            if (Input.GetMouseButtonDown(0))
            {
                CmdFire();
            }

        }
    }

    private void LateUpdate()
    {
        healthBar.transform.forward = Camera.main.transform.forward;//血条时刻面对摄像机
    }

    //该方法在服务器端执行
    [Command]
    void CmdFire()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, projectileMount.rotation);
        /*
         * 当你使用NetworkServer.Spawn(projectile)来生成和同步对象时，
         * 这个对象必须是一个已经在NetworkManager组件的Registered Spawnable Prefabs列表中注册过的预制体。
         * 只有这样，客户端才能正确地接收到并实例化这个对象。
         */
        NetworkServer.Spawn(projectile);//将服务器上的某些对象（如玩家、敌人、子弹等）同步到所有客户端上，使每个玩家都能看到相同的游戏状态
        RpcOnFire();
    }

    //所有能看到该对象的客户端都会执行这个方法
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
                NetworkServer.Destroy(gameObject);//服务器同步状态
        }
    }

}
