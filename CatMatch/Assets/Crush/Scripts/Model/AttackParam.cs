
using UnityEngine;

public struct AttackParam
{
    public Vector3 collidePosition;
    public bool alwayHit;
    public AttackerInfo attackerInfo;
    public bool ignoreDamageDoge;

    public AttackParam(Vector3 collide, AttackerInfo atkInfo)
    {
        collidePosition = collide;
        attackerInfo = atkInfo;
        alwayHit = false;
        ignoreDamageDoge = false;
    }
}