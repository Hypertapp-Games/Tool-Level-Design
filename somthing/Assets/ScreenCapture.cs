using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenCapture : MonoBehaviour
{
    public string NameLevelDesign;
    private int suffixes;
    private float timer;
    public float timerTick;
    

    public void TakeScreenShots()
    {
        timer = DateTime.Now.Second;
        if (timer != timerTick)
        {
            suffixes = 0;
            timerTick = timer;
        }
        var time = DateTime.Now.ToString("dd_MM_yyyy (HH:mm:ss)");
        var screenshotname = "Assets/ScreenCapture/" + NameLevelDesign + "_" + time + "_" + suffixes + ".png";
        suffixes++;
        UnityEngine.ScreenCapture.CaptureScreenshot(screenshotname);
    }
}
