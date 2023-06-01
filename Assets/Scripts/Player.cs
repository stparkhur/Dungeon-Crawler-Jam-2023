using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealthPoints = 100;
    public float healthPoints;
    public float interactRange = 1.5f;
    public GameObject targetObject = null;

    [Header("- Attack -")]
    public GameObject attackFX;
    public float attackCooldown = 1f;
    public AudioClip[] attackSFX;
    private bool canAttack = true;

    private Navigator navigator;
    private Vector3Int playerCoordinate;
    private Vector3Int playerFacing;
    private float moveSpeed = 4f;
    private float rotateSpeed = 180f;
    private bool isMoving = false;
    private GameObject currentTile;
    private GameObject targetTile;
    private void Awake()
    {
        navigator = GameObject.Find("Navigator").GetComponent<Navigator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        healthPoints = maxHealthPoints;
        UpdatePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isMoving)
        {
            StartCoroutine(Move(1));
        }
        if (Input.GetKeyDown(KeyCode.S) && !isMoving)
        {
            StartCoroutine(Move(-1));
        }
        if (Input.GetKeyDown(KeyCode.A) && !isMoving)
        {
            StartCoroutine(Rotate(-1));
        }
        if (Input.GetKeyDown(KeyCode.D) && !isMoving)
        {
            StartCoroutine(Rotate(1));
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (targetObject != null && canAttack)
            {
                canAttack = false;
                StartCoroutine(Attack());
            }
        }
    }
    private void UpdatePlayer()
    {
        //Get Coordinate and Occupy
        playerCoordinate = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        currentTile = navigator.GetNavTile(playerCoordinate, currentTile);
        currentTile.GetComponent<NavTile>().OccupyTile();

        //Get Facing
        int x = 0;
        int y = 0;
        int z = 0;
        // Round angle to int to snap to cardinal directions !!IMPORTANT!! D:
        var yAngle = Mathf.RoundToInt(transform.eulerAngles.y);
        if (yAngle == 0 || yAngle == 360 || yAngle == -360)
        {
            z = 1;
        }
        if (yAngle == 180 || yAngle == -180)
        {
            z = -1;
        }
        if (yAngle == 90 || yAngle == -270)
        {
            x = 1;
        }
        if (yAngle == -90 || yAngle == 270)
        {
            x = -1;
        }
        playerFacing = new Vector3Int(x, y, z);
    }
    IEnumerator Move(int moveDirection)
    {
        isMoving = true;
        // Check if NavTile exists forward
        Vector3Int targetCoordinate = playerCoordinate + new Vector3Int(playerFacing.x * moveDirection, playerFacing.y * moveDirection, playerFacing.z * moveDirection);
        GameObject targetTile = null;
        targetTile = navigator.GetNavTile(targetCoordinate, targetTile);

        // Move
        if (targetTile && !targetTile.GetComponent<NavTile>().isOccupied)
        {
            targetTile.GetComponent<NavTile>().OccupyTile();
            while (transform.position != targetTile.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetTile.transform.position, Time.deltaTime * moveSpeed);
                yield return null;
            }
            currentTile.GetComponent<NavTile>().LeaveTile();
            transform.position = targetTile.transform.position;
        }
        UpdatePlayer();
        isMoving = false;
    }
    IEnumerator Rotate(int rotateDirection)
    {
        isMoving = true;
        Quaternion targetRot = Quaternion.Euler(new Vector3(0, (transform.eulerAngles.y + (90 * rotateDirection)), 0));
        while (targetRot.eulerAngles.y - transform.eulerAngles.y > 0.1 || targetRot.eulerAngles.y - transform.eulerAngles.y < -0.1)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
            yield return null;
        }
        transform.rotation = targetRot;
        UpdatePlayer();
        isMoving = false;
	}
    // Player Actions
    IEnumerator Attack()
    {
        if (attackFX != null)
		{
            attackFX.transform.LookAt(targetObject.transform);
            attackFX.GetComponent<ParticleSystem>().Play();
		}
        if (attackSFX.Length != 0)
		{
            AudioSource.PlayClipAtPoint(attackSFX[Random.Range(0, attackSFX.Length)], transform.position);
        }
        // This Delay can cause you to miss the target if it has moved out from your cursor!
        yield return new WaitForSeconds(0.125f);
        if (targetObject != null)
		{
            DealDamage(targetObject);
        }
        else
		{
            // You missed!
		}
        StartCoroutine(Cooldown(attackCooldown));
        yield return null;
    }
    private void DealDamage(GameObject targetObject)
    {
        int damage = 1;
        targetObject.GetComponent<Entity>().TakeDamage(damage);
    }
    IEnumerator Cooldown(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }
    // Actions Against Player
    public void TakeDamage(float damage)
    {
        healthPoints = healthPoints - damage;
        Debug.Log("Player health = " + healthPoints);
    }
}
