using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnswerMath", menuName = "Game/Bless/AnswerMath")]
public class AnswerMath : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new AnswerMathInstance(this, rarity);
    }
}
