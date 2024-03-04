using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class MS_DettachableEffect : MonoBehaviour
    {
        [SerializeField] Animator animator;
        Transform parent;

        public void Call()
        {
            animator.SetTrigger("Reset");
            gameObject.SetActive(true);

            parent = transform.parent;
            transform.SetParent(null);
            if (parent != null)
                transform.position = parent.position;
        }

        public void _AnimationCall()
        {
            gameObject.SetActive(false);
            transform.SetParent(parent);
        }
    }
}
