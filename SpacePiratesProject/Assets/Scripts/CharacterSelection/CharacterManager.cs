using UnityEngine;
using System;

public class CharacterManager : Singleton< CharacterManager >
{
    public CharacterTemplate[] CharacterTemplates;

    private void OnValidate()
    {
        for ( int i = 0; i < CharacterTemplates.Length; ++i )
        {
            typeof( CharacterTemplate ).
                GetField( "m_Index", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).
                SetValue( CharacterTemplates[ i ], i );
        }
    }

    public static ICharacter CreateCharacter( IPlayer a_Owner, int a_CharacterIndex, int a_VariantIndex = 0 )
    {
        if ( a_CharacterIndex < 0 || a_CharacterIndex >= Instance.CharacterTemplates.Length || a_VariantIndex < 0 || a_Owner == null )
        {
            return null;
        }

        ref CharacterTemplate template = ref Instance.CharacterTemplates[ a_CharacterIndex ];

        if ( a_VariantIndex >= template.VariantCount )
        {
            return null;
        }

        ICharacter newCharacter = template.Instantiate( a_VariantIndex );
        newCharacter.transform.parent = a_Owner.transform;
        typeof( ICharacter ).GetField( "m_Player", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).SetValue( newCharacter, a_Owner );
        return newCharacter;
    }

    public static ICharacter CreateCharacter( IPlayer a_Owner, string a_CharacterName, int a_VariantIndex = 0 )
    {
        if ( a_VariantIndex < 0 || a_Owner == null )
        {
            return null;
        }

        int foundIndex = Array.FindIndex( Instance.CharacterTemplates, template => template.Character.name == a_CharacterName );

        if ( foundIndex == -1 )
        {
            return null;
        }

        ref CharacterTemplate template = ref Instance.CharacterTemplates[ foundIndex ];

        if ( a_VariantIndex >= template.VariantCount )
        {
            return null;
        }

        ICharacter newCharacter = template.Instantiate( a_VariantIndex );
        newCharacter.transform.parent = a_Owner.transform;
        typeof( ICharacter ).GetProperty( "m_Player" ).SetValue( newCharacter, a_Owner );
        return newCharacter;
    }

    public static Material GetVariantMaterial( int a_CharacterIndex, int a_VariantIndex )
    {
        if ( a_CharacterIndex < 0 || a_CharacterIndex >= Instance.CharacterTemplates.Length || a_VariantIndex < 0 )
        {
            return null;
        }

        ref CharacterTemplate template = ref Instance.CharacterTemplates[ a_CharacterIndex ];

        return Instance.CharacterTemplates[ a_CharacterIndex ].GetMaterial( a_VariantIndex );
    }

    public static int GetVariantCount( int a_CharacterIndex )
    {
        if ( a_CharacterIndex < 0 || a_CharacterIndex >= Instance.CharacterTemplates.Length )
        {
            return 0;
        }

        return Instance.CharacterTemplates[ a_CharacterIndex ].VariantCount;
    }
}

[ Serializable ]
public class CharacterTemplate
{
    public ICharacter Character
    {
        get
        {
            return m_Character;
        }
    }
    public int VariantCount
    {
        get
        {
            return m_Materials.Length;
        }
    }

    public ICharacter Instantiate( int a_VariantIndex = 0 )
    {
        a_VariantIndex = Mathf.Clamp( a_VariantIndex, 0, m_Materials.Length - 1 );
        ICharacter newCharacter = UnityEngine.Object.Instantiate( m_Character );
        typeof( ICharacter ).GetField( "m_CharacterName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).SetValue( newCharacter, m_Character.name );
        typeof( ICharacter ).GetField( "m_CharacterIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).SetValue( newCharacter, m_Index );
        typeof( ICharacter ).GetField( "m_VariantIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).SetValue( newCharacter, a_VariantIndex );
        newCharacter.GetComponent< MeshRenderer >().material = m_Materials[ a_VariantIndex ];
        return newCharacter;
    }

    public Material GetMaterial( int a_VariantIndex )
    {
        if ( a_VariantIndex < 0 || a_VariantIndex >= m_Materials.Length )
        {
            return null;
        }

        return m_Materials[ a_VariantIndex ];
    }

    private int m_Index;
    [ SerializeField ] private Material[] m_Materials;
    [ SerializeField ] private ICharacter m_Character;
}