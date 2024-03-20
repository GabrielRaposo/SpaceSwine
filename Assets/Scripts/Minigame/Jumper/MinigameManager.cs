using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minigame
{
    public class MinigameManager : MonoBehaviour
    {
        public static int MAX_SCORE1 = 100;
        public static int MAX_SCORE2 = 175;

        [SerializeField] GGSMinigame minigame;

        static GGSConsole CurrentConsole;
        public static MinigameManager Instance;

        private void Awake() 
        {
            Instance = this;
        }

        public static void ConnectToConsole (GGSConsole ggsConsole)
        {
            CurrentConsole = ggsConsole;
            Debug.Log("Connected.");
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
            if (CurrentConsole)
                CurrentConsole.ReloadMinigame();
            else
                SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
        }

        private string MinigameID
        {
            get 
            {
                switch (minigame)
                {
                    default:
                    case GGSMinigame.Jumper:   return GGSConsole.Jumper;
                    case GGSMinigame.Shooter:  return GGSConsole.Shooter;
                    case GGSMinigame.Traveler: return GGSConsole.Traveler;
                }
            }
        }

        public int GetHighscore ()
        {
            if (!CurrentConsole)
                return -1;

            return CurrentConsole.GetHighscore (MinigameID);
        }

        public void SetHighScore (int score)
        {
            if (!CurrentConsole)
                return;

            if (score >= MAX_SCORE1)
            {
                switch (minigame)
                {
                    case GGSMinigame.Jumper:
                        if (!AchievementsManager.GetAchievementState(AchievementEnum.JumperMaster))
                            AchievementsManager.SetAchievementState(AchievementEnum.JumperMaster, true);
                        break;

                    case GGSMinigame.Shooter:
                        if (!AchievementsManager.GetAchievementState(AchievementEnum.ShooterMaster))
                            AchievementsManager.SetAchievementState(AchievementEnum.ShooterMaster, true);
                        break;

                    case GGSMinigame.Traveler:
                        if (!AchievementsManager.GetAchievementState(AchievementEnum.LooperMaster))
                            AchievementsManager.SetAchievementState(AchievementEnum.LooperMaster, true);
                        break;
                }
            }
            if (score >= MAX_SCORE2 && !AchievementsManager.GetAchievementState(AchievementEnum.SundayAfternoon))
                AchievementsManager.SetAchievementState(AchievementEnum.SundayAfternoon, true);    

            CurrentConsole.SetHighscore(MinigameID, score);
        }
    }
}
