using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Jumper 
{
    public class MJ_ScoreManager : MonoBehaviour
    {
        [SerializeField] string baseText;
        [SerializeField] TextMeshProUGUI display;
        [SerializeField] MJ_Player player;
        [SerializeField] MJ_ScoreLine scoreLine;

        int score;
        static int BestScore;

        // Start is called before the first frame update
        void Start()
        {
            score = 0;
            UpdateScore();

            if (BestScore > 0)           
                scoreLine.SetValue(BestScore, BestScore.ToString() + baseText);
        }

        private void Update() 
        {
            int playerHighest = Mathf.RoundToInt( player.HighestYPosition() );
            if (playerHighest > score)
                score = playerHighest;

            if (score > BestScore)
                BestScore = score;

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
