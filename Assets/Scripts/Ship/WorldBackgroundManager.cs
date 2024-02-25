using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldBackgroundManager : MonoBehaviour
{
    [SerializeField] float fadeDuration;
    [SerializeField] GameObject World1BGGroup;
    [SerializeField] GameObject World2BGGroup;
    [SerializeField] GameObject World3BGGroup;
    [SerializeField] CanvasGroup backgroundOverlay;

    static WorldBackgroundManager Instance;

    Sequence s;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine( WaitForSaveInitiation() );
    }

    IEnumerator WaitForSaveInitiation()
    {
        if (!SaveManager.Initiated) 
            yield return new WaitUntil ( () => SaveManager.Initiated);

        SetWorldBackgroundAndWorld (SaveManager.CurrentWorld);
    }

    public static void ChangeTo (int world)
    {
        if (!Instance)
            return;

        Instance.ChangeToLocal(world);
    }

    private void ChangeToLocal(int world) 
    {
        if (s != null) s.Kill();

        s = DOTween.Sequence();
        s.Append( backgroundOverlay.DOFade(1f, fadeDuration).SetEase(Ease.OutCirc));
        s.AppendCallback( () => SetWorldBackgroundAndWorld(world) );
        s.Append( backgroundOverlay.DOFade(0f, fadeDuration).SetEase(Ease.InCirc));
    }

    private void SetWorldBackgroundAndWorld(int world)
    {
        world %= 3;
        SaveManager.CurrentWorld = world;

        if (World1BGGroup)
            World1BGGroup.SetActive(world == 0);

        if (World2BGGroup)
            World2BGGroup.SetActive(world == 1);
        
        if (World3BGGroup)
            World3BGGroup.SetActive(world == 2);

        if (SoundtrackManager.Instance && PlaylistReferences.Instance)
            SoundtrackManager.Instance.ChangePlaylistOnTheBack( PlaylistReferences.Instance.GetPlaylistBy(world) );
    }
}
