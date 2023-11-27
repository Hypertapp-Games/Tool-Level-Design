using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using Unity.VisualScripting;
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
            //Debug.Log("It work" + x + "  "+y);
            // this.x = x;
            // this.y = y;
            //this = new aTile(x, y);
            //Debug.Log("It after work" + this.x + "  "+this.y);
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
        // Di chuyen con ran
        public void SnakeMove(bool head, int x, int y)
        {
            
            if (head)
            {
                //Debug.Log("Head work");
                for (int i = 0; i < tiles.Count; i++)
                {
                    var tempx = tiles[i].x;
                    var tempy = tiles[i].y;
                    //tiles[i].ChangValue(x,y);
                    tiles[i] = new aTile(x, y);
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
                    //tiles[i].ChangValue(x,y);
                    tiles[i] = new aTile(x, y);
                    x = tempx;
                    y = tempy;
                }   
            }
            
        }
    }
    public struct SnakeMoveDirection
    {
        public aSnake Snake;
        public bool head;

        public SnakeMoveDirection(aSnake Snake, bool head)
        {
            this.Snake = Snake;
            this.head = head;
        }
    }
    
    [Header("Số ScreenCapture muốn trong 1 lần chạy ")]
    public int numberCaptures = 100;
    public ScreenCapture screenCapture;

    public List<aSnake> Snakes = new List<aSnake>();
    
    [SerializeField] private int[,] number;
    
    public List<int> allIdOfSnake = new List<int>();
    
    [Header("Nhập kích thước ma trận ở đây ")]
    public int Width = 0;
    public int Height = 0;
    void Start()
    {
        // 0 can move
        // -1 can't move
        // other: snake
        number = new int[10, 10]
        {
            { 12, 13,  0, 31, 32, 33, -1, -1, -1, -1 },
            { 11, -1, -1, -1, -1, 0 , -1, -1, -1, -1 },
            { 0 , -1, -1, -1, -1, 0 , -1, -1, -1, -1 },
            { 0 , -1, -1, -1, -1, 0 , -1, -1, -1, -1 },
            { 21, -1, -1, -1, -1, 43, -1, -1, -1, -1 },
            { 22, 23, 24,  0, 41, 42, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        };
        LoadSnakeIDFromMatrix();
        
        SnakeRandomDirectionMove();//TEST 1 SNAKE
        
    }
        
    // InPut: Matrix name "number"
    // Output: Get SnakeID from matrix, add to List "allIdOfSnake";
    void LoadSnakeIDFromMatrix()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
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
    // Input: all SnakeID form List "allIdOfSnake"
    // OutPut: each SnakeID, find value in matrix if have SnakeID add to List "tiles"
    public void LoadSnakeByIDFromMatrix()
    {
        for (int k = 0; k < allIdOfSnake.Count; k++)
        {
            List<aTile> tiles = new List<aTile>();
            int value = 0;
            
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    try
                    {
                        var snakeID = Int32.Parse(number[i, j].ToString()[0].ToString());
                        if (snakeID == allIdOfSnake[k] )
                        {
                            tiles.Add(new aTile(i, j));
                            value = allIdOfSnake[k];
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            SortByMarkedNumber(tiles, value);
        }
        GenerateTileByMatrix();
    }
    // Input: List "tiles", snakeID "snakeValue"
    // Output: Sort list "tiles" by number marked, , create "aSnake" with "aSnake.tiles" is "tiles" and "aSnake.value" = snakeValue, delete number marked
    public void SortByMarkedNumber(List<aTile> tiles, int snakeValue)
    {
        List<aTile> temp = new List<aTile>();
        var enum1 = from  _aTile  in tiles
            orderby Int32.Parse(number[_aTile.x, _aTile.y].ToString()[1].ToString()) // sort
            select _aTile;
            
        foreach (var e in enum1)
        {
            number[e.x, e.y] =  snakeValue; // delete number marked
            temp.Add(e);
        }
        
        // replace "tiles" by the "temp" list has been sorted
        tiles.Clear();
        tiles = new List<aTile>(temp); 
        
        var aSnake = new aSnake(tiles , snakeValue);
        Snakes.Add(aSnake); 
    }
    
    public int CurrentCheck = 0;
    private aSnake selectedSnake = new aSnake();
    // Intput: list "Snakes"
    // Output: select snake can move to list "movableSnakes", get random snake from "movableSnakes", check head and tail can move or not and add to list "snakeMoveDirections"
    //         select random a SnakeMoveDirection  in list "snakeMoveDirections" and call function "CheckDirectionInAStep" with "snakeMoveDirection.Snake" and "snakeMoveDirection.head"
    public void SnakeRandomDirectionMove()
    {
        CurrentCheck++;
        if (CurrentCheck >= numberCaptures)
        {
            // Kết thúc nếu đã đạt số lần kiểm tra đủ
            return;
        }

        List<SnakeMoveDirection> snakeMoveDirections = new List<SnakeMoveDirection>();
        List<aSnake> movableSnakes = Snakes
            .Where(snake => snake.value != selectedSnake.value && (CheckSnakeCanMoveOrNot(snake, true) || CheckSnakeCanMoveOrNot(snake, false)))
            .ToList();

        if (movableSnakes.Count > 0)
        {
            selectedSnake = movableSnakes[Random.Range(0, movableSnakes.Count)];
            
        }

        if (selectedSnake.value != 0)
        {
            bool canMoveHead = CheckSnakeCanMoveOrNot(selectedSnake, true);
            bool canMoveTail = CheckSnakeCanMoveOrNot(selectedSnake, false);

            if (canMoveHead)
            {
                snakeMoveDirections.Add(new SnakeMoveDirection(selectedSnake, true));
            }

            if (canMoveTail)
            {
                snakeMoveDirections.Add(new SnakeMoveDirection(selectedSnake, false));
            }

            if (snakeMoveDirections.Count > 0)
            {
                SnakeMoveDirection selectedMoveDirection = snakeMoveDirections[Random.Range(0, snakeMoveDirections.Count)];
                CheckDirectionInAStep(selectedMoveDirection.Snake, selectedMoveDirection.head);
            }
            else
            {
                // End
            }
        }
    }
  
    public bool CheckSnakeCanMoveOrNot(aSnake snake, bool head)
    {
        var index = head ? 0 : snake.tiles.Count - 1;
        for (int i = 1; i <= 4; i++)
        {
            int resultX = i > 2 ? 0  : i % 2 == 0 ? -1 : 1;
            int resultY = i <= 2 ? 0 : i % 2 == 0 ? -1 : 1;
            
            if (CheckDirectionCanMove(resultX, resultY, snake.tiles[index].x, snake.tiles[index].y, snake.value))
            {
                return true;
            }
        }
        return false;
    }
    // Input: increase x , increase y, current x, current y , value
    // Output: check number[targetX, targetY] = 0 or not, if true => can move, else => cant move
    public bool CheckDirectionCanMove(int x, int y, int currentX, int currentY, int value)
    {
        int targetX = currentX + x;
        int targetY = currentY + y;
        
        if (targetX < 0 || targetX >= number.GetLength(0) || targetY < 0 || targetY >= number.GetLength(1))
        {
            return false;
        }
        if (number[targetX, targetY] == 0)
        {
            return true;
        }
        return false;
    }
    
    public void CheckDirectionInAStep(aSnake snake, bool head)
    {
        List<int> stt = new List<int>();
        var index = head ? 0 : snake.tiles.Count - 1;
        for (int i = 1; i <= 4; i++)
        {
            int resultX = i > 2 ? 0  : i % 2 == 0 ? -1 : 1;
            int resultY = i <= 2 ? 0 : i % 2 == 0 ? -1 : 1;
            
            if (CheckDirectionCanMove(resultX, resultY, snake.tiles[index].x, snake.tiles[index].y, snake.value))
            {
                stt.Add(i);
            }
        }
        if (stt.Count > 0)
        {
            int a = Random.Range(0, stt.Count);
            
            int resultX = stt[a] > 2 ? 0  : stt[a] % 2 == 0 ? -1 : 1;
            int resultY = stt[a]<= 2 ? 0 : stt[a] % 2 == 0 ? -1 : 1;
            
            SnakeMove(resultX, resultY, snake, index, head);
            CheckDirectionInAStep(resultX, resultY, snake, index, head);
        }
        else
        {
            StartCoroutine(WaitScreenCapture());
            screenCapture.TakeScreenShots();
        }

    }
    public IEnumerator WaitScreenCapture()
    {
        yield return new WaitForSeconds(0.1f);
        SnakeRandomDirectionMove();
    }
    // Di chuyen theo huong do cho den kho khong con di chuyen duoc nua
    
    public void CheckDirectionInAStep(int x, int y ,aSnake snake, int index, bool head)
    {
        if (CheckDirectionCanMove(x, y, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            SnakeMove(x, y, snake, index, head);
            CheckDirectionInAStep(x, y, snake, index, head);
        }
        else
        {
            StartCoroutine(WaitForEndAStep(snake, head));
            WaitForEndAStep(snake, head);
        }
    }
    public IEnumerator WaitForEndAStep(aSnake snake, bool head)
    {
        yield return new WaitForSeconds(0.1f);
        CheckDirectionInAStep(snake, head);
    }
  
    // Output: Move Snake
    public void SnakeMove(int x, int y, aSnake snake, int index, bool head)
    {
        ChangeValueOfTheMatrixBeforeSnakeMove(snake);
        
        var _x = snake.tiles[index].x + x;
        var _y = snake.tiles[index].y + y;
        snake.SnakeMove(head,_x, _y);
        
        ChangeValueOfTheMatrixAfterSnakeMove(snake);

    }
    
  
    // public void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " + number[0, 3] + "       " + number[0, 4]+ "       " + number[0, 5]+ "       " + number[0, 6]+ "       " + number[0, 7]+ "       " + number[0, 8]+ "       " + number[0, 9]);
    //         Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " + number[1, 3] + "       " + number[1, 4]+ "       " + number[1, 5]+ "       " + number[1, 6]+ "       " + number[1, 7]+ "       " + number[1, 8]+ "       " + number[1, 9]);
    //         Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " + number[2, 3] + "       " + number[2, 4]+ "       " + number[2, 5]+ "       " + number[2, 6]+ "       " + number[2, 7]+ "       " + number[2, 8]+ "       " + number[2, 9]);
    //         Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " + number[3, 3] + "       " + number[3, 4]+ "       " + number[3, 5]+ "       " + number[3, 6]+ "       " + number[3, 7]+ "       " + number[3, 8]+ "       " + number[3, 9]);
    //         Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " + number[4, 3] + "       " + number[4, 4]+ "       " + number[4, 5]+ "       " + number[4, 6]+ "       " + number[4, 7]+ "       " + number[4, 8]+ "       " + number[4, 9]);
    //         Debug.Log("       " + number[5, 0] + "       " + number[5, 1] + "       " + number[5, 2] + "       " + number[5, 3] + "       " + number[5, 4]+ "       " + number[5, 5]+ "       " + number[5, 6]+ "       " + number[5, 7]+ "       " + number[5, 8]+ "       " + number[5, 9]);
    //         Debug.Log("       " + number[6, 0] + "       " + number[6, 1] + "       " + number[6, 2] + "       " + number[6, 3] + "       " + number[6, 4]+ "       " + number[6, 5]+ "       " + number[6, 6]+ "       " + number[6, 7]+ "       " + number[6, 8]+ "       " + number[6, 9]);
    //         Debug.Log("       " + number[7, 0] + "       " + number[7, 1] + "       " + number[7, 2] + "       " + number[7, 3] + "       " + number[7, 4]+ "       " + number[7, 5]+ "       " + number[7, 6]+ "       " + number[7, 7]+ "       " + number[7, 8]+ "       " + number[7, 9]);
    //         Debug.Log("       " + number[8, 0] + "       " + number[8, 1] + "       " + number[8, 2] + "       " + number[8, 3] + "       " + number[8, 4]+ "       " + number[8, 5]+ "       " + number[8, 6]+ "       " + number[8, 7]+ "       " + number[8, 8]+ "       " + number[8, 9]);
    //         Debug.Log("       " + number[9, 0] + "       " + number[9, 1] + "       " + number[9, 2] + "       " + number[9, 3] + "       " + number[9, 4]+ "       " + number[9, 5]+ "       " + number[9, 6]+ "       " + number[9, 7]+ "       " + number[9, 8]+ "       " + number[9, 9]);
    //     
    //     }
    //     
    // }
    
    // Input: aSnake  "snake"
    // OutPut: each aTile "snake.tile",  with row index = aTile.x and colum index = aTile.y, change value of "number" = 0
    public void ChangeValueOfTheMatrixBeforeSnakeMove(aSnake snake)
    {
        for (int i = 0; i < snake.tiles.Count; i++)
        {
            number[snake.tiles[i].x, snake.tiles[i].y] = 0;
            TileChangeColor(snake.tiles[i].x, snake.tiles[i].y);
        }
    }
    // Input: aSnake  "snake"
    // OutPut: each aTile "snake.tile",  with row index = aTile.x and colum index = aTile.y, change value of "number" = "snake.value"
    public void ChangeValueOfTheMatrixAfterSnakeMove(aSnake snake)
    {
        for (int i = 0; i < snake.tiles.Count; i++)
        {
            number[snake.tiles[i].x, snake.tiles[i].y] = snake.value;
            
            TileChangeColor(snake.tiles[i].x, snake.tiles[i].y);
        }
    }

    public GameObject TileSprite;
    public List<Color> Colors;
    private SpriteRenderer[,] tileObject = new SpriteRenderer[10,10];
    
    // InPut: "number"
    // Output: each value in the "number", Instantiate a square sprite in game scene
    public void GenerateTileByMatrix()
    {
        for(int i = 0; i< Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var obj = Instantiate(TileSprite, new Vector3( i,  j, 0), quaternion.identity);
                obj.transform.SetParent(gameObject.transform);
                var renderer = obj.GetComponent<SpriteRenderer>();
                tileObject[i, j] = renderer;
                
                TileChangeColor(i, j);
            }
        }
        gameObject.transform.Rotate(0.0f, 0.0f, 270.0f, Space.World);
    }
    // InPut: i is row index of the tile to change color.
    //        j is column index of the tile to change color.
    // OutPut: Change the color of a tile in the "tileObject" based on the value in the "number".
    public void TileChangeColor(int i, int j)
    {
        if (number[i, j] == 0)
        {
            tileObject[i, j].color = Color.white;
        }
        else if (number[i, j] == -1)
        {
            tileObject[i, j].color = Color.black;
        }
        else
        {
            tileObject[i, j].color = Colors[number[i, j]];
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
    // { -1 , 11  ,-1 , 0, 43 , 42, 41,    -1, -1, -1 },
    // { -1 , 12 , 0 , 0, 0  , -1 , 0,    -1, -1, -1 },
    // {  0 , 13 ,-1 , -1,-1  , -1 ,  0,    -1, -1, -1 },
    // { -1 , 0  ,-1 , -1,0  , 0 ,  0,    -1, -1, -1 },
    // { -1 , 0  , 0 , 0, 0  , 0 , -1,    -1, -1, -1 },
    // { 22 , 23 ,-1 , 0, 31 , 32, 33,    -1, -1, -1 },
    // { 21 ,-1  ,-1 ,-1, -1 , -1 , 34,    -1, -1, -1 },
    //             
    // { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    // { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    // { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
}
