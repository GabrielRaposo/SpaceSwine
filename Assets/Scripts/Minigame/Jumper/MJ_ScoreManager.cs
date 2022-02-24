using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Jumper 
{
    public class MJ_ScoreManager : MonoBehaviour
    {
        const int HEIGHT_OFFSET = 25;

        [SerializeField] TextMeshProUGUI display;
        [SerializeField] MJ_Player player;
        [SerializeField] MJ_ScoreLine scoreLine;
        [SerializeField] MJ_ScoreLine heightLine;
        [SerializeField] AK.Wwise.Event HighscoreAKEvent;

        int displayedHeight;
        int score;
        static string BaseText = " m";
        static int PlayerBestScore;
        static bool HasNewBestScore;

        void Start()
        {
            score = 0;
            UpdateScoreDisplay();

            displayedHeight = HEIGHT_OFFSET;
            heightLine.SetValue(displayedHeight, displayedHeight.ToString() + BaseText);

            if (PlayerBestScore > 0)           
                scoreLine.SetValue(PlayerBestScore, PlayerBestScore.ToString() + BaseText);
        }

        private void Update() 
        {
            int playerHighest = Mathf.RoundToInt( player.HighestYPosition() );
            
            if (playerHighest > displayedHeight + (HEIGHT_OFFSET/2) )
            {
                displayedHeight += HEIGHT_OFFSET;
                heightLine.SetValue(displayedHeight, displayedHeight.ToString() + BaseText);
            }

            if (playerHighest > score)
                score = playerHighest;

            if (score > PlayerBestScore)
            {
                if (PlayerBestScore > 0 && !scoreLine.wasBeatenThisRound)
                {
                    //Play effect
                    Debug.Log("new high score!");
                    HighscoreAKEvent?.Post(gameObject);
                }
                
                scoreLine.wasBeatenThisRound = true;

                HasNewBestScore = true;
                PlayerBestScore = score;
            }

            UpdateScoreDisplay();
        }

        private void UpdateScoreDisplay()
        {
            if (!display)
                return;

            display.text =  score.ToString() + BaseText;
        }

        public static (int, string) PlayerBest()
        {
            return (PlayerBestScore, PlayerBestScore.ToString() + BaseText);
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
    }

}
