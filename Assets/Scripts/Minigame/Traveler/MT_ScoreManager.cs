using Minigame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traveler
{ 
    public class MT_ScoreManager : MonoBehaviour
    {
        MT_ProgressDisplay progressDisplay;
        
        int score;

        static int PlayerBestScore;
        static bool HasNewBestScore;

        static public MT_ScoreManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            progressDisplay = MT_ProgressDisplay.Instance;

            score = 0;
            UpdateScoreDisplay();

            MinigameManager gameManager = MinigameManager.Instance;
            if (!gameManager)
                return;

            int savedHighscore = gameManager.GetHighscore();

            if (savedHighscore > PlayerBestScore)
                PlayerBestScore = savedHighscore;

            //Debug.Log("PlayerBestScore: " + PlayerBestScore);
        }

        public void ChangeScore (int value)
        {
            score += value;

            if (score > PlayerBestScore)
            {
                HasNewBestScore = true;
                PlayerBestScore = score;
            }

            UpdateScoreDisplay();

        }

        private void UpdateScoreDisplay()
        {
            if (!progressDisplay)
                return;

            progressDisplay.UpdateScore (score);
        }

        public static (int, string) PlayerBest()
        {
            return (PlayerBestScore, PlayerBestScore.ToString());
        }

        public static bool UseHasScoreTrigger() 
        {
            if (HasNewBestScore)
            {
                HasNewBestScore = false;
                return true;
            }

            return false;
        }

        private void OnDisable()
        {
            MinigameManager gameManager = MinigameManager.Instance;
            if (!gameManager)
                return;
            
            int savedHighscore = gameManager.GetHighscore();
            
            if (savedHighscore < PlayerBestScore)
                gameManager.SetHighScore(PlayerBestScore);
        }
    }
}
