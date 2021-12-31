using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TilemapMerger : MonoBehaviour
{
    public List<Tilemap> tilemaps;
    public Transform miscRoot;

    public TilemapSprite loadedSprite;
    public TilemapSprite emptySprite;

    public SpriteRenderer spritePrefab;
    public DisassembledTilemap disassembledTilemapPrefab;

    #if UNITY_EDITOR
    [Button]
    public void Save()
    {
        TilemapSprite prefab = loadedSprite;
        if (loadedSprite == null)
        {
            prefab = Instantiate(emptySprite);
        }

        prefab.tilemaps.Clear();
        foreach (Transform t in miscRoot)
        {
            t.SetParent(prefab.miscRoot);
        }

        foreach (Tilemap t in tilemaps)
        {
            t.CompressBounds();
            t.RefreshAllTiles();
            TileBase[] allTiles = t.GetTilesBlock(t.cellBounds);
            DisassembledTilemap tilemap = Instantiate(disassembledTilemapPrefab, prefab.transform);
            tilemap.sortingLayer = t.GetComponent<TilemapRenderer>().sortingLayerName;
            tilemap.sprites = new List<SpriteRenderer>();

            for (int x = t.cellBounds.x; x < t.cellBounds.x + t.cellBounds.size.x; x++)
            {
                for (int y = t.cellBounds.y; y < t.cellBounds.y + t.cellBounds.size.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y);
                    if (t.GetTile(pos) is TileBase tile && tile != null)
                    {
                        SpriteRenderer s = Instantiate(spritePrefab, tilemap.spriteRoot);
                        s.sortingLayerName = tilemap.sortingLayer;

                        s.sprite = t.GetSprite(pos);
                        s.transform.localPosition = pos;

                        tilemap.sprites.Add(s);
                    }
                }
            }

            //foreach (TileBase tile in allTiles)
            //{
            //    if (tile != null)
            //    {
            //        SpriteRenderer s = Instantiate(spritePrefab, tilemap.spriteRoot);
            //        s.sortingLayerName = tilemap.sortingLayer;

            //        if (tile is ExtendedRuleTile ert)
            //        {
            //            s.sprite = ert.sprite;
            //            s.transform.position = ert.floatPosition;
            //            s.color = Color.red;

            //        }
            //        else if (tile is Tile til)
            //        {
            //            s.sprite = til.sprite;
            //            s.transform.position = til.transform.GetPosition();
            //            s.color = Color.green;
            //        }
            //        else
            //        {
            //            s.color = Color.blue;
            //        }

            //        Debug.Log(tile.GetType());

            //        tilemap.sprites.Add(s);
            //    }
            //}

            if (t.GetComponent<TilemapCollider2D>() is TilemapCollider2D tmc && tmc != null)
            {
                CompositeCollider2D composite = prefab.gameObject.AddComponent<CompositeCollider2D>();
                Rigidbody2D body = prefab.GetComponent<Rigidbody2D>();
                body.gravityScale = 0;
                body.isKinematic = true;
                foreach(SpriteRenderer sr in tilemap.sprites)
                {
                    BoxCollider2D collider = sr.gameObject.AddComponent<BoxCollider2D>();
                    collider.usedByComposite = true;
                }
            }

            prefab.tilemaps.Add(tilemap);
        }

        string path = EditorUtility.SaveFilePanel("Save prefab", Application.dataPath, "blockset-XXXX-0000", "prefab");

        if (path.Length > 0)
        {
            prefab.name = path.Split('/').Last().Split('.').First();
            PrefabUtility.SaveAsPrefabAsset(prefab.gameObject, path);
        }

        DestroyImmediate(prefab.gameObject);
    }
    #endif
}
