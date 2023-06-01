using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    GameObject playerObject;
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, playerObject.transform.eulerAngles.y, 0);
    }
}
