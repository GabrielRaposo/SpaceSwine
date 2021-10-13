using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Jumper 
{
    public class MJ_ScoreManager : MonoBehaviour
    {
        const int HEIGHT_OFFSET = 25;

        [SerializeField] string baseText;
        [SerializeField] TextMeshProUGUI display;
        [SerializeField] MJ_Player player;
        [SerializeField] MJ_ScoreLine scoreLine;
        [SerializeField] MJ_ScoreLine heightLine;

        int displayedHeight;
        int score;
        static int PlayerBestScore;

        void Start()
        {
            score = 0;
            UpdateScore();

            displayedHeight = HEIGHT_OFFSET;
            heightLine.SetValue(displayedHeight, displayedHeight.ToString() + baseText);

            if (PlayerBestScore > 0)           
                scoreLine.SetValue(PlayerBestScore, PlayerBestScore.ToString() + baseText);
        }

        private void Update() 
        {
            int playerHighest = Mathf.RoundToInt( player.HighestYPosition() );
            
            if (playerHighest > displayedHeight + (HEIGHT_OFFSET/2) )
            {
                displayedHeight += HEIGHT_OFFSET;
                heightLine.SetValue(displayedHeight, displayedHeight.ToString() + baseText);
            }

            if (playerHighest > score)
                score = playerHighest;

            if (score > PlayerBestScore)
                PlayerBestScore = score;

            UpdateScore();
        }

        private void UpdateScore()
        {
            if (!display)
                return;

            display.text =  score.ToString() + baseText;
        }
    }

}
