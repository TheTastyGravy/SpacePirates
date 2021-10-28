using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadingController : MonoBehaviour
{


    void Start()
    {
        Player.GetPlayerBySlot(Player.PlayerSlot.P1).AddInputListener(Player.Control.A_PRESSED, OnAPressed);
    }

    void OnDestroy()
    {
        Player.GetPlayerBySlot(Player.PlayerSlot.P1).RemoveInputListener(Player.Control.A_PRESSED, OnAPressed);
    }

    private void OnAPressed(InputAction.CallbackContext context)
    {
        GameManager.ChangeState(GameManager.GameState.GAME);
    }
}
