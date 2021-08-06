using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class HUDController : Singleton< HUDController >
{
    public HUDManeuverDisplay ManeuverDisplay;
    public HUDOptionsMenu OptionsMenu;
    public RectTransform InteractPrompt;

    private void Awake()
    {
        PlayerCardP1.gameObject.SetActive( Player.all.Count > 0 );
        PlayerCardP2.gameObject.SetActive( Player.all.Count > 1 );
        PlayerCardP3.gameObject.SetActive( Player.all.Count > 2 );
        PlayerCardP4.gameObject.SetActive( Player.all.Count > 3 );

        foreach ( PlayerInput playerInput in PlayerInput.all )
        {
            Player player = playerInput as Player;
            player.AddInputListener( Player.Control.START_PRESSED, callback => OnStartPressed( player ) );
        }
    }

    private void Start()
    {
        m_InteractPrompts = new ( RectTransform, Interactable )[ 4 ];

        for ( int i = 0; i < 4; ++i )
        {
            RectTransform newInteractPrompt = Instantiate( InteractPrompt, transform );
            newInteractPrompt.gameObject.SetActive( false );
            m_InteractPrompts[ i ].Item1 = newInteractPrompt;
        }
    }

    //public static void ShowInteractPrompt( Interactable a_Interactable, Vector2 a_ScreenPosition )
    //{
    //    if ( Array.FindIndex( Instance.m_InteractPrompts, prompt => ReferenceEquals( prompt.Item2, a_Interactable ) ) != -1 )
    //    {
    //        return;
    //    }

    //    int index = Array.FindIndex( Instance.m_InteractPrompts, prompt => prompt.Item2 == null );
    //    Instance.m_InteractPrompts[ index ].Item2 = a_Interactable;
    //    a_Interactable.ActiveInteractPrompt = Instance.m_InteractPrompts[ index ].Item1;
    //    Instance.m_InteractPrompts[ index ].Item1.anchoredPosition = a_ScreenPosition;
    //    Instance.m_InteractPrompts[ index ].Item1.gameObject.SetActive( true );
    //}

    //public static void HideInteractPrompt( Interactable a_Interactable )
    //{
    //    if ( Instance == null )
    //    {
    //        return;
    //    }

    //    int index = Array.FindIndex( Instance.m_InteractPrompts, prompt => ReferenceEquals( prompt.Item2, a_Interactable ) );

    //    if ( index == -1 )
    //    {
    //        return;
    //    }

    //    a_Interactable.ActiveInteractPrompt = null;
    //    Instance.m_InteractPrompts[ index ].Item2 = null;
    //    Instance.m_InteractPrompts[ index ].Item1.gameObject.SetActive( false );
    //}

    private void OnStartPressed( Player a_Player )
    {
        if ( OptionsMenu.gameObject.activeSelf )
        {
            return;
        }

        OptionsMenu.ShowOptions( a_Player );
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

    private ( RectTransform, Interactable )[] m_InteractPrompts;

    [ SerializeField ] private HUDPlayerCard PlayerCardP1;
    [ SerializeField ] private HUDPlayerCard PlayerCardP2;
    [ SerializeField ] private HUDPlayerCard PlayerCardP3;
    [ SerializeField ] private HUDPlayerCard PlayerCardP4;
}
