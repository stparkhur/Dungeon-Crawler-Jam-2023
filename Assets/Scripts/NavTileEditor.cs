using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class NavTileEditor : MonoBehaviour
{
    private int gridSize = 2;
    void Update()
    {
        if (!Application.isPlaying)
		{
            transform.name = ("NavTile " + new Vector3Int(Mathf.RoundToInt(transform.position.x / gridSize), Mathf.RoundToInt(transform.position.y / gridSize), Mathf.RoundToInt(transform.position.z / gridSize)));
        }
        
    }
}
