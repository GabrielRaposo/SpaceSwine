using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Jumper
{
    public class MJ_PreMatchMenu : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] List<Color> blinkingColors;

        [Header("References")]
        [SerializeField] TextMeshProUGUI mainDisplay;
        [SerializeField] TextMeshProUGUI newRecordDisplay;
        [SerializeField] TextMeshProUGUI helpInputDisplay;
        [SerializeField] MJ_Player player;
        [SerializeField] AK.Wwise.Event newScoreAKEvent;

        
        void Start()
        {
            if (!mainDisplay || !newRecordDisplay)
            {
                gameObject.SetActive(false);
                return;
            }

            if (player)
                player.OnFirstMove += HideMenus;

            mainDisplay.text = string.Empty;

            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            mainDisplay.text = string.Empty;
            //mainDisplay.text += "-" + "HARD" + "-" + "\n";
            string bestScore = MJ_ScoreManager.PlayerBest().Item1 <= 0 ? 
                "---" : 
                MJ_ScoreManager.PlayerBest().Item2;
            mainDisplay.text += "BEST: " + bestScore;

            if (MJ_ScoreManager.UseHasScoreTrigger() )
            {
                newRecordDisplay.gameObject.SetActive (true);
                newScoreAKEvent?.Post(gameObject);
            }
            else
            {
                newRecordDisplay.gameObject.SetActive (false);
            }

            if (newRecordDisplay.gameObject.activeSelf)
                StartCoroutine( RecordBlinkRoutine() );

            StartCoroutine( SpacebarBlinkRoutine() );
        }

        private IEnumerator RecordBlinkRoutine()
        {
            while (true)
            {
                for (int i = 0; i < blinkingColors.Count; i++)
                {
                    newRecordDisplay.color = blinkingColors[i];

                    mainDisplay.text = string.Empty;
                    //mainDisplay.text += "-" + "HARD" + "-" + "\n";
                    string bestScore = MJ_ScoreManager.PlayerBest().Item1 <= 0 ? 
                        "---" : 
                        MJ_ScoreManager.PlayerBest().Item2;

                    mainDisplay.text += "BEST: " 
                        + "<color=#" + ColorUtility.ToHtmlStringRGBA(blinkingColors[i]) + ">" 
                        + bestScore
                        + "</color>";


                    yield return new WaitForSeconds(.15f);
                }
            }
        }

        private IEnumerator SpacebarBlinkRoutine()
        {
            while (true)
            {
                helpInputDisplay.enabled = !helpInputDisplay.enabled;

                yield return new WaitForSeconds(.35f);   
            }
        }

        private void HideMenus()
        {
            StopAllCoroutines();

            helpInputDisplay.enabled = false;
            gameObject.SetActive(false);
        }
    }

}
