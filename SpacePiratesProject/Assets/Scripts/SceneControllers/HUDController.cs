using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUDController : Singleton< HUDController >
{
    public HUDManeuvre ManeuverDisplay;
    public HUDOptionsMenu OptionsMenu;

    private void Awake()
    {
        PlayerCardP1.gameObject.SetActive( Player.all.Count > 0 );
        PlayerCardP2.gameObject.SetActive( Player.all.Count > 1 );
        PlayerCardP3.gameObject.SetActive( Player.all.Count > 2 );
        PlayerCardP4.gameObject.SetActive( Player.all.Count > 3 );

        foreach ( PlayerInput playerInput in PlayerInput.all )
        {
            Player player = playerInput as Player;
            player.AddInputListener( Player.Control.START_PRESSED, callback => OnStartPressed( player.Slot ) );
        }
    }

    private void OnStartPressed( Player.PlayerSlot a_PlayerSlot )
    {
        if ( OptionsMenu.gameObject.activeSelf )
        {
            return;
        }

        OptionsMenu.ShowOptions( a_PlayerSlot );
    }

    public static HUDPlayerCard GetPlayerCard( Player.PlayerSlot a_PlayerSlot )
    {
        switch ( a_PlayerSlot )
        {
            case Player.PlayerSlot.P1:
                {
                    return Instance.PlayerCardP1;
                }
            case Player.PlayerSlot.P2:
                {
                    return Instance.PlayerCardP2;
                }
            case Player.PlayerSlot.P3:
                {
                    return Instance.PlayerCardP3;
                }
            case Player.PlayerSlot.P4:
                {
                    return Instance.PlayerCardP4;
                }
        }

        return null;
    }

    [ SerializeField ] private HUDPlayerCard PlayerCardP1;
    [ SerializeField ] private HUDPlayerCard PlayerCardP2;
    [ SerializeField ] private HUDPlayerCard PlayerCardP3;
    [ SerializeField ] private HUDPlayerCard PlayerCardP4;
}
