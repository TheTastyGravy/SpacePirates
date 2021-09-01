using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class HUDOptionsMenu : MonoBehaviour
{
    public void ShowOptions( Player a_Player )
    {
        gameObject.SetActive( true );
        m_AssignedPlayer = a_Player;
        m_AssignedPlayer.AddInputListener( Player.Control.B_PRESSED, OnBPressed );
        Time.timeScale = 0.0f;
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).actionsAsset = m_AssignedPlayer.actions;
    }

    public void HideOptions()
    {
        m_AssignedPlayer.RemoveInputListener( Player.Control.B_PRESSED, OnBPressed );
        m_AssignedPlayer = null;
        gameObject.SetActive( false );
        Time.timeScale = 1.0f;
    }

    private void OnBPressed( InputAction.CallbackContext _ ) => HideOptions();

    private Player m_AssignedPlayer;
}
