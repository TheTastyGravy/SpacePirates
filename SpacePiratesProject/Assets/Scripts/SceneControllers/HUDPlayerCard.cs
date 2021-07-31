using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDPlayerCard : MonoBehaviour
{
    public Player.PlayerSlot PlayerSlot;
    public GameObject ReconnectController;
    public HUDHealthBar HealthBar;
    public Image PlayerSprite;

    private void Awake()
    {
        m_AssignedPlayer = Player.GetPlayerBySlot( PlayerSlot );
        m_AssignedPlayer.onDeviceLost += OnDeviceLost;
        m_AssignedPlayer.onDeviceRegained += OnDeviceRegained;
        ReconnectController.SetActive( !m_AssignedPlayer.IsDeviceConnected );
    }

    private void OnDeviceLost( PlayerInput _ )
    {
        ReconnectController.SetActive( true );
    }

    private void OnDeviceRegained( PlayerInput _ )
    {
        ReconnectController.SetActive( false );
    }

    private Player m_AssignedPlayer;
}
