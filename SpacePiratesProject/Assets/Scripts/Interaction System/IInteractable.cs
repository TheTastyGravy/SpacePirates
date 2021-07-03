using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInteractable : MonoBehaviour
{
    /// <summary>
    /// Called when the interactable is being interacted with
    /// </summary>
    /// <param name="user">The interactor activating this</param>
    public abstract void Activate(Interactor user);



    private Material mat;
    private Color startColor;
    private Color mult = new Color(0.95f, 1, 0.5f);

    protected virtual void Start()
	{
        mat = GetComponent<Renderer>().material;
        startColor = mat.color;
	}

    public virtual void HightlightObject()
	{
        mat.color = (startColor + mult) * 0.5f;
	}
    public virtual void UnhighlightObject()
	{
        mat.color = startColor;
    }
}
