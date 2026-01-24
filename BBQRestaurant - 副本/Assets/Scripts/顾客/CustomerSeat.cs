using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomerSeat:MonoBehaviour
{
    [HideInInspector]
    public Transform seatPoint;    // Âä×ùµã×ø±ê
    [HideInInspector]
    public bool isOccupied = false;

    private void Awake()
    {
        seatPoint = GetComponent<Transform>();
    }
}
