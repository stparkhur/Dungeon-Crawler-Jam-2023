using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    private Game game;
    private GameObject player;
	private bool mouseOver = false;
	private bool quitApp = false;
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        player = GameObject.Find("Player").gameObject;
    }
	private void Update()
	{
		if (mouseOver)
		{
			//Debug.Log("mouse is over");
			if (Vector3.Distance(transform.position, player.transform.position) <= player.GetComponent<Player>().interactRange)
			{
				//Debug.Log("player in range");
				game.OverEnemy(true);
				player.GetComponent<Player>().targetObject = transform.parent.gameObject;
			}
			else
			{
				SetOutOfRange();
			}
		}
	}
	private void OnMouseOver()
	{
		mouseOver = true;
	}
	private void OnMouseExit()
	{
		SetOutOfRange();
	}
	private void SetOutOfRange()
	{
		mouseOver = false;
		if (game != null)
		{
			game.OverEnemy(false);
		}
		if(this.gameObject != null)
		{
			player.GetComponent<Player>().targetObject = null;
		}
	}
	private void OnApplicationQuit()
	{
		quitApp = true;
	}
	private void OnDisable()
	{
		if (!quitApp)
		{
			SetOutOfRange();
		}
	}
}
