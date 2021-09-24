using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HUDController : Singleton< HUDController >
{
    public HUDOptionsMenu OptionsMenu;



    private void Start()
    {
        foreach ( PlayerInput playerInput in PlayerInput.all )
        {
            Player player = playerInput as Player;
            player.AddInputListener( Player.Control.START_PRESSED, callback => OnStartPressed( player ) );
        }

        // Delay by a frame
        Invoke(nameof(GetCamera), 0);
    }

    private void GetCamera()
	{
        // Setup canvas to use camera space so post processing effect can be applied
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
        canvas.planeDistance = 1;
    }

    private void OnStartPressed( Player a_Player )
    {
        if (OptionsMenu != null && OptionsMenu.gameObject.activeSelf)
        {
            return;
        }

        OptionsMenu.ShowOptions( a_Player );
    }

	void OnDestroy()
	{
        foreach (PlayerInput playerInput in PlayerInput.all)
        {
            Player player = playerInput as Player;
            player.RemoveInputListener(Player.Control.START_PRESSED, callback => OnStartPressed(player));
        }
    }
}
