using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;

public class Gate : EntityBase
{
    public Transform gateEnemy;
    public Transform gatePlayer;

    Transform gate;

    public void Setup(Team team, System.Numerics.BigInteger newHp, int index)
    {
        curDoge = 0;
        currentTeam = team;

        Setup(index, newHp, 0, def);

        if (team == Team.Enemy)
        {
            gateEnemy.gameObject.SetActive(true);
            gatePlayer.gameObject.SetActive(false);
            gate = gateEnemy;
        }
        else
        {
            gateEnemy.gameObject.SetActive(false);
            gatePlayer.gameObject.SetActive(true);
            gate = gatePlayer;
        }
        for(int k = 0; k < gate.childCount; k++)
        {
            gate.GetChild(k).gameObject.SetActive(true);
        }
      
        healthBar.gameObject.transform.localScale = new Vector3(1, 1, 1);

        var tmpTransfromPos = transform.position;
        tmpTransfromPos.z = transform.position.y;
        transform.position = tmpTransfromPos;
    }

    public override void Remove()
    {
        GameManager.instance.GameOver(currentTeam == Team.Enemy);
        gate.GetChild(0).gameObject.SetActive(false);
        gate.GetChild(2).gameObject.SetActive(false);
        // ObjectPool.Instance.ReleaseObject(gameObject);
    }
}
