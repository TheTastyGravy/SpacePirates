using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterManager : Singleton< CharacterManager >
{
    public CharacterTemplate[] CharacterTemplates;

    public static GameObject CreateCharacter( Character a_Character, bool a_PlayerEnabled = false )
    {
        if ( a_Character.Index < 0 || a_Character.Index >= Instance.CharacterTemplates.Length )
        {
            return null;
        }

        ref CharacterTemplate chosenTemplate = ref Instance.CharacterTemplates[ a_Character.Index ];
        
        if ( a_Character.Variant < 0 || a_Character.Variant >= chosenTemplate.CharacterMaterials.Length )
        {
            return null;
        }

        GameObject newCharacter = Instantiate( chosenTemplate.CharacterPrefab );
        newCharacter.GetComponent< Player >().enabled = a_PlayerEnabled;
        return newCharacter;
    }
}

[ Serializable ]
public struct CharacterTemplate
{
    public GameObject CharacterPrefab;
    public Material[] CharacterMaterials;
}