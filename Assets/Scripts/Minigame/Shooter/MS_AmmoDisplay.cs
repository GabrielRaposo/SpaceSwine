using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class MS_AmmoDisplay : MonoBehaviour
    {
        [SerializeField] Transform positionAnchor;
        [SerializeField] List<GameObject> ammoDisplays;

        static public MS_AmmoDisplay Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdateDisplay (int currentAmmo)
        {
            if (ammoDisplays == null)
                return;

            for (int i = 0; i < ammoDisplays.Count; i++) 
            {
                ammoDisplays[i].SetActive (i < currentAmmo);
            }
        }
    }
}
