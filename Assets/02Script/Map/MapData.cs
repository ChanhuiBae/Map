using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction
{
    None,
    Left,
    Right,
    Up,
    Down,
}
public enum TileType
{
    None = -1,
    Secret = 0,
    End = 1,
    Two = 2,
    Tree = 3,
    Four = 4,
    Exit = 5,
    Boss = 6,
    Monster = 7,
    Chest = 8,
    PositiveEvent = 9,
    NegativeEvent = 10,
}

public class MapData : MonoBehaviour
{
    private bool access = false;

    private static int size = 12;

    private static int max = 75;
    private static int min = 55;

    private int[,] map = new int[size, size];
    private bool[,] findMap = new bool[size, size];
    private bool[,] knownMap = new bool[size, size];    
    private bool[,] goneMap = new bool[size, size];
    private bool[,] blownupMap = new bool[size, size];

    [SerializeField]
    private int turn;
    private Vector2Int position;
    private Direction direction;

    private Vector2Int startPosition;

    [SerializeField]
    private int tileCount;
    [SerializeField]
    private int maxTile;

    #region setter & getter
    public bool Access
    {
        get => access;
    }

    public int MapSize
    {
        get => size;
    }

    public int TileCount
    {
        get => tileCount;
    }

    public int MinTile
    {
        get => min;
    }

    public int Turn
    {
        get => turn;
    }

    public Vector2Int GetPosition()
    {
        if (access)
            return position;
        return new Vector2Int(-1, -1);
    }
    public int GetMap(int i,int j)
    {
        if (access)
            return map[i,j];
        return -1;
    }
    public int GetCurrentMap()
    {
        if (access)
            return map[position.x, position.y];
        return -1;
    }

    public bool GetFind(int i, int j)
    {
        if (access)
            return findMap[i,j];
        return false;
    }
    public bool GetKnown(int i, int j)
    {
        if (access)
            return knownMap[i, j];
        return false;
    }

    public bool GetGone(int i, int j)
    {
        if (access)
            return goneMap[i,j];
        return false;
    }
    
    public bool GetBlownUp(int i, int j)
    {
        if(access)
            return blownupMap[i,j];
        return false;
    }
    #endregion

    public MapData()
    {
        access = false;
    }

