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
    public GameObject creditsBackground;
    [Space]
    public Button MenuButtonPlay;
    public Button MenuButtonOptions;
    public Button MenuButtonCredits;
    public Button MenuButtonExit;
    [Space]
    public Slider volumeSlider;

    private int currentMenu = 0;



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
        MenuButtonCredits.onClick.AddListener( OnButtonCredits );
        MenuButtonExit.onClick.AddListener( OnButtonExit );

        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100);

        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed += OnMenuCancel;
    }

    private void OnDestroy()
    {
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).cancel.action.performed -= OnMenuCancel;
        (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnOptionsCancel;
        (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnCreditsCancel;

        MenuButtonPlay.onClick.RemoveAllListeners();
        MenuButtonOptions.onClick.RemoveAllListeners();
        MenuButtonCredits.onClick.RemoveAllListeners();
        MenuButtonExit.onClick.RemoveAllListeners();
        volumeSlider.onValueChanged.RemoveAllListeners();
    }

    private void OnButtonPlay()
    {
        GameManager.ChangeState(GameManager.GameState.SHIP);
    }

    private void OnButtonOptions()
    {
        SetMenu(1);
    }

    private void OnButtonCredits()
    {
        SetMenu(2);
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
        SetMenu(0);
    }

    private void OnCreditsCancel(InputAction.CallbackContext _)
    {
        SetMenu(0);
    }

    private void SetMenu(int menu)
    {
        // Reset menu state
        (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnMenuCancel;
        (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnOptionsCancel;
        (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed -= OnCreditsCancel;

        //menu
        menuBackground.SetActive(menu == 0);
        MenuButtonPlay.interactable = menu == 0;
        MenuButtonOptions.interactable = menu == 0;
        MenuButtonCredits.interactable = menu == 0;
        MenuButtonExit.interactable = menu == 0;
        //options
        optionsBackground.SetActive(menu == 1);
        //credits
        creditsBackground.SetActive(menu == 2);

        // Set the selected object and return button
        switch (menu)
        {
            case 0:
                (currentMenu == 1 ? MenuButtonOptions : MenuButtonCredits).Select();
                (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed += OnMenuCancel;
                break;
            case 1:
                volumeSlider.Select();
                (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed += OnOptionsCancel;
                break;
            case 2:
                (EventSystem.current.currentInputModule as InputSystemUIInputModule).cancel.action.performed += OnCreditsCancel;
                break;
        }

        // Save prefs incase we were on the options menu
        PlayerPrefs.Save();
        currentMenu = menu;
    }

    private void OnVolumeSliderChanged(float value)
	{
        PlayerPrefs.SetFloat("MusicVolume", value);
        MusicManager.Instance.SetVolume(value);
    }
}