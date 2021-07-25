using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDock : MonoBehaviour
{
    public Transform CharacterAnchor;

    public void DisplayCharacter( ICharacter a_Character )
    {
        //CharacterManager.CreateCharacter( a_Character );
    }

    public void IncrementVariant()
    {

    }

    public void DecrementVariant()
    {

    }

    public ICharacter m_CurrentlySelected;
}