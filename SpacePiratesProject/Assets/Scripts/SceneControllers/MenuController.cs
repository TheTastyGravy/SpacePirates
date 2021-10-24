using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuController : Singleton< MenuController >
{
    public GameObject menuBackground;
    public GameObject optionsBackground;
    [Space]
    public Button MenuButtonPlay;
    public Button MenuButtonOptions;
    public Button MenuButtonExit;
    [Space]
    public Slider volumeSlider;



    private void Start()
    {
        // Edge case
        if (PlayerInputManager.instance.playerCount == 0)
        {
            GameManager.ChangeState(GameManager.GameState.START);
            return;
        }

        EventSystem.current.SetSelectedGameObject( MenuButtonPlay.gameObject );
        MenuButtonPlay.onClick.AddListener( OnButtonPlay );
        MenuButtonOptions.onClick.AddListener( OnButtonOptions );
        MenuButtonExit.onClick.AddListener( OnButtonExit );

        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100);

        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed += OnMenuCancel;
    }

    private void OnDestroy()
    {
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed -= OnMenuCancel;
        (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnOptionsCancel;

        MenuButtonPlay.onClick.RemoveAllListeners();
        MenuButtonOptions.onClick.RemoveAllListeners();
        MenuButtonExit.onClick.RemoveAllListeners();
        volumeSlider.onValueChanged.RemoveAllListeners();
    }

    private void OnButtonPlay()
    {
        GameManager.ChangeState(GameManager.GameState.SHIP);
    }

    private void OnButtonOptions()
    {
        ToggleOptionsMenu(true);
    }

    private void OnButtonExit()
    {
        Application.Quit();
    }

    private void OnMenuCancel( InputAction.CallbackContext _ )
    {
        if (Player.GetPlayerBySlot(Player.PlayerSlot.P1) != null)
            Destroy( Player.GetPlayerBySlot( Player.PlayerSlot.P1 ).gameObject );
        GameManager.ChangeState(GameManager.GameState.START);
    }

    private void OnOptionsCancel( InputAction.CallbackContext _ )
    {
        ToggleOptionsMenu(false);
    }

    private void ToggleOptionsMenu(bool showOptions)
	{
        // Update what the back button does
        if (showOptions)
		{
            (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnMenuCancel;
            (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed += OnOptionsCancel;
        }
		else
		{
            (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnOptionsCancel;
            (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed += OnMenuCancel;
        }
        
        //menu
        menuBackground.SetActive(!showOptions);
        MenuButtonPlay.interactable = !showOptions;
        MenuButtonOptions.interactable = !showOptions;
        MenuButtonExit.interactable = !showOptions;
        //options
        optionsBackground.SetActive(showOptions);

        //set selected button
        if (showOptions)
            volumeSlider.Select();
        else
            MenuButtonOptions.Select();

        if (!showOptions)
            PlayerPrefs.Save();
    }

    private void OnVolumeSliderChanged(float value)
	{
        PlayerPrefs.SetFloat("MusicVolume", value);
        MusicManager.Instance.SetVolume(value);
    }
}