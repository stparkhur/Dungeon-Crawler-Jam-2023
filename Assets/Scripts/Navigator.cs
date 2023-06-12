using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    public Dictionary<Vector3Int, GameObject> navTiles = new Dictionary<Vector3Int, GameObject>();
    public Dictionary<Vector3Int, Vector3Int> exploredFrom = new Dictionary<Vector3Int, Vector3Int>();
    private Vector3Int[] expCoord = { new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 1), new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, -1) };
    private Vector3Int startPos;
    private Vector3Int destPos;
    private Vector3Int searchPos;
    private Queue<Vector3Int> searchQueue = new Queue<Vector3Int>();
    private Queue<Vector3Int> exploredCoord = new Queue<Vector3Int>();
    private List<Vector3Int> navList = new List<Vector3Int>();
    private bool destFound;

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
    public List<Vector3Int> GetNavList(Vector3Int s, Vector3Int d)
	{
        // Set Positions
        startPos = s;
        destPos = d;

        // Clear Old
        exploredFrom.Clear();
        searchQueue.Clear();
        searchQueue.TrimExcess();
        exploredCoord.Clear();
        exploredCoord.TrimExcess();
        destFound = false;
        // navllist comes later, we feed the dictionary into it later
        //navList.Clear();

        Pathfind();
        return navList;
	}
    private void Pathfind()
    {
        searchPos = startPos;
        searchQueue.Enqueue(searchPos);
        while(searchQueue.Count > 0)
		{
            exploredCoord.Enqueue(searchPos);
            searchPos = searchQueue.Dequeue();
            CheckIfDestFound();
            if (!destFound)
            {
                ExploreNeighbors();
            }
        }
		if (!destFound && searchQueue.Count == 0)
        {
            navList = null;
		}
		else
		{
            CreatePath();
		}
    }
    void CheckIfDestFound()
	{
        if (exploredFrom.ContainsKey(destPos))
		{
            destFound = true;
		}
	}
    void ExploreNeighbors()
	{
        foreach (Vector3Int coord in expCoord)
		{
            Vector3Int neighborPos = searchPos + coord;
            if (navTiles.ContainsKey(neighborPos))
			{
                GameObject neighborTile = GetNavTile(neighborPos, null);
                if (Vector3.Distance(startPos, neighborPos) < 2.5f && neighborTile.GetComponent<NavTile>().isOccupied == true)
                {
                    // If Tile within range and isOccupied do not Enqueue
                }
                else 
				{
                    if (!searchQueue.Contains(neighborPos) && !exploredFrom.ContainsKey(neighborPos))
					{
                        Debug.Log("searching");
                        searchQueue.Enqueue(neighborPos);
                    }
                }
                if (!exploredFrom.ContainsKey(neighborPos))
				{
                    // Whether the Tile is occupied or not, it was explored, add to the Dictionary
                    exploredFrom.Add(neighborPos, searchPos);
                }
            }
		}
	}
    void CreatePath()
    {
        navList = new List<Vector3Int>();
        navList.Add(destPos);

        Vector3Int previousPos = exploredFrom[destPos];

        while (previousPos != startPos)
        {
            navList.Add(previousPos);
            previousPos = exploredFrom[previousPos];
        }
        navList.Reverse();
    }
}
