using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Random = System.Random;

public class SnakeFindWayUseBackTracking : MonoBehaviour
{
    private int[,] grid = {
     {1, 1 ,  1, 1, 1, 1, 1, 1,  1 , 1},
     {1, 1 ,  1, 1, 1, 1, 1, 1,  1 , 1},
     {1,004, 22,23, 0,31,32,33, 005, 1},
     {1, 1 , 21, 1, 1, 1, 1,34,  1 , 1},
     {1, 1 ,  0, 1, 1, 1, 1, 0,  1 , 1},
     {1, 1 , 51, 1, 1, 1, 1, 0,  1 , 1},
     {1, 1 , 52, 1, 1, 1, 1,44,  1 , 1},
     {1,003, 53,54, 0,41,42,43, 002, 1},
     {1, 1 ,  1, 1, 1, 1, 1, 1,  1 , 1},
     {1, 1 ,  1, 1, 1, 1, 1, 1,  1 , 1}
    };
    [Serializable]
    public struct Snake
    {
        public List<int[]> allTile;
        public int snakeID;

        public Snake(List<int[]> allTile, int snakeID)
        {
            this.allTile = allTile;
            this.snakeID = snakeID;
        }
    }
    private int ROWS;
    private int COLS;
    
    private Dictionary<int, Snake> allIdOfSnakes = new Dictionary<int, Snake>();
    private List<Snake> allSnakes;
    public int SPEED = 1;
    public void Start()
    {
        ROWS = grid.GetLength(0);
        COLS = grid.GetLength(1);
        LoadSnakeFromMatrix();
    }
    void LoadSnakeFromMatrix()
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLS; column++)
            {
                if (grid[row, column] != 0 && grid[row, column] != 1 && grid[row, column].ToString().Length ==2)
                {
                    int id = Int32.Parse(grid[row, column].ToString()[0].ToString());
                    if (allIdOfSnakes.ContainsKey(id))
                    {
                        allIdOfSnakes[id ].allTile.Add(new int[] {row,column});
                    }
                    else
                    {
                        List<int[]> allTile = new List<int[]>();
                        var tile = new int[] { row, column };
                        allTile.Add(tile);
                        
                        allIdOfSnakes[id] = new Snake(allTile,id);
                    }
                }
            }
        }

        foreach (var snake in allIdOfSnakes)
        {
            List<int[]>  temp = new List<int[]>();

            var enum1 = from aTile in snake.Value.allTile
                orderby Int32.Parse(grid[aTile[0], aTile[1]].ToString()[1].ToString()) // sort
                select aTile;
            
            foreach (var e in enum1)
            {
                grid[e[0], e[1]] =  snake.Key; // delete number marked
                temp.Add(e);
            }
            allSnakes.Add(new Snake(temp,snake.Key));
        }
      
    }
    static List<T> ShuffleList<T>(List<T> list)
    {
        Random random = new Random();
        List<T> newList = list.ToList(); // Tạo một bản sao của danh sách gốc
        int n = newList.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = newList[k];
            newList[k] = newList[n];
            newList[n] = value;
        }
        return newList;
    }

    public bool CheckCanMoveWithThisDirections(int row, int col)
    {
        if (grid[row, col] != 0)
        {
            return false;
        }
        return true;
    }
    public bool SolvePuzzle(int[,] grid)
    {
        //DrawGrid(30); // 30 is the width of each square on the grid
        Thread.Sleep((int)(1000 * (1 - SPEED)));
        List<Snake> shuffledAllSnake = ShuffleList(allSnakes);
        foreach (var snake in shuffledAllSnake)
        {
            for (int i = 1; i <= 2; i++)
            {
                List<string> directions = new List<string>();
                int index = 0;
                bool head = true;
                switch (i)
                {
                    case 1:
                        index = 0;
                        head = true;
                        break;
                    case 2:
                        index = snake.allTile.Count;
                        head = false; 
                        break;
                }
                if (CheckCanMoveWithThisDirections(snake.allTile[index][0], snake.allTile[index][1] + 1))
                {
                    directions.Add("right");       
                }
                if (CheckCanMoveWithThisDirections(snake.allTile[index][0], snake.allTile[index][1] - 1))
                {
                    directions.Add("left");       
                }
                if (CheckCanMoveWithThisDirections(snake.allTile[index][0] + 1, snake.allTile[index][1]))
                {
                    directions.Add("down");       
                }
                if (CheckCanMoveWithThisDirections(snake.allTile[index][0] - 1, snake.allTile[index][1]))
                {
                    directions.Add("up");       
                }
                 if (directions.Count == 0)
                    return false;

                foreach (string direction in directions)
                {
                    switch (direction)
                    {
                        case "right":
                            int x = snake.allTile[index][0];
                            int y = snake.allTile[index][1] + 1;
                            if (head)
                            {
                                for (int j = 0; j < snake.allTile.Count; j++)
                                {
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                    var tempx = snake.allTile[j][0];
                                    var tempy = snake.allTile[j][1];
                                    snake.allTile[j] = new int[] { x, y };
                                    x = tempx;
                                    y = tempy;
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                }    
                            }
                            else
                            {
                                for (int j = snake.allTile.Count -1; j >= 0; j--)
                                {
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                    var tempx = snake.allTile[j][0];
                                    var tempy = snake.allTile[j][1];
                                    snake.allTile[j] =  new int[] { x, y };
                                    x = tempx;
                                    y = tempy;
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                }   
                            }
                            if (SolvePuzzle(grid))
                                return true;
                            else
                            {
                                 x = snake.allTile[index][0];
                                 y = snake.allTile[index][1] - 1;
                                if (head)
                                {
                                    for (int j = 0; j < snake.allTile.Count; j++)
                                    {
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                        var tempx = snake.allTile[j][0];
                                        var tempy = snake.allTile[j][1];
                                        snake.allTile[j] = new int[] { x, y };
                                        x = tempx;
                                        y = tempy;
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                    }    
                                }
                                else
                                {
                                    for (int j = snake.allTile.Count -1; j >= 0; j--)
                                    {
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                        var tempx = snake.allTile[j][0];
                                        var tempy = snake.allTile[j][1];
                                        snake.allTile[j] =  new int[] { x, y };
                                        x = tempx;
                                        y = tempy;
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                    }   
                                }
                            }
                            break;
                        case "left":
                            x = snake.allTile[index][0];
                            y = snake.allTile[index][1] - 1;
                            if (head)
                            {
                                for (int j = 0; j < snake.allTile.Count; j++)
                                {
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                    var tempx = snake.allTile[j][0];
                                    var tempy = snake.allTile[j][1];
                                    snake.allTile[j] = new int[] { x, y };
                                    x = tempx;
                                    y = tempy;
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                }    
                            }
                            else
                            {
                                for (int j = snake.allTile.Count -1; j >= 0; j--)
                                {
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                    var tempx = snake.allTile[j][0];
                                    var tempy = snake.allTile[j][1];
                                    snake.allTile[j] =  new int[] { x, y };
                                    x = tempx;
                                    y = tempy;
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                }   
                            }
                            if (SolvePuzzle(grid))
                                return true;
                            else
                            {
                                 x = snake.allTile[index][0];
                                 y = snake.allTile[index][1] + 1;
                                if (head)
                                {
                                    for (int j = 0; j < snake.allTile.Count; j++)
                                    {
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                        var tempx = snake.allTile[j][0];
                                        var tempy = snake.allTile[j][1];
                                        snake.allTile[j] = new int[] { x, y };
                                        x = tempx;
                                        y = tempy;
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                    }    
                                }
                                else
                                {
                                    for (int j = snake.allTile.Count -1; j >= 0; j--)
                                    {
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                        var tempx = snake.allTile[j][0];
                                        var tempy = snake.allTile[j][1];
                                        snake.allTile[j] =  new int[] { x, y };
                                        x = tempx;
                                        y = tempy;
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                    }   
                                }
                            }
                            break;
                        case "up":
                            x = snake.allTile[index][0] -1;
                            y = snake.allTile[index][1] ;
                            if (head)
                            {
                                for (int j = 0; j < snake.allTile.Count; j++)
                                {
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                    var tempx = snake.allTile[j][0];
                                    var tempy = snake.allTile[j][1];
                                    snake.allTile[j] = new int[] { x, y };
                                    x = tempx;
                                    y = tempy;
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                }    
                            }
                            else
                            {
                                for (int j = snake.allTile.Count -1; j >= 0; j--)
                                {
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                    var tempx = snake.allTile[j][0];
                                    var tempy = snake.allTile[j][1];
                                    snake.allTile[j] =  new int[] { x, y };
                                    x = tempx;
                                    y = tempy;
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                }   
                            }
                            if (SolvePuzzle(grid))
                                return true;
                            else
                            {
                                 x = snake.allTile[index][0] + 1;
                                 y = snake.allTile[index][1];
                                if (head)
                                {
                                    for (int j = 0; j < snake.allTile.Count; j++)
                                    {
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                        var tempx = snake.allTile[j][0];
                                        var tempy = snake.allTile[j][1];
                                        snake.allTile[j] = new int[] { x, y };
                                        x = tempx;
                                        y = tempy;
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                    }    
                                }
                                else
                                {
                                    for (int j = snake.allTile.Count -1; j >= 0; j--)
                                    {
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                        var tempx = snake.allTile[j][0];
                                        var tempy = snake.allTile[j][1];
                                        snake.allTile[j] =  new int[] { x, y };
                                        x = tempx;
                                        y = tempy;
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                    }   
                                }
                            }
                            break;
                        case "down":
                            x = snake.allTile[index][0] + 1;
                            y = snake.allTile[index][1] ;
                            if (head)
                            {
                                for (int j = 0; j < snake.allTile.Count; j++)
                                {
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                    var tempx = snake.allTile[j][0];
                                    var tempy = snake.allTile[j][1];
                                    snake.allTile[j] = new int[] { x, y };
                                    x = tempx;
                                    y = tempy;
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                }    
                            }
                            else
                            {
                                for (int j = snake.allTile.Count -1; j >= 0; j--)
                                {
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                    var tempx = snake.allTile[j][0];
                                    var tempy = snake.allTile[j][1];
                                    snake.allTile[j] =  new int[] { x, y };
                                    x = tempx;
                                    y = tempy;
                                    grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                }   
                            }
                            if (SolvePuzzle(grid))
                                return true;
                            else
                            {
                                 x = snake.allTile[index][0] -1;
                                 y = snake.allTile[index][1];
                                if (head)
                                {
                                    for (int j = 0; j < snake.allTile.Count; j++)
                                    {
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                        var tempx = snake.allTile[j][0];
                                        var tempy = snake.allTile[j][1];
                                        snake.allTile[j] = new int[] { x, y };
                                        x = tempx;
                                        y = tempy;
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                    }    
                                }
                                else
                                {
                                    for (int j = snake.allTile.Count -1; j >= 0; j--)
                                    {
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = 0;
                                        var tempx = snake.allTile[j][0];
                                        var tempy = snake.allTile[j][1];
                                        snake.allTile[j] =  new int[] { x, y };
                                        x = tempx;
                                        y = tempy;
                                        grid[snake.allTile[j][0], snake.allTile[j][1]] = snake.snakeID;
                                    }   
                                }
                            }
                            break;
                    }
                }
            }
        }
        return false;
    }

    public bool Solved(int[,] grid)
    {
        for (int row = 1; row < ROWS - 1; row++)
        {
            for (int column = 1; column < COLS - 1; column++)
            {
                if (grid[row, column] != 0 || grid[row, column] != 1)
                {
                    return false;
                }
            }
        }

        return true;
    }
    void Log() //Khong quan trong
    {
        foreach (var snake in allSnakes)
        {
            Debug.Log($"Snake ID: {snake.snakeID}");
            foreach (var tile in snake.allTile)
            {
                Debug.Log($"  Tile: ({tile[0]}, {tile[1]})");
            }
        }
    }
   
    
}
