using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
 
    [SerializeField] private int[,] number;
    
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

    void Start()
    {
        number = new int[5, 5]
      {
            { 1, 0, 0, 0, 0 },
            { 5, 0, 0, 5, 0 },
            { 0, 0, 0, 3, 0 },
            { 0, 3, 2, 0, 0 },
            { 2, 0, 0, 0, 1 }
      };
        CheckAllDotInMatrix();
    }
    public void DeBugNumberAray()
    {
        Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " + number[0, 3] + "       " + number[0, 4]);
        Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " + number[1, 3] + "       " + number[1, 4]);
        Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " + number[2, 3] + "       " + number[2, 4]);
        Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " + number[3, 3] + "       " + number[3, 4]);
        Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " + number[4, 3] + "       " + number[4, 4]);
    }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         DeBugNumberAray();
    //     }
    // }

    public List<adot> allDotInMatrix = new List<adot>();
    
    //Kiem tra xem co bao nhieu dot tren ma tran
    public void CheckAllDotInMatrix()
    {
        for(int i = 0; i< 5; i++)
        {
            for (int j = 0; j < 5; j++)
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

        if (allDotInMatrix.Count >= 2)
        {
            CheckAllCoupleDotInMatrix();
        }
        else
        {
            StartAstarByDistance();
            DeBugNumberAray();//log maxtrix sau khi giai ra man hin de kiem tra
        }
    }
    // kiem tra nhung cap dot nao co khoang cach gan nhau nhat thi cho A* truoc, tang dan tu tap den cao
    public void StartAstarByDistance()
    {
        var enum1 = from  cpdot  in allCoupleDotInMatrix
            orderby cpdot.distance
            select cpdot;

        foreach (var e in enum1)
        {
            var dotStart = new dots(e.start.x, e.start.y);
            var dotEnd = new dots(e.end.x, e.end.y);
            Astar(dotStart, dotEnd, e.value);
        }
    }

    public void Astar(dots Start, dots End , int value)
    {
        var CurrentDot = Start;
        int CountLoop = 0;
        List <value> Q = new List<value>(); // list cac dot dang xet 
        List <value> dotExpanded = new List<value>(); //list cac dot da xet

        List<value> allDot = new List<value>();// tat ca cac dot da va dang xet

        AstarLoop(Start, End, Q, CurrentDot, dotExpanded, allDot, value,CountLoop);
    }
  
    public void AstarLoop(dots Start, dots End, List<value> Q, dots CurrentDot, List<value> dotExpanded, List<value> allDot, int value, int CountLoop)
    {
        CountLoop++;
        ConsiderAdjacentDot(1, 0, Start, Q, CurrentDot, allDot, value);
        ConsiderAdjacentDot(-1, 0, Start, Q, CurrentDot, allDot,value);
        ConsiderAdjacentDot(0, 1, Start, Q, CurrentDot, allDot,value);
        ConsiderAdjacentDot(0, -1, Start, Q, CurrentDot, allDot,value);

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
                EndAstarLoop(Start,dot, dotExpanded, Result,value);
        
            }
            else
            {
                CurrentDot = Q[index].end;
                Q.Remove(Q[index]);
                if (CountLoop < 125) // gioi han so lan loop, neu nhieu qua 5*5 =125 tuc la da loi
                {
                    AstarLoop(Start, End, Q, CurrentDot, dotExpanded,allDot,value,CountLoop);
               
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Loi giai sai");
        }
      
      
    }
    // duyet lai duong di ngan nhat
    public void EndAstarLoop(dots Start,dots dot ,List<value> dotExpanded, List<dots> Result , int value)
    {
        for (int i = 0; i < dotExpanded.Count; i++)
        {
            if (dot.x == dotExpanded[i].end.x && dot.y == dotExpanded[i].end.y)
            {
                var _dot = dotExpanded[i].start;
                Result.Add(_dot);
                if(dotExpanded[i].start.x == Start.x && dotExpanded[i].start.y == Start.y)
                {
                    ChangNumberListValue(Result, value);
                }
                else
                {
                    EndAstarLoop(Start, _dot, dotExpanded, Result,value);
                }
                   
            }
        }
    }

    // sau khi da xet xong het thi thay doi gia tri tren matrix 
    public void ChangNumberListValue(List<dots> Result,int value)
    {
        for(int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
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

    // kiem tra cac doiot lien ke
    public void ConsiderAdjacentDot(int x, int y , dots Start, List<value> Q, dots CurrentDot, List<value> allDot, int value)
    {
        
        try
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
}
