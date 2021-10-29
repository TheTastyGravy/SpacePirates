using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : Singleton<InteractionManager>
{
    internal List<Interactable> interactables = new List<Interactable>();
    internal List<Interactor> interactors = new List<Interactor>();

    internal List<Interactable> interactablesInRange = new List<Interactable>();
    private List<Interactable> inRangeLastFrame = new List<Interactable>();



    void LateUpdate()
    {
        if (interactors.Count == 0)
        {
            Debug.LogWarning("Interactions have broken");
        }

        foreach (var obj in interactors)
		{
            obj.FindClosestUsableInteractable(Player.Control.A_PRESSED, true);
        }

        foreach (var obj in interactablesInRange)
		{
            // In range this frame
            if (!inRangeLastFrame.Contains(obj))
			{
                obj.interactionPrompt.Selected();
			}
		}

        foreach (var obj in inRangeLastFrame)
		{
            // Out of range this frame
            if (!interactablesInRange.Contains(obj))
			{
                obj.interactionPrompt.Unselected();
			}
		}

        inRangeLastFrame.Clear();
        inRangeLastFrame.AddRange(interactablesInRange);
        interactablesInRange.Clear();
    }
}
