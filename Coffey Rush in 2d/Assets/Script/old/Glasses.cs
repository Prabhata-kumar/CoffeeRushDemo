using System.Collections.Generic;
using UnityEngine;

public class Glasses : MonoBehaviour
{
    public Color glassColor;
    public SpriteRenderer spriteRenderer;

    [HideInInspector] public List<Transform> targetPath;
    [HideInInspector] public int pathIndex;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetSpritColor(Color color)
    {
        glassColor = color;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    // Later you’ll call something like AnimateToPathPosition()
}
