using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : Singleton<InteractionManager>
{
    internal List<Interactable> interactables = new List<Interactable>();
    internal List<Interactor> interactors = new List<Interactor>();

    internal List<Interactable> interactablesInRange = new List<Interactable>();
    private List<Interactable> inRangeLastFrame = new List<Interactable>();

    public List<Interactable> Interactables => interactables;
    private bool isErroring = false;



    void LateUpdate()
    {
        try
        {
            if (isErroring)
            {
                Debug.Log("Interaction - start");
            }

            foreach (var obj in interactors)
            {
                obj.FindClosestUsableInteractable(Player.Control.A_PRESSED, true);
            }

            if (isErroring)
            {
                Debug.Log("Interaction - interactors pass");
            }

            foreach (var obj in interactablesInRange)
            {
                // In range this frame
                if (!inRangeLastFrame.Contains(obj))
                {
                    if (obj.interactionPrompt != null)
                        obj.interactionPrompt.Selected();
                }
            }

            if (isErroring)
            {
                Debug.Log("Interaction - in range pass");
            }

            foreach (var obj in inRangeLastFrame)
            {
                // Out of range this frame
                if (!interactablesInRange.Contains(obj))
                {
                    if (obj.interactionPrompt != null)
                        obj.interactionPrompt.Unselected();
                }
            }

            if (isErroring)
            {
                Debug.Log("Interaction - last frame pass");
            }

            inRangeLastFrame.Clear();
            inRangeLastFrame.AddRange(interactablesInRange);
            interactablesInRange.Clear();

            if (isErroring)
            {
                Debug.Log("Interaction - finish");
            }
        }
        catch (Exception e)
        {
            isErroring = true;
            Debug.LogError(e);
            Debug.Log("Interactor count: " + interactors.Count + "\nInteractable count: " + interactables.Count);
            Debug.Log("This frame count: " + interactablesInRange.Count + "\nLast frame count: " + inRangeLastFrame.Count);
        }
    }
}
