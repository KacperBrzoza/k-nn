using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa reprezentuj�ca wy�wietlane obserwacje
/// </summary>
public class PointScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    int color = -1;

    public Image imageColor;
    public Text textValue;

    void Start()
    {
        GetColorFromName();
        SetColor();
        imageColor = GameObject.FindGameObjectWithTag("OutputColor").GetComponent<Image>();
        textValue = GameObject.FindGameObjectWithTag("OutputValue").GetComponentInChildren<Text>();
    }

    /// <summary>
    /// Metoda "wyci�gaj�ca" warto�� zmiennej celu z w�asnego imienia
    /// </summary>
    private void GetColorFromName()
    {
        string[] parts = this.name.Split(' ');
        color = int.Parse(parts[3]);
    }


    /// <summary>
    /// Metoda ustawia kolor obserwacji w zale�no�ci od warto�ci zmiennej celu
    /// </summary>
    private void SetColor()
    {
        _ = new Color();
        Color col;
        if (color == 0)
        {
            col = Color.cyan;
        }
        else if (color == 1)
        {
            col = Color.red;
        }
        else if (color == 2)
        {
            col = Color.yellow;
        }
        else if (color == 3)
        {
            col = Color.green;
        }
        else if (color == 4)
        {
            col = Color.magenta;
        }
        else
        {
            col = Color.blue;
        }
        spriteRenderer.color = new Color(col.r, col.g, col.b);
    }

    /// <summary>
    /// Wy�wietlanie informacji w powi�kszeniu po najechaniu na obserwacj�
    /// </summary>
    public void OnMouseEnter()
    {
        if(GetComponentInChildren<TextMeshPro>().text != "")
        {
            imageColor.color = spriteRenderer.color;
            textValue.text = GetComponentInChildren<TextMeshPro>().text;
        }
    }

    /// <summary>
    /// Usuwanie powi�kszonych informacji po opuszczeniu obserwacji przez kursor
    /// </summary>
    public void OnMouseExit()
    {
        imageColor.color = Color.white;
        textValue.text = "";
    }
}
