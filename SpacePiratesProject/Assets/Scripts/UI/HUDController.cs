using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class HUDController : Singleton< HUDController >
{
    //public HUDManeuverDisplay ManeuverDisplay;
    public HUDOptionsMenu OptionsMenu;



    private void Awake()
    {
        foreach ( PlayerInput playerInput in PlayerInput.all )
        {
            Player player = playerInput as Player;
            player.AddInputListener( Player.Control.START_PRESSED, callback => OnStartPressed( player ) );
        }
    }

    private void OnStartPressed( Player a_Player )
    {
        if ( OptionsMenu.gameObject.activeSelf )
        {
            return;
        }

        OptionsMenu.ShowOptions( a_Player );
    }
}
