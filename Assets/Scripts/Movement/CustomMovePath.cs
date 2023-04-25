using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMovePath : MonoBehaviour
{
    [SerializeField] CustomMove customMove;

    [Header("Prefabs")]
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject dotPrefab;

    void Start()
    {
        if (!customMove || !linePrefab || !dotPrefab)
        {
            gameObject.SetActive(false);
            return;
        }

        SetPath();
    }

    private void SetPath()
    {
        for (int i = 0; i < customMove.Count; i++)
        {
            GameObject dotObject = Instantiate(dotPrefab, transform);
            dotObject.transform.position = customMove.PosAtIndex(i);
            dotObject.SetActive(true);

            if (i + 1 >= customMove.Count)
                continue;
            
            GameObject lineObject = Instantiate(linePrefab, transform);
            lineObject.transform.position = customMove.PosAtIndex(i);

            Vector2 direction = customMove.PosAtIndex(i + 1) - customMove.PosAtIndex(i);
            lineObject.transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction.normalized);
            
            SpriteRenderer lineRenderer = lineObject.GetComponentInChildren<SpriteRenderer>();
            if (lineRenderer)
                lineRenderer.size = new Vector2 (lineRenderer.size.x, direction.magnitude);

            lineObject.SetActive(true);
        }

        dotPrefab.SetActive(false);
        linePrefab.SetActive(false);
    }
}