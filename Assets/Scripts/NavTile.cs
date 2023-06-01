using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavTile : MonoBehaviour
{
    public bool debugMode = false;
    [SerializeField]
    private Vector3Int tileCoordinate;
    public bool isOccupied;
    void Awake()
    {
        tileCoordinate = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        GameObject.Find("Navigator").GetComponent<Navigator>().AddNavTile(tileCoordinate, this.gameObject);
    }
    public void OccupyTile()
	{
        isOccupied = true;
        if(debugMode)
		{
            GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", Color.red);
		}
	}
    public void LeaveTile()
	{
        isOccupied = false;
        if (debugMode)
        {
            GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
    }
}
