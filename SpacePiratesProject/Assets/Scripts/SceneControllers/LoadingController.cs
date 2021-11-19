using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class LoadingController : MonoBehaviour
{
    public Image loadingBar;
    public TextMeshProUGUI continueText;
    private bool canContinue = false;



    void Start()
    {
        loadingBar.fillAmount = 0;
        continueText.enabled = false;
    }

    void Update()
    {
        // Progress caps at 90%, since allowSceneActivation is used
        loadingBar.fillAmount = GameManager.Instance.gameLoadProgress * 1.11111111111f;
        if (GameManager.Instance.gameLoadProgress >= 0.9f && !canContinue)
        {
            canContinue = true;
            Player.GetPlayerBySlot(Player.PlayerSlot.P1).AddInputListener(Player.Control.A_PRESSED, OnAPressed);
            InvokeRepeating(nameof(ToggleText), 0, 0.5f);
        }
    }

    void OnDestroy()
    {
        Player.GetPlayerBySlot(Player.PlayerSlot.P1).RemoveInputListener(Player.Control.A_PRESSED, OnAPressed);
    }

    private void OnAPressed(InputAction.CallbackContext context)
    {
        GameManager.Instance.continueFromLoadingScene = true;
    }

    private void ToggleText()
    {
        continueText.enabled = !continueText.enabled;
    }
}
