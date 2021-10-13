using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper 
{
    public class MJ_BlockStacker : MonoBehaviour
    {
        [SerializeField] Transform activeGroup;
        [SerializeField] MJ_BlockGroup easyGroup;
        [SerializeField] MJ_LevelBlock initialBlock;
        [Space(5)]
        [SerializeField] MJ_Player player;

        MJ_LevelBlock currentTop;

        void Start()
        {
            if (initialBlock)
            {
                initialBlock.player = player;
                currentTop = initialBlock;
            }

            StackUp(easyGroup.GetRandomBlock());
        }

        private void Update() 
        {
            if (currentTop.FitPosition < player.transform.position.y + 12)
                StackUp(easyGroup.GetRandomBlock());
        }

        private void StackUp (MJ_LevelBlock block)
        {
            if (block == null)
                return;

            float fitPosition = currentTop ? currentTop.FitPosition : 0;

            block.transform.position = new Vector2
            (
                0,
                fitPosition - block.lowerBorder
            );
            block.gameObject.SetActive(false);
            block.gameObject.SetActive(true);
            block.transform.SetParent(activeGroup);
            block.player = player;

            currentTop = block;
        }
    }
}
