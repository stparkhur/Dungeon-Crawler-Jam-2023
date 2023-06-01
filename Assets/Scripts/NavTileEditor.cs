using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class NavTileEditor : MonoBehaviour
{
    void Update()
    {
        if (!Application.isPlaying)
		{
            transform.name = ("NavTile " + new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z)));
        }
        
    }
}
