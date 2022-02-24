using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTrack : MonoBehaviour
{
    [SerializeField] List<Vector2> positions;

    [Header("Prefabs")]
    [SerializeField] GameObject skippedLinePrefab;
    [SerializeField] GameObject lineDotPrefab;

    void Start()
    {
        DrawLines();
    }

    private void DrawLines()
    {
        if (positions.Count < 1 && !skippedLinePrefab && !lineDotPrefab)
            return;

        List<Vector2> allPositions = new List<Vector2>() {Vector2.zero};
        allPositions.AddRange(positions);

        Vector2 anchoredPosition = transform.position;
        Instantiate(lineDotPrefab, anchoredPosition, Quaternion.identity, transform);

        for (int i = 0; i < allPositions.Count - 1; i++)
        {
            GameObject trackLineObject = Instantiate(skippedLinePrefab, transform);
            TrackLine trackLine = trackLineObject.GetComponent<TrackLine>();

            trackLine.Set
            (
                anchoredPosition + allPositions[i],  
                anchoredPosition + allPositions[i] + allPositions[i+1]
            );
            anchoredPosition += allPositions[i];
        }
        Instantiate(lineDotPrefab, anchoredPosition + allPositions[allPositions.Count - 1], Quaternion.identity, transform);

        skippedLinePrefab.SetActive(false);
        lineDotPrefab.SetActive(false);
    }

    private void OnDrawGizmos() 
    {
        if (positions.Count < 1)
            return;
            
        List<Vector2> allPositions = new List<Vector2>() {Vector2.zero};
        allPositions.AddRange(positions);

        Vector2 anchoredPosition = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(anchoredPosition, .1f);

        for (int i = 0; i < allPositions.Count - 1; i++)
        {
            Gizmos.DrawLine
            (
                anchoredPosition + allPositions[i],  
                anchoredPosition + allPositions[i] + allPositions[i+1]
            );
            anchoredPosition += allPositions[i];
        }

        Gizmos.DrawWireSphere(anchoredPosition + allPositions[allPositions.Count - 1], .1f);
    }
}
