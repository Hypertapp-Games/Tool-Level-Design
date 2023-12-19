using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class SnakeFindWayUseBackTracking : MonoBehaviour
{
    private int[,] grid = {
     {1, 1 ,  1, 1, 1, 1, 1, 1,  1 , 1},
     {1, 1 ,  1, 1, 1, 1, 1, 1,  1 , 1},
     {1,004, 22,21, 0,31,32,33, 005, 1},
     {1, 1 , 23, 1, 0, 1, 1,34,  1 , 1},
     {1, 1 ,  0, 1, 0, 1, 1, 1,  1 , 1},
     {1, 1 , 51, 1, 0, 1, 1, 1,  1 , 1},
     {1, 1 , 52, 1, 0, 1, 1,44,  1 , 1},
     {1,003, 53,54, 0,41,42,43, 002, 1},
     {1, 1 ,  1, 1, 1, 1, 1, 1,  1 , 1},
     {1, 1 ,  1, 1, 1, 1, 1, 1,  1 , 1}
    };
    [Serializable]
    public struct Snake
    {
        public List<int[]> allTile;
        public int[] hole;
        public int snakeID;

        public Snake(List<int[]> allTile, int[] hole, int snakeID)
        {
            this.allTile = allTile;
            this.hole = hole;
            this.snakeID = snakeID;
        }
    }
    private int ROWS;
    private int COLS;
    
    private Dictionary<int, Snake> allIdOfSnakes = new Dictionary<int, Snake>();
    private List<Snake> allSnakes;
    public float SPEED = 1;
    public void Start()
    {
        allSnakes = new List<Snake>();
        ROWS = grid.GetLength(0);
        COLS = grid.GetLength(1);
        LoadSnakeFromMatrix();
        GenerateTileByMatrix();
        SolvePuzzle(grid);
    }
    void LoadSnakeFromMatrix()
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLS; column++)
            {
                if (grid[row, column] != 0 && grid[row, column] != 1)
                {
                    string cellValue = grid[row, column].ToString();

                    if (cellValue.Length == 2) // thêm list tile vào danh sách
                    {
                        int id = Int32.Parse(cellValue[0].ToString());

                        if (!allIdOfSnakes.TryAdd(id, new Snake(new List<int[]> { new int[] { row, column } }, new int[] { }, id)))
                        {
                            allIdOfSnakes[id].allTile.Add(new int[] { row, column });
                        }
                    }
                    else if (cellValue.Length == 3) // thêm hole vào danh sách
                    {
                        int id = Int32.Parse(cellValue[2].ToString());

                        if (!allIdOfSnakes.TryAdd(id, new Snake(new List<int[]> { }, new int[] { row, column }, id)))
                        {
                            allIdOfSnakes[id].hole[0] = row;
                            allIdOfSnakes[id].hole[1] = column;
                        }
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
            allSnakes.Add(new Snake(temp,snake.Value.hole,snake.Key));
        }
      
    }
    public bool CheckCanMoveWithThisDirections(int row, int col , Snake snake)
    {
        if (grid[row, col] == 0 || grid[row, col] == grid[snake.hole[0], snake.hole[1]] )
        {
            return true;
        }
        return false;
    }
    public bool CheckSnakeFindHole(int row, int col, Snake snake)
    {
        if (grid[row, col].ToString().Length == 3)
        {
            if (Int32.Parse(grid[row, col].ToString()[2].ToString()) == snake.snakeID)
            {
                for (int i = 0; i < snake.allTile.Count; i++)
                {
                    var _row = snake.allTile[i][0];
                    var _cols = snake.allTile[i][1];
                    grid[_row, _cols] = 0;
                }

                grid[snake.hole[0], snake.hole[1]] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public bool SolvePuzzle(int[,] grid)
    {
        DrawGird();

        //List<Snake> shuffledAllSnake = new List<Snake>(ShuffleList(allSnakes));

        if (Solved(grid))
            return true;

        foreach (var snake in allSnakes)
        {
            List<string> directions = new List<string>();
            int[] startNode = snake.allTile[0];
            int[] endNode = snake.hole;
            
            if (CheckCanMoveWithThisDirections(startNode[0], startNode[1] + 1, snake))
            {
                if (endNode[1] > startNode[1])
                    directions.Insert(0, "right");
                else
                    directions.Add("right");
            }
            if(CheckSnakeFindHole(startNode[0], startNode[0] + 1,snake))
            {
                allSnakes.Remove(snake);
                SolvePuzzle(grid);
                return true;
            }
            if (CheckCanMoveWithThisDirections(startNode[0], startNode[1] - 1, snake))
            {
                if (endNode[1] < startNode[1])
                    directions.Insert(0, "left");
                else
                    directions.Add("left");
            }
            if(CheckSnakeFindHole(startNode[0], startNode[0] - 1,snake))
            {
                allSnakes.Remove(snake);
                SolvePuzzle(grid);
                return true;
            }

            if (CheckCanMoveWithThisDirections(startNode[0] + 1, startNode[1], snake))
            {
                if (endNode[0] > startNode[0])
                    directions.Insert(0, "down");
                else
                    directions.Add("down");
            }
            if(CheckSnakeFindHole(startNode[0]  + 1, startNode[0] ,snake))
            {
                allSnakes.Remove(snake);
                SolvePuzzle(grid);
                return true;
            }
            if (CheckCanMoveWithThisDirections(startNode[0] - 1, startNode[1], snake))
            {
                if (endNode[0] < startNode[0])
                    directions.Insert(0, "up");
                else
                    directions.Add("up");
            }
            if(CheckSnakeFindHole(startNode[0] - 1, startNode[0] ,snake))
            {
                allSnakes.Remove(snake);
                SolvePuzzle(grid);
                return true;
            }

            if (directions.Count == 0)
                return false;
            
            List<int[]> tempTile = new List<int[]>();

            foreach (var array in snake.allTile)
            {
                int[] newArray = new int[array.Length];
                Array.Copy(array, newArray, array.Length);
                tempTile.Add(newArray);
            }

            int[,] tempGrid = (int[,])grid.Clone();
            foreach (string direction in directions)
            {
                switch (direction)
                {
                    case "right":

                        int headX = snake.allTile[0][0];
                        int headY = snake.allTile[0][1] + 1;

                        for (int j = 0; j < snake.allTile.Count; j++)
                        {
                            int currentX = snake.allTile[j][0];
                            int currentY = snake.allTile[j][1];

                            grid[currentX, currentY] = 0;

                            snake.allTile[j] = new int[] { headX, headY };

                            grid[headX, headY] = snake.snakeID;

                            headX = currentX;
                            headY = currentY;
                        }

                        if (SolvePuzzle(grid))
                            return true;
                        else
                        {
                            snake.allTile.Clear();
                            foreach (var array in tempTile)
                            {
                                int[] newArray = new int[array.Length];
                                Array.Copy(array, newArray, array.Length);
                                snake.allTile.Add(newArray);
                            }
                            grid = (int[,])tempGrid.Clone();
                        }

                        break;
                    case "left":
                         headX = snake.allTile[0][0];
                         headY = snake.allTile[0][1] - 1;

                        for (int j = 0; j < snake.allTile.Count; j++)
                        {
                            int currentX = snake.allTile[j][0];
                            int currentY = snake.allTile[j][1];

                            grid[currentX, currentY] = 0;

                            snake.allTile[j] = new int[] { headX, headY };

                            grid[headX, headY] = snake.snakeID;

                            headX = currentX;
                            headY = currentY;
                        }

                        if (SolvePuzzle(grid))
                            return true;
                        else
                        {
                            snake.allTile.Clear();
                            foreach (var array in tempTile)
                            {
                                int[] newArray = new int[array.Length];
                                Array.Copy(array, newArray, array.Length);
                                snake.allTile.Add(newArray);
                            }
                            grid = (int[,])tempGrid.Clone();
                        }
                        break;
                    case "up":
                        headX = snake.allTile[0][0] - 1;
                        headY = snake.allTile[0][1];

                        for (int j = 0; j < snake.allTile.Count; j++)
                        {
                            int currentX = snake.allTile[j][0];
                            int currentY = snake.allTile[j][1];

                            grid[currentX, currentY] = 0;

                            snake.allTile[j] = new int[] { headX, headY };

                            grid[headX, headY] = snake.snakeID;

                            headX = currentX;
                            headY = currentY;
                        }

                        if (SolvePuzzle(grid))
                            return true;
                        else
                        {
                            snake.allTile.Clear();
                            foreach (var array in tempTile)
                            {
                                int[] newArray = new int[array.Length];
                                Array.Copy(array, newArray, array.Length);
                                snake.allTile.Add(newArray);
                            }
                            grid = (int[,])tempGrid.Clone();
                        }
                        break;
                    case "down":
                        headX = snake.allTile[0][0] + 1;
                        headY = snake.allTile[0][1];

                        for (int j = 0; j < snake.allTile.Count; j++)
                        {
                            int currentX = snake.allTile[j][0];
                            int currentY = snake.allTile[j][1];

                            grid[currentX, currentY] = 0;

                            snake.allTile[j] = new int[] { headX, headY };

                            grid[headX, headY] = snake.snakeID;

                            headX = currentX;
                            headY = currentY;
                        }

                        if (SolvePuzzle(grid))
                            return true;
                        else
                        {
                            snake.allTile.Clear();
                            foreach (var array in tempTile)
                            {
                                int[] newArray = new int[array.Length];
                                Array.Copy(array, newArray, array.Length);
                                snake.allTile.Add(newArray);
                            }
                            grid = (int[,])tempGrid.Clone();
                        }
                        break;
                }
            }

            return false;
        }
        return false;
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
            (newList[k], newList[n]) = (newList[n], newList[k]);
        }
        return newList;
    }

    public bool Solved(int[,] grid)
    {
        if (allSnakes.Count > 0)
        {
            return false;
        }
        Debug.Log("solved");
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

    public GameObject tileSprite;
    public List<Color> Colors;
    public List<Color> HoleColors;
    private SpriteRenderer[,] tileObject = new SpriteRenderer[10,10];
    public void GenerateTileByMatrix()
    {
        for(int i = 0; i< ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                var obj = Instantiate(tileSprite, new Vector3( i,  j, 0), quaternion.identity);
                obj.transform.SetParent(gameObject.transform);
                var renderer = obj.GetComponent<SpriteRenderer>();
                tileObject[i, j] = renderer;
                
                TileChangeColor(i, j);
            }
        }
        gameObject.transform.Rotate(0.0f, 0.0f, 270.0f, Space.World);
    }

    public void DrawGird()
    {
        for(int i = 0; i< ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                TileChangeColor(i, j);
            }
        }
    }
    public void TileChangeColor(int row, int col)
    {
        if (grid[row, col].ToString().Length == 1)
        {
            if (grid[row, col] == 0)
            {
                tileObject[row, col].color = Color.white;
            }
            else if (grid[row, col] == -1)
            {
                tileObject[row, col].color = Color.black;
            }
            else 
            {
                tileObject[row, col].color = Colors[grid[row, col]];
            }
        }
        else
        {
            int index = Int32.Parse(grid[row, col].ToString()[2].ToString());
            tileObject[row, col].color = HoleColors[index];
            
        }
      
    }
   
   
    
}
