using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using Color = System.Drawing.Color;

[RequireComponent(typeof(Rigidbody2D))]
public class DraggableBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 offset;
    private Collider2D col;
    [Header("Drag Settings")]
    private float moveSpeed = 15f; // Adjust this to control drag speed

     public Color32 spr;
    [SerializeField] public List<Transform> dropPos;
    private void OnEnable()
    {
        GameManager.SetOtherRigidBodys += SetRigidbodyConstraints;
    }
    private void OnDisable()
    {
        GameManager.SetOtherRigidBodys -= SetRigidbodyConstraints;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spr = GetComponentInChildren<SpriteRenderer>().color;
    }
    void Start()
    {
        rb.gravityScale = 0f;
        Vector3 closestGridPoint = GameManager.Instance.GetClosestGridPoint(transform.position);
        rb.MovePosition(closestGridPoint); // Moves using physics
    }
    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, 0f);
        GameManager.SetOtherRigidBodys?.Invoke(true);
    }
    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPos = new Vector3(mousePos.x, mousePos.y, 0f) + offset;

            // Smooth movement using MovePosition with speed
            Vector3 newPos = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
    }
    void OnMouseUp()
    {
        isDragging = false;
        GameManager.SetOtherRigidBodys?.Invoke(false);

        Vector3 closestGridPoint = GameManager.Instance.GetClosestGridPoint(transform.position);
        rb.MovePosition(closestGridPoint);

        int x = Mathf.RoundToInt((closestGridPoint.x - GameManager.Instance.startPoint.position.x) / GameManager.Instance.cellSize);
        int y = Mathf.RoundToInt(-(closestGridPoint.y - GameManager.Instance.startPoint.position.y) / GameManager.Instance.cellSize);

        //GameManager.Instance.blockGrid[x, y] = this;
        CheckColorMatch(x, y);
    }
    void SetRigidbodyConstraints( bool freezeAll)
    {
        rb.velocity = Vector3.zero;
        if (isDragging) return;
        if (freezeAll)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None; // Reset first
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<ColorBorder>(out var border))
        {
            if (spr == border.currentColor)
            {
                col.enabled = false;
                OnMouseUp();
                Debug.Log("Color dissolved!" + gameObject.name);
                GameManager.Instance.PlaceForTray(transform);
            }
        }
    }
    void CheckColorMatch(int x, int y)
    {
        ColorBorder border = FindBorderNear(x, y);
        if (border != null && border.currentColor == spr)
        {
            GameManager.Instance.blockGrid[x, y] = null;
            GameManager.Instance.PlaceForTray(transform);
            Debug.Log("Matched color and destroyed: " + gameObject.name);
        }
    }
    ColorBorder FindBorderNear(int gridX, int gridY)
    {
        float searchRadius = GameManager.Instance.cellSize * 0.6f; // Just a bit over half a cell
        Vector3 center = GameManager.Instance.startPoint.position + new Vector3(gridX * GameManager.Instance.cellSize, -gridY * GameManager.Instance.cellSize, 0f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, searchRadius);
        foreach (var hit in hits)
        {
            ColorBorder border = hit.GetComponent<ColorBorder>();
            if (border != null)
            {
                return border;
            }
        }
        return null;
    }

}
