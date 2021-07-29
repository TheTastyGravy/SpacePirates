using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ RequireComponent( typeof( MeshRenderer ) ) ]
public class ICharacter : MonoBehaviour
{
    public string CharacterName;
    public int CharacterIndex
    {
        get
        {
            return m_CharacterIndex;
        }
    }
    public int VariantIndex
    {
        get
        {
            return m_VariantIndex;
        }
        set
        {
            if ( m_VariantIndex == value )
            {
                return;
            }

            value = Mathf.Clamp( value, 0, CharacterManager.GetVariantCount( m_CharacterIndex ) );
            m_Renderer.material = CharacterManager.GetVariantMaterial( m_CharacterIndex, value );
            m_VariantIndex = value;
        }
    }
    public Player Player
    {
        get
        {
            return m_Player;
        }
    }

    private void Start()
    {
        m_Renderer = GetComponent< MeshRenderer >();
    }

    private string m_CharacterName;
    private int m_CharacterIndex;
    private int m_VariantIndex;
    private MeshRenderer m_Renderer;
    private Player m_Player;
}