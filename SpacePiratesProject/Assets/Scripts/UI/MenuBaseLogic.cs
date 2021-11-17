using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBaseLogic : Singleton<MenuBaseLogic>
{
    public Image sidePanels;

    public void SetSidePanelsVisible(bool value)
    {
        sidePanels.enabled = value;
    }
}
