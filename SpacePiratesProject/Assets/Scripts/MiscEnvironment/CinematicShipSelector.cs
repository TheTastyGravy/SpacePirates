using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicShipSelector : MonoBehaviour
{
    public GameObject ship1;
    public GameObject ship2;
    public GameObject ship3;



    void Awake()
    {
        ship1.SetActive(false);
        ship2.SetActive(false);
        ship3.SetActive(false);

        switch (GameManager.SelectedShip)
        {
            case 0:
                ship1.SetActive(true);
                break;
            case 1:
                ship2.SetActive(true);
                break;
            case 2:
                ship3.SetActive(true);
                break;
        }
    }
}
