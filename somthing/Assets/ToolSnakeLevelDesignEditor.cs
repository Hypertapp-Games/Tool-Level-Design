using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void Start()
    {
         cam = Camera.main;
         GenerateTile();
         ChangeColorButton();
    }

     private GameObject objInRay1;
     private GameObject objInRay2;
     public List<Color> Colors;
  
    private void Update()
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

        if (Input.GetMouseButtonUp(0) && rayCastDown)
        {
            if (colorChange != null)
            {
                ChangeTileButton(objInRay1, colorChange, isHole);
            }
        }
    }

    public void GenerateTile()
    {
        if (gameObject.transform.childCount == 0)
        {
            currentRows = rows;
            currentColumns = cols;
            for (int i = 0; i < currentRows; i++)
            {
                for (int j = 0; j < currentColumns; j++)
                {
                    var tile = Instantiate(tileSprite, new Vector3( j,  currentRows - i, 0), quaternion.identity);
                    tile.transform.SetParent(gameObject.transform);
                }
            }
        }

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

    public void ChangeColorButton()
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

    public void ChangeTileButton(GameObject tile, Color color, bool isHole)
    {
        if (isHole)
        {
            tile.transform.GetChild(0).gameObject.SetActive(true);
            tile.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        }
        else
        {
            tile.transform.GetChild(0).gameObject.SetActive(false);
            tile.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private Color colorChange;
    private bool isHole;
    public Image currentButtonSelect;
    
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
        if (ClickButton.name[0] == 'H')
        {
            isHole = true;
        }
        else
        {
            isHole = false;
        }
    }
}
