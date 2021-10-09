using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper 
{
    public class MJ_BlockGroup : MonoBehaviour
    {
        public MJ_LevelBlock GetRandomBlock ()
        {
            if (transform.childCount < 1)
                return null;

            int maxNum = 3;
            if (transform.childCount < maxNum)
                maxNum = transform.childCount;

            int randomInt = Random.Range(0, maxNum);
            MJ_LevelBlock block = transform.GetChild(randomInt).GetComponent<MJ_LevelBlock>();
            if (!block)
                return null;

            if (block.originalParent == null)
                block.originalParent = this;

            return block;
        }
    }
}