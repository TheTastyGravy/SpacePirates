using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDock : MonoBehaviour
{
    public RectTransform DockTransform;
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
                        PressXToCycleVariant?.SetActive( false );
                        m_AssignedPlayer.RemoveInputListener( IPlayer.Control.DPAD_PRESSED, OnDPADPressed );
                        m_AssignedPlayer.RemoveInputListener( IPlayer.Control.X_PRESSED, OnXPressed );

                        if ( m_AssignedPlayer.Slot != IPlayer.PlayerSlot.P1 )
                        {
                            m_AssignedPlayer.RemoveInputListener( IPlayer.Control.B_PRESSED, OnBPressed );
                            PressBToLeave?.SetActive( false );
                        }
                        
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
                        PressXToCycleVariant?.SetActive( true );
                        m_AssignedPlayer.AddInputListener( IPlayer.Control.DPAD_PRESSED, OnDPADPressed );
                        m_AssignedPlayer.AddInputListener( IPlayer.Control.X_PRESSED, OnXPressed );

                        if ( m_AssignedPlayer.Slot != IPlayer.PlayerSlot.P1 )
                        {                            
                            m_AssignedPlayer.AddInputListener( IPlayer.Control.B_PRESSED, OnBPressed );
                            PressBToLeave?.SetActive( true );
                        }
                    }
                    break;
            }
        }
    }
    public IPlayer AssignedPlayer
    {
        get
        {
            return m_AssignedPlayer;
        }
    }

    private void OnDestroy()
    {
        if ( CurrentStage == Stage.CHOOSE_CHARACTER )
        {
            m_AssignedPlayer.RemoveInputListener( IPlayer.Control.DPAD_PRESSED, OnDPADPressed );
            m_AssignedPlayer.RemoveInputListener( IPlayer.Control.X_PRESSED, OnXPressed );

            if ( m_AssignedPlayer.Slot != IPlayer.PlayerSlot.P1 )
            {
                m_AssignedPlayer.RemoveInputListener( IPlayer.Control.B_PRESSED, OnBPressed );
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
        CharacterSelector.InstantiateSelector( m_AssignedPlayer.Slot, a_Player.Character.CharacterIndex );
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
        bool moveSuccess = false;

        if ( value.x < 0 )
        {
            moveSuccess = CharacterSelector.ShiftSelector( m_AssignedPlayer.Slot, CharacterSelector.Direction.LEFT );
        }
        else if ( value.x > 0 )
        {
            moveSuccess = CharacterSelector.ShiftSelector( m_AssignedPlayer.Slot, CharacterSelector.Direction.RIGHT );
        }
        else if ( value.y < 0 )
        {
            moveSuccess = CharacterSelector.ShiftSelector( m_AssignedPlayer.Slot, CharacterSelector.Direction.DOWN );
        }
        else if ( value.y > 0 )
        {
            moveSuccess = CharacterSelector.ShiftSelector( m_AssignedPlayer.Slot, CharacterSelector.Direction.UP );
        }

        if ( moveSuccess )
        {
            Vector2Int selectorPosition = CharacterSelector.GetSelectorGridPosition( m_AssignedPlayer.Slot );
            int index = CharacterSelector.GridSize.x * selectorPosition.y + selectorPosition.x;
            m_AssignedPlayer.ChangeCharacter( index, 0 );
        }
    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {
        CharacterSelector.DestroySelector( m_AssignedPlayer.Slot );
        Destroy( m_AssignedPlayer.gameObject );
        CurrentStage = Stage.WAIT_ON_JOIN;
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