using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterManager : Singleton< CharacterManager >
{
    [ SerializeField ] private CharacterTemplate[] CharacterTemplates;

    public static GameObject CreateCharacter( Character a_Character, Transform a_Parent )
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

        GameObject newCharacter = Instantiate( chosenTemplate.CharacterPrefab, a_Parent );
        newCharacter.GetComponent< MeshRenderer >().material = chosenTemplate.CharacterMaterials[ a_Character.Variant ];
        return newCharacter;
    }

    public static Material GetCharacterMaterial( Character a_Character )
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

        return Instance.CharacterTemplates[ a_Character.Index ].CharacterMaterials[ a_Character.Variant ];
    }
}

[ Serializable ]
public struct CharacterTemplate
{
    public GameObject CharacterPrefab;
    public Material[] CharacterMaterials;
}

public struct Character
{
    public int Index
    {
        get
        {
            return m_Index;
        }
    }
    public int Variant
    {
        get
        {
            return m_Variant;
        }
    }

    public Character( int a_Index, int a_Variant )
    {
        m_Index = a_Index;
        m_Variant = a_Variant;
    }

    private int m_Index;
    private int m_Variant;
}