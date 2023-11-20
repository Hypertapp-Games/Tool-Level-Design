using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
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

    public List<aSnake> Snakes = new List<aSnake>();
    [SerializeField] private int[,] number;
    public List<int> allIdOfSnake = new List<int>();
    public ScreenCapture screenCapture;
    void Start()
    {
        // 0 la cac vung trong va co the di chuyen duoc
        // -1 la cac vung trong va khong the di chuyen duoc
        // cac so khac tuong truong cho cac phan cua cac con ran
        number = new int[10, 10]
        {
            { 12,  13,  0 , 31, 32 ,   -1, -1, -1, -1, -1 },
            { 11, -1 , -1 , -1, 33 ,   -1, -1, -1, -1, -1 },
            { 0 , -1 , -1 , -1, 0 ,   -1, -1, -1, -1, -1 },
            { 0 , -1 , -1 , -1, 43 ,   -1, -1, -1, -1, -1 },
            { 21,  22,  23, 41, 42 ,   -1, -1, -1, -1, -1 },
            
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        };
        LoadSnakeIDFromMatrix();
        
        SnakeRandomDirectionMove();//TEST 1 SNAKE
    }
        
    // Lay tat ca id cua cac con ran co trong ma tran
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
    // Voi moi id. tim kiem trong ma tran cac tile cua 1 con ran voi id tuong ung
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
                            value = allIdOfSnake[k];
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            // Sap sep lai thu tu cac con ran theo so thu tu
            List<aTile> temp = new List<aTile>();
            var enum1 = from  _aTile  in tiles
                orderby Int32.Parse(number[_aTile.x, _aTile.y].ToString()[1].ToString())
                select _aTile;
            
            foreach (var e in enum1)
            {
                number[e.x, e.y] = value; // sau khi su dung xong so thu tu se loai bo so thu tu , chi giu lai id
                temp.Add(e);
                //Debug.Log("this do");
            
            }
            tiles.Clear();
            tiles = new List<aTile>(temp);
            var aSnake = new aSnake(tiles , value);
            Snakes.Add(aSnake);
        }

        GenerateTileByMatrix();
        
    }
    // Check cac huong co the di chuyen duoc va chon 1 huong ngau nhien de di cho den khi khong con di duoc theo huong do nua
    public int CurrentCheck = 0;
    public void SnakeRandomDirectionMove()
    {
        CurrentCheck++;
        if (CurrentCheck < 100)
        {
            //var snakeTest = Snakes[0];
            List<SnakeMoveDirection> snakeMoveDirections = new List<SnakeMoveDirection>();
            for (int i = 0; i < Snakes.Count; i++)
            {
                if (CheckSnakeCanMoveOrNot(Snakes[i], true))
                {
                    var snakeMoveDirection = new SnakeMoveDirection(Snakes[i], true);
                    snakeMoveDirections.Add(snakeMoveDirection);
                }
                if (CheckSnakeCanMoveOrNot(Snakes[i], false))
                {
                    var snakeMoveDirection = new SnakeMoveDirection(Snakes[i], false);
                    snakeMoveDirections.Add(snakeMoveDirection);
                }
            }

            if (snakeMoveDirections.Count <= 0)
            {
                CurrentCheck = 20;
                // khong tiep tuc check nua
            }
            else
            {
                var snakeMoveDirection = snakeMoveDirections[Random.Range(0, snakeMoveDirections.Count - 1)];
                var snake = snakeMoveDirection.Snake;
                var head = snakeMoveDirection.head;
                CheckDirectionInAStep(snake, head);
            }
            //CheckDirectionInAStep(snakeTest);
            
        }
        
    }
  
    public bool CheckSnakeCanMoveOrNot(aSnake snake, bool head)
    {
        var index = 0;
        if (head)
        {
            index = 0;
        }
        else
        {
            index = snake.tiles.Count - 1;
        }
        List<int> stt = new List<int>();
        if(CheckDirectionCanMove(1, 0, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            stt.Add(1);
        }

        if (CheckDirectionCanMove(-1, 0, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            stt.Add(2);
        }

        if (CheckDirectionCanMove(0, 1, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            stt.Add(3);
        }

        if (CheckDirectionCanMove(0, -1, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            stt.Add(4);
        }

        bool right = true;
        if (stt.Count > 0)
        {
            right = true;
        }
        else
        {
            right = false;
        }
        return right;
    }
    // Check cac huong co the di chuyen duoc
    public void CheckDirectionInAStep(aSnake snake, bool head)
    {
        var index = 0;
        if (head)
        {
            index = 0;
        }
        else
        {
            index = snake.tiles.Count - 1;
        }
        List<int> stt = new List<int>();
        if(CheckDirectionCanMove(1, 0, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            stt.Add(1);
        }

        if (CheckDirectionCanMove(-1, 0, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            stt.Add(2);
        }

        if (CheckDirectionCanMove(0, 1, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            stt.Add(3);
        }

        if (CheckDirectionCanMove(0, -1, snake.tiles[index].x, snake.tiles[index].y, snake.value))
        {
            stt.Add(4);
        }

        if (stt.Count > 0)
        {
            int a = Random.Range(0, stt.Count);
            if(stt[a] == 1)
            {
                SnakeMove(1, 0, snake, index, head);
                CheckDirectionInAStep(1, 0, snake, index, head);
            }
            else if(stt[a] == 2)
            {
                SnakeMove(-1, 0, snake, index, head);
                CheckDirectionInAStep(-1, 0, snake, index, head);
            }
            else if(stt[a] == 3)
            {
                SnakeMove(0, 1, snake, index, head);
                CheckDirectionInAStep(0, 1, snake, index, head);
            }
            else if(stt[a] == 4)
            {
                SnakeMove(0, -1, snake, index, head);
                CheckDirectionInAStep(0, -1, snake, index, head);

            }
            
        }

    }
    // Di chuyen theo huong do cho den kho khong con di chuyen duoc nua
    public void CheckDirectionInAStep(int x, int y ,aSnake snakeTest, int index, bool head)
    {
        if (CheckDirectionCanMove(x, y, snakeTest.tiles[index].x, snakeTest.tiles[index].y, snakeTest.value))
        {
            SnakeMove(x, y, snakeTest, index, head);
            CheckDirectionInAStep(x, y, snakeTest, index, head);
        }
        else
        {
            screenCapture.TakeScreenShots();
            Debug.Log("Da het duonqg theo huong da chon");
            StartCoroutine(WaitScreenCapture());
        }
    }
    public IEnumerator WaitScreenCapture()
    {
        yield return new WaitForSeconds(0.1f);
        SnakeRandomDirectionMove();
    }
    // Di chuyen con ran va thay doi cac gia tri tren ma tran
    public void SnakeMove(int x, int y, aSnake snakeTest, int index, bool head)
    {
        ChangeValueOfTheMatrixBeforeSnakeMove(snakeTest);
        
        var _x = snakeTest.tiles[index].x + x;
        var _y = snakeTest.tiles[index].y + y;
        snakeTest.SnakeMove(head,_x, _y);
        
        ChangeValueOfTheMatrixAfterSnakeMove(snakeTest);

    }
    
    public bool CheckDirectionCanMove(int x, int y, int currentX, int currentY, int value) // check cac huong co the di chuyen duoc
    {
        bool right = false;
        var _x = currentX + x;
        var _y = currentY + y;
        try
        {
            if (number[_x, _y] != -1)
            {
                if (number[_x, _y] == 0 && number[_x, _y] != value)
                {
                    right = true;
                }
                else
                {
                    //Debug.Log("Da chan");
                    right = false;
                }
            }
            else
            {
                //Debug.Log("Da chan");
                right = false;

            }
            
        }
        catch
        {
            //Debug.Log("Da chan");
            right = false;

        }

        return right;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " + number[0, 3] + "       " + number[0, 4]+ "       " + number[0, 5]+ "       " + number[0, 6]+ "       " + number[0, 7]+ "       " + number[0, 8]+ "       " + number[0, 9]);
            Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " + number[1, 3] + "       " + number[1, 4]+ "       " + number[1, 5]+ "       " + number[1, 6]+ "       " + number[1, 7]+ "       " + number[1, 8]+ "       " + number[1, 9]);
            Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " + number[2, 3] + "       " + number[2, 4]+ "       " + number[2, 5]+ "       " + number[2, 6]+ "       " + number[2, 7]+ "       " + number[2, 8]+ "       " + number[2, 9]);
            Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " + number[3, 3] + "       " + number[3, 4]+ "       " + number[3, 5]+ "       " + number[3, 6]+ "       " + number[3, 7]+ "       " + number[3, 8]+ "       " + number[3, 9]);
            Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " + number[4, 3] + "       " + number[4, 4]+ "       " + number[4, 5]+ "       " + number[4, 6]+ "       " + number[4, 7]+ "       " + number[4, 8]+ "       " + number[4, 9]);
            Debug.Log("       " + number[5, 0] + "       " + number[5, 1] + "       " + number[5, 2] + "       " + number[5, 3] + "       " + number[5, 4]+ "       " + number[5, 5]+ "       " + number[5, 6]+ "       " + number[5, 7]+ "       " + number[5, 8]+ "       " + number[5, 9]);
            Debug.Log("       " + number[6, 0] + "       " + number[6, 1] + "       " + number[6, 2] + "       " + number[6, 3] + "       " + number[6, 4]+ "       " + number[6, 5]+ "       " + number[6, 6]+ "       " + number[6, 7]+ "       " + number[6, 8]+ "       " + number[6, 9]);
            Debug.Log("       " + number[7, 0] + "       " + number[7, 1] + "       " + number[7, 2] + "       " + number[7, 3] + "       " + number[7, 4]+ "       " + number[7, 5]+ "       " + number[7, 6]+ "       " + number[7, 7]+ "       " + number[7, 8]+ "       " + number[7, 9]);
            Debug.Log("       " + number[8, 0] + "       " + number[8, 1] + "       " + number[8, 2] + "       " + number[8, 3] + "       " + number[8, 4]+ "       " + number[8, 5]+ "       " + number[8, 6]+ "       " + number[8, 7]+ "       " + number[8, 8]+ "       " + number[8, 9]);
            Debug.Log("       " + number[9, 0] + "       " + number[9, 1] + "       " + number[9, 2] + "       " + number[9, 3] + "       " + number[9, 4]+ "       " + number[9, 5]+ "       " + number[9, 6]+ "       " + number[9, 7]+ "       " + number[9, 8]+ "       " + number[9, 9]);
        
        }
        
    }
    public void ChangeValueOfTheMatrixBeforeSnakeMove(aSnake snakeTest)
    {
        for (int i = 0; i < snakeTest.tiles.Count; i++)
        {
            number[snakeTest.tiles[i].x, snakeTest.tiles[i].y] = 0;
            TileChangeColor(snakeTest.tiles[i].x, snakeTest.tiles[i].y);
        }
    }
    public void ChangeValueOfTheMatrixAfterSnakeMove(aSnake snakeTest)
    {
        for (int i = 0; i < snakeTest.tiles.Count; i++)
        {
            //Debug.Log("It after work in ma trix" +snakeTest.tiles[i].x + "  "+snakeTest.tiles[i].y);
            number[snakeTest.tiles[i].x, snakeTest.tiles[i].y] = snakeTest.value;
            
            TileChangeColor(snakeTest.tiles[i].x, snakeTest.tiles[i].y);
        }
    }

    public GameObject TileSprite;
    public List<Color> Colors;
    private SpriteRenderer[,] tileObject = new SpriteRenderer[10,10];
    
    // Tu cac gia tri cua ma tran se gen ra map tuong ung
    public void GenerateTileByMatrix()
    {
        for(int i = 0; i< 10; i++)
        {
            for (int j = 0; j < 10; j++)
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
    // thay doi mau cua cac tile tai cac toa do tuong ung
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
}
