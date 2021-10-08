using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HUDController : Singleton< HUDController >
{
    public HUDOptionsMenu OptionsMenu;

    private List<System.Action<InputAction.CallbackContext>> actions;



    private void Start()
    {
        actions = new List<System.Action<InputAction.CallbackContext>>();
        foreach ( PlayerInput playerInput in PlayerInput.all )
        {
            Player player = playerInput as Player;
            System.Action<InputAction.CallbackContext> action = callback => OnStartPressed(player);
            player.AddInputListener(Player.Control.START_PRESSED, action);
            actions.Add(action);
        }

        // Delay by a frame
        Invoke(nameof(GetCamera), 0);
    }

    private void GetCamera()
	{
        // Setup canvas to use camera space so post processing effect can be applied
        Canvas canvas = GetComponent<Canvas>();

        // Get the UI camera from the camera with multiple children. This is nessesary becasue of scene loading stuff
        Camera cam = null;
        if (Camera.main.transform.childCount > 0)
		{
            cam = Camera.main.transform.GetChild(0).GetComponent<Camera>();
        }
		else
		{
            foreach (var obj in Camera.allCameras)
			{
                if (obj.transform.childCount > 0)
				{
                    cam = obj.transform.GetChild(0).GetComponent<Camera>();
                    break;
                }
            }
		}
        canvas.worldCamera = cam;
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
        int count = Mathf.Max(PlayerInput.all.Count, actions.Count);
        for (int i = 0; i < count; i++)
		{
            Player player = PlayerInput.all[i] as Player;
            player.RemoveInputListener(Player.Control.START_PRESSED, actions[i]);
        }
        actions.Clear();
    }
}
