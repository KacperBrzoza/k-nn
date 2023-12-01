using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Klasa reprezentuj¹ca punkt (pojedyncz¹ obserwacjê)
/// </summary>
public class Point
{
    public float x; //predyktor 1
    public float y; //predyktor 2
    public int val; //zmienna celu

    public Point() { }

    /// <summary>
    /// Konstruktor buduj¹cy punkt na podstawie wczytanych danych
    /// </summary>
    /// <param name="line"></param>
    public Point(string line)
    {
        string[] parts = line.Split(',');
        x = float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
        y = float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
        val = int.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
    }
}
