using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper 
{
    public class MJ_BlockStacker : MonoBehaviour
    {
        [SerializeField] Transform activeGroup;

        [SerializeField] MJ_LevelBlock initialBlock;
        [SerializeField] List<MJ_LevelBlock> blocks;

        MJ_LevelBlock currentTop;

        void Start()
        {
            currentTop = initialBlock;

            //temp
            foreach(MJ_LevelBlock b in blocks)
                StackUp(b);
        }

        private void StackUp (MJ_LevelBlock block)
        {
            float fitPosition = currentTop ? currentTop.FitPosition : 0;

            block.transform.position = new Vector2
            (
                0,
                fitPosition - block.lowerBorder
            );
            block.gameObject.SetActive(true);

            currentTop = block;
        }
    }
}
