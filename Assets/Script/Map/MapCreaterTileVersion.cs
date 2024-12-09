using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreaterTileVersion : MonoBehaviour
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

    private GameObject player;

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

    private static int maxSize = 12;
    private static int minSize = 0;

    [SerializeField]
    private int turn;
    private Direction playerDirection;
    private Vector2Int playerPosition;

    private int[,] map;
    private bool[,] findMap;
    private bool[,] goneMap;

    [SerializeField]
    private Vector2Int startPosition;
    public Vector2 GetStartPosition()
    {
        return startPosition;
    }
    [SerializeField]
    private Vector2Int bossPosition;
    public Vector2 GetbossPostion() 
    { 
        return bossPosition; 
    }
    [SerializeField]
    private int blockCount;
    [SerializeField]
    private int maxBlock;

    private void UpdateFindMap(Vector2Int point)
    {
        findMap[point.x, point.y] = true;

        if (point.x - 1 > minSize)
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
        if (point.y - 1 > minSize)
        {
            findMap[point.x, point.y - 1] = true;
        }
    }

    private bool UpdateMap(Vector2Int point)
    {
        if (map[point.x, point.y] == -1 && blockCount < maxBlock)
        {
            blockCount++;
        }

        bool leftOut = point.x - 1 > minSize;
        bool rightOut = point.x + 1 < maxSize;
        bool upOut = point.y + 1 < maxSize;
        bool downOut = point.y - 1 > minSize;
        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;
        if (point.x - 1 > minSize)
        {
            left = map[point.x - 1, point.y] > -1;
        }
        if (point.x + 1 < maxSize)
        {
            right = map[point.x + 1, point.y] > -1;
        }
        if (point.y + 1 < maxSize)
        {
            up = map[point.x, point.y + 1] > -1;
        }
        if (point.y - 1 > minSize)
        {
            down = map[point.x, point.y - 1] > -1;
        }

        map[point.x, point.y] = 0;
        if (leftOut && left)
            map[point.x, point.y] += 1;
        if (rightOut && right)
            map[point.x, point.y] += 1;
        if (upOut && up)
            map[point.x, point.y] += 1;
        if (downOut && down)
            map[point.x, point.y] += 1;

        if (left)
        {
            map[point.x - 1, point.y] = 1;
            if (point.x - 2 > minSize && map[point.x - 2, point.y] > -1)
                map[point.x - 1, point.y] += 1;
            if (upOut && map[point.x - 1, point.y + 1] > -1)
                map[point.x - 1, point.y] += 1;
            if (downOut && map[point.x - 1, point.y - 1] > -1)
                map[point.x - 1, point.y] += 1;
        }

        if (right)
        {
            map[point.x + 1, point.y] = 1;
            if (point.x + 2 < maxSize && map[point.x + 2, point.y] > -1)
                map[point.x + 1, point.y] += 1;
            if (upOut && map[point.x + 1, point.y + 1] > -1)
                map[point.x + 1, point.y] += 1;
            if (downOut && map[point.x + 1, point.y - 1] > -1)
                map[point.x + 1, point.y] += 1;
        }

        if (up)
        {
            map[point.x, point.y + 1] = 1;
            if (point.y + 2 < maxSize && map[point.x, point.y + 2] > -1)
                map[point.x, point.y + 1] += 1;
            if (leftOut && map[point.x - 1, point.y + 1] > -1)
                map[point.x, point.y + 1] += 1;
            if (rightOut && map[point.x + 1, point.y + 1] > -1)
                map[point.x, point.y + 1] += 1;
        }

        if (down)
        {
            map[point.x, point.y - 1] = 1;
            if (point.y - 2 > minSize && map[point.x, point.y - 2] > -1)
                map[point.x, point.y - 1] += 1;
            if (leftOut && map[point.x - 1, point.y - 1] > -1)
                map[point.x, point.y - 1] += 1;
            if (rightOut && map[point.x + 1, point.y - 1] > -1)
                map[point.x, point.y - 1] += 1;
        }
        return true;
    }
    private bool Check3Connect2(Vector2Int point)
    {
        int connect = 0;

        if (point.x - 1 > minSize && map[point.x - 1, point.y] > -1)
            connect++;
        if (point.x + 1 < maxSize && map[point.x + 1, point.y] > -1)
            connect++;
        if (point.y + 1 < maxSize && map[point.x, point.y + 1] > -1)
            connect++;
        if (point.y - 1 > minSize && map[point.x, point.y - 1] > -1)
            connect++;

        if (connect > 2)
        {
            if (point.x - 1 > minSize && map[point.x - 1, point.y] > 1)
                return false;
            if (point.x + 1 < maxSize && map[point.x + 1, point.y] > 1)
                return false;
            if (point.y + 1 < maxSize && map[point.x, point.y + 1] > 1)
                return false;
            if (point.y - 1 > minSize && map[point.x, point.y - 1] > 1)
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
            if (point.x - 1 > minSize && map[point.x - 1, point.y] > 2)
                return false;
            if (point.x + 1 < maxSize && map[point.x + 1, point.y] > 2)
                return false;
            if (point.y + 1 < maxSize && map[point.x, point.y + 1] > 2)
                return false;
            if (point.y - 1 > minSize && map[point.x, point.y - 1] > 2)
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
        if (point.x - 1 > minSize)
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
        if (point.y - 1 > minSize)
        {
            checkDown = CheckDerived3Connect2(new Vector2Int(point.x, point.y - 1));
        }
        return checkPoint && checkLeft && checkRight && checkUp && checkDown;
    }
    private bool Check4Connect44(Vector2Int point)
    {
        bool leftOut = point.x - 1 > minSize;
        bool rightOut = point.x + 1 < maxSize;
        bool upOut = point.y + 1 < maxSize;
        bool downOut = point.y - 1 > minSize;

        int connect = 0;

        if (point.x - 1 > minSize && map[point.x - 1, point.y] > -1)
            connect++;
        if (point.x + 1 < maxSize && map[point.x + 1, point.y] > -1)
            connect++;
        if (point.y + 1 < maxSize && map[point.x, point.y + 1] > -1)
            connect++;
        if (point.y - 1 > minSize && map[point.x, point.y - 1] > -1)
            connect++;

        if (connect == 4)
        {
            int around = 1;
            if (leftOut && map[point.x - 1, point.y] > 2)
            {
                around++;
            }
            if (point.x - 2 > minSize && map[point.x - 2, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (rightOut && map[point.x + 1, point.y] > 2)
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
            if (upOut && map[point.x, point.y + 1] > 2)
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
            if (downOut && map[point.x, point.y - 1] > 2)
            {
                around++;
            }
            if (point.y - 2 > minSize && map[point.x, point.y - 2] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (leftOut && map[point.x - 1, point.y] > 2)
            {
                around++;
            }
            if (rightOut && map[point.x + 1, point.y] > 2)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (upOut && map[point.x, point.y + 1] > 2)
            {
                around++;
            }
            if (downOut && map[point.x, point.y - 1] > 2)
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
            if (point.x - 2 > minSize && map[point.x - 2, point.y] > 3)
            {
                around++;
            }
            if (point.x - 3 > minSize && map[point.x - 3, point.y] > 3)
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
            if (point.y - 2 > minSize && map[point.x, point.y - 2] > 3)
            {
                around++;
            }
            if (point.y - 3 > minSize && map[point.x, point.y - 3] > 3)
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
        if (point.x - 1 > minSize)
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
        if (point.y - 1 > minSize)
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
            if (point.x - 1 > minSize && CheckArea(new Vector2Int(point.x - 1, point.y)))
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
            if (point.y - 1 > minSize && CheckArea(new Vector2Int(point.x, point.y - 1)))
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
                if (playerPosition.x - 1 > minSize && map[playerPosition.x - 1, playerPosition.y] > -1)
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
                if (playerPosition.y - 1 > minSize && map[playerPosition.x, playerPosition.y - 1] > -1)
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
            if (playerPosition.x - 1 > minSize && UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x - 1, playerPosition.y)))
                PickPath(new Vector2Int(playerPosition.x - 1, playerPosition.y));
            if (playerPosition.x + 1 < maxSize && UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x + 1, playerPosition.y)))
                PickPath(new Vector2Int(playerPosition.x + 1, playerPosition.y));
            if (playerPosition.y + 1 < maxSize && UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x, playerPosition.y + 1)))
                PickPath(new Vector2Int(playerPosition.x, playerPosition.y + 1));
            if (playerPosition.y - 1 > minSize && UpdateMap(playerPosition) && CheckArea(new Vector2Int(playerPosition.x, playerPosition.y - 1)))
                PickPath(new Vector2Int(playerPosition.x, playerPosition.y - 1));
        }

        UpdateFindMap(playerPosition);
    }

    private IEnumerator SetDirection()
    {
        while (blockCount < maxBlock)
        {
            yield return null;
            if (turn > 900)
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

        if (blockCount < 55)
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
        blockCount = 0;
        turn = 0;
        playerDirection = Direction.None;
        bossPosition = new Vector2Int(-1, -1);
        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                map[(int)i, (int)j] = -1;
                findMap[(int)i, (int)j] = false;
                goneMap[(int)i, (int)j] = false;
            }
        }
        
        maxBlock = Random.Range(55, 75);
 

        int x = Random.Range(6, 9);
        int y = Random.Range(6, 9);
        // 중앙 행열 안에서 시작점을 랜덤 선택
        startPosition = new Vector2Int(x, y);
        map[x, y] = 0;
        goneMap[x, y] = true;
        blockCount++;

        playerPosition = startPosition;
        player.transform.position = new Vector3(x * 12 + 6, y * 12 + 6, 0);
        PickPath(playerPosition);
        StartCoroutine(SetDirection());
    }
    private void TestTile()
    {
        Vector2Int point = playerPosition;
        switch (playerDirection)
        {
            case Direction.Left:
                if (point.x - 1 > minSize && map[point.x - 1, point.y] > -1)
                {
                    turn++;
                    point.x -= 1;
                    if (UpdateMap(point) && CheckArea(new Vector2Int(point.x, point.y)))
                        PickPath(new Vector2Int(point.x, point.y));
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Right:
                if (point.x + 1 < maxSize && map[point.x + 1, point.y] > -1)
                {
                    turn++;
                    point.x += 1;
                    if (UpdateMap(point) && CheckArea(new Vector2Int(point.x, point.y)))
                        PickPath(new Vector2Int(point.x, point.y));
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Up:
                if (point.y + 1 < maxSize && map[point.x, point.y + 1] > -1)
                {
                    turn++;
                    point.y += 1;
                    if (UpdateMap(point) && CheckArea(new Vector2Int(point.x, point.y)))
                        PickPath(new Vector2Int(point.x, point.y));
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
            case Direction.Down:
                if (point.y - 1 > minSize && map[point.x, point.y - 1] > -1)
                {
                    turn++;
                    point.y -= 1;
                    if (UpdateMap(point) && CheckArea(new Vector2Int(point.x, point.y)))
                        PickPath(new Vector2Int(point.x, point.y));
                }
                else
                {
                    playerDirection = Direction.None;
                }
                break;
        }

        if (playerDirection != Direction.None)
        {
            if (point.x - 1 > minSize && UpdateMap(point) && CheckArea(new Vector2Int(point.x - 1, point.y)))
                PickPath(new Vector2Int(point.x - 1, point.y));
            if (point.x + 1 < maxSize && UpdateMap(point) && CheckArea(new Vector2Int(point.x + 1, point.y)))
                PickPath(new Vector2Int(point.x + 1, point.y));
            if (point.y + 1 < maxSize && UpdateMap(point) && CheckArea(new Vector2Int(point.x, point.y + 1)))
                PickPath(new Vector2Int(point.x, point.y + 1));
            if (point.y - 1 > minSize && UpdateMap(point) && CheckArea(new Vector2Int(point.x, point.y - 1)))
                PickPath(new Vector2Int(point.x, point.y - 1));
        }
    }

    private bool CreateSecretTile()
    {
        for (int k = 4; k > 0; k--)
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
        if (point.x - 2 > minSize && map[point.x - 2, point.y] == 0)
            return false;
        if (point.x + 2 < maxSize && map[point.x + 2, point.y] == 0)
            return false;
        if (point.y + 2 < maxSize && map[point.x, point.y + 2] == 0)
            return false;
        if (point.y - 2 > minSize && map[point.x, point.y - 2] == 0)
            return false;

        bool left = point.x - 1 > minSize;
        bool right = point.x + 1 < maxSize;
        bool up = point.y + 1 < maxSize;
        bool down = point.y - 1 > minSize;

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
        bool left = point.x - 1 > minSize;
        bool right = point.x + 1 < maxSize;
        bool up = point.y + 1 < maxSize;
        bool down = point.y - 1 > minSize;

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
        if (point.x - 1 > minSize && point.x + 1 < maxSize && point.y + 1 < maxSize && point.y - 1 > minSize)
        {
            int count = 0;

            if (map[point.x - 1, point.y] > 0 && map[point.x - 1, point.y] < 7)
                count++;
            if (map[point.x + 1, point.y] > 0 && map[point.x + 1, point.y] < 7)
                count++;
            if (map[point.x, point.y + 1] > 0 && map[point.x, point.y + 1] < 7)
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

    private void CheckTileCase()
    {
        if (CreateBossExitTile())
        {
            if (CheckSecretTile())
            {
                DrawAllMap();
            }
        }
    }
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

    private void DrawDark(int x, int y)
    {
        for (int i = 2; i < 10; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                darkMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), dark[i, j]);
            }
        }
    }

    private void DrawAllMap()
    {
        player.transform.position = new Vector3(startPosition.x * 12 + 6, startPosition.y * 12 + 6);

        for (int i = minSize; i < maxSize; i++)
        {
            for (int j = minSize; j < maxSize; j++)
            {
                Vector2Int v = new Vector2Int(i * 12, j * 12);
                if (map[i, j] > 0 && map[i,j] < 7)
                {
                    DrawRoom(i, j);

                    if (map[i, j] == 6)
                    {
                        props.DrawAltar(v);
                    }
                    else if (map[i, j] == 5)
                    {
                        props.DrawAltarRune(v);
                        props.DrawAltar(v);
                        props.DrawPillar(v);
                    }

                    if (i - 1 > minSize && map[i - 1, j] > 0)
                    {
                        DrawLeftPath(i, j);
                    }
                    if (i + 1 < maxSize && map[i + 1, j] > 0)
                    {
                        DrawRightPath(i, j);
                    }

                    if (j + 1 < maxSize)
                    {
                        if (map[i, j + 1] > 0)
                        {
                            DrawUpPath(i, j);
                            props.DrawClosedGate(v, false);
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

                    if (j - 1 > minSize)
                    {
                        if (map[i, j - 1] > 0)
                        {
                            DrawDownPath(i, j);
                            props.DrawClosedGate(v, false);
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

                    //if (!goneMap[i, j])
                    //{
                    //   DrawDark(i, j);
                    //}
                }
                else if (map[i,j] == 0)
                {
                    DrawRoom(i, j); 
                    props.DrawPillar(new Vector2Int(i,j) * 12);
                }
            }
        }
    }
    private void DrawMap()
    {
        player.transform.position = new Vector3(startPosition.x * 12 + 6, startPosition.y * 12 + 6);

        for (int i = minSize; i < maxSize; i++)
        {
            for (int j = minSize; j < maxSize; j++)
            {
                Vector2Int v = new Vector2Int(i * 12, j * 12);
                if (findMap[i, j] && map[i, j] > 0 && map[i,j] < 7)
                {
                    DrawRoom(i, j);
                    if (startPosition == new Vector2Int(i, j))
                    {
                        props.DrawPillar(startPosition * 12);
                    }

                    if (map[i, j] == 6)
                    {
                        props.DrawAltar(v);
                    }
                    else if (map[i, j] == 5)
                    {
                        props.DrawAltarRune(v);
                        props.DrawAltar(v);
                        props.DrawPillar(v);
                    }

                    if (i - 1 > minSize && findMap[i - 1, j] && map[i - 1, j] > 0)
                    {
                        DrawLeftPath(i, j);
                    }
                    if (i + 1 < maxSize && findMap[i + 1, j] && map[i + 1, j] > 0)
                    {
                        DrawRightPath(i, j);
                    }
                    if (j + 1 < maxSize && findMap[i, j + 1])
                    {
                        if (map[i, j + 1] > 0)
                        {
                            DrawUpPath(i, j);
                            if (goneMap[i, j])
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
                    if (j - 1 > minSize && findMap[i, j - 1])
                    {
                        if (map[i, j - 1] > 0)
                        {
                            DrawDownPath(i, j);
                            if (goneMap[i, j])
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

                    //if (!goneMap[i, j])
                    //{
                    //   DrawDark(i, j);
                    //}
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
        maxBlock = 0;

        player = GameObject.Find("Player");
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

            map = new int[maxSize, maxSize];
            findMap = new bool[maxSize, maxSize];
            goneMap = new bool[maxSize, maxSize];

            ClearTilemaps();
            CreateMap();
        }
    }

    private void LateUpdate()
    {

        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
    }
        
}
