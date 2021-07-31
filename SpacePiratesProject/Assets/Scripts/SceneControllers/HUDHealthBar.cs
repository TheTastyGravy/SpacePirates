using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDHealthBar : UIBehaviour
{
    public RectTransform Bar;

    public float MaxValue
    {
        get
        {
            return m_MaxValue;
        }
        set
        {
            value = Mathf.Max( 0, value );

            if ( value < m_MaxValue )
            {
                m_Value = Mathf.Min( m_MaxValue, m_Value );
            }

            m_MaxValue = value;
            m_MaxValueInverse = 1.0f / value;
            UpdateBar();
        }
    }
    public float Value
    {
        get
        {
            return m_Value;
        }
        set
        {
            m_Value = Mathf.Min( value, m_MaxValue );
            UpdateBar();
        }
    }
    public float Normalized
    {
        get
        {
            return m_Value * m_MaxValueInverse;
        }
        set
        {
            m_Value = value * m_MaxValue;
            UpdateBar();
        }
    }

    protected override void Awake()
    {
        m_InitialWidth = Bar.rect.width;
    }

    protected override void OnValidate()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        Bar.offsetMax = new Vector2( Bar.offsetMax.x, ( 1.0f - Normalized ) * m_InitialWidth );
    }

    [ SerializeField ] [ Range( 0.0f, 100.0f ) ] private float m_Value;
    [ SerializeField ] [ Range( 0.0f, 100.0f ) ] private float m_MaxValue;
    private float m_MaxValueInverse;
    private float m_InitialWidth;
}