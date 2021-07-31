using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDOptionsMenu : MonoBehaviour
{
    public void ShowOptions( Player.PlayerSlot a_PlayerSlot )
    {
        m_AssignedPlayer = Player.GetPlayerBySlot( a_PlayerSlot );
    }

    public void HideOptions()
    {

    }

    private Player m_AssignedPlayer;
}
