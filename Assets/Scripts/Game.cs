using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [Header("- Cursor -")]
    public Texture2D cursorPrimary = null;
    public Texture2D cursorInteract = null;
    public Texture2D cursorInteractDown = null;
    public Texture2D cursorAttack = null;
    private Vector2 hotspot = new Vector2(0, 0);
    //private bool overInteract = false;
    private bool overEnemy = false;

    [Header("- Canvas -")]
    public Text hpText;

    // Some Private Stuff
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (( !(Input.mousePosition.x > Screen.width) && !(Input.mousePosition.x < 0)) && (!(Input.mousePosition.y > Screen.height) && !(Input.mousePosition.y < 0)))
		{
            if (!overEnemy)
			{
                if (Input.GetMouseButton(0))
                {
                    Cursor.SetCursor(cursorInteractDown, hotspot, CursorMode.Auto);
                }
                else
                {
                    Cursor.SetCursor(cursorPrimary, hotspot, CursorMode.Auto);
                }
            }
            if(overEnemy)
			{
                Cursor.SetCursor(cursorAttack, hotspot, CursorMode.Auto);
			}
        }
        if (player != null)
		{
            hpText.text = player.healthPoints.ToString();
		}
        if (Input.GetKeyDown(KeyCode.Escape))
		{
            Application.Quit();
		}
    }
    public void OverEnemy(bool b)
	{
        if (b)
		{
            overEnemy = true;
		}
		else
		{
            overEnemy = false;
		}
	}
}
