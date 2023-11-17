using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeFindWay : MonoBehaviour
{
    [Serializable]
    public struct aTile
    {
        public int x;
        public int y;

        public aTile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void ChangValue(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    [Serializable]
    public struct aSnake
    {
        public List<aTile> tiles;

        public aSnake(List<aTile> tiles)
        {
            this.tiles = new List<aTile>(tiles);
        }
        public void SnakeMove(bool head, int x, int y)
        {
            if (head)
            {
                for (int i = 1; i < tiles.Count; i++)
                {
                    var tempx = tiles[i].x;
                    var tempy = tiles[i].y;
                    tiles[i].ChangValue(x,y);
                    x = tempx;
                    y = tempy;
                }    
            }
            else
            {
                for (int i = tiles.Count -1; i >= 0; i--)
                {
                    var tempx = tiles[i].x;
                    var tempy = tiles[i].y;
                    tiles[i].ChangValue(x,y);
                    x = tempx;
                    y = tempy;
                }   
            }
            
        }
    }

    public List<aSnake> Snakes = new List<aSnake>();
    [SerializeField] private int[,] number;
    public List<int> allIdOfSnake = new List<int>();
    void Start()
    {
        number = new int[10, 10]
        {
            { 1 ,  1,  0,  3, 3 ,   -1, -1, -1, -1, -1 },
            { 1 , -1, -1, -1, 3 ,   -1, -1, -1, -1, -1 },
            { 0 , -1, -1, -1, 3 ,   -1, -1, -1, -1, -1 },
            { 0 , -1, -1, -1, 4 ,   -1, -1, -1, -1, -1 },
            { 2 ,  2,  2,  4, 4 ,   -1, -1, -1, -1, -1 },
            
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        };
        LoadSnakeIDFromMatrix();
    }

    void LoadSnakeIDFromMatrix()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (number[i, j] != 0 && number[i, j] != -1)
                {
                    allIdOfSnake.Add(number[i, j]);
                }
            }
        }
        allIdOfSnake =  allIdOfSnake.Distinct().ToList();
        LoadSnakeByIDFromMatrix();

    }

    public void LoadSnakeByIDFromMatrix()
    {
        for (int k = 0; k < allIdOfSnake.Count; k++)
        {
            List<aTile> tiles = new List<aTile>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (number[i, j] == allIdOfSnake[k])
                    {
                        var aTile = new aTile(i, j);
                        tiles.Add(aTile);
                    }
                }
            }
            var aSnake = new aSnake(tiles);
            Snakes.Add(aSnake);
        }
    }
    
    // public void SetSnakeDate()
    // {
    //     for (int i = 0; i < lengthOfSnake.Count; i++)
    //     {
    //         for (int j = 0; j < 10; j++)
    //         {
    //             for (int k = 0; k < 10; k++)
    //             {
    //                 if (number[j, k].ToString()[0].ToString() == (i+1).ToString())
    //                 {
    //                     number[j, k] = i + 1;
    //                     CreateSnake(i+1);
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // public void CreateSnake(int SnakeID)
    // {
    //     
    // }
    // public void CheckDirectionCanMove(int x, int y, int currentX, int currentY, int value)
    // {
    //     var _x = currentX + x;
    //     var _y = currentY + y;
    //     try
    //     {
    //         if (number[_x, _y] != -1)
    //         {
    //             if (number[_x, _y] == 0  || number[_x, _y]  == value)
    //             {
    //                 
    //             }
    //         }
    //         
    //     }
    //     catch
    //     {
    //
    //     }
    // }
}
