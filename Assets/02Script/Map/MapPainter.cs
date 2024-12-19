using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapPainter : MonoBehaviour
{
    private Tilemap darkMap;
    private Tilemap wall3Map;
    private Tilemap wall2Map;
    private Tilemap wall1Map;
    private Tilemap shadowMap;
    private Tilemap groundMap;

    private TileBase[,] dark;
    private TileBase[,] wall3;
    private TileBase[,] wall2;
    private TileBase[,] wall1;
    private TileBase[,] shadow;
    private TileBase[,] ground;

    private MapProps props;

    private MapData data;


    private void DrawRoom(int x, int y)
    {
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                if (i > 1 && i < 10)
                {
                    groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), ground[i, j]);
                }
                if (i > 1 && i < 10)
                {
                    shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), shadow[i, j]);
                }
                if (i > 1 && i < 10)
                {
                    wall1Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall1[i, j]);
                }
                if (i > 1 && i < 10)
                {
                    wall2Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall2[i, j]);
                }
                if (i == 1 || i == 10)
                {
                    wall3Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall3[i, j]);
                }
            }
        }
    }

    private void DrawLeftPath(int x, int y)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 3; j < 9; j++)
            {
                wall1Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall1[i, j]);
                wall2Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall2[i, j]);
                shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), shadow[i, j]);
                groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), ground[i, j]);
            }
        }
    }

    private void DrawRightPath(int x, int y)
    {
        for (int i = 10; i < 12; i++)
        {
            for (int j = 3; j < 9; j++)
            {
                wall1Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall1[i, j]);
                wall2Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall2[i, j]);
                shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), shadow[i, j]);
                groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), ground[i, j]);
            }
        }
    }

    private void DrawUpPath(int x, int y)
    {
        for (int i = 4; i < 8; i++)
        {
            wall3Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + 11), wall3[i, 11]);
            shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + 11), shadow[i, 11]);
            groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + 11), ground[i, 11]);
        }
    }

    private void DrawDownPath(int x, int y)
    {
        for (int i = 4; i < 8; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                wall3Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall3[i, j]);
                shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), shadow[i, j]);
                groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), ground[i, j]);
            }
        }
    }

    private void DrawUpNoGate(int x, int y)
    {
        wall1Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 9), wall1[4, 1]);
        wall1Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 9), wall1[4, 1]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 10), wall2[4, 2]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 10), wall2[4, 2]);
    }
    private void DrawDownNoGate(int x, int y)
    {
        wall1Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 1), wall1[4, 1]);
        wall1Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 1), wall1[4, 1]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 2), wall2[4, 2]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 2), wall2[4, 2]);
    }

    private void DeleteUpNoGate(int x, int y)
    {
        wall1Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 9), wall1[5, 9]);
        wall1Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 9), wall1[6, 9]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 10), wall2[5, 10]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 10), wall2[6, 10]);
    }
    private void DeleteDownNoGate(int x, int y)
    {
        wall1Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 1), wall1[5, 1]);
        wall1Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 1), wall1[6, 1]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 2), wall2[5, 2]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 2), wall2[6, 2]);
    }
    private void DrawPathes(int i, int j, int size, Vector2Int v)
    {
        if (i - 1 > -1 && data.GetFind(i - 1, j))
        {
            if(data.GetMap(i - 1, j) > 0)
            {
                DrawLeftPath(i, j);
            }
            else if(data.GetMap(i - 1, j) == 0 && data.GetGone(i - 1, j))
            {
                DrawLeftPath(i, j);
            }
        }

        if (i + 1 < size && data.GetFind(i + 1, j))
        {
            if(data.GetMap(i + 1, j) > 0)
            {
                DrawRightPath(i, j);
            }
            else if(data.GetMap(i + 1,j) == 0 && data.GetGone(i + 1, j))
            {
                DrawRightPath(i, j);
            }
        }

        if (j + 1 < size && data.GetFind(i, j + 1))
        {
            if (data.GetMap(i, j + 1) > 0)
            {
                DrawUpPath(i, j);
                if (data.GetGone(i, j))
                {
                    props.DrawOpenedGate(v, true);
                }
                else
                {
                    props.DrawClosedGate(v, true);
                }
            }
            else if (data.GetMap(i, j + 1) == 0 && data.GetGone(i,j+1))
            {
                DrawUpPath(i, j);
                if (data.GetGone(i, j))
                {
                    props.DrawOpenedGate(v, true);
                }
                else
                {
                    props.DrawClosedGate(v, true);
                }
            }
            else
            {
                DrawUpNoGate(i, j);
            }
        }
        else
        {
            DrawUpNoGate(i, j);
        }


        if (j - 1 > -1 && data.GetFind(i, j - 1))
        {
            if (data.GetMap(i, j - 1) > 0)
            {
                DrawDownPath(i, j);
                if (data.GetGone(i, j))
                {
                    props.DrawOpenedGate(v, false);
                }
                else
                {
                    props.DrawClosedGate(v, false);
                }
            }
            else if(data.GetMap(i,j - 1) == 0 && data.GetGone(i,j - 1))
            {
                DrawDownPath(i, j);
                if (data.GetGone(i, j))
                {
                    props.DrawOpenedGate(v, false);
                }
                else
                {
                    props.DrawClosedGate(v, false);
                }
            }
            else
            {
                DrawDownNoGate(i, j);
            }
        }
        else
        {
            DrawDownNoGate(i, j);
        }
    }

    private void DrawBlownUpPath(int i, int j, int size, Vector2Int v)
    {
        if (i - 1 > -1 && data.GetFind(i - 1, j) && data.GetMap(i - 1, j) == 0)
        {
            DrawLeftPath(i, j);
            DrawRightPath(i - 1, j);
        }
        if (i + 1 < size && data.GetFind(i + 1, j) && data.GetMap(i + 1, j) == 0)
        {
            DrawRightPath(i, j);
            DrawLeftPath(i + 1, j);
        }
        if (j + 1 < size && data.GetFind(i, j + 1))
        {
            if (data.GetMap(i, j + 1) == 0)
            {
                DrawUpPath(i, j);
                DrawDownPath(i, j + 1); 
                DeleteUpNoGate(i, j);
                if (data.GetGone(i, j + 1))
                {
                    props.DrawOpenedGate(new Vector2Int(i * 12, (j + 1) * 12), false);
                }
                else
                {
                    props.DrawClosedGate(new Vector2Int(i * 12, (j + 1) * 12), false);
                }
            }
        }
        if (j - 1 > -1 && data.GetFind(i, j - 1))
        {
            if (data.GetMap(i, j - 1) == 0)
            {
                DrawDownPath(i, j);
                DrawUpPath(i, j - 1);
                DeleteDownNoGate(i, j);
                if (data.GetGone(i, j - 1))
                {
                    props.DrawOpenedGate(new Vector2Int(i * 12, (j - 1) * 12), false);
                }
                else
                {
                    props.DrawClosedGate(new Vector2Int(i * 12, (j - 1) * 12), false);
                }
            }
        }
    }

    private void DrawDark(int x, int y)
    {
        for (int i = 2; i < 10; i++)
        {
            for (int j = 3; j < 11; j++)
            {
                darkMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), dark[i, j]);
            }
        }
    }

    private void DeleteDark(int x, int y)
    {
        for (int i = 2; i < 10; i++)
        {
            for (int j = 3; j < 11; j++)
            {
                darkMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), dark[0, 0]);
            }
        }
    }

    public void DrawMap()
    {
        if (data == null)
            data = GameManager.Inst.MapData;

        int size = data.MapSize;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (data.GetFind(i,j))
                {
                    Vector2Int v = new Vector2Int(i * 12, j * 12);
                    if (data.GetMap(i,j) > 0 && data.GetMap(i, j) < 11)
                    {
                        DrawRoom(i, j);

                        if (data.GetKnown(i, j))
                        {
                            if (data.GetMap(i, j) == (int)TileType.Exit)
                            {
                                props.DrawAltarRune(v);
                                props.DrawAltar(v);
                                props.DrawPillar(v);
                            }
                            else if (data.GetMap(i, j) == (int)TileType.Boss)
                            {
                                props.DrawAltar(v);
                            }
                        }

                        DrawPathes(i, j, size, v);

                        if (data.GetBlownUp(i, j))
                        {
                            DrawBlownUpPath(i,j,size, v);
                        }

                        if (!data.GetGone(i, j))
                        {
                            DrawDark(i, j);
                        }
                        else
                        {
                            DeleteDark(i, j);
                        }

                    }
                    else if (data.GetMap(i, j) == 0)
                    {
                        DrawRoom(i, j);

                        if (!data.GetGone(i, j))
                        {
                            props.DrawClosedGate(v, true);
                            props.DrawClosedGate(v, false);
                            DrawDark(i, j);
                        }
                        else
                        {
                            props.DrawOpenedGate(v, true);
                            props.DrawOpenedGate(v, false);
                            DeleteDark(i, j);
                        }
                    }
                }
            }
        }
    }

    private void ClearTilemaps()
    {
        darkMap.ClearAllTiles();
        wall3Map.ClearAllTiles();
        wall2Map.ClearAllTiles();
        wall1Map.ClearAllTiles();
        shadowMap.ClearAllTiles();
        groundMap.ClearAllTiles();
    }

    private void Awake()
    {
        if (!GameObject.Find("MapProps").TryGetComponent<MapProps>(out props))
        {
            Debug.Log("Map Creater - Awake - MapProps");
        }

        dark = new TileBase[12, 12];
        wall3 = new TileBase[12, 12];
        wall2 = new TileBase[12, 12];
        wall1 = new TileBase[12, 12];
        shadow = new TileBase[12, 12];
        ground = new TileBase[12, 12];

        if (transform.childCount == 6)
        {
            if (transform.GetChild(0).TryGetComponent<Tilemap>(out darkMap)
                && transform.GetChild(1).TryGetComponent<Tilemap>(out wall3Map)
                && transform.GetChild(2).TryGetComponent<Tilemap>(out wall2Map)
                && transform.GetChild(3).TryGetComponent<Tilemap>(out wall1Map)
                && transform.GetChild(4).TryGetComponent<Tilemap>(out shadowMap)
                && transform.GetChild(5).TryGetComponent<Tilemap>(out groundMap))
            {
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        dark[i, j] = darkMap.GetTile(new Vector3Int(i, j));
                        wall3[i, j] = wall3Map.GetTile(new Vector3Int(i, j));
                        wall2[i, j] = wall2Map.GetTile(new Vector3Int(i, j));
                        wall1[i, j] = wall1Map.GetTile(new Vector3Int(i, j));
                        shadow[i, j] = shadowMap.GetTile(new Vector3Int(i, j));
                        ground[i, j] = groundMap.GetTile(new Vector3Int(i, j));
                    }
                }
            }

            ClearTilemaps();
        }
    }

}
