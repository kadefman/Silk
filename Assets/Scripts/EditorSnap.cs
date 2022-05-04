using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSnap : MonoBehaviour
{
    private Vector3 gridSize = new Vector3(.96f, .56f, 1f);

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && transform.hasChanged)
            SnapToGrid();
    }

    private void SnapToGrid()
    {
        transform.position = new Vector3(Mathf.Round(this.transform.position.x / gridSize.x) * gridSize.x,
                                        Mathf.Round(this.transform.position.y / gridSize.y) * gridSize.y,
                                        Mathf.Round(this.transform.position.z / gridSize.x) * gridSize.z);

    }
}
