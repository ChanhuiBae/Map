using System.Collections.Generic;
public struct BagItem
{
    public int id;
    public string name;
    public string description;
    public int row;
    public int column;
    public bool rotation;
}
public class Bag
{
    public static int row = 11;
    public static int column = 11;
    public int[,] space;
    public Dictionary<int,BagItem> items;
    private int idCounter;

    public Bag() 
    { 
        space = new int[row, column];
        items = new Dictionary<int,BagItem>();
        idCounter = 0;
        for(int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                space[i, j] = 0;
            }
        }
    }

    private int GetNewID()
    {
        return ++idCounter;
    }

    public bool TryTakeOut(int x, int y, out BagItem item)
    {
        if (space[x, y] != 0)
        {
            if(items.TryGetValue(space[x, y], out item))
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        if (space[i, j] == item.id)
                        {
                            space[i, j] = 0;
                        }
                    }
                }
                items.Remove(item.id);
                return true;
            }
        }
        item = new BagItem();
        item.id = 0;
        return false;
    }

    public bool TryPutItem(int x, int y, BagItem item)
    {
        int size = item.row * item.column;
        int count = 0;
        if (item.rotation)
        {
            for(int i = x; i < x + item.column && i < row; i++)
            {
                for(int j = y; j < y + item.row && j < column; j++)
                {
                    if (space[i,j] == 0)
                        count++;
                }
            }
            if(count == size)
            {
                item.id = GetNewID();
                items.Add(item.id, item);
                for (int i = x; i < x + item.column; i++)
                {
                    for (int j = y; j < y + item.row; j++)
                    {
                        space[i, j] = item.id;
                    }
                }
                return true;
            }
        }
        else
        {
            for (int i = x; i < x + item.row && i < row; i++)
            {
                for (int j = y; j < y + item.column && j < column; j++)
                {
                    if (space[i, j] == 0)
                        count++;
                }
            }
            if (count == size)
            {
                item.id = GetNewID();
                items.Add(item.id, item);
                for (int i = x; i < x + item.row; i++)
                {
                    for (int j = y; j < y + item.column; j++)
                    {
                        space[i, j] = item.id;
                    }
                }
                return true;
            }
        }
        return false;
    }

    public bool TryOutoPutItem(BagItem item)
    {
        int size = item.row * item.column;
        for(int i = 0; i < row - item.row; i++)
        {
            for(int j = 0; j < column - item.column; j++)
            {
                if (space[i,j] == 0)
                {
                    int count = 0;
                    for (int x = 0; x < item.row; x++)
                    {
                        for (int y = 0; y < item.column; y++)
                        {
                            if (space[i+x, j+y] == 0)
                                count++;
                        }
                    }
                    if(count == size)
                    {
                        item.id = GetNewID();
                        items.Add(item.id, item);

                        for (int x = 0; x < item.row; x++)
                        {
                            for (int y = 0; y < item.column; y++)
                            {
                                space[i + x, j + y] = item.id;
                            }
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
