using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HUDController : Singleton< HUDController >
{
    public HUDOptionsMenu OptionsMenu;
    public Image[] images;
    public Image bgImage;

    private List<System.Action<InputAction.CallbackContext>> actions;



    private void Start()
    {
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
        bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, 0);

        actions = new List<System.Action<InputAction.CallbackContext>>();
        foreach ( PlayerInput playerInput in PlayerInput.all )
        {
            Player player = playerInput as Player;
            System.Action<InputAction.CallbackContext> action = callback => OnStartPressed(player);
            player.AddInputListener(Player.Control.START_PRESSED, action);
            actions.Add(action);
        }

        // Delay by a frame
        Invoke(nameof(GetCamera), 0.1f);
    }

    public void SetDisplayHUD(bool value)
    {
        StartCoroutine(Fade(value));
    }

    private IEnumerator Fade(bool fadeIn)
    {
        float time = 2;
        float t = 0;
        while (t < time)
        {
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(fadeIn ? 0 : 1, fadeIn ? 1 : 0, t / time));
            }
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, Mathf.Lerp(fadeIn ? 0 : 0.5f, fadeIn ? 0.5f : 0, t / time));
            
            t += Time.deltaTime;
            yield return null;
        }
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
        for (int i = 0; i < PlayerInput.all.Count; i++)
		{
            Player player = PlayerInput.all[i] as Player;
            player.RemoveInputListener(Player.Control.START_PRESSED, actions[i]);
        }
        actions.Clear();
    }
}
