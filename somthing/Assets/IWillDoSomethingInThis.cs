using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IWillDoSomethingInThis : MonoBehaviour
{
    public struct num
    {
        public int i;
        public int j;
        public int id;

        public num(int i, int j, int id)
        {
            this.i = i;
            this.j = j;
            this.id = id;
        }
    }

    [SerializeField] private int[,] number;
    [SerializeField] private num[,] listID;
    private List<num> num2 = new List<num>();
    private List<num> num3 = new List<num>();
    private List<num> num4 = new List<num>();
    private List<num> num5 = new List<num>();

    private List<num> numWay = new List<num>();
    void Start()
    {
        listID = new num[5, 5];
        number = new int[5, 5]
        {
            { 2, 2, 3, 3, 3 },
            { 2, 0, 0, 0, 3 },
            { 1, 0, 0, 0, 1 },
            { 5, 0, 0, 0, 4 },
            { 5, 5, 1, 4, 4 }
        };

        LoadArrayID();

       

    }
    public void LoadArrayID()
    {
        for(int i = 0; i< 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                listID[i, j].id = number[i, j];
                listID[i, j].i = i;
                listID[i, j].j = j;
            }
        }
        num2.Add(listID[1, 0]);
        num2.Add(listID[0, 0]);
        num2.Add(listID[0, 1]);

        num3.Add(listID[0, 2]);
        num3.Add(listID[0, 3]);
        num3.Add(listID[0, 4]);
        num3.Add(listID[1, 4]);

        num4.Add(listID[4, 3]);
        num4.Add(listID[4, 4]);
        num4.Add(listID[3, 4]);

        num5.Add(listID[3, 0]);
        num5.Add(listID[4, 0]);
        num5.Add(listID[4, 1]);

        numWay.Add(listID[0, 0]);
        numWay.Add(listID[0, 1]);
        numWay.Add(listID[0, 2]);
        numWay.Add(listID[0, 3]);
        numWay.Add(listID[0, 4]);

        numWay.Add(listID[1, 0]);
        numWay.Add(listID[1, 4]);

        numWay.Add(listID[2, 0]);
        numWay.Add(listID[2, 4]);

        numWay.Add(listID[3, 0]);
        numWay.Add(listID[3, 4]);

        numWay.Add(listID[4, 0]);
        numWay.Add(listID[4, 1]);
        numWay.Add(listID[4, 2]);
        numWay.Add(listID[4, 3]);
        numWay.Add(listID[4, 4]);

        Num3Start = num3[0];

    }
    public List<num> went3 = new List<num>();
    num Num3Start;
    public void findWay3()
    {
        //if(num3[0]) i = 0; j =2;
        List<num> forks = new List<num>();
        int i = num3[0].i;
        Debug.Log(i);
     
        int j = num3[0].j;
        Debug.Log(j);
        Debug.Log(num3[0].id);
        try
        {
            if (listID[i + 1, j].id != 0 && listID[i + 1, j].id != num3[1].id)
            {
                forks.Add(listID[i + 1, j]);
                Debug.Log("xuong");
            }
            
        }
        catch
        {

        }
        try
        {
            if (listID[i - 1, j].id != 0 && listID[i - 1, j].id != num3[1].id)
            {
                forks.Add(listID[i - 1, j]);
                Debug.Log("len");
            }
           
        }
        catch
        {

        }
        try
        {
            if (listID[i, j + 1].id != 0 && listID[i, j + 1].id != num3[1].id)
            {
                forks.Add(listID[i, j + 1]);
                Debug.Log("lui");
            }
           
        }
        catch
        {

        }
        try
        {
            if (listID[i, j - 1].id != 0 && listID[i, j - 1].id != num3[1].id)
            {
                forks.Add(listID[i, j - 1]);
                Debug.Log("tien");
            }
        
        }
        catch
        {

        }



      
        if(forks.Count > 0)
        {
            if (forks.Count > 1)
            {
                var a = UnityEngine.Random.Range(0, forks.Count);
                var frok = forks[a];
                num3[1] = num3[0];
                num3[0] = frok;
                went3.Add(frok);
            }
            else if (forks.Count == 1)
            {
                var frok = forks[0];
                num3[1] = num3[0];
                num3[0] = frok;
            }
            if(num3[0].i == 0 && num3[0].j == 0)
            {
                Debug.Log("Da den dich"  );
            }
            else
            {
                Debug.Log("Di tiep");
                findWay3();
            }
        }  
        else
        {
            Debug.Log("HetDuong");
        }

    }
    //  0,0   ;   0,4
    //  4,0   ;   4,4
    // if 3 find to 0,0
    // if 2 find to 4,0
    // if 5 find to 4,4
    // if 4 find to 0,4
    private void LogToInt()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Debug.Log(i + "   "+j+"   " + "   " +number[i,j]);
            }
        }
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Debug.Log("Struct " + i + "   " + j + "   " + "   " + listID[i, j].id);
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            findWay3();
        }
    }
