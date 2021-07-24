using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : UIBehaviour
{
    public Vector2Int GridPosition
    {
        get
        {
            return m_GridPosition;
        }
    }
    public RectTransform RectTransform
    {
        get
        {
            return m_RectTransform;
        }
    }

    public void SetPosition( Vector2 a_Postion )
    {
        m_RectTransform.anchoredPosition = a_Postion;
    }

    public void SetPosition( Vector2Int a_NewGridPosition, Vector2 a_GridCellSize, Vector2 a_GridCellSpacing )
    {
        Vector2 newGridPosition = new Vector2( a_NewGridPosition.x, a_NewGridPosition.y );
        Vector2 newPosition = ( Vector2.one + newGridPosition ) * a_GridCellSpacing + newGridPosition * a_GridCellSize;
        newPosition.y  *= -1;
        m_RectTransform.anchoredPosition = newPosition;
        m_GridPosition = a_NewGridPosition;
    }

    [ SerializeField ] private RectTransform m_RectTransform;
    private Vector2Int m_GridPosition;
}
