using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float maxHealthPoints = 1;
    private float healthPoints;
    [Header("- Actions -")]
    public bool wander = false;
    public bool patrol = false;
    public bool hunt = false;
    // public float abilityCoodldown = 1f;
    public GameObject sprite;
    public GameObject hitFX;
    public GameObject deathFX;

    [Header("- Movement -")]
    public float moveCooldown = 1f;

    // Game related vars
    private Game game;
    private GameObject player;

    // Nav related vars
    private Navigator navigator;
    private Vector3Int entityCoordinate;
    private Vector3Int entityStartCoordinate;
    private float moveSpeed = 1f;
    private bool isMoving = false;
    private GameObject currentTile;
    private GameObject targetTile;

    private bool readyForAction = false;

    // Wander Vars
    [Header("- Wander Variables -")]
    public bool aggressiveWander = false;
    public int wanderRange = 1;
    private List<Vector3Int> wanderCoordiantes = new List<Vector3Int>();
    private List<Vector3Int> adjacentCoordinates = new List<Vector3Int>();
    private List<Vector3Int> aggroWanderCoordinates = new List<Vector3Int>();

    // Patrol Vars
    [Header("- Patrol Variables -")]
    public bool aggressivePatrol = false;
    private Vector3Int patStart;
    public GameObject[] patWaypoints;
    private List<Vector3Int> patCoordinates = new List<Vector3Int>();

    // Hunt Vars
    [Header("- Hunt Variables -")]
    public int huntRange = 1;

    // Base Attack Vars
    [Header("- Attack Variables -")]
    public float attackPower = 1f;
    public float attackSpeed = 4f;
    public float attackCooldown = 1f;

    private Animator animator;
	private void Awake()
	{
        game = GameObject.Find("Game").GetComponent<Game>();
        player = GameObject.Find("Player");
        navigator = GameObject.Find("Navigator").GetComponent<Navigator>();
        animator = GetComponentInChildren<Animator>();
    }
	// Start is called before the first frame update
	void Start()
    {
        healthPoints = maxHealthPoints;
        //UpdateEntity needs to come first
        UpdateEntity();
        entityStartCoordinate = entityCoordinate;
        if (wander)
		{
            List<Vector3Int> area = new List<Vector3Int>();
            wanderCoordiantes = GetArea(wanderRange, area);
        }
        if (aggressiveWander)
		{
            List<Vector3Int> area = new List<Vector3Int>();
            aggroWanderCoordinates = GetArea(wanderRange +1, area);
        }
        if (patrol)
		{
            patStart = entityCoordinate;
            if (patWaypoints.Length != 0)
			{
                List<GameObject> list = new List<GameObject>();
                foreach (GameObject waypoint in patWaypoints)
				{
                    list.Add(waypoint);
				}
                patCoordinates.Add(patStart);
                foreach (GameObject waypoint in list)
				{
                    Vector3Int coordinate = new Vector3Int(Mathf.RoundToInt(waypoint.transform.position.x), Mathf.RoundToInt(waypoint.transform.position.y), Mathf.RoundToInt (waypoint.transform.position.z));
                    int length = patCoordinates.Count;
                    if (patCoordinates[length - 1] != coordinate)
					{
                        patCoordinates.Add(coordinate);
					}
				}
                if (list.Count > 1)
				{
					list.Reverse();
                    foreach (GameObject waypoint in list)
                    {
                        Vector3Int coordinate = new Vector3Int(Mathf.RoundToInt(waypoint.transform.position.x), Mathf.RoundToInt(waypoint.transform.position.y), Mathf.RoundToInt(waypoint.transform.position.z));
                        int length = patCoordinates.Count;
                        if (patCoordinates[length - 1] != coordinate)
                        {
                            patCoordinates.Add(coordinate);
                        }
                    }
                }
			}
			else
			{
                Debug.LogError("Patrol needs at least one static game object as a waypoint");
			}
            Debug.Log("Pat Coord list contains " + patCoordinates.Count + " coordinates");
		}
        readyForAction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (readyForAction)
		{
            Action();
		}
    }
    private void UpdateEntity()
	{
        entityCoordinate = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        currentTile = navigator.GetNavTile(entityCoordinate, currentTile);
        currentTile.GetComponent<NavTile>().OccupyTile();
    }
    IEnumerator MoveTo(Vector3Int targetCoordinate)
    {
        // MoveTo differs from the Player Move in that it requires a adjacent tile and not the entity facing.
        isMoving = true;
        targetTile = navigator.GetNavTile(targetCoordinate, targetTile);

        // Move
        if (targetTile && !targetTile.GetComponent<NavTile>().isOccupied)
        {
            targetTile.GetComponent<NavTile>().OccupyTile();
            while (transform.position != targetTile.transform.position)
            {
                animator.SetBool("Moving", true);
                transform.position = Vector3.MoveTowards(transform.position, targetTile.transform.position, Time.deltaTime * moveSpeed);
                if (Vector3.Distance(transform.position, targetTile.transform.position) < (0.5f))
                {
                    currentTile.GetComponent<NavTile>().LeaveTile();
                }
                yield return null;
            }
            currentTile.GetComponent<NavTile>().LeaveTile();
            transform.position = targetTile.transform.position;
            animator.SetBool("Moving", false);
        }
		else
		{
            targetTile = null;
        }
        UpdateEntity();
        isMoving = false;
        StartCoroutine(Cooldown(moveCooldown));
    }
    IEnumerator Attack()
	{
        animator.SetBool("Attack", true);
        Vector3 lungePos = new Vector3((transform.position.x + player.transform.position.x) / 2f, (transform.position.y + player.transform.position.y) / 2f, (transform.position.z + player.transform.position.z) / 2f);
        while (transform.position != lungePos)
		{
            transform.position = Vector3.MoveTowards(transform.position, lungePos, Time.deltaTime * attackSpeed);
            yield return null;
        }
        player.GetComponent<Player>().TakeDamage(attackPower);
        while (new Vector3(transform.position.x, transform.position.y, transform.position.z) != entityCoordinate)
		{
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(entityCoordinate.x, entityCoordinate.y, entityCoordinate.z), Time.deltaTime * (attackSpeed * 2f));
            yield return null;
        }
        animator.SetBool("Attack", false);
        StartCoroutine(Cooldown(attackCooldown));
        yield return null;
	}
    private List<Vector3Int> GetArea(int range, List<Vector3Int> area)
    {
        area = new List<Vector3Int>();
        // Set Range
        Vector3Int wanderMax = entityStartCoordinate + new Vector3Int(range, range, range);
        Vector3Int wanderMin = entityStartCoordinate - new Vector3Int(range, range, range);
        foreach (Vector3Int coordinate in navigator.navTiles.Keys)
		{
            if (coordinate.x <= wanderMax.x && coordinate.y <= wanderMax.y && coordinate.z <= wanderMax.z && coordinate.x >= wanderMin.x && coordinate.y >= wanderMin.y && coordinate.z >= wanderMin.z)
			{
                area.Add(coordinate);
			}
		}
        // area is not zero, but this isn't getting returned????
        return area;
    }
    private void GetAdjacentCoordinates(List<Vector3Int> toCheckAgainst)
	{
        adjacentCoordinates = new List<Vector3Int>();
        List<Vector3Int> adjacents = new List<Vector3Int>();
        adjacents.Add(new Vector3Int(1, 0, 0));
        adjacents.Add(new Vector3Int(0, 0, 1));
        adjacents.Add(new Vector3Int(-1, 0, 0));
        adjacents.Add(new Vector3Int(0, 0, -1));
        foreach (Vector3Int adjacent in adjacents)
		{
            if(toCheckAgainst.Contains(entityCoordinate + adjacent))
            {
                adjacentCoordinates.Add(entityCoordinate + adjacent);
			}
		}
	}
    IEnumerator Wait()
	{
        float r = Random.Range(1, 5);
        yield return new WaitForSeconds(1f * (r / 4));
        // No UpdateEntity needed
        // No Cooldown needed
        StartCoroutine(Cooldown(0f));
	}
    IEnumerator Wander()
	{
        bool playerAdjacent = false;
        if (aggressiveWander)
		{
            GetAdjacentCoordinates(aggroWanderCoordinates);
            foreach (Vector3Int adjacent in adjacentCoordinates)
			{
                if (adjacent == new Vector3Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y), Mathf.RoundToInt(player.transform.position.z)))
				{
                    playerAdjacent = true;
                    break;
				}
			}
		}
        // If agressiveWander && playerAdjacent
        if (playerAdjacent)
		{
            StartCoroutine(Attack());
        }
        // Else do normal wander
		else
		{
            GetAdjacentCoordinates(wanderCoordiantes);
            int r = Random.Range(0, adjacentCoordinates.Count);
            Vector3Int coordinate = new Vector3Int(0, 0, 0);
            if (adjacentCoordinates.Count != 0)
            {
                coordinate = adjacentCoordinates[r];
                if (coordinate != new Vector3Int(0, 0, 0))
                {
                    StartCoroutine(MoveTo(coordinate));
                }
            }
            else
            {
                // Nowhere to move
                Cooldown(0f);
            }
        }
        yield return null;
        // Cooldown after Move
    }
    IEnumerator Patrol()
	{
        yield return null;
	}
    private void Action()
	{
        readyForAction = false;
        int r = Random.Range(0, 2);
        if (r == 0)
		{
            StartCoroutine(Wait());
		}
        if (r == 1 && wander)
		{
            StartCoroutine(Wander());
		}
	}
    IEnumerator Cooldown(float time)
	{
        yield return new WaitForSeconds(time);
        readyForAction = true;
	}
    //Things that can happen to and entity
    public void TakeDamage(float damage)
	{
        healthPoints = healthPoints - damage;
        if (healthPoints <= 0)
		{
            StopAllCoroutines();
            // Unoccupy the tile, probably do other things too...
            currentTile.GetComponent<NavTile>().LeaveTile();
            if(targetTile != null)
			{
                targetTile.GetComponent<NavTile>().LeaveTile();
            }
            if (sprite != null)
			{
                sprite.SetActive(false);
			}
            if (deathFX != null)
			{
                deathFX.SetActive(true);
                deathFX.GetComponent<ParticleSystem>().Play();
            }
            Invoke("DestroyEntity", 2f);
		}
		else
		{
            if (hitFX != null)
			{
                hitFX.GetComponent<ParticleSystem>().Play();
			}
		}
	}
    public void DestroyEntity()
	{
        //Then perish
        Destroy(gameObject);
    }
}
