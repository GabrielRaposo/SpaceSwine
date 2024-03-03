using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class MS_BulletPool : MonoBehaviour
    {
        [SerializeField] int poolSize;
        [SerializeField] GameObject baseBullet;

        bool Initiated;
        int count;
        List<MS_Bullet> bullets;

        static public MS_BulletPool Instance;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (baseBullet == null)
                return;

            bullets = new List<MS_Bullet>();

            for (int i = 0; i < poolSize; i++) 
            {
                MS_Bullet bullet = Instantiate(baseBullet, transform).GetComponent<MS_Bullet>();
                bullet.gameObject.SetActive(false);

                bullets.Add(bullet);
            }

            baseBullet.SetActive(false);
            Initiated = true;
        }

        public MS_Bullet Get()
        {
            if (!Initiated)
                return null;

            MS_Bullet b = bullets[count];
            count = (count + 1) % poolSize;

            return b;
        }
    }
}
