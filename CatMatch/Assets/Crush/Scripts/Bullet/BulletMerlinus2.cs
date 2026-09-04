using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMerlinus2 : BulletBase
{
    private ParticleSystem eff, eff2;

    private EntityBase targetEntity;

    public void Setup(Team team, EntityBase attacker, BulletParam param, EntityBase target)
    {
        base.Setup(team, attacker.transform.position, param);
        targetEntity = target;

        transform.localScale = new Vector3(1, 1, 1);

        var effGo = ObjectPool.Instance.GetGameObject(effPref, Vector3.zero, Quaternion.identity);
        eff = effGo.GetComponent<ParticleSystem>();
        eff.transform.position = attacker.transform.position;
        eff.Play();

        effGo = ObjectPool.Instance.GetGameObject(explosionEffPref, Vector3.zero, Quaternion.identity);
        eff2 = effGo.GetComponent<ParticleSystem>();
        eff2.transform.position = target.transform.position;
        eff2.Play();

        var maxHp = target.curHp + attacker.curHp;
        if (maxHp > target.maxHP)
            maxHp = target.maxHP;
        target.curHp = maxHp;
        
        (target as BeastBase).RemoveAllStatus();
    }

    protected override void Update()
    {
        if (eff.isStopped && eff2.isStopped)
        {
            ObjectPool.Instance.ReleaseObject(eff.gameObject);
            ObjectPool.Instance.ReleaseObject(eff2.gameObject);
            ObjectPool.Instance.ReleaseObject(gameObject);
        }

        if (targetEntity != null && !targetEntity.isDie)
        {
            eff2.transform.position = targetEntity.transform.position;
        }
    }
}
