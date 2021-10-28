using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LoadingController : MonoBehaviour
{
    public TextMeshProUGUI continueText;



    void Start()
    {
        Player.GetPlayerBySlot(Player.PlayerSlot.P1).AddInputListener(Player.Control.A_PRESSED, OnAPressed);
        InvokeRepeating(nameof(ToggleText), 0.5f, 0.5f);
    }

    void OnDestroy()
    {
        Player.GetPlayerBySlot(Player.PlayerSlot.P1).RemoveInputListener(Player.Control.A_PRESSED, OnAPressed);
    }

    private void OnAPressed(InputAction.CallbackContext context)
    {
        GameManager.ChangeState(GameManager.GameState.GAME);
    }

    private void ToggleText()
    {
        continueText.enabled = !continueText.enabled;
    }
}
