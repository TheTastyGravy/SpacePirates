using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuController : Singleton< MenuController >
{
    public GameObject OptionsBackground;
    public Button MenuButtonPlay;
    public Button MenuButtonOptions;
    public Button MenuButtonExit;
    public Button OptionsButtonSetting1;
    public Button OptionsButtonSetting2;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject( MenuButtonPlay.gameObject );
        MenuButtonPlay.onClick.AddListener( OnButtonPlay );
        MenuButtonOptions.onClick.AddListener( OnButtonOptions );
        MenuButtonExit.onClick.AddListener( OnButtonExit );

        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed += OnMenuCancel;
    }

    private void OnDestroy()
    {
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed -= OnMenuCancel;
    }

    private void OnButtonPlay()
    {
        GameManager.CurrentState = GameManager.GameState.SHIP;
    }

    private void OnButtonOptions()
    {
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed -= OnMenuCancel;
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed += OnOptionsCancel;
        OptionsBackground.SetActive( true );

        MenuButtonPlay.interactable = false;
        MenuButtonOptions.interactable = false;
        MenuButtonExit.interactable = false;
        OptionsButtonSetting1.Select();
    }

    private void OnButtonExit()
    {
        Application.Quit();
    }

    private void OnMenuCancel( InputAction.CallbackContext _ )
    {
        Destroy( Player.GetPlayerBySlot( Player.PlayerSlot.P1 ).gameObject );
        GameManager.CurrentState = GameManager.GameState.START;
    }

    private void OnOptionsCancel( InputAction.CallbackContext _ )
    {
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed -= OnOptionsCancel;
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed += OnMenuCancel;
        OptionsBackground.SetActive( false );

        MenuButtonPlay.interactable = true;
        MenuButtonOptions.interactable = true;
        MenuButtonExit.interactable = true;
        MenuButtonOptions.Select();
    }
}