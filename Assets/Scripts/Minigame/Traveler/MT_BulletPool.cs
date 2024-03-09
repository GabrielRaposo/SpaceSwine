using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traveler
{
    public class MT_BulletPool : MonoBehaviour
    {
        [SerializeField] int poolSize;
        [SerializeField] int aimPoolSize;

        [SerializeField] GameObject bulletBase;
        [SerializeField] GameObject aimBulletBase;

        int mainIndex;
        int aimIndex;

        List<MT_Bullet> mainBullets; 
        List<MT_Bullet> aimBullets; 

        public static MT_BulletPool Instance;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (bulletBase == null)
                return;

            {
                mainBullets = new List<MT_Bullet>();
            
                for (int i = 0; i < poolSize; i++) 
                {
                    MT_Bullet b = Instantiate (bulletBase, transform).GetComponent<MT_Bullet>();
                    b.gameObject.SetActive (false);
                    mainBullets.Add(b);
                }

                bulletBase.SetActive (false);
            }

            {
                aimBullets = new List<MT_Bullet>();
            
                for (int i = 0; i < aimPoolSize; i++) 
                {
                    MT_Bullet b = Instantiate (aimBulletBase, transform).GetComponent<MT_Bullet>();
                    b.gameObject.SetActive (false);
                    aimBullets.Add(b);
                }

                aimBulletBase.SetActive (false);
            }

            mainIndex = 0;
        }

        public MT_Bullet GetMainBullet ()
        {
            MT_Bullet bullet = mainBullets[mainIndex % mainBullets.Count];
            mainIndex = (mainIndex + 1) % mainBullets.Count;
            return bullet;
        }

        public MT_Bullet GetAimBullet ()
        {
            MT_Bullet bullet = aimBullets[aimIndex % aimBullets.Count];
            aimIndex = (aimIndex + 1) % aimBullets.Count;
            return bullet;
        }

    }
}
