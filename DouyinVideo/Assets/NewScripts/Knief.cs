using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knief : MonoBehaviour
{
    private Transform ball;
    private bool isAttached = false;

    public void AttachToBall(Transform ballTransform)
    {
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        ball = ballTransform;
        isAttached = true;
        transform.parent.position = ball.position;
        //Broadcast.instance.BroadCastNews($"{ball.GetComponent<BaseRole>().RoleName}拾取了小刀", ball.GetComponent<BaseRole>().roleColor);
        // 取消父子关系更好，避免嵌套旋转干扰
        // 或者保留，看需求
        transform.parent.SetParent(ball);

        StartCoroutine(DestroyThis());
    }

    void Update()
    {
        if (VideoGameManager.instance.gameEnd)
        {
            gameObject.SetActive(false);
            return;
        }
        if (isAttached && ball != null)
        {
            transform.parent.Rotate(0, 0, VideoGameManager.instance.KniefRotateSpeed * Time.deltaTime);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("碰到了");
        if (!isAttached)
        {
            if (collision.CompareTag("Role")&& collision.GetComponent<BaseRole>().canGetWeapon)
            {
                AttachToBall(collision.transform);
            }
        }
        else
        {
            if (collision.CompareTag("Role")&& collision.transform!= ball)
            {
                collision.GetComponent<BaseRole>().TakeDamage(10);

                Broadcast.instance.BroadCastNews($"{ball.GetComponent<BaseRole>().RoleName}" +
                    $"攻击了{collision.GetComponent<BaseRole>().RoleName}，获得一点能量", ball.GetComponent<BaseRole>().roleColor);

                VideoGameManager.instance.KniefHoldTime += 1;
                VideoGameManager.instance.KniefRotateSpeed += 5;

                ball.GetComponent<BaseRole>().AddBig();
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.transform.parent.gameObject.SetActive(false);
                isAttached = false;
                gameObject.transform.localPosition = new Vector3(-2.214f, 0, 0);
                gameObject.transform.parent.localEulerAngles = Vector3.zero;
                VideoGameManager.instance.SpawnWeapons();
                
            }
        }
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(VideoGameManager.instance.KniefHoldTime);
        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.transform.localPosition = new Vector3(-2.214f, 0, 0);
        gameObject.transform.parent.localEulerAngles = Vector3.zero;
        isAttached =false;
        VideoGameManager.instance.SpawnWeapons();
        //Broadcast.instance.BroadCastNews($"{ball.GetComponent<BaseRole>().RoleName}的小刀消失了", ball.GetComponent<BaseRole>().roleColor);
        VideoGameManager.instance.KniefHoldTime += 1f;
        VideoGameManager.instance.KniefRotateSpeed += 5;

        yield break;
    }
}
