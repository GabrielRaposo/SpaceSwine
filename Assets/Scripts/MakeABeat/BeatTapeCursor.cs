﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeABeat
{
    public class BeatTapeCursor : MonoBehaviour
    {
        [SerializeField] float duration;
        [SerializeField] float arrowDistance;
        [SerializeField] Vector2 sizeExtension;
        [SerializeField] Gradient gradient;
        [SerializeField] Gradient arrowGradient;
        [SerializeField] GameObject arrowPrefab;

        List <(Transform transf, SpriteRenderer renderer)> arrows;

        float t;
        bool highlighted;

        Vector2 startSize;

        SpriteRenderer display;
        BeatMaster beatMaster;

        private void Awake() 
        {
            beatMaster = GetComponentInParent<BeatMaster>();
            display = GetComponent<SpriteRenderer>();

            startSize = display.size;
            arrows = new List<(Transform transf, SpriteRenderer renderer)>();
        }

        public void InitArrows (List <BeatNavigationItem.NavigationData> dataList)
        {
            if (!arrowPrefab || dataList.Count < 1)
                return;

            arrows = new List<(Transform transf, SpriteRenderer renderer)>();

            foreach (var data in dataList)
            {
                GameObject arrowObj = Instantiate(arrowPrefab, transform);
                {
                    Vector2 origin = transform.position;
                    float angle = Vector2.SignedAngle(Vector2.up, data.Direction(origin));
                    arrowObj.transform.eulerAngles = Vector3.forward * ( angle + 90 );
                    arrowObj.transform.position = transform.position + ( data.Direction(origin) * (arrowDistance + data.distanceModifier) );
                }
            
                SpriteRenderer arrowRenderer = arrowObj.GetComponentInChildren<SpriteRenderer>();

                arrows.Add ( (arrowObj.transform, arrowRenderer) );
            }

            arrowPrefab.SetActive(false);
        }

        public void SetState (bool value)
        {
            display.enabled = value;
            SetArrowsVisibility(value);

            if (!value)
            {
                display.size = startSize;
                display.color = gradient.Evaluate(0);
            }

            highlighted = value;
        }

        public void SetArrowsVisibility(bool value)
        {
            if (arrows.Count > 0)
            {
                foreach (var arrow in arrows)
                {
                    arrow.renderer.enabled = value;
                    if (!value)
                        arrow.renderer.color = arrowGradient.Evaluate(0);
                }
            }
        }

        void Update()
        {
            if (!highlighted)
                return;

            t += Time.deltaTime;
            if (t > duration)
                t = 0;

            float percent = t / duration;

            if (beatMaster.IsRunning)
            {
                t = percent = beatMaster.GetTimePerBeat(speed: 2);
            }

            display.size = Vector2.Lerp(startSize, startSize + sizeExtension, percent);
        
            display.color = gradient.Evaluate(percent);
            if (arrows.Count > 0)
            {
                foreach (var arrow in arrows)
                {
                    arrow.renderer.color = arrowGradient.Evaluate(percent);
                    arrow.renderer.transform.localPosition = Vector2.Lerp(Vector2.right * .05f, Vector2.zero, percent);
                }
            }

        }
    }

}