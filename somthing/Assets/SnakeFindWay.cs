using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeFindWay : MonoBehaviour
{
    
    [SerializeField] private int[,] number;
    void Start()
    {
        number = new int[10, 10]
        {
            { 10,  0,  0,  0,  0, 30, -1, -1, -1, -1 },
            { 0 , -1, -1, -1, -1, 0 , -1, -1, -1, -1 },
            { 0 , -1, -1, -1, -1, 0 , -1, -1, -1, -1 },
            { 0 , -1, -1, -1, -1, 0 , -1, -1, -1, -1 },
            { 0 , -1, -1, -1, -1, 0 , -1, -1, -1, -1 },
            { 20,  0,  0,  0,  0, 40, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        };
    }
}
