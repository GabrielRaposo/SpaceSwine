using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Minigame;

namespace Jumper
{
    public class MJ_GameManager : MonoBehaviour
    {
        void Start()
        {
        
        }

        public void ResetScene (float delay = 0) 
        {
            if (delay <= 0)
                Minigame_Transition.Call(() => SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex ));
            else
            {
                StartCoroutine
                (
                    RaposUtil.WaitSeconds
                    (
                        delay, () => 
                        Minigame_Transition.Call(() => SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex ))
                    )
                );
            }
        }
    }
}
