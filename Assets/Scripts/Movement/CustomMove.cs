using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class CustomMove : MonoBehaviour
{
    [SerializeField] List <Vector2> positions;
    [SerializeField] float speed;
    [SerializeField] float startDelay;
    [SerializeField] bool loop;
    [SerializeField] bool easeExtremities;
    [SerializeField] bool invertAutoEase;
    [SerializeField] protected CustomEase customEase;
    [SerializeField] protected bool moveOnStart;
    [SerializeField] bool stopDuringOutro;

    protected float startingT = 0; 
    int direction = 1;
    int index = 1;
    float t;
    float hold;
    float startupCount;

    bool moving;
    bool initiated;

    Vector3 startingPosition;

    public UnityAction OnStepReached;
    public UnityAction OnLoopCompleted;
    public UnityAction OnDirectionInverted;

    public UnityAction OnStart;

    public enum EventEnum { OnStepReached, OnLoopCompleted, OnDirectionInverted }

    private void OnEnable() 
    {
        if (speed <= 0)
        {
            enabled = false;
            return;
        }

        if (!initiated)
        {
            InitList();
            
            startingPosition = transform.position;
            initiated = true;
        }

    }
    private void InitList()
    {
        positions.Insert(0, transform.position);
    }

    protected virtual void Start() 
    {
        Restart();

        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += Restart;
            if (stopDuringOutro)
                round.OnPortalReached += PauseMovement;
        }

        OnStart?.Invoke();
    }

    public void Restart()
    {
        if (initiated)
            transform.position = startingPosition;

        direction = 1;
        index = 1;
        t = startingT; 
        hold = 0;
        startupCount = startDelay;
        moving = moveOnStart;
    }

    public void StartMovement()
    {
        if (!moveOnStart)
        {
            direction = 1;
            index = 1;
            t = startingT;    
            hold = 0;
        }

        startupCount = startDelay;
        moving = true;
    }

    public void ResumeMovement()
    {
        moving = true;
    }

    public void PauseMovement()
    {
        moving = false;
    }

    public void StopMovement()
    {
        index = 1;
        t = startingT;    
        moving = false;
    }

    //private void Update() 
    //{
    //    if (Input.GetKeyDown(KeyCode.O))
    //        ResumeMovement();

    //    if (Input.GetKeyDown(KeyCode.P))
    //        PauseMovement();
    //}

    private void FixedUpdate() 
    {
        if (!moving)
            return;

        if (startupCount > 0)
        {
            startupCount -= Time.fixedDeltaTime;
            return;
        }

        if (hold > 0)
        {
            hold -= Time.fixedDeltaTime;
            return;
        }

        int previous = PreviousIndex();

        float distance = Vector2.Distance(PosAtIndex(previous), PosAtIndex(index));
        if (PosAtIndex(previous) == PosAtIndex(index))
            distance = 0;
        
        float duration = distance / speed;

        if (duration == 0)
        {
            //Debug.Log($"previous :{previous}, index : {index}");
            t = 0;
            MoveIndex();
            return;
        }

        t += Time.fixedDeltaTime;
        if (t > duration)
            t = duration;

        float x = t / duration; // go through EASE
        if (customEase == CustomEase.None)
        {
            if (!loop && easeExtremities)
            {
                (bool entering, bool exiting) onExtremity = OnExtremity();
                if (onExtremity.entering && onExtremity.exiting)
                    x = x.SetEase (CustomEase.EaseInOut);
                else if (onExtremity.entering)
                    x = x.SetEase (!invertAutoEase ? CustomEase.EaseOut : CustomEase.EaseIn);
                else if (onExtremity.exiting)
                    x = x.SetEase (!invertAutoEase ? CustomEase.EaseIn : CustomEase.EaseOut);
            }
        }
        else x = x.SetEase(customEase);

        Vector3 position = Vector2.Lerp( PosAtIndex(previous), PosAtIndex(index), x);
        position.z = transform.position.z;
        transform.position = position;

        if (t >= duration)
        {
            t = 0;
            MoveIndex();
        }
    }

    private (bool entering, bool exiting) OnExtremity()
    {
        int i = index + direction;
        int j = PreviousIndex();
        return (i < 0 || i >= positions.Count, j <= 0 || j + 1 >= positions.Count);
    }

    public Vector2 PosAtIndex (int localIndex)
    {
        if (localIndex < 0)
            return Vector2.zero;

        Vector2 pos = Vector2.zero;
        for (int i = 0; i < positions.Count; i++)
        {
            if (i > localIndex)
                return pos;

            pos += positions[i];
        }

        return pos;
    }

    private int PreviousIndex()
    {
        int previous = index - direction;
        if (previous < 0)
        {
            if (loop)
                previous = positions.Count - 1;
            else
                previous = 0;
        } 
        else if (previous >= positions.Count)
        {
            previous = positions.Count - 1;
        }

        //Debug.Log($"Index: {index}, Previous Index: {previous}, Direction: {direction}");
        return previous;
    }

    private void MoveIndex()
    {
        index += direction;
        OnStepReached?.Invoke();

        if (index < 0 || index >= positions.Count) 
        {
            if (loop) 
            {
                index = 0;
            } 
            else 
            {
                index -= 2 * direction;
                direction *= -1;
                OnDirectionInverted?.Invoke();
            }
        }

        if (loop & PreviousIndex() == 0)
        {
            OnLoopCompleted?.Invoke();
        }
    }

    public void SetStartingT (float startingT)
    {
        this.startingT = startingT;
    } 

    public void JumpToIndex (int i)
    {
        if (positions == null || positions.Count < 1)
            return;

        Vector3 pos = positions [i % positions.Count];
        pos.z = transform.position.z;
        transform.position += pos;

        index = i;
        MoveIndex();
    }

    public int Count
    {
        get { return positions.Count; } 
    }

    private void OnDisable() 
    {
        StopAllCoroutines();

        if (initiated)
        {
            //transform.position = startingPosition;
        }    
    }

    private void OnDrawGizmosSelected() 
    {
        if (Application.isPlaying)
            return;

        if (positions == null)
            return;

        Vector2 previousPos = transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(previousPos, Vector2.one * (loop ? .15f : .3f));

        for (int i = 0; i < positions.Count; i++)
        {
            float size = (i + 1 < positions.Count || loop) ? .15f : .3f;

            Vector2 pos = previousPos + positions[i];
            Gizmos.DrawLine(previousPos, pos);
            Gizmos.DrawCube(pos, Vector2.one * size);

            previousPos = pos;

            if (loop && i + 1 >= positions.Count)
            {
                Gizmos.DrawLine(pos, transform.position);
            }
        }
    }   

    public void SetHold (float hold)
    {
        this.hold = hold;
    }
}