// public int dotGreen1X = 1;
    // public int dotGreen1Y = 0;
    //
    // public float dotGreen2X = 4;
    // public float dotGreen2Y = 4;
    //
    // public float dotBlue1X = 4;
    // public float dotBlue1Y = 0;
    //
    // public float dotBlue2X = 3;
    // public float dotBlue2Y = 2;
    //
    // public float dotOrange1X = 3;
    // public float dotOrange1Y = 1;
    //
    // public float dotOrange2X = 2;
    // public float dotOrange2Y = 3;
    // var distanceBetween2GreenDots = Math.Sqrt(Math.Pow(Math.Abs(dotGreen1X - dotGreen2X), 2) + Math.Pow(Math.Abs(dotGreen1Y - dotGreen2Y), 2));
    // var distanceBetween2BlueDots = Math.Sqrt(Math.Pow(Math.Abs(dotBlue1X - dotBlue2X), 2) + Math.Pow(Math.Abs(dotBlue1Y - dotBlue2Y), 2));
    // var distanceBetween2OrangeDots = Math.Sqrt(Math.Pow(Math.Abs(dotOrange1X - dotOrange2X), 2) + Math.Pow(Math.Abs(dotOrange1Y - dotOrange2Y), 2));
    //
    // //Debug.Log(distanceBetween2GreenDots);
    // //Debug.Log(distanceBetween2BlueDots);
    // //Debug.Log(distanceBetween2OrangeDots);
    // var dotOrange1 = new dots(dotOrange1X, dotOrange1Y);
    // var dotOrange2 = new dots(dotOrange2X, dotOrange2Y);
    // Astar(dotOrange1, dotOrange2, 3);
    // var dotBlue1 = new dots(dotBlue1X, dotBlue1Y);
    // var dotBlue2 = new dots(dotBlue2X, dotBlue2Y);
    // Astar(dotBlue1, dotBlue2, 2);
    // var dotGreen1 = new dots(dotGreen1X, dotGreen1Y);
    // var dotGreen2 = new dots(dotGreen2X, dotGreen2Y);
    // Astar(dotGreen1, dotGreen2, 1);
    
    // number = new int[10, 10]
    // {
    //     { 0 , 0, 1, 0, 2, 0, 0, 0 , 3, 4 },
    //     { 0 , 0, 0, 0, 0, 0, 0, 3 , 0, 0 },
    //     { 0 , 0, 0, 0, 0, 5, 0, 0 , 0, 6 },
    //     { 0 , 0, 0, 5, 0, 0, 0, 0 , 6, 0 },
    //     { 0 , 0, 0, 0, 0, 0, 0, 0 , 4, 7 },
    //     { 0 , 0, 0, 0, 0, 2, 0, 0 , 0, 0 },
    //     { 0 , 0, 7, 0, 0, 0, 0, 8 , 0, 8 },
    //     { 9 , 0, 0, 0, 0, 0, 0, 0 , 0, 0 },
    //     { 0 , 0, 0, 0, 0, 0, 0, 0 , 9, 0 },
    //     { 10, 0, 0, 0, 0, 0, 0, 10, 1, 0 },
    // };


}
