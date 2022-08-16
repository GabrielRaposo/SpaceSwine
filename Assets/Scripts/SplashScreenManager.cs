using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(VideoPlayer))]
public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] private RawImage fadeImage;
    [SerializeField] private float fadeDuration;
    [SerializeField] float delay;

    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        StartCoroutine( RaposUtil.WaitSeconds(delay, Play) );
    }

    private void Play()
    {
        if (!videoPlayer)
            return;

        if(fadeImage)
        {
            fadeImage.DOFade(0, fadeDuration);
        }

        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;
    }

    private void EndReached(VideoPlayer vp)
    {
        SceneTransition.LoadScene( (int) BuildIndex.Title, SceneTransition.TransitionType.BlackFade );
    }

}
