using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Klasa odpowiadaj�ca za wczytywanie plik�w
/// </summary>
public class FileManagerScript : MonoBehaviour
{
    private string path;
    private string newPath;
    private LogicManager logic;

    public void Start()
    {
        logic = GetComponent<LogicManager>();
    }

    /// <summary>
    /// Metoda otwiera eksplorator plik�w
    /// </summary>
    public void LoadFile()
    {
        newPath = EditorUtility.OpenFilePanel("File loader", " ", "*");
    }

    /// <summary>
    /// Metoda wykrywa czy u�ytkownik wskaza� now� �cie�k� do pliku lub nie poda� �adnej
    /// </summary>
    public void Update()
    {
        if (newPath != null)
        {
            //niszczenie aktualnie wy�wietlanych punkt�w
            foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("Point"))
            {
                Destroy(gameObject);
            }
            path = newPath;
            newPath = null;

            //wczytywanie nowych danych
            if(path != "")
            {
                string[] lines = File.ReadAllLines(path);
                logic.ReadPoints(lines);
            }
            //czyszczenie wykresu rozrzutu w przypadku pustej �cie�ki
            else
            {
                logic.ClearPoints();
            }
        }
    }
}
