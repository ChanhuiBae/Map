using UnityEngine;
using UnityEngine.Tilemaps;

public class MapProps : MonoBehaviour
{
    private Tilemap runeMap;
    private Tilemap propsMap;
    private Tilemap shadowMap;

    private TileBase[,] rune;
    private TileBase[,] props;
    private TileBase[,] shadow;

    public void DrawClosedGate(Vector2Int point, bool h)
    {
        if(h) // up
            propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 9), props[5, 1]);
        else // down
            propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 1), props[5, 1]);
    }
    public void DrawOpenedGate(Vector2Int point, bool h)
    {
        if (h) // up
            propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 9), props[5, 9]);
        else // down
            propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 1), props[5, 9]);
    }

    public void DrawAltarRune(Vector2Int point)
    {
        runeMap.SetTile(new Vector3Int(point.x + 4, point.y + 6), rune[4, 6]);
        runeMap.SetTile(new Vector3Int(point.x + 5, point.y + 7), rune[5, 7]);
        runeMap.SetTile(new Vector3Int(point.x + 6, point.y + 6), rune[6, 6]);
        runeMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), rune[5, 5]);
    }

    public void DrawAltar(Vector2Int point)
    {
        propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), props[5, 5]);
        shadowMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), shadow[5, 5]);
    }

    public void DrawPillar(Vector2Int point)
    {
        runeMap.SetTile(new Vector3Int(point.x + 3, point.y + 4), rune[3, 4]);
        propsMap.SetTile(new Vector3Int(point.x + 3, point.y + 4), props[3, 4]);
        shadowMap.SetTile(new Vector3Int(point.x + 3, point.y + 4), shadow[3, 4]);

        runeMap.SetTile(new Vector3Int(point.x + 3, point.y + 7), rune[3, 4]);
        propsMap.SetTile(new Vector3Int(point.x + 3, point.y + 7), props[3, 4]);
        shadowMap.SetTile(new Vector3Int(point.x + 3, point.y + 7), shadow[3, 4]);

        runeMap.SetTile(new Vector3Int(point.x + 8, point.y + 7), rune[3, 4]);
        propsMap.SetTile(new Vector3Int(point.x + 8, point.y + 7), props[3, 4]);
        shadowMap.SetTile(new Vector3Int(point.x + 8, point.y + 7), shadow[3, 4]);

        runeMap.SetTile(new Vector3Int(point.x + 8, point.y + 4), rune[3, 4]);
        propsMap.SetTile(new Vector3Int(point.x + 8, point.y + 4), props[3, 4]);
        shadowMap.SetTile(new Vector3Int(point.x + 8, point.y + 4), shadow[3, 4]);
    }


    private void Awake()
    {
        rune = new TileBase[12, 12];
        props = new TileBase[12, 12];
        shadow = new TileBase[12, 12];

        if (transform.childCount == 3)
        {
            if (transform.GetChild(0).TryGetComponent<Tilemap>(out runeMap)
                && transform.GetChild(1).TryGetComponent<Tilemap>(out propsMap)
                && transform.GetChild(2).TryGetComponent<Tilemap>(out shadowMap))
            {
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        rune[i, j] = runeMap.GetTile(new Vector3Int(i, j));
                        props[i, j] = propsMap.GetTile(new Vector3Int(i, j));
                        shadow[i, j] = shadowMap.GetTile(new Vector3Int(i, j));
                    }
                }
            }
        }

        runeMap.ClearAllTiles();
        propsMap.ClearAllTiles();
        shadowMap.ClearAllTiles();
    }
}
