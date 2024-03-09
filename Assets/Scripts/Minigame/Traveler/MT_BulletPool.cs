using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traveler
{
    public class MT_BulletPool : MonoBehaviour
    {
        [SerializeField] int poolSize;
        [SerializeField] GameObject bulletBase;

        int index;

        List<MT_Bullet> bullets; 

        public static MT_BulletPool Instance;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (bulletBase == null)
                return;

            bullets = new List<MT_Bullet>();
            
            for (int i = 0; i < poolSize; i++) 
            {
                MT_Bullet b = Instantiate (bulletBase, transform).GetComponent<MT_Bullet>();
                b.gameObject.SetActive (false);
                bullets.Add(b);
            }

            bulletBase.SetActive (false);
            index = 0;
        }

        public MT_Bullet Get ()
        {
            MT_Bullet bullet = bullets[index % bullets.Count];
            index = (index + 1) % bullets.Count;
            return bullet;
        }
    }

}
