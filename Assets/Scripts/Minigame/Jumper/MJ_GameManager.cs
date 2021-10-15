using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Minigame;

namespace Jumper
{
    public class MJ_GameManager : MonoBehaviour
    {
        GGSConsole ggsConsole;
        public static MJ_GameManager Instance;

        private void Awake() 
        {
            Instance = this;    
        }

        public void ConnectToConsole (GGSConsole ggsConsole)
        {
            this.ggsConsole = ggsConsole;
        }

        public void ResetScene (float delay = 0) 
        {
            if (delay <= 0)
                Minigame_Transition.Call(ResetCall);
            else
            {
                StartCoroutine
                (
                    RaposUtil.WaitSeconds
                    (
                        delay, () => 
                        Minigame_Transition.Call(ResetCall)
                    )
                );
            }
        }

        public void ResetCall ()
        {
            if (ggsConsole)
                ggsConsole.ReloadMinigame();
            else
                SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
        }
    }
}
