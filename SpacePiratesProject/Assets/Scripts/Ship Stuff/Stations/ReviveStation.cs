using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveStation : Interactable
{
	[ Tooltip( "How many times does a player need to interact to revive" ) ]
	[ Range( 0.0f, 3.0f ) ] public int NumberOfInteractions = 3;

	void Start()
	{
		m_Character = GetComponent< Character >();
	}

	void OnEnable()
	{
		m_InteractionCount = 0;
	}

	protected override void OnActivate( Interactor a_User )
	{
		++m_InteractionCount;

		if ( m_InteractionCount >= NumberOfInteractions )
		{
			m_Character.Revive();
		}
	}

	private int m_InteractionCount;
	private Character m_Character;
}
