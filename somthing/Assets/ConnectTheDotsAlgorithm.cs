using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class ConnectTheDotsAlgorithm : MonoBehaviour
{
    [Serializable]
    public struct dots
    {
        public float x;
        public float y;

        public dots(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
    [Serializable]
    public struct value
    {
        public dots start;
        public dots end;
        public float distance;

        public value(dots start, dots end, float distance)
        {
            this.start = start;
            this.end = end;
            this.distance = distance;
        } 
    }
 
    
    
    [Serializable]
    public struct  adot
    {
        public float x;
        public float y;
        public int value;

        public adot(float x, float y, int value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        } 
    }
    
    [Serializable]
    public struct  coupledots
    {
        public adot start;
        public adot end;
        public int value;
        public float distance;
        

        public coupledots(adot start, adot end, int value)
        {
            this.start = start;
            this.end = end;
            this.value = value;
            distance = (float)Math.Sqrt(Math.Pow(Math.Abs(start.x - end.x), 2) + Math.Pow(Math.Abs(start.y - end.y), 2));
        } 
    }
    [SerializeField] private int[,] number;
    [SerializeField] private int[,] _tempNumber;
    public GameObject cloneSquare;
    public List<Color> Colors;
    public Transform parent;
    void Start()
    {
        // 0 la cac vung trong va co the di chuyen duoc
        // -1 la cac vung trong va khong the di chuyen duoc
        // cac so khac tuong truong cho cac dot
        number = new int[10, 10]
      {
            { 0, 0, 1, 0, 0, 0, -1, -1, -1, -1 },
            { 0, 2, 0, 0, 4, 0, -1, -1, -1, -1 },
            { 1, 0, 0, 0, 0, 0, -1, -1, -1, -1 },
            { 2, 3, 4, 0, 3, 0, -1, -1, -1, -1 },
            { 0, 0, 0, 0, 0, 0, -1, -1, -1, -1 },
            { 0, 0, 0, 0, 0, 0, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
      };
        _tempNumber = new int[10, 10];
        
        CreateCloneNumberMatrix();
        
        CheckAllDotInMatrix();
        
    }
    
    //Kiem tra xem co bao nhieu dot tren ma tran
    // 
    public List<adot> allDotInMatrix = new List<adot>();
    public void CheckAllDotInMatrix()
    {
        for(int i = 0; i< 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (number[i, j] != 0)
                {
                    var newdot = new adot(i, j, number[i, j]);
                    allDotInMatrix.Add(newdot);
                }
            }
        }

        CheckAllCoupleDotInMatrix();
    }
    // kiem tra xem co bao nhieu cap dot
    public List<coupledots> allCoupleDotInMatrix = new List<coupledots>();
    public void CheckAllCoupleDotInMatrix()
    {
        int value = allDotInMatrix[0].value;
        if (value != -1)
        {
            for(int i = 1; i< allDotInMatrix.Count; i++)
            {
                if (allDotInMatrix[i].value == value)
                {
                    var newcoupledot = new coupledots(allDotInMatrix[0], allDotInMatrix[i], value);
                    allCoupleDotInMatrix.Add(newcoupledot);
                    allDotInMatrix.Remove(allDotInMatrix[i]);
                    allDotInMatrix.Remove(allDotInMatrix[0]);
                    break;
                }
            }
        }
        else
        {
            allDotInMatrix.Remove(allDotInMatrix[0]);
        }
        

        if (allDotInMatrix.Count >= 2)
        {
            CheckAllCoupleDotInMatrix();
        }
        else
        {
            StartAstarByDistance();
            DeBugNumberAray(number);//log maxtrix sau khi giai ra man hin de kiem tra
        }
    }
    public bool currentDotRight = true;
    public  List<coupledots> temp = new List<coupledots>();

    // kiem tra nhung cap dot nao co khoang cach gan nhau nhat thi cho A* truoc, tang dan tu tap den cao 
    public void StartAstarByDistance()
    {
        var enum1 = from  cpdot  in allCoupleDotInMatrix
            orderby cpdot.distance
            select cpdot;

        foreach (var e in enum1)
        {
            temp.Add(e);

        }
        var CurrentStep = 0;
        Backtracking(CurrentStep);

    }
    // Kiem tra trong qua trinh tim duong co dot nao khong tim duoc duong khong
    public void Backtracking( int CurrentStep)
    {
        CurrentStep++;
        if (CurrentStep < 100)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                var dotStart = new dots(temp[i].start.x, temp[i].start.y);
                var dotEnd = new dots(temp[i].end.x, temp[i].end.y);
                Astar(dotStart, dotEnd, temp[i].value, number);
                if (!currentDotRight)
                {
                    //Debug.Log("Sai va bat dau check tu day" + temp[i].value + i);
                    int a = 0;
                    var b = 0;
                    CheckInAStep(i  ,a,  CurrentStep,b);
                    
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Khong co loi giai");
        }
        
    }

    bool sumFalse = true;
    // Neu dot khong tim duoc duong thi ta se cho no tang them 1 thu tu khi sap xep
    // vi du như stt tim duong dang la  1, 7, 8, 3, 4, 2, 5, 6
    // khi phat hien số 2 khong tim duoc duong ta se cho so 2 len truoc (so 2 luc nay dang o vi tri so 6)
    // luc nay ta co 1, 7, 8, 3, 2, 4, 5, 6 
    // lap lai cho den khi neu set den vi tri thu 6 thi dot nao cung co duong di
    public void CheckInAStep( int step, int a,  int CurrentStep, int b)
    {
        b++;
        if (b < 100)
        {
            sumFalse = true;
            ClearNumberMatrix();
            for (int i = 0; i <= step; i++)
            {
                var dotStart = new dots(temp[i].start.x, temp[i].start.y);
                var dotEnd = new dots(temp[i].end.x, temp[i].end.y);
                Astar(dotStart, dotEnd, temp[i].value, number);
                if (!currentDotRight)
                {
                    sumFalse = false;
                    Debug.Log(i);
                }
            }

            if (!sumFalse)
            {
                var arrayTemp = temp.ToArray();
                var index = step - a;
                try
                {
               
                    var dot1 = temp[index];
                    var dot2 = temp[index-1];
                    arrayTemp[index-1] = dot1;
                    arrayTemp[index] = dot2;
                    a++;
                }
                catch (Exception e)
                {
                }
                temp.Clear();
                temp = arrayTemp.ToList();
                CheckInAStep(step, a, CurrentStep,b);
            }
            else
            {
                //Debug.Log("Da check xong va dung" );
                ClearNumberMatrix();
                
                Backtracking(CurrentStep);
            }
        }
        else
        {
            Debug.Log("Khong co loi giai");
        }
        
    }
    public void Astar(dots Start, dots End , int value, int[,] number)
    {
        var CurrentDot = Start;
        int CountLoop = 0;
        List <value> Q = new List<value>(); // list cac dot dang xet 
        List <value> dotExpanded = new List<value>(); //list cac dot da xet 

        List<value> allDot = new List<value>();// tat ca cac dot da va dang xet

        AstarLoop(Start, End, Q, CurrentDot, dotExpanded, allDot, value,CountLoop, number);
    }
    // Thuc hien A* loop
    public void AstarLoop(dots Start, dots End, List<value> Q, dots CurrentDot, List<value> dotExpanded, List<value> allDot, int value, int CountLoop, int[,] number)
    {
        CountLoop++;
        ConsiderAdjacentDot(1 , 0 , Start, Q, CurrentDot, allDot, value , number);
        ConsiderAdjacentDot(-1, 0 , Start, Q, CurrentDot, allDot, value, number);
        ConsiderAdjacentDot(0 , 1 , Start, Q, CurrentDot, allDot, value, number);
        ConsiderAdjacentDot(0 , -1, Start, Q, CurrentDot, allDot, value, number);

        float minDis = 10000;
        int index = 0;

        try
        {
            for (int i = 0; i < Q.Count; i++) // kiem tra nut nao dang la nut ngan nhat
            {
                if(Q[i].distance < minDis)
                {
                    minDis = Q[i].distance;
                    index = i;
                }
            }
            dotExpanded.Add(Q[index]); 
        
            if (Q[index].end.x == End.x && Q[index].end.y == End.y) // neu nut duoc xet la nut finish thi end loop
            {
                //Debug.Log("Da den dich");
            
                List<dots> Result = new List<dots>();
                var dotEnd = dotExpanded[dotExpanded.Count - 1].end;
                var dotStart = dotExpanded[dotExpanded.Count - 1].start;

                Result.Add(dotEnd);
                Result.Add(dotStart);

                var dot = dotExpanded[dotExpanded.Count - 1].start;
                EndAstarLoop(Start,dot, dotExpanded, Result,value, number);
                currentDotRight = true;

            }
            else
            {
                CurrentDot = Q[index].end;
                Q.Remove(Q[index]);
                if (CountLoop < 10*10) // gioi han so lan loop, neu nhieu qua 5*5 =125 tuc la da loi
                {
                    AstarLoop(Start, End, Q, CurrentDot, dotExpanded,allDot,value,CountLoop,number);
               
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Loi giai sai");
            currentDotRight = false;
        }
      
      
    }
    // duyet lai duong di ngan nhat
    public void EndAstarLoop(dots Start,dots dot ,List<value> dotExpanded, List<dots> Result , int value, int[,] number)
    {
        for (int i = 0; i < dotExpanded.Count; i++)
        {
            if (dot.x == dotExpanded[i].end.x && dot.y == dotExpanded[i].end.y)
            {
                var _dot = dotExpanded[i].start;
                Result.Add(_dot);
                if(dotExpanded[i].start.x == Start.x && dotExpanded[i].start.y == Start.y)
                {
                    ChangNumberListValue(Result, value, number);
                }
                else
                {
                    EndAstarLoop(Start, _dot, dotExpanded, Result,value,number);
                }
                
            }
        }
    }

    // sau khi da xet xong het thi thay doi gia tri tren matrix 
    public void ChangNumberListValue(List<dots> Result,int value, int[,] number)
    {
        for(int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                for (int k = 0; k < Result.Count; k++)
                {
                    if (i == Result[k].x && j == Result[k].y)
                    {
                        number[i, j] = value;
                    }
                }
            }
        }
    }

    // kiem tra cac dot lien ke dot dang xet
    public void ConsiderAdjacentDot(int x, int y , dots Start, List<value> Q, dots CurrentDot, List<value> allDot, int value,int[,] number)
    {
        
        try
        {
            if (number[(int)CurrentDot.x + x, (int)CurrentDot.y + y] != -1)
            {
                if (number[(int)CurrentDot.x + x, (int)CurrentDot.y +y] == 0  || number[(int)CurrentDot.x + x, (int)CurrentDot.y +y]  == value)//&& (int)Start.x + x != CurrentDot.x && (int)Start.y + y != CurrentDot.y)
                {
                    var endot = new dots(CurrentDot.x + x, CurrentDot.y + y);
                    if (!CheckDotInAllDot(endot, allDot))
                    {
                        var distance = Math.Abs(Start.x - endot.x) + Math.Abs(Start.y - endot.y) + Math.Sqrt(Math.Pow(Math.Abs(Start.x - endot.x), 2) + Math.Pow(Math.Abs(Start.y - endot.y), 2));
                        var _value = new value(CurrentDot, endot, (float)distance);
                        allDot.Add(_value);
                        Q.Add(_value);
                    }
                }
            }
            
        }
        catch
        {

        }
    }
    // kiem tra xem dot duoc xe da tung ton tai trong tat ca cac dot dang xet chua
    public bool CheckDotInAllDot(dots endot, List<value> allDot)
    {
        bool right = true;
        if (allDot.Count == 0)
        {
            right = false;
        }
        else
        {
            for(int i = 0; i< allDot.Count; i++)
            {

                if (endot.x == allDot[i].start.x && endot.y == allDot[i].start.y)
                {
                    right = true;
                   // Debug.Log("da trung");
                    break;

                }
                else
                {
                    right = false;
                }
                if (endot.x == allDot[i].end.x && endot.y == allDot[i].end.y)
                {
                    right = true;
                  //  Debug.Log("da trung");
                    break;
                }
                else
                {
                    right = false;
                }
            }
        }
        

        return right;
    }
    // tao 1 ma tran clone ma tran goc de dung cho sau nay clear lai m tran goc
    public void CreateCloneNumberMatrix()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                _tempNumber[i, j] = number[i, j];
            }
        }
    }    

    public void ClearNumberMatrix()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                number[i, j] = _tempNumber[i, j];
            }
        }
    }
    
    // debug ma tran sau khi tim dc duong
    public void DeBugNumberAray(int [,] number)
    { 
        // Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " + number[0, 3] + "       " + number[0, 4]);
        // Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " + number[1, 3] + "       " + number[1, 4]);
        // Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " + number[2, 3] + "       " + number[2, 4]);
        // Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " + number[3, 3] + "       " + number[3, 4]);
        // Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " + number[4, 3] + "       " + number[4, 4]);
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
        
        // ve len man hinh
        for(int i = 0; i< 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var obj = Instantiate(cloneSquare, new Vector3( 9-i,  9-j, 0), quaternion.identity);
                try
                {
                    obj.GetComponent<SpriteRenderer>().color = Colors[number[i, j]];
                   
                }
                catch (Exception e)
                {
                }
                obj.transform.SetParent(parent);
            }
        }
    }
}
