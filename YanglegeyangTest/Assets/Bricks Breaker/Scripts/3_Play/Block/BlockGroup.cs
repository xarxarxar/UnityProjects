using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockGroup : MonoBehaviour
{
    public List<BlockBase> blockBases = new List<BlockBase>();
    public int id = 0;
    BlockBase blockBase;

    public void SetBlockGroup(int num = 0, int health = 0)
    {
        id = num;
        CtrBlock.instance.ShuffleSlot();
        int bCount = GetBlockCount();

        for (int i = 0; i < CtrBlock.instance.firstTransform.Count; i++)
        {
            //Addball
            if (bCount > i)
            {
                if (i == 0)
                {
                    //Add Ball block
                    blockBase = PoolManager.Spawn(CtrPool.instance.pBlockAddBall, Vector3.zero, Quaternion.identity)
                        .GetComponent<BlockBase>();
                }
                else
                {
                    //Basic block
                    blockBase = PoolManager.Spawn(CtrPool.instance.pBlockDefault, Vector3.zero, Quaternion.identity)
                        .GetComponent<BlockBase>();
                }

                blockBase.Reset();

                //Block parent setting
                blockBase.transform.SetParent(transform);

                //Add to active block list
                blockBases.Add(blockBase);

                //Coordinate setting
                blockBase.transform.position = CtrBlock.instance.firstTransform[i].transform.position;

                //Setting block group
                blockBase.blockGroup = this;

                //Block data setting
                blockBase.SetData(health);
            }
        }
    }


    int per1;
    int per2;
    int per3;
    int per4;
    int per5;

    /// <summary>
    /// Difficulty adjustment for each block according to the number of turns
    /// </summary>
    int GetBlockCount()
    {
        int turnCount = CtrGame.instance.turnCount;
        int n = Random.Range(0, 100);
        int blockcount = 0;

        if (turnCount >= 0 && turnCount <= 10)
        {
            per1 = 20;
            per2 = 50;
            per3 = 100;

            if (n <= per1)
            {
                blockcount = 2;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 3;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 4;
            }

        }
        else if (turnCount > 10 && turnCount <= 20)
        {
            per1 = 5;
            per2 = 35;
            per3 = 75;
            per4 = 100;

            if (n <= per1)
            {
                blockcount = 2;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 3;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 4;
            }
            else if (n > per3 && n <= per4)
            {
                blockcount = 5;
            }

        }
        else if (turnCount > 20 && turnCount <= 30)
        {

            per1 = 25;
            per2 = 60;
            per3 = 85;
            per4 = 100;

            if (n <= per1)
            {
                blockcount = 3;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 4;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 5;
            }
            else if (n > per3 && n <= per4)
            {
                blockcount = 6;
            }

        }
        else if (turnCount > 30 && turnCount <= 40)
        {

            per1 = 10;
            per2 = 40;
            per3 = 75;
            per4 = 100;

            if (n <= per1)
            {
                blockcount = 3;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 4;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 5;
            }
            else if (n > per3 && n <= per4)
            {
                blockcount = 6;
            }




        }
        else if (turnCount > 40 && turnCount <= 50)
        {
            per1 = 5;
            per2 = 35;
            per3 = 75;
            per4 = 100;

            if (n <= per1)
            {
                blockcount = 3;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 4;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 5;
            }
            else if (n > per3 && n <= per4)
            {
                blockcount = 6;
            }

        }
        else if (turnCount > 50 && turnCount <= 60)
        {
            per1 = 30;
            per2 = 70;
            per3 = 100;

            if (n <= per1)
            {
                blockcount = 4;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 5;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 6;
            }
        }
        else if (turnCount > 60 && turnCount <= 70)
        {
            per1 = 25;
            per2 = 65;
            per3 = 100;

            if (n <= per1)
            {
                blockcount = 4;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 5;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 6;
            }
        }
        else if (turnCount > 70 && turnCount <= 80)
        {
            per1 = 20;
            per2 = 60;
            per3 = 100;

            if (n <= per1)
            {
                blockcount = 4;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 5;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 6;
            }
        }
        else if (turnCount > 80 && turnCount <= 90)
        {
            per1 = 20;
            per2 = 60;
            per3 = 100;

            if (n <= per1)
            {
                blockcount = 4;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 5;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 6;
            }
        }
        else if (turnCount > 90 && turnCount <= 100)
        {
            per1 = 15;
            per2 = 55;
            per3 = 100;

            if (n <= per1)
            {
                blockcount = 4;
            }
            else if (n > per1 && n <= per2)
            {
                blockcount = 5;
            }
            else if (n > per2 && n <= per3)
            {
                blockcount = 6;
            }
        }
        else if (turnCount > 100 && turnCount <= 200)
        {
            per1 = 40;

            if (n <= per1)
            {
                blockcount = 5;
            }
            else
            {
                blockcount = 6;
            }

        }
        else if (turnCount > 200 && turnCount <= 300)
        {
            per1 = 30;

            if (n <= per1)
            {
                blockcount = 5;
            }
            else
            {
                blockcount = 6;
            }
        }
        else
        {
            blockcount = 6;
        }

        return blockcount;
    }


    /// <summary>
    /// Move down one space
    /// </summary>
    public void Move(float time)
    {
        id += 1;
        transform.DOMove(CtrBlock.instance.transfomY[id].position, time).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            if (id >= CtrBlock.instance.transfomY.Length - 1)
            {
                Destory();
            }
        });
    }

    /// <summary>
    //  Delete one line
    /// </summary>
    public void Destory(bool isContinue = false)
    {
        int count = blockBases.Count;
        for (int i = 0; i < count; i++)
        {
            blockBases[0].Destory();
        }

        if (!isContinue && count > 0)
        {
            CtrGame.instance.GameOver();
        }

        blockBases.Clear();
        CtrBlock.instance.blockGroups.Remove(this);
        PoolManager.Despawn(this.gameObject);
    }

}
