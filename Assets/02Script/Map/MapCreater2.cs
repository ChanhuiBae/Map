using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapCreater2 : MonoBehaviour
{ 
    private static int minDistance(Vector2Int start, Vector2Int target, int[,] area)
    {
        int result = -1; // distance
        int failResult = -1;
        int obstacle = 0;
        List<List<MapTile>> tiles = new List<List<MapTile>>();
        List<MapTile> openList = new List<MapTile>();
        List<MapTile> closeList = new List<MapTile>();
        List<MapTile> tile = new List<MapTile>();
        MapTile startTile = null;
        MapTile targetTile = null;
        // Initial values
        for (int i = 0; i < area.GetLength(0); i++)
        {
            List<MapTile> t = new List<MapTile>();
            for (int j = 0; j < area.GetLength(1); j++)
            {
                MapTile temp = new MapTile();
                temp.X = i;
                temp.Y = j;
                t.Add(temp);
                if (i == target.x && j == target.y)
                {
                    targetTile = temp;
                }
            }
            tiles.Add(t);
        }

        startTile = tiles[start.x][start.y];
        openList.Add(startTile);
        if (targetTile == null)
        {
            // can not found target
            return failResult;
        }
        MapTile currentTile = null;
        do
        {
            if (openList.Count == 0)
            {
                break;
            }
            currentTile = openList.OrderBy(o => o.F).First();
            openList.Remove(currentTile);
            closeList.Add(currentTile);
            if (currentTile == targetTile)
            {
                break;
            }
            for (int i = 0; i < area.GetLength(0); i++)
            {
                for (int j = 0; j < area.GetLength(1); j++)
                {
                    //// 8 way
                    //bool near = (System.Math.Abs(currentPath.X - pathes[i][j].X) <= 1)
                    //         && (System.Math.Abs(currentPath.Y - pathes[i][j].Y) <= 1);
                    // 4 way
                    bool near = (System.Math.Abs(currentTile.X - tiles[i][j].X) <= 1)
                             && (System.Math.Abs(currentTile.Y - tiles[i][j].Y) <= 1)
                             && (currentTile.Y == tiles[i][j].Y || currentTile.X == tiles[i][j].X);
                    if (area[i, j] == obstacle
                     || closeList.Contains(tiles[i][j])
                     || (!near))
                    {
                        continue;
                    }
                    if (!openList.Contains(tiles[i][j]))
                    {
                        openList.Add(tiles[i][j]);
                        tiles[i][j].Execute(currentTile, targetTile);
                    }
                    else
                    {
                        if (MapTile.CalcGValue(currentTile, tiles[i][j]) < tiles[i][j].G)
                        {
                            tiles[i][j].Execute(currentTile, targetTile);
                        }
                    }
                }
            }
        } while (currentTile != null);
        if (currentTile != targetTile)
        {
            // can not found root
            return failResult;
        }
        do
        {
            tile.Add(currentTile);
            currentTile = currentTile.Parent;
        }
        while (currentTile != null);
        tile.Reverse();
        result = tile.Count - 1;
        return result;
    }

    private Tilemap roadMap;
    private Tilemap backgroundMap;

    private TileBase roadTile;
    private TileBase tile0;
    private TileBase tile1;
    private TileBase tile2;
    private TileBase tile3;
    private TileBase tile4;
    private TileBase coverTile;

    private TileBase box;
    private TileBase lattern;
    private TileBase cube;

    private static int maxTile = 75;
    private static int minTile = 55; 

    private static int maxSize = 11;

    [SerializeField]
    private int turn;
    private Direction playerDirection;
    private Vector2Int playerPosition;

    private int[,] map;
    private bool[,] findMap;
    private bool[,] goneMap;

    [SerializeField]
    private Vector2Int startPosition;
    [SerializeField]
    private int blockCount;
    [SerializeField]
    private int maxBlock;
    public Vector2 GetStartPosition()
    {
        return startPosition;
    }

    private void UpdateFindMap(Vector2Int point)
    {
        findMap[point.x, point.y] = true;

        if (point.x - 1 > -1)
        {
            findMap[point.x - 1, point.y] = true;
        }
        if (point.x + 1 < maxSize)
        {
            findMap[point.x + 1, point.y] = true;
        }
        if (point.y + 1 < maxSize)
        {
            findMap[point.x, point.y + 1] = true;
        }
        if (point.y - 1 > -1)
        {
            findMap[point.x, point.y - 1] = true;
        }
    }

    private bool UpdateMap(Vector2Int point)
    {
        if (map[point.x, point.y] == -1)
        {
            blockCount++;
        }

        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < maxSize;
        bool upIn = point.y + 1 < maxSize;
        bool downIn = point.y - 1 > -1;
        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;
        if (leftIn)
        {
            left = map[point.x - 1, point.y] > -1;
        }
        if (rightIn)
        {
            right = map[point.x + 1, point.y] > -1;
        }
        if (upIn)
        {
            up = map[point.x, point.y + 1] > -1;
        }
        if (downIn)
        {
            down = map[point.x, point.y - 1] > -1;
        }

        map[point.x, point.y] = 0;
        if (leftIn && left)
            map[point.x, point.y] += 1;
        if (rightIn && right)
            map[point.x, point.y] += 1;
        if (upIn && up)
            map[point.x, point.y] += 1;
        if (downIn && down)
            map[point.x, point.y] += 1;

        if (left)
        {
            map[point.x - 1, point.y] = 1;
            if (point.x - 2 > -1 && map[point.x - 2, point.y] > -1)
                map[point.x - 1, point.y] += 1;
            if (upIn && map[point.x - 1, point.y + 1] > -1)
                map[point.x - 1, point.y] += 1;
            if (downIn && map[point.x - 1, point.y - 1] > -1)
                map[point.x - 1, point.y] += 1;
        }

        if (right)
        {
            map[point.x + 1, point.y] = 1;
            if (point.x + 2 < maxSize && map[point.x + 2, point.y] > -1)
                map[point.x + 1, point.y] += 1;
            if (upIn && map[point.x + 1, point.y + 1] > -1)
                map[point.x + 1, point.y] += 1;
            if (downIn && map[point.x + 1, point.y - 1] > -1)
                map[point.x + 1, point.y] += 1;
        }

        if (up)
        {
            map[point.x, point.y + 1] = 1;
            if (point.y + 2 < maxSize && map[point.x, point.y + 2] > -1)
                map[point.x, point.y + 1] += 1;
            if (leftIn && map[point.x - 1, point.y + 1] > -1)
                map[point.x, point.y + 1] += 1;
            if (rightIn && map[point.x + 1, point.y + 1] > -1)
                map[point.x, point.y + 1] += 1;
        }

        if (down)
        {
            map[point.x, point.y - 1] = 1;
            if (point.y - 2 > -1 && map[point.x, point.y - 2] > -1)
                map[point.x, point.y - 1] += 1;
            if (leftIn && map[point.x - 1, point.y - 1] > -1)
                map[point.x, point.y - 1] += 1;
            if (rightIn && map[point.x + 1, point.y - 1] > -1)
                map[point.x, point.y - 1] += 1;
        }
        return true;
    }
    private bool Check3Connect2(Vector2Int point)
    {
        int connect = 0;

        if (point.x - 1 > -1 && map[point.x - 1, point.y] > -1)
            connect++;
        if (point.x + 1 < maxSize && map[point.x + 1, point.y] > -1)
            connect++;
        if (point.y + 1 < maxSize && map[point.x, point.y + 1] > -1)
            connect++;
        if (point.y - 1 > -1 && map[point.x, point.y - 1] > -1)
            connect++;

        if (connect > 2)
        {
            if (point.x - 1 > -1 && map[point.x - 1, point.y] > 1)
                return false;
            if (point.x + 1 < maxSize && map[point.x + 1, point.y] > 1)
                return false;
            if (point.y + 1 < maxSize && map[point.x, point.y + 1] > 1)
                return false;
            if (point.y - 1 > -1 && map[point.x, point.y - 1] > 1)
                return false;
        }
        return true;
    }
    private bool CheckDerived3Connect2(Vector2Int point)
    {
        if (map[point.x, point.y] == -1)
            return true;

        if (map[point.x, point.y] > 1)
        {
            if (point.x - 1 > -1 && map[point.x - 1, point.y] > 2)
                return false;
            if (point.x + 1 < maxSize && map[point.x + 1, point.y] > 2)
                return false;
            if (point.y + 1 < maxSize && map[point.x, point.y + 1] > 2)
                return false;
            if (point.y - 1 > -1 && map[point.x, point.y - 1] > 2)
                return false;
        }
        return true;
    }

    private bool CheckConnect1(Vector2Int point)
    {
        bool checkPoint = Check3Connect2(point);
        bool checkLeft = true;
        bool checkRight = true;
        bool checkUp = true;
        bool checkDown = true;
        if (point.x - 1 > -1)
        {
            checkLeft = CheckDerived3Connect2(new Vector2Int(point.x - 1, point.y));
        }
        if (point.x + 1 < maxSize)
        {
            checkRight = CheckDerived3Connect2(new Vector2Int(point.x + 1, point.y));
        }
        if (point.y + 1 < maxSize)
        {
            checkUp = CheckDerived3Connect2(new Vector2Int(point.x, point.y + 1));
        }
        if (point.y - 1 > -1)
        {
            checkDown = CheckDerived3Connect2(new Vector2Int(point.x, point.y - 1));
        }
        return checkPoint && checkLeft && checkRight && checkUp && checkDown;
    }
    private bool Check4Connect44(Vector2Int point)
    {
        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < maxSize;
        bool upIn = point.y + 1 < maxSize;
        bool downIn = point.y - 1 > -1;

        int connect = 0;

        if (point.x - 1 > -1 && map[point.x - 1, point.y] > -1)
            connect++;
        if (point.x + 1 < maxSize && map[point.x + 1, point.y] > -1)
            connect++;
        if (point.y + 1 < maxSize && map[point.x, point.y + 1] > -1)
            connect++;
        if (point.y - 1 > -1 && map[point.x, point.y - 1] > -1)
            connect++;

        if (connect == 4)
        {
            int around = 1;
            if (leftIn && map[point.x - 1, point.y] > 2)
            {
                around++;
            }
            if (point.x - 2 > -1 && map[point.x - 2, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (rightIn && map[point.x + 1, point.y] > 2)
            {
                around++;
            }
            if (point.x + 2 < maxSize && map[point.x + 2, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (upIn && map[point.x, point.y + 1] > 2)
            {
                around++;
            }
            if (point.y + 2 < maxSize && map[point.x, point.y + 2] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (downIn && map[point.x, point.y - 1] > 2)
            {
                around++;
            }
            if (point.y - 2 > -1 && map[point.x, point.y - 2] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (leftIn && map[point.x - 1, point.y] > 2)
            {
                around++;
            }
            if (rightIn && map[point.x + 1, point.y] > 2)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (upIn && map[point.x, point.y + 1] > 2)
            {
                around++;
            }
            if (downIn && map[point.x, point.y - 1] > 2)
            {
                around++;
            }
            if (around == 3)
                return false;
        }
        return true;
    }
    private bool CheckDerived4Connect44(Vector2Int point)
    {
        if (map[point.x, point.y] == -1)
            return true;

        if (map[point.x, point.y] > 2)
        {
            int around = 1;
            if (point.x - 2 > -1 && map[point.x - 2, point.y] > 3)
            {
                around++;
            }
            if (point.x - 3 > -1 && map[point.x - 3, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (point.x + 2 < maxSize && map[point.x + 2, point.y] > 3)
            {
                around++;
            }
            if (point.x + 3 < maxSize && map[point.x + 3, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (point.y + 2 < maxSize && map[point.x, point.y + 2] > 3)
            {
                around++;
            }
            if (point.y + 3 < maxSize && map[point.x, point.y + 3] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (point.y - 2 > -1 && map[point.x, point.y - 2] > 3)
            {
                around++;
            }
            if (point.y - 3 > -1 && map[point.x, point.y - 3] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
        }
        return true;
    }

    private bool CheckConnect2(Vector2Int point)
    {
        bool checkPoint = Check4Connect44(point);
        bool checkLeft = true;
        bool checkRight = true;
        bool checkUp = true;
        bool checkDown = true;
        if (point.x - 1 > -1)
        {
            checkLeft = CheckDerived4Connect44(new Vector2Int(point.x - 1, point.y));
        }
        if (point.x + 1 < maxSize)
        {
            checkRight = CheckDerived4Connect44(new Vector2Int(point.x + 1, point.y));
        }
        if (point.y + 1 < maxSize)
        {
            checkUp = CheckDerived4Connect44(new Vector2Int(point.x, point.y + 1));
        }
        if (point.y - 1 > -1)
        {
            checkDown = CheckDerived4Connect44(new Vector2Int(point.x, point.y - 1));
        }
        return checkPoint && checkLeft && checkRight && checkUp && checkDown;
    }

    private bool CheckArea(Vector2Int point)
    {
        if (map[point.x, point.y] == -1
            && !findMap[point.x, point.y]
            && blockCount < maxBlock
            && CheckConnect1(point)
            && CheckConnect2(point))
            return true;
        return false;
    }

    private void PickPath(Vector2Int point)
    {
        playerDirection = Direction.None;
        if (!findMap[point.x, point.y] && UpdateMap(point))
        {
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;

            // left
            if (point.x - 1 > -1 && CheckArea(new Vector2Int(point.x - 1, point.y)))
            {
                left = true;
            }
            // right
            if (point.x + 1 < maxSize && CheckArea(new Vector2Int(point.x + 1, point.y)))
            {
                right = true;
            }
            // up
            if (point.y + 1 < maxSize && CheckArea(new Vector2Int(point.x, point.y + 1)))
            {
                up = true;
            }
            // down
            if (point.y - 1 > -1 && CheckArea(new Vector2Int(point.x, point.y - 1)))
            {
                down = true;
            }

            int pathCase = 0;
            if (left)
                pathCase++;
            if (right)
                pathCase++;
            if (up)
                pathCase++;
            if (down)
                pathCase++;

            if (pathCase == 0)
            {
                return;
            }
            else
            {
                bool isUsed = false;
                int pickCount = 0;
                int first = -1;
                int second = -1;
                int third = -1;
                int pathCount;

                pathCount = Random.Range(0, pathCase + 1);
                if (turn == 0)
                {
                    pathCount = Random.Range(3, pathCase + 1);
                }
                while (pickCount < pathCount)
                {
                    int pick = Random.Range(0, 4);
                    if (pick != first && pick != second && pick != third)
                    {
                        if (pick == 0 && left)
                        {
                            isUsed = true;
                            UpdateMap(new Vector2Int(point.x - 1, point.y));
                        }
                        else if (pick == 1 && right)
                        {
                            isUsed = true;

                            UpdateMap(new Vector2Int(point.x + 1, point.y));
                        }
                        else if (pick == 2 && up)
                        {
                            isUsed = true;
                            UpdateMap(new Vector2Int(point.x, point.y + 1));
                        }
                        else if (pick == 3 && down)
                        {
                            isUsed = true;
                            UpdateMap(new Vector2Int(point.x, point.y - 1));
                        }

                        if (isUsed)
                        {
                            isUsed = false;
                            pickCount++;
                            if (first == -1)
                            {
                                first = pick;
                            }
                            else if (second == -1)
                            {
                                second = pick;
                            }
                            else if (third == -1)
                            {
                                third = pick;
                            }
                        }
                    }
                }
            }
        }
    }

    private void MoveDirection()
    {
        switch (playerDirection)
        {
            case Direction.Left:
                if (playerPosition.x - 1 > -1 && map[playerPosition.x - 1, playerPosition.y] > -1)
                {
                    turn++;
                    playerPosition.x -= 1;
                    if (UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x, playerPosition.y)))
                        PickPath(new Vector2Int(playerPosition.x, playerPosition.y));
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Right:
                if (playerPosition.x + 1 < maxSize && map[playerPosition.x + 1, playerPosition.y] > -1)
                {
                    turn++;
                    playerPosition.x += 1;
                    if (UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x, playerPosition.y)))
                        PickPath(new Vector2Int(playerPosition.x, playerPosition.y));
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Up:
                if (playerPosition.y + 1 < maxSize && map[playerPosition.x, playerPosition.y + 1] > -1)
                {
                    turn++;
                    playerPosition.y += 1;
                    if (UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x, playerPosition.y)))
                        PickPath(new Vector2Int(playerPosition.x, playerPosition.y));
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Down:
                if (playerPosition.y - 1 > -1 && map[playerPosition.x, playerPosition.y - 1] > -1)
                {
                    turn++;
                    playerPosition.y -= 1;
                    if (UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x, playerPosition.y)))
                        PickPath(new Vector2Int(playerPosition.x, playerPosition.y));
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
        }

        if (playerDirection != Direction.None)
        {
            if (playerPosition.x - 1 > -1 && UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x - 1, playerPosition.y)))
                PickPath(new Vector2Int(playerPosition.x - 1, playerPosition.y));
            if (playerPosition.x + 1 < maxSize && UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x + 1, playerPosition.y)))
                PickPath(new Vector2Int(playerPosition.x + 1, playerPosition.y));
            if (playerPosition.y + 1 < maxSize && UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x, playerPosition.y + 1)))
                PickPath(new Vector2Int(playerPosition.x, playerPosition.y + 1));
            if (playerPosition.y - 1 > -1 && UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x, playerPosition.y - 1)))
                PickPath(new Vector2Int(playerPosition.x, playerPosition.y - 1));
        }

        UpdateFindMap(playerPosition);
    }


    private IEnumerator SetDirection()
    {
        while(blockCount < maxBlock)
        {
            yield return null;
            if(turn > 900)
            {
                break;
            }

            switch (Random.Range(0, 4))
            {
                case 0:
                    playerDirection = Direction.Left;
                    MoveDirection();
                    break;
                case 1:
                    playerDirection = Direction.Right;
                    MoveDirection();
                    break;
                case 2:
                    playerDirection = Direction.Up;
                    MoveDirection();
                    break;
                case 3:
                    playerDirection = Direction.Down;
                    MoveDirection();
                    break;
            }
        }
        
        if (blockCount < minTile)
        {
            CreateMap();
        }
        else
        {
            CheckTileCase();
        }
    }

    private void CreateMap()
    {
        turn = 0;
        blockCount = 0;
        playerDirection = Direction.None;

        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                map[i, j] = -1;
                findMap[i, j] = false;
            }
        }
        
        maxBlock = Random.Range(minTile, maxTile);

        int x = Random.Range(6, 9);
        int y = Random.Range(6, 9);
        // 중앙 행열 안에서 시작점을 랜덤 선택
        startPosition = new Vector2Int(x, y);
        map[x, y] = 0;
        blockCount++;

        playerPosition = startPosition;
        transform.GetChild(0).transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
        PickPath(startPosition);
        StartCoroutine(SetDirection());
    }


    private bool CreateSecretTile()
    {
        for(int k = 4; k > 0; k--)
        {
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if ((map[i, j] < 0 || map[i, j] > 6) 
                        && Check4AdjacentTile(new Vector2Int(i, j)) == k 
                        && CheckTwoSecretTile(new Vector2Int(i, j)))
                    {
                        map[i, j] = 0;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CheckTwoSecretTile(Vector2Int point)
    {
        if (point.x -2 > -1 && map[point.x - 2, point.y] == 0)
            return false;
        if (point.x + 2 < maxSize && map[point.x + 2, point.y] == 0)
            return false;
        if(point.y + 2 < maxSize && map[point.x, point.y + 2] == 0)
            return false;
        if(point.y - 2 > -1 && map[point.x, point.y - 2] == 0)
            return false;

        bool left = point.x - 1 > -1;
        bool right = point.x + 1 < maxSize;
        bool up = point.y + 1 < maxSize;
        bool down = point.y - 1 > -1;

        if (left && up && map[point.x - 1, point.y + 1] == 0)
            return false;
        if (right && down && map[point.x + 1, point.y - 1] == 0)
            return false;
        if (up && right && map[point.x + 1, point.y + 1] == 0)
            return false;
        if (down && left && map[point.x - 1, point.y + -1] == 0)
            return false;

        return true;
    }


    private int CountAdjacentTile(Vector2Int point)
    {
        int count = 0;
        bool left = point.x - 1 > -1;
        bool right = point.x + 1 < maxSize;
        bool up = point.y + 1 < maxSize;
        bool down = point.y - 1 > -1;

        if (left && up && map[point.x - 1, point.y + 1] > 0)
        {
            count++;
        }
        if (right && down && map[point.x + 1, point.y - 1] > 0)
        {
            count++;
        }
        if (up && right && map[point.x + 1, point.y + 1] > 0)
        {
            count++;
        }
        if (down && left && map[point.x - 1, point.y + -1] > 0)
        {
            count++;
        }
        return count;
    }

    private int Check4AdjacentTile(Vector2Int point)
    {
        if(point.x - 1 > -1 && point.x + 1 < maxSize && point.y + 1 < maxSize && point.y - 1 > -1)
        {
            int count = 0;

            if (map[point.x - 1, point.y] > 0 && map[point.x - 1, point.y] < 7)
                count++;
            if(map[point.x + 1, point.y] > 0 && map[point.x + 1, point.y] < 7)
                count++;
            if(map[point.x, point.y + 1] > 0 && map[point.x, point.y + 1] < 7)
                count++;
            if (map[point.x, point.y - 1] > 0 && map[point.x, point.y - 1] < 7)
                count++;

            return count;
        }
        return 0;
    }

    private bool CheckSecretTile()
    {
        int secretCount = 0;

        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                if ((map[i, j] < 1 || map[i, j] > 7)
                    && Check4AdjacentTile(new Vector2Int(i, j)) == 4
                    && CheckTwoSecretTile(new Vector2Int(i, j))
                    && CountAdjacentTile(new Vector2Int(i, j)) == 4)
                {
                    map[i, j] = 0;
                    secretCount++;
                }
            }
        }

        while ((blockCount < 66 && secretCount < 2) || (blockCount > 65 && secretCount < 3))
        {
            if (CreateSecretTile())
                secretCount++;
        }

        return true;
    }

    private bool CreateBossExitTile()
    {
        int bossCount = 0;
        List<Vector2Int> bossCase = new List<Vector2Int>();
        int exitCount = 0;
        List<Vector2Int> exitCase = new List<Vector2Int>();

        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                if (map[i, j] == 1)
                {
                    int distance = minDistance(new Vector2Int(i, j), startPosition, map);
                    if (distance > 6)
                    {
                        bossCount++;
                        bossCase.Add(new Vector2Int(i, j));
                    }
                    if (distance > 4)
                    {
                        exitCount++;
                        exitCase.Add(new Vector2Int(i, j));
                    }
                }
            }
        }

        if (bossCount == 0)
        {
            CreateMap();
            return false;
        }
        else if ((blockCount < 71 && exitCount < 3) || (blockCount > 70 && exitCount < 4))
        {
            CreateMap();
            return false;
        }

        int boss = Random.Range(0, bossCase.Count);
        map[bossCase[boss].x, bossCase[boss].y] = 6;
        exitCase.Remove(bossCase[boss]);

        int exit = Random.Range(0, exitCase.Count);
        map[exitCase[exit].x, exitCase[exit].y] = 5;
        exitCase.RemoveAt(exit);
        exit = Random.Range(0, exitCase.Count);
        map[exitCase[exit].x, exitCase[exit].y] = 5;
        exitCase.RemoveAt(exit);

        if (blockCount > 59 && exitCase.Count > 0)
        {
            exit = Random.Range(0, exitCase.Count);
            map[exitCase[exit].x, exitCase[exit].y] = 5;
            exitCase.RemoveAt(exit);
        }
        return true;
    }

    private bool CheckContinue4(Vector2Int point, int type)
    {
        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < maxSize;
        bool upIn = point.y + 1 < maxSize;
        bool downIn = point.y - 1 > -1;

        if (leftIn && map[point.x - 1, point.y] == type)
        {
            if(!CheckContinue3(new Vector2Int(point.x - 1, point.y), type))
                return false;

        }
        if (rightIn && map[point.x + 1, point.y] == type)
        {
            if (!CheckContinue3(new Vector2Int(point.x + 1, point.y), type))
                return false;

        }
        if (upIn && map[point.x, point.y + 1] == type)
        {
            if (!CheckContinue3(new Vector2Int(point.x, point.y + 1), type))
                return false;
        }
        if (downIn && map[point.x, point.y - 1] == type)
        {
            if (!CheckContinue3(new Vector2Int(point.x, point.y - 1), type))
                return false;
        }

        return true;
    }

    private bool CheckContinue3(Vector2Int point, int type)
    {
        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < maxSize;
        bool upIn = point.y + 1 < maxSize;
        bool downIn = point.y - 1 > -1;

        if (leftIn && map[point.x - 1, point.y] == type)
        {
            if (upIn && map[point.x - 1, point.y + 1] == type)
                return false;
            if (downIn && map[point.x - 1, point.y - 1] == type)
                return false;
        }
        if(rightIn && map[point.x + 1, point.y] == type)
        {
            if (upIn && map[point.x + 1, point.y + 1] == type)
                return false;
            if (downIn && map[point.x + 1, point.y - 1] == type)
                return false;
        }
        if(upIn && map[point.x, point.y + 1] == type)
        {
            if (leftIn && map[point.x - 1, point.y + 1] == type)
                return false;
            if (rightIn && map[point.x + 1, point.y + 1] == type)
                return false;
        }
        if (downIn && map[point.x, point.y - 1] == type)
        {
            if (leftIn && map[point.x - 1, point.y - 1] == type)
                return false;
            if (rightIn && map[point.x + 1, point.y - 1] == type)
                return false;
        }

        return true;
    }


    private bool CheckContinueStright(Vector2Int point, int length, int type)
    {
        int count = 0;
        for(int i = 1; i < length; i++)
        {
            if (point.x - i > -1 && map[point.x - i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.x + i < maxSize && map[point.x + i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y + i < maxSize && map[point.x, point.y + i] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y - i > -1 && map[point.x, point.y - i] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        int end = length / 2;
        if (length % 2 == 1)
            end++;

        for(int i = -length/2; i < end; i++)
        {
            if (point.x + i > -1 && point.x + i < maxSize && map[point.x + i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = -length / 2; i < end; i++)
        {
            if (point.y + i > -1 && point.y + i < maxSize && map[point.x, point.y + i] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        return true;
    }

    private bool CreateEventTile()
    {
        int count = 0;
        int eventTile;
        if (blockCount < 60)
            eventTile = Random.Range(12, 15);
        else if (blockCount < 66)
            eventTile = Random.Range(13, 17);
        else if (blockCount < 71)
            eventTile = Random.Range(15, 19);
        else
            eventTile = Random.Range(17, 21);

        for (int k = 0; k < 5; k++)
        {
            if (count >= eventTile)
                break;
            for (int i = 0; i < maxSize; i++)
            {
                if (count >= eventTile)
                    break;
                for (int j = 0; j < maxSize; j++)
                {
                    if (count >= eventTile)
                        break;

                    if (i != startPosition.x && j != startPosition.y
                        && map[i, j] > 0 && map[i, j] < 5
                        && Random.Range(0, 100) < 50
                        && CheckContinueStright(new Vector2Int(i, j), 4, 10)
                        && CheckContinue4(new Vector2Int(i, j), 10))
                    {
                        map[i, j] = 10;
                        count++;
                    }
                }
            }
        }

        if (count < eventTile)
            Debug.Log("Event: " + count);

        int positive = Random.Range(1, count);
        int negative = eventTile - positive;
        if (negative < positive)
        {
            positive = negative;
        }

        count = 0;
        while (count < positive)
        {
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (count >= positive)
                        return true;

                    if (map[i, j] == 10
                        && Random.Range(0, 100) < 50)
                    {
                        map[i, j] = 9;
                        count++;
                    }
                }
            }
        }

        return true;
    }

    private bool CreateChestTiles()
    {
        int count = 0;
        int chest;
        if (blockCount < 60)
            chest = Random.Range(4, 9);
        else if(blockCount < 66)
            chest = Random.Range(5,10);
        else if (blockCount < 71) 
            chest= Random.Range(6,11);
        else
            chest = Random.Range(7,12);
  

        for(int k = 0; k < 5; k++)
        {
            if (count >= chest)
                break;
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (count >= chest)
                        return true;

                    if (i != startPosition.x && j != startPosition.y
                        && map[i, j] > 0 && map[i, j] < 5
                        && Random.Range(0, 100) < 50
                        && CheckContinueStright(new Vector2Int(i, j), 3, 8)
                        && CheckContinue3(new Vector2Int(i, j), 8))
                    {
                        map[i, j] = 8;
                        count++;
                    }
                }
            }
        }

        if (count < chest)
            Debug.Log("Chest: " + count);

        return true;
    }

    private bool CreateMonsterTile()
    {
        int count = 0;
        int monster;
        if (blockCount < 60)
            monster = Random.Range(17, 21);
        else if (blockCount < 66)
            monster = Random.Range(19, 23);
        else if (blockCount < 71)
            monster = Random.Range(21, 25);
        else
            monster = Random.Range(23, 27);

        for(int k = 0; k < 5; k++)
        {
            if (count >= monster)
                break;
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (count >= monster)
                        return true;

                    if (i != startPosition.x && j != startPosition.y
                        && map[i, j] > 0 && map[i, j] < 5
                        && Random.Range(0, 100) < 50
                        && CheckContinueStright(new Vector2Int(i, j), 4, 7)
                        && CheckContinue4(new Vector2Int(i, j), 7))
                    {
                        map[i, j] = 7;
                        count++;
                    }
                }
            }
        }

        if (count < monster)
            Debug.Log("Monster: " + count);

        return true;
    }

    private void CheckTileCase()
    {
        if (CreateBossExitTile() 
            && CheckSecretTile() 
            && CreateMonsterTile()
            && CreateChestTiles()
            && CreateEventTile())
        {
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    findMap[i, j] = false;
                }
            }

            UpdateFindMap(startPosition);
            playerPosition = startPosition;
            transform.GetChild(0).position = new Vector3(startPosition.x + 0.5f, startPosition.y + 0.5f, 0);
            DrawMap();
            goneMap[startPosition.x, startPosition.y] = true;
            playerDirection = Direction.None;
        }
    }

    private void DrawMap()
    {
        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                if (findMap[i, j])
                {
                    if (!goneMap[i, j] && map[i,j] >= 0)
                    {
                        
                        if (map[i, j] == 0)
                        {
                            roadMap.SetTile(new Vector3Int(i, j), lattern);
                        }
                        else if (map[i, j] < 5)
                        {
                            backgroundMap.SetTile(new Vector3Int(i, j, 0), roadTile);
                        }
                        else if (map[i, j] == 5)
                        {
                            backgroundMap.SetTile(new Vector3Int(i, j, 0), roadTile);
                            roadMap.SetTile(new Vector3Int(i, j), cube);
                        }
                        else if (map[i, j] == 6)
                        {
                            backgroundMap.SetTile(new Vector3Int(i, j, 0), tile1);
                        }
                        else if (map[i, j] == 7)
                        {
                            backgroundMap.SetTile(new Vector3Int(i, j, 0), roadTile);
                            roadMap.SetTile(new Vector3Int(i, j, 0), tile2);
                        }
                        else if (map[i,j] == 8)
                        {
                            backgroundMap.SetTile(new Vector3Int(i, j, 0), roadTile);
                            roadMap.SetTile(new Vector3Int(i, j), box);
                        }
                        else if (map[i,j] == 9)
                        {
                            backgroundMap.SetTile(new Vector3Int(i, j, 0), roadTile);
                            roadMap.SetTile(new Vector3Int(i, j, 0), tile4);
                        }
                        else if (map[i, j] == 10)
                        {
                            backgroundMap.SetTile(new Vector3Int(i, j, 0), roadTile);
                            roadMap.SetTile(new Vector3Int(i, j, 0), tile3);
                        }
                    }
                }
            }
        }
    }
    
    private void Awake()
    {
        maxBlock = 0;

        if (transform.childCount == 3)
        {
            if (transform.GetChild(1).TryGetComponent<Tilemap>(out roadMap)
                && transform.GetChild(2).TryGetComponent<Tilemap>(out backgroundMap))
            {
                roadTile = backgroundMap.GetTile(Vector3Int.up);
                coverTile = backgroundMap.GetTile(Vector3Int.zero);
                tile0 = backgroundMap.GetTile(new Vector3Int(-1, -1, 0));
                tile1 = backgroundMap.GetTile(new Vector3Int(-1, 0, 0));
                tile2 = backgroundMap.GetTile(new Vector3Int(-1, 1, 0));
                tile3 = backgroundMap.GetTile(new Vector3Int(-1, 2, 0));
                tile4 = backgroundMap.GetTile(new Vector3Int(-1, 3, 0));

                box = backgroundMap.GetTile(Vector3Int.right);
                cube = backgroundMap.GetTile(new Vector3Int(1, 1, 0));
                lattern = backgroundMap.GetTile(new Vector3Int(2, 0, 0));
            }

            map = new int[maxSize, maxSize];
            findMap = new bool[maxSize, maxSize];
            goneMap = new bool[maxSize, maxSize];
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    goneMap[i, j] = false;
                }
            }

            roadMap.ClearAllTiles();
            backgroundMap.ClearAllTiles();

            CreateMap();
        }
    }

    private void Notify()
    {
        if (!goneMap[playerPosition.x, playerPosition.y])
        {
            switch (map[playerPosition.x, playerPosition.y])
            {
                case 5:
                    Debug.Log("Exit");
                    break;
                case 6:
                    Debug.Log("Boss");
                    break;
                case 7:
                    Debug.Log("Monster");
                    break;
                case 8:
                    Debug.Log("Chest");
                    break;
                case 9:
                    Debug.Log("Positive");
                    break;
                case 10:
                    Debug.Log("Negative");
                    break;
            }
        }
    }

    private void MovePlayer()
    {
        switch (playerDirection)
        {
            case Direction.Left:
                if (playerPosition.x - 1 > -1 && map[playerPosition.x - 1, playerPosition.y] > -1)
                {
                    playerPosition.x -= 1;
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Right:
                if (playerPosition.x + 1 < maxSize && map[playerPosition.x + 1, playerPosition.y] > -1)
                {
                    playerPosition.x += 1;
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Up:
                if (playerPosition.y + 1 < maxSize && map[playerPosition.x, playerPosition.y + 1] > -1)
                {
                    playerPosition.y += 1;
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Down:
                if (playerPosition.y - 1 > -1 && map[playerPosition.x, playerPosition.y - 1] > -1)
                {
                    playerPosition.y -= 1;
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
        }

        if(playerDirection != Direction.None)
        {
            transform.GetChild(0).position = new Vector3(playerPosition.x + 0.5f, playerPosition.y + 0.5f, 0);
            UpdateFindMap(playerPosition);
            Notify();
            DrawMap();
            goneMap[playerPosition.x, playerPosition.y] = true;
            playerDirection = Direction.None;
        }
    }

    private void Update()
    {
        if(playerDirection == Direction.None)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                playerDirection = Direction.Left;
                MovePlayer();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                playerDirection = Direction.Right;
                MovePlayer();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                playerDirection = Direction.Up;
                MovePlayer();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                playerDirection = Direction.Down;
                MovePlayer();
            }
        }
    }

}
