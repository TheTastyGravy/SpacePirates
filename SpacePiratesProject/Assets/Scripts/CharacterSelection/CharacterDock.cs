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
    public GameObject ReadyIndicator;

    public Phase ConnectPhase
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
                case Phase.WAIT_ON_DEVICE:
                    {
                        ConnectController?.SetActive( false );
                    }
                    break;
                case Phase.WAIT_ON_RECONNECT:
                    {
                        ReconnectController?.SetActive( false );
                    }
                    break;
                case Phase.WAIT_ON_JOIN:
                    {
                        PressStartToJoin?.SetActive( false );
                    }
                    break;
                case Phase.CHOOSE_CHARACTER:
                    {
                        PressXToCycleVariant?.SetActive( false );
                        m_AssignedPlayer.RemoveInputListener( Player.Control.DPAD_PRESSED, OnDPADPressed );
                        m_AssignedPlayer.RemoveInputListener( Player.Control.X_PRESSED, OnXPressed );
                        m_AssignedPlayer.RemoveInputListener(Player.Control.A_PRESSED, OnReady);

                        if ( m_AssignedPlayer.Slot != Player.PlayerSlot.P1 )
                        {
                            m_AssignedPlayer.RemoveInputListener( Player.Control.B_PRESSED, OnBPressed );
                            PressBToLeave?.SetActive( false );
                        }
                    }
                    break;
                case Phase.PLAYER_READY:
                    {
                        if (ReadyIndicator != null)
                            ReadyIndicator.SetActive(false);
                        m_AssignedPlayer.RemoveInputListener(Player.Control.A_PRESSED, OnUnready);
                        m_AssignedPlayer.RemoveInputListener(Player.Control.B_PRESSED, OnUnready);
                    }
                    break;
            }

            m_CurrentStage = value;

            // Set new events.
            switch ( m_CurrentStage )
            {
                case Phase.WAIT_ON_DEVICE:
                    {
                        ConnectController?.SetActive( true );
                    }
                    break;
                case Phase.WAIT_ON_RECONNECT:
                    {
                        ReconnectController?.SetActive( true );
                    }
                    break;
                case Phase.WAIT_ON_JOIN:
                    {
                        PressStartToJoin?.SetActive( true );
                    }
                    break;
                case Phase.CHOOSE_CHARACTER:
                    {
                        PressXToCycleVariant?.SetActive( true );
                        m_AssignedPlayer.AddInputListener( Player.Control.DPAD_PRESSED, OnDPADPressed );
                        m_AssignedPlayer.AddInputListener( Player.Control.X_PRESSED, OnXPressed );
                        m_AssignedPlayer.AddInputListener(Player.Control.A_PRESSED, OnReady);

                        if ( m_AssignedPlayer.Slot != Player.PlayerSlot.P1 )
                        {                            
                            m_AssignedPlayer.AddInputListener( Player.Control.B_PRESSED, OnBPressed );
                            PressBToLeave?.SetActive( true );
                        }
                    }
                    break;
                case Phase.PLAYER_READY:
					{
                        if (ReadyIndicator != null)
                            ReadyIndicator.SetActive(true);
                        m_AssignedPlayer.AddInputListener(Player.Control.A_PRESSED, OnUnready);
                        m_AssignedPlayer.AddInputListener(Player.Control.B_PRESSED, OnUnready);
                    }
                    break;
            }
        }
    }
    public Player AssignedPlayer
    {
        get
        {
            return m_AssignedPlayer;
        }
    }

    private void OnDestroy()
    {
        if ( ConnectPhase == Phase.CHOOSE_CHARACTER )
        {
            m_AssignedPlayer.RemoveInputListener( Player.Control.DPAD_PRESSED, OnDPADPressed );
            m_AssignedPlayer.RemoveInputListener( Player.Control.X_PRESSED, OnXPressed );
            m_AssignedPlayer.RemoveInputListener(Player.Control.A_PRESSED, OnReady);

            if ( m_AssignedPlayer.Slot != Player.PlayerSlot.P1 )
            {
                m_AssignedPlayer.RemoveInputListener( Player.Control.B_PRESSED, OnBPressed );
            }
        }
        else if (ConnectPhase == Phase.PLAYER_READY)
		{
            m_AssignedPlayer.RemoveInputListener(Player.Control.A_PRESSED, OnUnready);
            m_AssignedPlayer.RemoveInputListener(Player.Control.B_PRESSED, OnUnready);
        }
    }

    public void SetPlayer( Player a_Player )
    {
        if ( a_Player == null )
        {
            return;
        }

        // Fix scale before making it a child to prevent weirdness
        a_Player.transform.localScale = Vector3.one;

        m_AssignedPlayer = a_Player;
        ConnectPhase = a_Player.IsDeviceConnected ? Phase.CHOOSE_CHARACTER : Phase.WAIT_ON_RECONNECT;
        a_Player.onDeviceLost += device => ConnectPhase = Phase.WAIT_ON_RECONNECT;
        a_Player.onDeviceRegained += device => ConnectPhase = Phase.CHOOSE_CHARACTER;
        a_Player.transform.parent = DockTransform;
        a_Player.transform.localPosition = Vector3.zero;
        a_Player.transform.localRotation = Quaternion.identity;
        a_Player.Character.transform.localPosition = Vector3.zero;
        a_Player.Character.transform.localRotation = Quaternion.identity;
        (a_Player.Character as Character).IsKinematic = true;
        CharacterSelector.InstantiateSelector( m_AssignedPlayer.Slot, a_Player.Character != null ? a_Player.Character.CharacterIndex : 0 );
    }

    private void IncrementVariant( bool a_Loop = false )
    {
        if ( m_AssignedPlayer.Character == null )
        {
            return;
        }
        
        int variantCount = m_AssignedPlayer.Character.GetVariantCount();

        if ( m_AssignedPlayer.Character.VariantIndex == variantCount - 1 && !a_Loop )
        {
            return;
        }

        int characterVariant = m_AssignedPlayer.Character.VariantIndex;

        if ( ++characterVariant >= variantCount )
        {
            characterVariant = 0;
        }

        m_AssignedPlayer.Character.SetVariant(characterVariant);
    }

    private void DecrementVariant( bool a_Loop = false )
    {
        if ( m_AssignedPlayer.Character == null )
        {
            return;
        }

        int variantCount = m_AssignedPlayer.Character.GetVariantCount();

        if ( m_AssignedPlayer.Character.VariantIndex == 0 && !a_Loop )
        {
            return;
        }

        int characterVariant = m_AssignedPlayer.Character.VariantIndex;

        if ( --characterVariant < 0 )
        {
            characterVariant = variantCount - 1;
        }

        m_AssignedPlayer.Character.SetVariant(characterVariant);
    }

    private void OnDPADPressed( InputAction.CallbackContext a_CallbackContext )
    {
        Vector2 value = a_CallbackContext.ReadValue< Vector2 >();
        bool moveSuccess = false;

        if ( value.x < 0 )
        {
            moveSuccess = CharacterSelector.ShiftSelector( m_AssignedPlayer.Slot, CharacterSelector.Direction.LEFT, true );
        }
        else if ( value.x > 0 )
        {
            moveSuccess = CharacterSelector.ShiftSelector( m_AssignedPlayer.Slot, CharacterSelector.Direction.RIGHT, true );
        }
        else if ( value.y < 0 )
        {
            moveSuccess = CharacterSelector.ShiftSelector( m_AssignedPlayer.Slot, CharacterSelector.Direction.DOWN, true );
        }
        else if ( value.y > 0 )
        {
            moveSuccess = CharacterSelector.ShiftSelector( m_AssignedPlayer.Slot, CharacterSelector.Direction.UP, true );
        }

        if ( moveSuccess )
        {
            Vector2Int selectorPosition = CharacterSelector.GetSelectorGridPosition( m_AssignedPlayer.Slot );
            int index = CharacterSelector.GridSize.x * selectorPosition.y + selectorPosition.x;
            m_AssignedPlayer.ChangeCharacter( index, 0 );
            ( m_AssignedPlayer.Character as Character).IsKinematic = true;
        }
    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {
        // Disconnect player
        CharacterSelector.DestroySelector( m_AssignedPlayer.Slot );
        Destroy( m_AssignedPlayer.gameObject );
        ConnectPhase = Phase.WAIT_ON_JOIN;
        // Check if other players are ready
        CharacterSelector.Instance.CheckReadyState();
    }

    private void OnXPressed( InputAction.CallbackContext _ )
    {
        IncrementVariant( true );
    }

    private void OnReady(InputAction.CallbackContext _)
	{
        ConnectPhase = Phase.PLAYER_READY;
        CharacterSelector.Instance.CheckReadyState();
    }

    private void OnUnready(InputAction.CallbackContext _)
	{
        ConnectPhase = Phase.CHOOSE_CHARACTER;
    }

    private Player m_AssignedPlayer;
    private Phase m_CurrentStage;

    public enum Phase
    {
        NONE,             // Initial phase to switch from.
        WAIT_ON_DEVICE,   // No device connected.
        WAIT_ON_RECONNECT,// Device disconnected on player.
        WAIT_ON_JOIN,     // Device not joined.
        CHOOSE_CHARACTER, // Selecting Character.
        PLAYER_READY      // Player is ready to continue
    }
}