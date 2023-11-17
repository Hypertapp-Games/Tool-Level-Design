using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

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
        public int value;

        public aSnake(List<aTile> tiles , int value)
        {
            this.tiles = new List<aTile>(tiles);
            this.value = value;
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
            { 0,  0,  0 , 0, 0 ,   -1, -1, -1, -1, -1 },
            { 0, -1 , -1 , -1, 0 ,   -1, -1, -1, -1, -1 },
            { 0 , -1 , -1 , -1, 0 ,   -1, -1, -1, -1, -1 },
            { 0 , -1 , -1 , -1, 0 ,   -1, -1, -1, -1, -1 },
            { 21,  22,  23, 0, 0 ,   -1, -1, -1, -1, -1 },
            
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
                    allIdOfSnake.Add(Int32.Parse(number[i, j].ToString()[0].ToString()));
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
            int value = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    try
                    {
                        var snakeID = Int32.Parse(number[i, j].ToString()[0].ToString());
                        if (snakeID == allIdOfSnake[k] )
                        {
                            tiles.Add(new aTile(i, j));
                            number[i, j] = allIdOfSnake[k];
                            value = allIdOfSnake[k];
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            List<aTile> temp = new List<aTile>();
            var enum1 = from  _aTile  in tiles
                orderby Int32.Parse(number[_aTile.x, _aTile.y].ToString()[1].ToString())
                select _aTile;
          
            foreach (var e in enum1)
            {
                temp.Add(e);
                Debug.Log("this do");

            }
            tiles.Clear();
            tiles = new List<aTile>(temp);
            var aSnake = new aSnake(tiles , value);
            Snakes.Add(aSnake);
        }

        SnakeRandomDirectionMove();//TEST 1 SNAKE
    }

    public void SnakeRandomDirectionMove()
    {
        var snakeTest = Snakes[0];
        CheckDirectionInAStep(snakeTest);

    }

    public void CheckDirectionInAStep(aSnake snakeTest)
    {
        List<int> stt = new List<int>();
        if(CheckDirectionCanMove(1, 0, snakeTest.tiles[0].x, snakeTest.tiles[0].y, snakeTest.value))
        {
            stt.Add(1);
        }

        if (CheckDirectionCanMove(-1, 0, snakeTest.tiles[0].x, snakeTest.tiles[0].y, snakeTest.value))
        {
            stt.Add(2);
        }

        if (CheckDirectionCanMove(0, 1, snakeTest.tiles[0].x, snakeTest.tiles[0].y, snakeTest.value))
        {
            stt.Add(3);
        }

        if (CheckDirectionCanMove(0, -1, snakeTest.tiles[0].x, snakeTest.tiles[0].y, snakeTest.value))
        {
            stt.Add(4);
        }

        if (stt.Count > 0)
        {
            int a = Random.Range(0, stt.Count);
            if(stt[a] == 1)
            {
                SnakeMove(1, 0, snakeTest);
            }
            else if(stt[a] == 2)
            {
                SnakeMove(-1, 0, snakeTest);
            }
            else if(stt[a] == 3)
            {
                SnakeMove(0, 1, snakeTest);
            }
            else if(stt[a] == 4)
            {
                SnakeMove(0, -1, snakeTest);
            }
            
        }

    }

    public void SnakeMove(int x, int y, aSnake snakeTest)
    {
        var _x = snakeTest.tiles[0].x + x;
        var _y = snakeTest.tiles[0].y + y;
        try
        {
            if (number[_x, _y] != -1)
            {
                if (number[_x, _y] == 0 )
                {
                    snakeTest.SnakeMove(true,_x, _y);
                }
            }
            else
            {
                Debug.Log("Da cham");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Da cham");
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
    public bool CheckDirectionCanMove(int x, int y, int currentX, int currentY, int value)
    {
        bool right = false;
        var _x = currentX + x;
        var _y = currentY + y;
        try
        {
            if (number[_x, _y] != -1)
            {
                if (number[_x, _y] == 0 )
                {
                    right = true;
                }
            }
            
        }
        catch
        {
            
        }

        return right;
    }
}
