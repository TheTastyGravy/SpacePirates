using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : Singleton<InteractionManager>
{
    internal List<Interactable> interactables = new List<Interactable>();
    internal List<Interactor> interactors = new List<Interactor>();



    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
