using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class SetCanvasToCameraMode : MonoBehaviour
{
	// Sorting Layer
	// Order in Layer
	[SerializeField] int orderInLayer;

    void Start()
    {
		Camera cam = Camera.main;
		if (!cam)
			return;

		Canvas canvas = GetComponent<Canvas>();
		if (!canvas)
			return;

		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.worldCamera = cam;

		canvas.sortingLayerID = SortingLayer.NameToID("UI");
		canvas.sortingOrder = orderInLayer;
	}
}
