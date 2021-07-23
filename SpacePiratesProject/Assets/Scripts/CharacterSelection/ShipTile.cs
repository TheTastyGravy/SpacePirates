using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ Serializable ]
public class ShipTile : Tile
{
    [ Range( 1, 4 ) ] public int MaxPlayers = 1;
}
