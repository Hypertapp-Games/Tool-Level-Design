using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectTheDotsAlgorithm : MonoBehaviour
{
    public struct dots
    {
        public float x;
        public float y;

        public dots(float x, float y)
        {
            this.x = y;
            this.y = y;
        }
    }
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
    public int dotGreen1X = 0;
    public int dotGreen1Y = 0;

    public float dotGreen2X = 4;
    public float dotGreen2Y = 4;

    public float dotBlue1X = 0;
    public float dotBlue1Y = 4;

    public float dotBlue2X = 2;
    public float dotBlue2Y = 3;

    public float dotOrange1X = 1;
    public float dotOrange1Y = 3;

    public float dotOrange2X = 3;
    public float dotOrange2Y = 2;

    void Start()
    {
        number = new int[5, 5]
      {
            { 1, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 3, 0 },
            { 0, 3, 2, 0, 0 },
            { 2, 0, 0, 0, 1 }
      };

        var distanceBetween2GreenDots = Math.Sqrt(Math.Pow(Math.Abs(dotGreen1X - dotGreen2X), 2) + Math.Pow(Math.Abs(dotGreen1Y - dotGreen2Y), 2));
        var distanceBetween2BlueDots = Math.Sqrt(Math.Pow(Math.Abs(dotBlue1X - dotBlue2X), 2) + Math.Pow(Math.Abs(dotBlue1Y - dotBlue2Y), 2));
        var distanceBetween2OrangeDots = Math.Sqrt(Math.Pow(Math.Abs(dotOrange1X - dotOrange2X), 2) + Math.Pow(Math.Abs(dotOrange1Y - dotOrange2Y), 2));

        //Debug.Log(distanceBetween2GreenDots);
        //Debug.Log(distanceBetween2BlueDots);
        //Debug.Log(distanceBetween2OrangeDots);
    }
    public void DeBugNumberAray()
    {
        Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " + number[0, 3] + "       " + number[0, 4]);
        Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " + number[1, 3] + "       " + number[1, 4]);
        Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " + number[2, 3] + "       " + number[2, 4]);
        Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " + number[3, 3] + "       " + number[3, 4]);
        Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " + number[4, 3] + "       " + number[4, 4]);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DeBugNumberAray();
        }
    }
    public void Astar(dots Start, dots End)
    {
        var CurrentDot = Start;
        List <value> Q = new List<value>();
        List <value> dotExpanded = new List<value>();

        AstarLoop(Start, End, Q, CurrentDot, dotExpanded);
    }
    public void AstarLoop(dots Start, dots End, List<value> Q, dots CurrentDot, List<value> dotExpanded)
    {
        ConsiderAdjacentDot(1, 0, Start, Q, CurrentDot);
        ConsiderAdjacentDot(-1, 0, Start, Q, CurrentDot);
        ConsiderAdjacentDot(0, 1, Start, Q, CurrentDot);
        ConsiderAdjacentDot(0, -1, Start, Q, CurrentDot);

        float minDis = 10000;
        int index = 0;
        for (int i = 0; i < Q.Count; i++)
        {
            if(Q[i].distance < minDis)
            {
                minDis = Q[i].distance;
                index = i;
            }
        }
        dotExpanded.Add(Q[index]);
        if (Q[index].end.x == End.x && Q[index].end.y == End.y)
        {
            Debug.Log("Da den dich");
        }
        else
        {
            CurrentDot = Q[index].end;
            Q.Remove(Q[index]);
            AstarLoop(Start, End, Q, CurrentDot, dotExpanded);
        }
    }
    public void EndAstarLoop(List<value> dotExpanded)
    {

    }


    public void ConsiderAdjacentDot(int x, int y , dots Start, List<value> Q, dots CurrentDot)
    {
        try
        {
            if (number[(int)CurrentDot.x + x, (int)CurrentDot.y +y] == 0 )//&& (int)Start.x + x != CurrentDot.x && (int)Start.y + y != CurrentDot.y)
            {
                var endot = new dots(CurrentDot.x + x, CurrentDot.y + y);
                var distance = Math.Abs(Start.x - endot.x) + Math.Abs(Start.y - endot.y)+ Math.Sqrt(Math.Pow(Math.Abs(Start.x - endot.x), 2) + Math.Pow(Math.Abs(Start.y - endot.y), 2));
                var _value = new value(CurrentDot, endot, (float)distance);
                Q.Add(_value);
            }
        }
        catch
        {

        }
    }
}
