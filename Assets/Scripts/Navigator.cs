using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    public Dictionary<Vector3Int, GameObject> navTiles = new Dictionary<Vector3Int, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(navTiles.Count);
    }
    public void AddNavTile(Vector3Int coordinate, GameObject tileObject)
	{
        navTiles.Add(coordinate, tileObject);
	}
    public GameObject GetNavTile(Vector3Int coordinate, GameObject tileObject)
	{
        if(navTiles.ContainsKey(coordinate))
		{
            //Debug.Log("return " + navTiles[coordinate]);
            return (navTiles[coordinate]);
		}
		else
		{
            return (null);
		}
	}
}
