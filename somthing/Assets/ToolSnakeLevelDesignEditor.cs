using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolSnakeLevelDesignEditor : MonoBehaviour
{
    public int rows;
    public int cols;
    public GameObject tileSprite;
    private Camera cam;
    
    private int currentRows;
    private int currentColumns;
    
    public List<Color> tileColors;
    public GameObject tileButtonPanel;
    public GameObject holeButtonPanel;
    private int[,] grid;
    private GameObject[,] gridObject;

    private void Start()
    {
         cam = Camera.main;
         ChangeButtonColor();
    }

     private GameObject objInRay1;
     private GameObject objInRay2;
     public List<Color> Colors;
     public bool isPlayMode = false;
  
    private void Update()
    {
        if (!isPlayMode)
        {
            ChoseTileUseRayCast();
        }
        else
        {
            ChoseSnakeRayCast();
            SnakeDrag();
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            DeBugNumberAray(grid);
        }
    }

    void ChoseTileUseRayCast()
    {
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        
        Debug.DrawRay(cam.transform.position, mousePos - cam.transform.position, Color.blue);
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool rayCastDown = Physics.Raycast(ray, out hit, 100);
        if (rayCastDown)
        {
            objInRay1 = hit.transform.gameObject;
            objInRay1.transform.GetChild(1).gameObject.SetActive(true);
            if (objInRay1 != objInRay2)
            {
                if (objInRay2 != null)
                {
                    objInRay2.transform.GetChild(1).gameObject.SetActive(false);
                }

                objInRay2 = objInRay1;
            }
        }
        else
        {
            if (objInRay1 != null)
            {
                objInRay1.transform.GetChild(1).gameObject.SetActive(false);
            }
            
        }

        if (Input.GetMouseButtonDown(0) && rayCastDown)
        {
            if (colorChange != null)
            {
                ChangeTileColor(objInRay1, colorChange, isHole,id);
            }
        }
    }

    public TMP_InputField WidthInputField;
    public TMP_InputField HeightInputField;
    public void GenerateTileButton()
    {
        rows = int.Parse(WidthInputField.text);
        cols = int.Parse(HeightInputField.text);
        GenerateTile();
    }
    void GenerateTile()
    {
        if (transform.childCount != 0)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            transform.DetachChildren();
        }
        if (gameObject.transform.childCount == 0)
        {
            currentRows = rows;
            currentColumns = cols;
            gridObject = new GameObject[currentRows, currentColumns];
            for (int i = 0; i < currentRows; i++)
            {
                for (int j = 0; j < currentColumns; j++)
                {
                    var tile = Instantiate(tileSprite, new Vector3( j,  currentRows - i, 0), quaternion.identity);
                    gridObject[i, j] = tile;
                    tile.transform.SetParent(gameObject.transform);
                }
            }
            grid = new int[currentRows, currentColumns];
        }

        ChangCameraView();
    }

    void ChangCameraView()
    {
        float minSize = 0;
        if (currentRows > minSize)
        {
            minSize = currentRows;
        }

        if (currentColumns > minSize)
        {
            minSize = currentColumns;
        }

        cam.transform.position = new Vector3((minSize / 10) - 0.5f, (minSize+1) / 2, -10);
        cam.orthographicSize = (minSize+1)/2;
    }

    void ChangeButtonColor()
    {
        List<Image> tileButton = tileButtonPanel.GetComponentsInChildren<Image>().ToList();
        List<Image> listTileButtonOut = (from element in tileButton

            where element.GetComponent<Button>() == null

            select (Image)element).ToList();
        tileButton = new List<Image>(listTileButtonOut);
        
        for (int i = 0; i < tileButton.Count; i++)
        {
            tileButton[i].color = tileColors[i];
        }
        
        
        List<Image> holeButton = holeButtonPanel.GetComponentsInChildren<Image>().ToList();
        
        List<Image> listHoleButtonOut = (from element in holeButton

            where element.GetComponent<Button>() == null

            select (Image)element).ToList();
        holeButton = new List<Image>(listHoleButtonOut);
        for (int i = 0; i < holeButton.Count; i++)
        {
            holeButton[i].color = tileColors[i+2];
        }
    }

    void ChangeTileColor(GameObject tile, Color color, bool isHole, int id)
    {
        // hoan doi id cua mau den va mau trang
        if (id == 1)
        {
            id = 0;
        }
        else if(id == 0)
        {
            id = 1;
        }
        
        
        if (isHole)
        {
            tile.transform.GetChild(0).gameObject.SetActive(true);
            tile.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
            grid[(int)(currentRows - tile.transform.position.y), (int)tile.transform.position.x] = 10 + id;
        }
        else
        {
            tile.transform.GetChild(0).gameObject.SetActive(false);
            tile.GetComponent<SpriteRenderer>().color = color;
            grid[(int)(currentRows - tile.transform.position.y), (int)tile.transform.position.x] =id;
        }
        
    }

    private Color colorChange;
    private bool isHole;
    private int id;
    private Image currentButtonSelect;
    
    public void TileAndHoleButtonClick()
    {
        if (currentButtonSelect != null)
        {
            currentButtonSelect.color = Colors[0];
        }
        
        GameObject ClickButton = EventSystem.current.currentSelectedGameObject;
        ClickButton.GetComponent<Image>().color = Colors[1];
        currentButtonSelect = ClickButton.GetComponent<Image>();
        colorChange = ClickButton.transform.GetChild(0).GetComponent<Image>().color;
        for (int i = 0; i < tileColors.Count; i++)
        {
            if (colorChange == tileColors[i])
            {
                id = i;
                break;
            }
        }
        
        if (ClickButton.name[0] == 'H')
        {
            isHole = true;
        }
        else
        {
            isHole = false;
        }

       
    }
     void DeBugNumberAray(int[,] number)
        {
            // Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " + number[0, 3] + "       " + number[0, 4]);
            // Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " + number[1, 3] + "       " + number[1, 4]);
            // Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " + number[2, 3] + "       " + number[2, 4]);
            // Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " + number[3, 3] + "       " + number[3, 4]);
            // Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " + number[4, 3] + "       " + number[4, 4]);
            Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " +
                      number[0, 3] + "       " + number[0, 4] + "       " + number[0, 5] + "       " + number[0, 6] +
                      "       " + number[0, 7] + "       " + number[0, 8] + "       " + number[0, 9]);
            Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " +
                      number[1, 3] + "       " + number[1, 4] + "       " + number[1, 5] + "       " + number[1, 6] +
                      "       " + number[1, 7] + "       " + number[1, 8] + "       " + number[1, 9]);
            Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " +
                      number[2, 3] + "       " + number[2, 4] + "       " + number[2, 5] + "       " + number[2, 6] +
                      "       " + number[2, 7] + "       " + number[2, 8] + "       " + number[2, 9]);
            Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " +
                      number[3, 3] + "       " + number[3, 4] + "       " + number[3, 5] + "       " + number[3, 6] +
                      "       " + number[3, 7] + "       " + number[3, 8] + "       " + number[3, 9]);
            Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " +
                      number[4, 3] + "       " + number[4, 4] + "       " + number[4, 5] + "       " + number[4, 6] +
                      "       " + number[4, 7] + "       " + number[4, 8] + "       " + number[4, 9]);
            Debug.Log("       " + number[5, 0] + "       " + number[5, 1] + "       " + number[5, 2] + "       " +
                      number[5, 3] + "       " + number[5, 4] + "       " + number[5, 5] + "       " + number[5, 6] +
                      "       " + number[5, 7] + "       " + number[5, 8] + "       " + number[5, 9]);
            Debug.Log("       " + number[6, 0] + "       " + number[6, 1] + "       " + number[6, 2] + "       " +
                      number[6, 3] + "       " + number[6, 4] + "       " + number[6, 5] + "       " + number[6, 6] +
                      "       " + number[6, 7] + "       " + number[6, 8] + "       " + number[6, 9]);
            Debug.Log("       " + number[7, 0] + "       " + number[7, 1] + "       " + number[7, 2] + "       " +
                      number[7, 3] + "       " + number[7, 4] + "       " + number[7, 5] + "       " + number[7, 6] +
                      "       " + number[7, 7] + "       " + number[7, 8] + "       " + number[7, 9]);
            Debug.Log("       " + number[8, 0] + "       " + number[8, 1] + "       " + number[8, 2] + "       " +
                      number[8, 3] + "       " + number[8, 4] + "       " + number[8, 5] + "       " + number[8, 6] +
                      "       " + number[8, 7] + "       " + number[8, 8] + "       " + number[8, 9]);
            Debug.Log("       " + number[9, 0] + "       " + number[9, 1] + "       " + number[9, 2] + "       " +
                      number[9, 3] + "       " + number[9, 4] + "       " + number[9, 5] + "       " + number[9, 6] +
                      "       " + number[9, 7] + "       " + number[9, 8] + "       " + number[9, 9]);
        }

     public void PlayModeOn_Button()
     {
         isPlayMode = true;
         PlayMode();
     }
     public void EditModeOn_Button()
     {
         isPlayMode = false;
     }
     // This is code PlayMode
     [Serializable]
     public  struct Tile
     {
         public int x;
         public int y;
         public GameObject ob;
         public int tileID;
         public Tile(int x, int y , GameObject ob,int tileID)
         {
             this.x = x;
             this.y = y;
             this.ob = ob;
             this.tileID = tileID;
         }
     }
     [Serializable]
     public struct Snake
     {
         public List<Tile> allTile;
         public Dictionary<int,List<Tile>> adjacentTile;
         public int snakeID;

         public Snake(List<Tile> allTile, Dictionary<int,List<Tile>> adjacentTile, int snakeID )
         {
             this.allTile = allTile;
             this.adjacentTile = adjacentTile;
             this.snakeID = snakeID;
         }

         public void SnakeMove(bool head, int x, int y, GameObject gameobject)
         {
             if (head)
             {
                 for (int i = 0; i < allTile.Count; i++)
                 {
                     var tempx = allTile[i].x;
                     var tempy = allTile[i].y;
                     var tempob = allTile[i].ob;
                     var id = allTile[i].tileID;
                     allTile[i] = new Tile(x, y, gameobject, id);
                     x = tempx;
                     y = tempy;
                     gameobject = tempob;
                 }
             }
             else
             {
                 for (int i = allTile.Count - 1; i >= 0; i--)
                 {
                     var tempx = allTile[i].x;
                     var tempy = allTile[i].y;
                     var tempob = allTile[i].ob;
                     var id = allTile[i].tileID;
                     allTile[i] = new Tile(x, y, gameobject, id);
                     x = tempx;
                     y = tempy;
                     gameobject = tempob;
                 }
             }

         }
     }
     private Dictionary<int, Snake> allIdOfSnakes = new Dictionary<int, Snake>();
     public List<Snake> allSnakes;
     void PlayMode()
     {
         allIdOfSnakes.Clear();
         allSnakes.Clear();
         LoadSnakeFromMatrix();
     }
     void LoadSnakeFromMatrix()
     {
         for (int row = 0; row < currentRows; row++)
         {
             for (int column = 0; column < currentColumns; column++)
             {
                 if (grid[row, column] != 0 && grid[row, column] != 1)
                 {
                     int index = grid[row, column];
                     
                     if (allIdOfSnakes.ContainsKey(index))
                     {
                         //new Vector3( column,  currentRows - row, 0)
                         var ob = gridObject[row, column];
                         allIdOfSnakes[index].allTile.Add(new Tile(row, column, ob,100));
                     }
                     else
                     {
                         List<Tile> allTile = new List<Tile>();
                         Dictionary<int,List<Tile>> adjacentTile = new Dictionary<int,List<Tile>>();
                         var ob = gridObject[row, column];
                         var tile = new Tile(row, column, ob,100);
                         allTile.Add(tile);
                        
                         allIdOfSnakes[index] = new Snake(allTile,adjacentTile,index);
                     }
                 }
             }
         }
         foreach (var snake in allIdOfSnakes)
         {
             allSnakes.Add(snake.Value);
         }

         LoadAdjacentTileOfEachTile();

     }

     void LoadAdjacentTileOfEachTile()
     {
         for (int i = 0; i < allSnakes.Count; i++)
         {
             for (int j = 0; j < allSnakes[i].allTile.Count; j++)
             {
                 var startTile = allSnakes[i].allTile[j];
                 for (int k = 0; k < allSnakes[i].allTile.Count; k++)
                 {
                    var endTile =  allSnakes[i].allTile[k];
                    var distance = Math.Sqrt(Math.Pow(startTile.x - endTile.x, 2) + Math.Pow(startTile.y - endTile.y, 2));
                    
                    if (distance == 1)
                    {
                        if (allSnakes[i].adjacentTile.ContainsKey(j))
                        {
                            allSnakes[i].adjacentTile[j].Add(endTile);
                        }
                        else
                        {
                            List<Tile> tiles = new List<Tile>();
                            tiles.Add(endTile);
                            allSnakes[i].adjacentTile[j] = tiles;
                        }
                        //Debug.Log(startTile.x + "  " + startTile.y +"  "+endTile.x + "  " + endTile.y +"  "+ distance);
                    }
                 }
              
             }
         }
         // foreach (var adjacentTile in  allSnakes[0].adjacentTile)
         // {
         //     for (int i = 0; i < adjacentTile.Value.Count; i++)
         //     {
         //         Debug.Log(adjacentTile.Key +"  "+ adjacentTile.Value[i].x +"  "+adjacentTile.Value[i].y);
         //     }
         // }

         LoadHeadAndTailOfSnakeByAdjacentTile();
     }

     void LoadHeadAndTailOfSnakeByAdjacentTile()
     {
         for (int i = 0; i < allSnakes.Count; i++)
         {
             foreach (var adjacentTile in  allSnakes[i].adjacentTile)
             {
                 if (adjacentTile.Value.Count == 1)
                 {
                     var tempTile = allSnakes[i].allTile[adjacentTile.Key];
                     allSnakes[i].allTile[adjacentTile.Key] = new Tile(tempTile.x, tempTile.y, tempTile.ob, 0);
                     LoadHeadAndTileOfSnake(allSnakes[i], 0, adjacentTile.Key);
                     break;
                 }
             }
         }

         SortTileOfSnakeByTileID();
     }

     void LoadHeadAndTileOfSnake(Snake snake, int tileID, int index)
     {
         tileID++;
         if (tileID < snake.allTile.Count)
         {
             foreach (var adT in  snake.adjacentTile)
             {
                 for (int i = 0; i < adT.Value.Count; i++)
                 {
                     if (adT.Value[i].x == snake.allTile[index].x &&
                         adT.Value[i].y == snake.allTile[index].y)
                     {
                         
                         var tempTile = snake.allTile[adT.Key];
                         if (tempTile.tileID == 100)
                         {
                             snake.allTile[adT.Key] = new Tile(tempTile.x, tempTile.y, tempTile.ob, tileID);
                             LoadHeadAndTileOfSnake(snake, tileID, adT.Key);
                             break;
                         }
                     }
                 }
             }
         }
     }

     void SortTileOfSnakeByTileID()
     {
         for (int i = 0; i < allSnakes.Count(); i++)
         {
             List<Tile> temp = new  List<Tile>();

             var enum1 = from aTile in allSnakes[i].allTile
                 orderby aTile.tileID
                 select aTile;
            
             foreach (var e in enum1)
             {
                 temp.Add(e);
             }

             var adT = allSnakes[i].adjacentTile;
             var id = allSnakes[i].snakeID;
             allSnakes[i] = new Snake(temp, adT,id);
         }
     }

     private int snakeIndex = 100;
     private bool isSnakeHead = true;
     void ChoseSnakeRayCast()
     {
         Vector3 mousePos = Input.mousePosition;
         mousePos.z = 10f;
         mousePos = cam.ScreenToWorldPoint(mousePos);
        
         Debug.DrawRay(cam.transform.position, mousePos - cam.transform.position, Color.blue);
        
         Ray ray = cam.ScreenPointToRay(Input.mousePosition);
         RaycastHit hit;
         bool rayCastDown = Physics.Raycast(ray, out hit, 100);

         if (Input.GetMouseButtonDown(0) && rayCastDown)
         {
             var color = hit.transform.gameObject.GetComponent<SpriteRenderer>().color;
             for (int i = 0; i < tileColors.Count; i++)
             {
                 if (color == tileColors[i])
                 {
                     for (int j = 0; j < allSnakes.Count; j++)
                     {
                         if (allSnakes[j].snakeID == i)
                         {
                             snakeIndex = j;
                         }
                     }
                     CheckSnakeIsChosen(snakeIndex, hit.transform.gameObject);
                 }
             }
         }

         if (Input.GetMouseButtonUp(0))
         {
             snakeIndex = 100;
         }
     }

     void CheckSnakeIsChosen(int index , GameObject ob)
     {
         CheckSnakeTileIsChosen_HeadOrTail(allSnakes[index], ob);
     }

     void CheckSnakeTileIsChosen_HeadOrTail(Snake snake, GameObject ob)
     {
         //new Vector3( column,  currentRows - row, 0)
         for (int i = 0; i < snake.allTile.Count; i++)
         {
             if (snake.allTile[i].ob == ob)
             {
                 if (snake.allTile[i].tileID == 0)
                 {
                     Debug.Log("Head");
                     isSnakeHead = true;
                 }
                 else if (snake.allTile[i].tileID == snake.allTile.Count-1)
                 {
                     Debug.Log("Tail");
                     isSnakeHead = false;
                 }
             }
         }
     }
     
     
     // This is Drag Code
     private readonly Vector2 mXAxis = new Vector2(1, 0);
     private readonly Vector2 mYAxis = new Vector2(0, 1);
     private const float mAngleRange = 30;

     private const float mMinSwipeDist = 80.0f;

     private Vector2 mStartPosition;
     void SnakeDrag()
     {
         if (Input.GetMouseButtonDown(0))
         {
             mStartPosition = new Vector2(Input.mousePosition.x,
                 Input.mousePosition.y);
         }
         Vector2 endPosition = new Vector2(Input.mousePosition.x,
             Input.mousePosition.y);
         Vector2 swipeVector = endPosition - mStartPosition;
         
         if (swipeVector.magnitude > mMinSwipeDist)
         {
             mStartPosition = endPosition;
             swipeVector.Normalize();

             float angleOfSwipe = Vector2.Dot(swipeVector, mXAxis);
             angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;

             if (angleOfSwipe < mAngleRange)
             {
                 OnSwipeRight();
             }
             else if ((180.0f - angleOfSwipe) < mAngleRange)
             {
                 OnSwipeLeft();
             }
             else
             {
                 angleOfSwipe = Vector2.Dot(swipeVector, mYAxis);
                 angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                 if (angleOfSwipe < mAngleRange)
                 {
                     OnSwipeTop();
                 }
                 else if ((180.0f - angleOfSwipe) < mAngleRange)
                 {
                     OnSwipeBottom();
                 }
             }
         }
     }

     void OnSwipeRight()
     {
         //Debug.Log("Right");
         if (snakeIndex != 100)
         {
             SnakeMove(0, 1);
         }
       
     }

     void OnSwipeLeft()
     {
         //Debug.Log("Left");
         if (snakeIndex != 100)
         {
             SnakeMove(0, -1);
         }
     }

     void OnSwipeTop()
     {
         //Debug.Log("Top");
         if (snakeIndex != 100)
         {
             SnakeMove(-1, 0);
         }
     }

     void OnSwipeBottom()
     {
         //Debug.Log("Bottom");
         if (snakeIndex != 100)
         {
             SnakeMove(1, 0);
         }
     }

     void SnakeMove(int x, int y)
     {
         int currentX;
         int currentY;
         
         if (isSnakeHead)
         {
             Debug.Log(snakeIndex);
             currentX = allSnakes[snakeIndex].allTile[0].x;
             currentY = allSnakes[snakeIndex].allTile[0].y;
         }
         else
         {
             currentX = allSnakes[snakeIndex].allTile[allSnakes[snakeIndex].allTile.Count - 1].x;
             currentY = allSnakes[snakeIndex].allTile[allSnakes[snakeIndex].allTile.Count - 1].y;
         }

         if (CheckCanMove(x, y, currentX, currentY, allSnakes[snakeIndex].snakeID))
         {
             ChangeValueOfTheMatrixBeforeSnakeMove(allSnakes[snakeIndex]);
             allSnakes[snakeIndex].SnakeMove(isSnakeHead, currentX + x, currentY + y, gridObject[currentX + x, currentY + y]);
             ChangeValueOfTheMatrixAfterSnakeMove(allSnakes[snakeIndex]);
         }
     }

     bool CheckCanMove(int x, int y, int currentX, int currentY, int snakeID)
     {
         int targetX = currentX + x;
         int targetY = currentY + y;
        
         if (targetX < 0 || targetX >= currentRows || targetY < 0 || targetY >= currentColumns)
         {
             return false;
         }
         if (grid[targetX, targetY] == 0)
         {
             return true;
         }

         if (grid[targetX, targetY].ToString().Length == 2)
         {
             if (grid[targetX, targetY].ToString()[1].ToString() == snakeID.ToString())
             {
                 Debug.Log("SnakeInHole");
                 ChangeValueOfTheMatrixBeforeSnakeMove(allSnakes[snakeIndex]);
                 allSnakes.Remove(allSnakes[snakeIndex]);
                 snakeIndex = 100;
                 return false;
             }
         }
         return false;
     }
     public void ChangeValueOfTheMatrixBeforeSnakeMove(Snake snake)
     {
         for (int i = 0; i < snake.allTile.Count; i++)
         {
             grid[snake.allTile[i].x, snake.allTile[i].y] = 0;
             ChangeTileColor(gridObject[snake.allTile[i].x, snake.allTile[i].y], tileColors[1], 0);
         }
     }
     public void ChangeValueOfTheMatrixAfterSnakeMove(Snake snake)
     {
         for (int i = 0; i < snake.allTile.Count; i++)
         {
             grid[snake.allTile[i].x, snake.allTile[i].y] = snake.snakeID;
             ChangeTileColor(gridObject[snake.allTile[i].x, snake.allTile[i].y], tileColors[snake.snakeID], snake.snakeID);
         }
     }
     void ChangeTileColor(GameObject tile, Color color, int id)
     {
         tile.transform.GetChild(0).gameObject.SetActive(false);
         tile.GetComponent<SpriteRenderer>().color = color;
     }

}
//new Vector3( column,  currentRows - row, 0)