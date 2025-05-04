using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBorder : MonoBehaviour
{
    public Color currentColor;
    
    public List<Color> ColorList;
    SpriteRenderer ownSprit;
    public int colorCount;
   
    private void Start()
    {
        ownSprit = GetComponent<SpriteRenderer>();
        ColorIndex();
    }
    private void OnMouseDown()
    {
        ColorIndex();
    }

    void ColorIndex()
    {
        colorCount++;
        if (colorCount == ColorList.Count)
        {
            colorCount = 0;
        }
        ownSprit.color = ColorList[colorCount];
        currentColor = ColorList[colorCount];
    }
}
