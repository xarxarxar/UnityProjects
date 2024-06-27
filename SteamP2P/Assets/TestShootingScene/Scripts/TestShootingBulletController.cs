using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShootingBulletController : NetworkBehaviour
{
    public float destroyAfter = 2;//2秒后销毁
    public Rigidbody rigidBody;
    public float force = 10;//力的大小

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    // 为子弹添加力
    void Start()
    {
        rigidBody.AddForce(transform.forward * force);
    }

    // 在服务器端为每个客户端销毁这个物体
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    // 该方法如果在客户端上执行，可能会导致安全问题或游戏状态的不一致，
    // 因此使用ServerCallback告诉服务器在服务器端允运行
    //与 [Server] 属性不同，[ServerCallback] 不会引发 Mirror 自动拒绝调用的行为，而是静默地忽略客户端的调用
    [ServerCallback]
    void OnTriggerEnter(Collider co) 
    {
        Debug.Log("子弹：子弹碰撞到敌人");
        DestroySelf();
    } 
}
