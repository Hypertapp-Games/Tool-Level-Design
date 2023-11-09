using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IWillDoSomethingInThis : MonoBehaviour
{
    public struct num
    {
        public int x;
        public int y;
        public int id;

        public num(int x, int y, int id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }
    }
    [SerializeField] private int[,] number;
    private List<num> num2;
    private List<num> num3;
    private List<num> num4;
    private List<num> num5;
    void Start()
    {
        number = new int[5, 5]
        {
            { 2, 2, 3, 3, 3 },
            { 2, 0, 0, 0, 3 },
            { 1, 0, 0, 0, 1 },
            { 5, 0, 0, 0, 4 },
            { 5, 5, 1, 4, 4 }
        };
        
        
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
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            LogToInt();
        }
    }
}
