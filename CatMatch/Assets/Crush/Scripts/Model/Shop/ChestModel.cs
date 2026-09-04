using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestModel : ChestData
{
    public int typeInt;

    public int curStock;
    public int maxStock;

    public int buyAmount;
    public int useAmount;
    public int freeAmount;

    public List<BeastId> beastIds;// fortune chest
}
