using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDock : MonoBehaviour
{
    public Transform DockTransform;
    public GameObject ConnectController;
    public GameObject ReconnectController;
    public GameObject PressStartToJoin;
    public GameObject PressBToLeave;
    public GameObject PressXToCycleVariant;

    public Stage CurrentStage
    {
        get
        {
            return m_CurrentStage;
        }
        set
        {
            if ( m_CurrentStage == value )
            {
                return;
            }

            // Unset old events.
            switch ( m_CurrentStage )
            {
                case Stage.WAIT_ON_DEVICE:
                    {
                        ConnectController?.SetActive( false );
                    }
                    break;
                case Stage.WAIT_ON_RECONNECT:
                    {
                        ReconnectController?.SetActive( false );
                    }
                    break;
                case Stage.WAIT_ON_JOIN:
                    {
                        PressStartToJoin?.SetActive( false );
                    }
                    break;
                case Stage.CHOOSE_CHARACTER:
                    {
                        PressBToLeave?.SetActive( false );
                        PressXToCycleVariant?.SetActive( false );
                        m_AssignedPlayer.GetInputAction( IPlayer.Control.DPAD_PRESSED ).performed -= OnDPADPressed;
                        m_AssignedPlayer.GetInputAction( IPlayer.Control.B_PRESSED ).performed -= OnBPressed;
                        m_AssignedPlayer.GetInputAction( IPlayer.Control.X_PRESSED ).performed -= OnXPressed;
                    }
                    break;
            }

            m_CurrentStage = value;

            // Set new events.
            switch ( m_CurrentStage )
            {
                case Stage.WAIT_ON_DEVICE:
                    {
                        ConnectController?.SetActive( true );
                    }
                    break;
                case Stage.WAIT_ON_RECONNECT:
                    {
                        ReconnectController?.SetActive( true );
                    }
                    break;
                case Stage.WAIT_ON_JOIN:
                    {
                        PressStartToJoin?.SetActive( true );
                    }
                    break;
                case Stage.CHOOSE_CHARACTER:
                    {
                        PressBToLeave?.SetActive( true );
                        PressXToCycleVariant?.SetActive( true );
                        m_AssignedPlayer.GetInputAction( IPlayer.Control.DPAD_PRESSED ).performed += OnDPADPressed;
                        m_AssignedPlayer.GetInputAction( IPlayer.Control.B_PRESSED ).performed += OnBPressed;
                        m_AssignedPlayer.GetInputAction( IPlayer.Control.X_PRESSED ).performed += OnXPressed;
                    }
                    break;
            }
        }
    }

    public void SetPlayer( IPlayer a_Player )
    {
        if ( a_Player == null )
        {
            return;
        }

        m_AssignedPlayer = a_Player;
        CurrentStage = a_Player.IsDeviceConnected ? Stage.CHOOSE_CHARACTER : Stage.WAIT_ON_RECONNECT;
        a_Player.onDeviceLost += device => CurrentStage = Stage.WAIT_ON_RECONNECT;
        a_Player.onDeviceRegained += device => CurrentStage = Stage.CHOOSE_CHARACTER;
        a_Player.transform.parent = DockTransform;
        a_Player.transform.localPosition = Vector3.zero;
        a_Player.transform.localRotation = Quaternion.identity;
    }

    private void IncrementVariant( bool a_Loop = false )
    {
        if ( m_AssignedPlayer.Character == null )
        {
            return;
        }

        int variantCount = CharacterManager.GetVariantCount( m_AssignedPlayer.Character.CharacterIndex );

        if ( m_AssignedPlayer.Character.VariantIndex == variantCount - 1 && !a_Loop )
        {
            return;
        }

        int characterVariant = m_AssignedPlayer.Character.VariantIndex;

        if ( ++characterVariant >= variantCount )
        {
            characterVariant = 0;
        }

        m_AssignedPlayer.Character.VariantIndex = characterVariant;
    }

    private void DecrementVariant( bool a_Loop = false )
    {
        if ( m_AssignedPlayer.Character == null )
        {
            return;
        }

        int variantCount = CharacterManager.GetVariantCount( m_AssignedPlayer.Character.CharacterIndex );

        if ( m_AssignedPlayer.Character.VariantIndex == 0 && !a_Loop )
        {
            return;
        }

        int characterVariant = m_AssignedPlayer.Character.VariantIndex;

        if ( --characterVariant < 0 )
        {
            characterVariant = variantCount - 1;
        }

        m_AssignedPlayer.Character.VariantIndex = characterVariant;
    }

    private void OnDPADPressed( InputAction.CallbackContext a_CallbackContext )
    {
        Vector2 value = a_CallbackContext.ReadValue< Vector2 >();

    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {

    }

    private void OnXPressed( InputAction.CallbackContext _ )
    {
        IncrementVariant( true );
    }

    private IPlayer m_AssignedPlayer;
    private Stage m_CurrentStage;

    public enum Stage
    {
        NONE,             
        WAIT_ON_DEVICE,   // No device connected.
        WAIT_ON_RECONNECT,// Device disconnected on player.
        WAIT_ON_JOIN,     // Device not joined.
        CHOOSE_CHARACTER  // Selecting Character.
    }
}