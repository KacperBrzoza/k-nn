using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skrypt przestrzeni rysowania punktów (wykresu rozrzutu)
/// </summary>
public class AreaScript : MonoBehaviour
{

    public GameObject square;
    public BoxCollider2D col;
    public LogicManager manager;

    public void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LogicManager>();
    }


    /// <summary>
    /// Reakcja na klikniêcie myszk¹ na wykres rozrzutu
    /// </summary>
    public void OnMouseDown()
    {
        Vector3 mousePositionPixels = Input.mousePosition;

        Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(mousePositionPixels);

        float x = mousePositionWorld.x;
        float y = mousePositionWorld.y;

        square.transform.position = new Vector3(x, y, 0);

        manager.SetTestPoint(x, y);
    }
}
