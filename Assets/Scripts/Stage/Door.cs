using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Door : MonoBehaviour
{
    [Header("Values")] 
    [SerializeField] float starRadius = 1f;
    [SerializeField] float openDuration = .1f;
    [SerializeField] bool startClosed;
    [SerializeField] bool explorationSetup;

    [Header("References")] 
    [SerializeField] Transform visualComponent;
    [SerializeField] RoundPortal portal;
    [SerializeField] TextMeshPro healthPreview;
    [SerializeField] CircleCollider2D mainCollider;
    [SerializeField] Animator animator;
    [SerializeField] AK.Wwise.Event collectAKEvent;
    [SerializeField] AK.Wwise.Event openAKEvent;

    [Space]
    
    [SerializeField] private List<Lock> externalLocks; 

    Sequence openSequence;
    DoorAnimation doorAnimation;

    //private List<Lock> internalLocks;
    int health;

    private int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
            if (healthPreview)
                healthPreview.text = health.ToString();
        }
    }
    
    Round round;
    

    private void OnValidate() 
    {
        //UpdateAttributes();
    }

    private void Start()
    {
        doorAnimation = GetComponent<DoorAnimation>();
        round = GetComponentInParent<Round>();

        Health = externalLocks.Count;

        if (explorationSetup)
        {
            animator.SetBool("Open", true);
            //Debug.Log("hey explo");
            
            if (portal)
                portal.gameObject.SetActive(true);

            return;
        }

        SetLocks();
        ResetStates();

        if (round)
        {
            round.OnReset += ResetStates;
            //round.OnPassRound += () =>
            //{
                //if(gameObject)
                //    gameObject.SetActive(false);

                //if(portal)
                //    portal.gameObject.SetActive(false);
            //};
        }
            
    }

    private void SetLocks()
    {
        /**
        // internalLocks = new List<Lock>();
        //
        // for (int i = 0; i < originalInternalHealth; i++)
        // {
        //     var go = Instantiate(lockReference, transform);
        //     float angle = 360f * i/ originalInternalHealth;
        //     go.transform.position = transform.position + Mathg.AngleToDirection(angle) * starRadius * 0.65f;
        //     Lock l = go.GetComponent<Lock>();
        //     l.Init(this, true);
        //     internalLocks.Add(l);
        // }    
        **/

        for (int i = 0; i < externalLocks.Count; i++)
        {
            externalLocks[i].Init(this, false);
        }
    }

    private void ResetStates()
    {
        gameObject.SetActive(true);
        
        for (int j = 0; j < externalLocks.Count; j++)
            externalLocks[j].OnReset();

        if (portal) 
        {
            portal.transform.SetParent(transform);
            portal.transform.position = transform.position;
            portal.gameObject.SetActive(false);
        }
        
        animator.SetBool("Open", false);
        //Debug.Log("reset states!");
        
        Health = externalLocks.Count;
        if (Health < 1 && !startClosed)
            SpawnPortal(initiation: true);

        SetInteractable(true);
    }

    public void Collect (Collectable collectable)
    {
        // Debug.Log("Collect");
        // internalLocks[internalHealth-1].Unlock();
        // internalHealth--;
        // TakeHealth();
        //
        // collectable.gameObject.SetActive(false);
        // collectAKEvent?.Post(gameObject);
    }

    public void TakeHealth()
    {
        Health--;

        if (Health < 1)
            SpawnPortal();
    }

    public void SpawnPortal(bool initiation = false)
    {
        if (!portal || !round)
            return;

        if (initiation)
        {
            portal.transform.position = transform.position;
            PortalSetup();

            animator.SetTrigger("InstantOpen");
            animator.SetBool("Open", true);
            Debug.Log("spawn portal!");
            return;
        }

        OpenAnimation();
    }

    private void OpenAnimation()
    {
        if (openSequence != null)
            openSequence.Kill();

        portal.transform.position = transform.position;
        portal.VisualSetup();
        animator.SetBool("Open", true);
        Debug.Log("open animation");
        openAKEvent?.Post(gameObject);

        openSequence = DOTween.Sequence();

        openSequence.AppendInterval( openDuration );
        openSequence.AppendCallback( PortalSetup );
        openSequence.Join( visualComponent.DOPunchRotation(Vector3.back * 5f, .3f, vibrato: 0, elasticity: 0) );
        //openSequence.AppendInterval( VFXs call );
    }

    private void PortalSetup()
    {
        portal.Setup
        (
            (player) => 
            {
                if (doorAnimation)
                {
                    round.OnPortalReached?.Invoke();    

                    doorAnimation.SetupAnimationStageTransiton( this, player, 
                    
                        OnAnimationEnd: () => 
                        {
                            round.RoundCleared();
                            portal.transform.SetParent(transform);
                        }

                    );
                    
                    return;
                }

                round.RoundCleared();
                portal.transform.SetParent(transform);
            }
        );
    }

    public void SetInteractable (bool value)
    {
        if (mainCollider)
            mainCollider.enabled = value;
    }

    public void SetOpenState (bool value, bool instant)
    {
        if (!animator)
            return;

        Debug.Log("huh? " + value);

        if (instant)
        {
            animator.SetBool("InstantOpen", value);
        }
        animator.SetBool("Open", value);
    }
}
