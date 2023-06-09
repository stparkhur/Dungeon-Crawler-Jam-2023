using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]

public class Waypoint : MonoBehaviour
{
    void Start()
    {
        this.gameObject.SetActive(false);
    }
}