    private int minDistance(Vector2Int start, Vector2Int target)
    { // Astar
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
        for (int i = 0; i < size; i++)
        {
            List<MapTile> t = new List<MapTile>();
            for (int j = 0; j < size; j++)
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
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    //// 8 way
                    //bool near = (System.Math.Abs(currentPath.X - pathes[i][j].X) <= 1)
                    //         && (System.Math.Abs(currentPath.Y - pathes[i][j].Y) <= 1);
                    // 4 way
                    bool near = (System.Math.Abs(currentTile.X - tiles[i][j].X) <= 1)
                             && (System.Math.Abs(currentTile.Y - tiles[i][j].Y) <= 1)
                             && (currentTile.Y == tiles[i][j].Y || currentTile.X == tiles[i][j].X);
                    if (map[i, j] == obstacle
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


    #region CreateMap
    private void UpdateFindMap(Vector2Int point)
    {
        findMap[point.x, point.y] = true;

        if (point.x - 1 > -1 && map[point.x - 1, point.y] != (int)TileType.Secret)
        {
            findMap[point.x - 1, point.y] = true;
        }
        if (point.x + 1 < size && map[point.x + 1, point.y] != (int)TileType.Secret)
        {
            findMap[point.x + 1, point.y] = true;
        }
        if (point.y + 1 < size && map[point.x, point.y + 1] != (int)TileType.Secret)
        {
            findMap[point.x, point.y + 1] = true;
        }
        if (point.y - 1 > -1 && map[point.x, point.y - 1] != (int)TileType.Secret)
        {
            findMap[point.x, point.y - 1] = true;
        }
    }
   

    private bool UpdateMap(Vector2Int point)
    {
        if (map[point.x, point.y] == -1 && tileCount < maxTile)
        {
            tileCount++;
        }

        bool leftOut = point.x - 1 > -1;
        bool rightOut = point.x + 1 < size;
        bool upOut = point.y + 1 < size;
        bool downOut = point.y - 1 > -1;
        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;
        if (point.x - 1 > -1)
        {
            left = map[point.x - 1, point.y] > -1;
        }
        if (point.x + 1 < size)
        {
            right = map[point.x + 1, point.y] > -1;
        }
        if (point.y + 1 < size)
        {
            up = map[point.x, point.y + 1] > -1;
        }
        if (point.y - 1 > -1)
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
            if (point.x - 2 > -1 && map[point.x - 2, point.y] > -1)
                map[point.x - 1, point.y] += 1;
            if (upOut && map[point.x - 1, point.y + 1] > -1)
                map[point.x - 1, point.y] += 1;
            if (downOut && map[point.x - 1, point.y - 1] > -1)
                map[point.x - 1, point.y] += 1;
        }

        if (right)
        {
            map[point.x + 1, point.y] = 1;
            if (point.x + 2 < size && map[point.x + 2, point.y] > -1)
                map[point.x + 1, point.y] += 1;
            if (upOut && map[point.x + 1, point.y + 1] > -1)
                map[point.x + 1, point.y] += 1;
            if (downOut && map[point.x + 1, point.y - 1] > -1)
                map[point.x + 1, point.y] += 1;
        }

        if (up)
        {
            map[point.x, point.y + 1] = 1;
            if (point.y + 2 < size && map[point.x, point.y + 2] > -1)
                map[point.x, point.y + 1] += 1;
            if (leftOut && map[point.x - 1, point.y + 1] > -1)
                map[point.x, point.y + 1] += 1;
            if (rightOut && map[point.x + 1, point.y + 1] > -1)
                map[point.x, point.y + 1] += 1;
        }

        if (down)
        {
            map[point.x, point.y - 1] = 1;
            if (point.y - 2 > -1 && map[point.x, point.y - 2] > -1)
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

        if (point.x - 1 > -1 && map[point.x - 1, point.y] > -1)
            connect++;
        if (point.x + 1 < size && map[point.x + 1, point.y] > -1)
            connect++;
        if (point.y + 1 < size && map[point.x, point.y + 1] > -1)
            connect++;
        if (point.y - 1 > -1 && map[point.x, point.y - 1] > -1)
            connect++;

        if (connect > 2)
        {
            if (point.x - 1 > -1 && map[point.x - 1, point.y] > 1)
                return false;
            if (point.x + 1 < size && map[point.x + 1, point.y] > 1)
                return false;
            if (point.y + 1 < size && map[point.x, point.y + 1] > 1)
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
            if (point.x + 1 < size && map[point.x + 1, point.y] > 2)
                return false;
            if (point.y + 1 < size && map[point.x, point.y + 1] > 2)
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
        if (point.x + 1 < size)
        {
            checkRight = CheckDerived3Connect2(new Vector2Int(point.x + 1, point.y));
        }
        if (point.y + 1 < size)
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
        bool leftOut = point.x - 1 > -1;
        bool rightOut = point.x + 1 < size;
        bool upOut = point.y + 1 < size;
        bool downOut = point.y - 1 > -1;

        int connect = 0;

        if (point.x - 1 > -1 && map[point.x - 1, point.y] > -1)
            connect++;
        if (point.x + 1 < size && map[point.x + 1, point.y] > -1)
            connect++;
        if (point.y + 1 < size && map[point.x, point.y + 1] > -1)
            connect++;
        if (point.y - 1 > -1 && map[point.x, point.y - 1] > -1)
            connect++;

        if (connect == 4)
        {
            int around = 1;
            if (leftOut && map[point.x - 1, point.y] > 2)
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
            if (rightOut && map[point.x + 1, point.y] > 2)
            {
                around++;
            }
            if (point.x + 2 < size && map[point.x + 2, point.y] > 3)
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
            if (point.y + 2 < size && map[point.x, point.y + 2] > 3)
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
            if (point.y - 2 > -1 && map[point.x, point.y - 2] > 3)
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
            if (point.x + 2 < size && map[point.x + 2, point.y] > 3)
            {
                around++;
            }
            if (point.x + 3 < size && map[point.x + 3, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (point.y + 2 < size && map[point.x, point.y + 2] > 3)
            {
                around++;
            }
            if (point.y + 3 < size && map[point.x, point.y + 3] > 3)
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
        if (point.x + 1 < size)
        {
            checkRight = CheckDerived4Connect44(new Vector2Int(point.x + 1, point.y));
        }
        if (point.y + 1 < size)
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
            && tileCount < maxTile
            && CheckConnect1(point)
            && CheckConnect2(point))
            return true;
        return false;
    }

    private void PickPath(Vector2Int point)
    {
        direction = Direction.None;
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
            if (point.x + 1 < size && CheckArea(new Vector2Int(point.x + 1, point.y)))
            {
                right = true;
            }
            // up
            if (point.y + 1 < size && CheckArea(new Vector2Int(point.x, point.y + 1)))
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
        if (tileCount >= maxTile)
            return;

        switch (direction)
        {
            case Direction.Left:
                if (position.x - 1 > -1 && map[position.x - 1, position.y] > -1)
                {
                    turn++;
                    position.x -= 1;
                    if (UpdateMap(position) && CheckArea(new Vector2Int(position.x, position.y)))
                        PickPath(new Vector2Int(position.x, position.y));
                }
                else
                {
                    direction = Direction.None;
                }
                break;
            case Direction.Right:
                if (position.x + 1 < size && map[position.x + 1, position.y] > -1)
                {
                    turn++;
                    position.x += 1;
                    if (UpdateMap(position) && CheckArea(new Vector2Int(position.x, position.y)))
                        PickPath(new Vector2Int(position.x, position.y));
                }
                else
                {
                    direction = Direction.None;
                }
                break;
            case Direction.Up:
                if (position.y + 1 < size && map[position.x, position.y + 1] > -1)
                {
                    turn++;
                    position.y += 1;
                    if (UpdateMap(position) && CheckArea(new Vector2Int(position.x, position.y)))
                        PickPath(new Vector2Int(position.x, position.y));
                }
                else
                {
                    direction = Direction.None;
                }
                break;
            case Direction.Down:
                if (position.y - 1 > -1 && map[position.x, position.y - 1] > -1)
                {
                    turn++;
                    position.y -= 1;
                    if (UpdateMap(position) && CheckArea(new Vector2Int(position.x, position.y)))
                        PickPath(new Vector2Int(position.x, position.y));
                }
                else
                {
                    direction = Direction.None;
                }
                break;
        }

        if (direction != Direction.None)
        {
            if (position.x - 1 > -1 && UpdateMap(position) && CheckArea(new Vector2Int(position.x - 1, position.y)))
                PickPath(new Vector2Int(position.x - 1, position.y));
            if (position.x + 1 < size && UpdateMap(position) && CheckArea(new Vector2Int(position.x + 1, position.y)))
                PickPath(new Vector2Int(position.x + 1, position.y));
            if (position.y + 1 < size && UpdateMap(position) && CheckArea(new Vector2Int(position.x, position.y + 1)))
                PickPath(new Vector2Int(position.x, position.y + 1));
            if (position.y - 1 > -1 && UpdateMap(position) && CheckArea(new Vector2Int(position.x,position.y - 1)))
                PickPath(new Vector2Int(position.x, position.y - 1));
        }

        UpdateFindMap(position);
    }

    public void CreateMap()
    {
        access = false;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                map[i, j] = -1;
                findMap[i, j] = false;
            }
        }

        tileCount = 0;
        turn = 0;
        direction = Direction.None;
        maxTile = Random.Range(min, max);

        // 중앙 행열 안에서 시작점을 랜덤 선택
        startPosition = new Vector2Int(Random.Range(6, 9), Random.Range(6, 9));
        position = startPosition;
        map[startPosition.x, startPosition.y] = 0;
        tileCount++;

        PickPath(position);
        StartCoroutine(SetDirection());
    }

    private IEnumerator SetDirection()
    {
        while (tileCount < maxTile)
        {
            yield return null;
            if (turn > 900)
            {
                break;
            }

            switch (Random.Range(0, 4))
            {
                case 0:
                    direction = Direction.Left;
                    MoveDirection();
                    break;
                case 1:
                    direction = Direction.Right;
                    MoveDirection();
                    break;
                case 2:
                    direction = Direction.Up;
                    MoveDirection();
                    break;
                case 3:
                    direction = Direction.Down;
                    MoveDirection();
                    break;
            }
        }

        if (tileCount < min)
        {
            CreateMap();
        }
        else
        {
            CheckTileCase();
        }
    }


    private bool CreateSecretTile()
    {
        for (int k = 4; k > 0; k--)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
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
        if (point.x - 2 > -1 && map[point.x - 2, point.y] == 0)
            return false;
        if (point.x + 2 < size && map[point.x + 2, point.y] == 0)
            return false;
        if (point.y + 2 < size && map[point.x, point.y + 2] == 0)
            return false;
        if (point.y - 2 > -1 && map[point.x, point.y - 2] == 0)
            return false;

        bool left = point.x - 1 > -1;
        bool right = point.x + 1 < size;
        bool up = point.y + 1 < size;
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
        bool right = point.x + 1 < size;
        bool up = point.y + 1 < size;
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
        if (point.x - 1 > -1 && point.x + 1 < size && point.y + 1 < size && point.y - 1 > -1)
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

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
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

        while ((tileCount < 66 && secretCount < 2) || (tileCount > 65 && secretCount < 3))
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

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (map[i, j] == 1)
                {
                    int distance = minDistance(new Vector2Int(i, j), startPosition);
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
        else if ((tileCount < 71 && exitCount < 3) || (tileCount > 70 && exitCount < 4))
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

        if (tileCount > 59 && exitCase.Count > 0)
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
        bool rightIn = point.x + 1 < size;
        bool upIn = point.y + 1 < size;
        bool downIn = point.y - 1 > -1;

        if (leftIn && map[point.x - 1, point.y] == type)
        {
            if (!CheckContinue3(new Vector2Int(point.x - 1, point.y), type))
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
        bool rightIn = point.x + 1 < size;
        bool upIn = point.y + 1 < size;
        bool downIn = point.y - 1 > -1;

        if (leftIn && map[point.x - 1, point.y] == type)
        {
            if (upIn && map[point.x - 1, point.y + 1] == type)
                return false;
            if (downIn && map[point.x - 1, point.y - 1] == type)
                return false;
        }
        if (rightIn && map[point.x + 1, point.y] == type)
        {
            if (upIn && map[point.x + 1, point.y + 1] == type)
                return false;
            if (downIn && map[point.x + 1, point.y - 1] == type)
                return false;
        }
        if (upIn && map[point.x, point.y + 1] == type)
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
        for (int i = 1; i < length; i++)
        {
            if (point.x - i > -1 && map[point.x - i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.x + i < size && map[point.x + i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y + i < size && map[point.x, point.y + i] == type)
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

        for (int i = -length / 2; i < end; i++)
        {
            if (point.x + i > -1 && point.x + i < size && map[point.x + i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = -length / 2; i < end; i++)
        {
            if (point.y + i > -1 && point.y + i < size && map[point.x, point.y + i] == type)
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
        if (tileCount < 60)
            eventTile = Random.Range(12, 15);
        else if (tileCount < 66)
            eventTile = Random.Range(13, 17);
        else if (tileCount < 71)
            eventTile = Random.Range(15, 19);
        else
            eventTile = Random.Range(17, 21);

        for (int k = 0; k < 5; k++)
        {
            if (count >= eventTile)
                break;
            for (int i = 0; i < size; i++)
            {
                if (count >= eventTile)
                    break;
                for (int j = 0; j < size; j++)
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
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
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
        if (tileCount < 60)
            chest = Random.Range(4, 9);
        else if (tileCount < 66)
            chest = Random.Range(5, 10);
        else if (tileCount < 71)
            chest = Random.Range(6, 11);
        else
            chest = Random.Range(7, 12);


        for (int k = 0; k < 5; k++)
        {
            if (count >= chest)
                break;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
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
        if (tileCount < 60)
            monster = Random.Range(17, 21);
        else if (tileCount < 66)
            monster = Random.Range(19, 23);
        else if (tileCount < 71)
            monster = Random.Range(21, 25);
        else
            monster = Random.Range(23, 27);

        for (int k = 0; k < 5; k++)
        {
            if (count >= monster)
                break;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
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
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    findMap[i, j] = false;
                    knownMap[i, j] = false;
                    blownupMap[i, j] = false;
                }
            }

            UpdateFindMap(startPosition);
            position = startPosition;
            goneMap[startPosition.x, startPosition.y] = true;
            knownMap[startPosition.x, startPosition.y] = true;
            direction = Direction.None;

            access = true;
        }
    }
    #endregion

    #region Exploration
    public bool MovePlayer(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                if (position.x - 1 > -1 && map[position.x - 1, position.y] > -1)
                {
                    if (map[position.x - 1, position.y] == (int)TileType.Secret)
                    {
                        if (!blownupMap[position.x, position.y])
                            return false;
                        else
                        {
                            if (position.x - 2 > -1)
                                blownupMap[position.x - 2, position.y] = true;
                            if (position.y + 1 < size)
                                blownupMap[position.x - 1, position.y + 1] = true;
                            if (position.y - 1 > -1)
                                blownupMap[position.x - 1, position.y - 1] = true;
                        }
                    }
                    position.x -= 1;
                    UpdateFindMap(position);
                    knownMap[position.x, position.y] = true;
                    return true;
                }
                else
                {
                    return false;
                }
            case Direction.Right:
                if (position.x + 1 < size && map[position.x + 1, position.y] > -1)
                {
                    if (map[position.x + 1, position.y] == (int)TileType.Secret)
                    {
                        if (!blownupMap[position.x, position.y])
                            return false;
                        else
                        {
                            if (position.x + 2 < size)
                                blownupMap[position.x + 2, position.y] = true;
                            if (position.y + 1 < size)
                                blownupMap[position.x + 1, position.y + 1] = true;
                            if (position.y - 1 > -1)
                                blownupMap[position.x + 1, position.y - 1] = true;
                        }
                    }
                    position.x += 1;
                    UpdateFindMap(position);
                    knownMap[position.x, position.y] = true;
                    return true;
                }
                else
                {
                    return false;
                }
            case Direction.Up:
                if (position.y + 1 < size && map[position.x, position.y + 1] > -1)
                {
                    if (map[position.x, position.y + 1] == (int)TileType.Secret)
                    {
                        if (!blownupMap[position.x, position.y])
                            return false;
                        else
                        {
                            if (position.x - 1 > -1)
                                blownupMap[position.x - 1, position.y + 1] = true;
                            if (position.x + 1 < size)
                                blownupMap[position.x + 1, position.y + 1] = true;
                            if (position.y + 2 < size)
                                blownupMap[position.x, position.y + 2] = true;
                        }
                    }
                    position.y += 1;
                    UpdateFindMap(position);
                    knownMap[position.x, position.y] = true;
                    return true;
                }
                else
                {
                    return false;
                }
            case Direction.Down:
                if (position.y - 1 > -1 && map[position.x, position.y - 1] > -1)
                {
                    if (map[position.x, position.y - 1] == (int)TileType.Secret)
                    {
                        if (!blownupMap[position.x, position.y])
                            return false;
                        else
                        {
                            if (position.x - 1 > -1)
                                blownupMap[position.x - 1, position.y - 1] = true;
                            if (position.x + 1 < size)
                                blownupMap[position.x + 1, position.y - 1] = true;
                            if (position.y - 2 > -1)
                                blownupMap[position.x, position.y - 2] = true;
                        }
                    }
                    position.y -= 1;
                    UpdateFindMap(position);
                    knownMap[position.x, position.y] = true;
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                return false;
        }
    }
    public void UpdateGoneMap()
    {
        goneMap[position.x, position.y] = true;
    }
    public void UpdateKnownMap() // Reconnaissance scusses
    {
        if(position.x - 1 > -1)
            knownMap[position.x - 1, position.y] = true;
        if(position.x + 1 < size)
            knownMap[position.x + 1, position.y] = true;
        if(position.y + 1 < size)
            knownMap[position.x, position.y + 1] = true;
        if (position.y - 1 > -1)
            knownMap[position.x, position.y - 1] = true;
    }

    public void Search()
    {
        blownupMap[position.x, position.y] = true; // todo: 지우기

        if (position.x - 1 > -1)
        {
            findMap[position.x - 1, position.y] = true;
        }
        if (position.x + 1 < size)
        {
            findMap[position.x + 1, position.y] = true;
        }
        if (position.y + 1 < size)
        {
            findMap[position.x, position.y + 1] = true;
        }
        if (position.y - 1 > -1)
        {
            findMap[position.x, position.y - 1] = true;
        }
    }

    public void BlownUp()
    {
        blownupMap[position.x, position.y] = true;
    }
    #endregion


    #region LoadData
    public void Copy(MapData data)
    {
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                map[i,j] = data.map[i,j];
                findMap[i,j] = data.findMap[i,j];
                knownMap[i,j] = data.knownMap[i,j];
                goneMap[i,j] = data.goneMap[i,j];
                blownupMap[i,j] = data.blownupMap[i,j];
            }
        }

        turn = data.turn;
        position = data.position;
        direction = data.direction;
        startPosition = data.startPosition;
        tileCount = data.tileCount;
        maxTile = data.maxTile;
    }

    #endregion
}
