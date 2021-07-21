using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInteractable : MonoBehaviour
{
    /// <summary>
    /// Called when an interactor presses the interaction key
    /// </summary>
    /// <param name="user">The interactor activating this</param>
    public virtual void OnActivateDown(Interactor user) { }
    /// <summary>
    /// Called when an interactor releases the interaction key
    /// </summary>
    /// <param name="user">The interactor activating this</param>
    public virtual void OnActivateUp(Interactor user) { }



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
