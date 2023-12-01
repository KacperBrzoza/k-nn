using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasa reprezentuj¹ca kwadrat (obserwacjê testow¹)
/// </summary>
public class SquareScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Metoda ustawia kolor w zale¿noœci do której kategorii nale¿y testowa obserwacja
    /// </summary>
    /// <param name="color">Wartoœæ zmiennej celu</param>
    public void SetColor(int color)
    {
        _ = new Color();
        Color col;
        if(color == -1)
        {
            col = Color.black;
        }
        else if (color == 0)
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
        spriteRenderer.color = col;
    }
}
