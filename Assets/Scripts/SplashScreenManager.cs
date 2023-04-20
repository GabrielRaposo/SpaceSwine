using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DevLocker.Utils;

[RequireComponent(typeof(VideoPlayer))]
public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] SceneReference nextScene;
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

        if (fadeImage)
        {
            fadeImage.DOFade(0, fadeDuration);
        }

        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;
    }

    private void EndReached (VideoPlayer vp)
    {
        if (nextScene == null)
            return;

        SceneTransition.LoadScene( nextScene.ScenePath, SceneTransition.TransitionType.BlackFade );
    }

}
