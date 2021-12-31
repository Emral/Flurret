using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class ExtendedRuleTile : CustomTypeRuleTile
{
    public Sprite sprite;

    // Start is called before the first frame update
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
        TileData data = new TileData();
        GetTileData(position, tilemap, ref data);
        sprite = data.sprite;
    }
}
