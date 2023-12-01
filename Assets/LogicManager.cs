using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Klasa odpowiadaj¹ca za logikê programu
/// </summary>
public class LogicManager : MonoBehaviour
{

    //elementy UI
    public Text number;
    public Slider slider;
    public BoxCollider2D col;
    public Transform areaTransform;
    public Toggle metric;
    public Toggle voting;
    public SquareScript squareScript;

    //prefab punktu do instancjowania
    public GameObject p;

    /// <summary>
    /// Zbiór ucz¹cy
    /// </summary>
    private HashSet<Point> points;

    /// <summary>
    /// Iloœci poszczególnych typów obserwacji
    /// </summary>
    private int[] typesQuantities = new int[6];

    //krawêdzie "wykresu rozrzutu" czyli obszaru roboczego (Area)
    private float leftEdge;
    private float rightEdge;
    private float bottomEdge;
    private float topEdge;

    /// <summary>
    /// Liczba s¹siadów
    /// </summary>
    int neighbors = 1;
    bool isEuclideanMetric = true;
    bool isSimpleVoting = true;

    //wspó³rzêdne punktu testowego
    private float xTest = 0, yTest = 0;

    void Start()
    {
        number = GameObject.FindGameObjectWithTag("NN").GetComponent<Text>();
        slider = GameObject.FindGameObjectWithTag("Slider").GetComponent <Slider>();
        col = GameObject.FindGameObjectWithTag("Area").GetComponent<BoxCollider2D>();
        areaTransform = GameObject.FindGameObjectWithTag("Area").GetComponent<Transform>();
        metric = GameObject.FindGameObjectWithTag("Metric").GetComponent<Toggle>();
        voting = GameObject.FindGameObjectWithTag("Voting").GetComponent<Toggle>();
        squareScript = GameObject.FindGameObjectWithTag("Square").GetComponent<SquareScript>();

        leftEdge = col.bounds.min.x;
        rightEdge = col.bounds.max.x;
        bottomEdge = col.bounds.min.y;
        topEdge = col.bounds.max.y;
        points = new HashSet<Point>();

    }

    /// <summary>
    /// Metoda obs³uguj¹ca ruch suwaka liczby s¹siadów
    /// </summary>
    public void SetNeighborsNumber()
    {
        neighbors = (int) slider.value;
        number.text = $"{neighbors}";

        KNearestNeighbors();
    }

    /// <summary>
    /// Metoda obs³uguj¹ca zmianê trybu metryki
    /// </summary>
    public void ChangeMetricType()
    {
        if (metric.isOn)
        {
            isEuclideanMetric = true;
        }
        else
        {
            isEuclideanMetric = false;
        }

        KNearestNeighbors();
    }

    /// <summary>
    /// Metoda obs³uguj¹ca zmianê trybu g³osowania
    /// </summary>
    public void ChangeVotingType()
    {
        if(voting.isOn)
        {
            isSimpleVoting = true;
        }
        else
        {
            isSimpleVoting = false;
        }

        KNearestNeighbors();
    }

    /// <summary>
    /// Metoda obs³uguj¹ca zmianê po³o¿enia punktu wskazanego przez u¿ytkownika
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetTestPoint(float x, float y)
    {
        squareScript = GameObject.FindGameObjectWithTag("Square").GetComponent<SquareScript>();
        xTest = x;
        yTest = y;

        KNearestNeighbors();
    }

    /// <summary>
    /// Metoda wczytuj¹ca nowe punkty zbioru ucz¹cego
    /// </summary>
    /// <param name="lines">Dane w formie tektsowej</param>
    public void ReadPoints(string[] lines)
    {

        points = new HashSet<Point>();

        for (int i = 0; i < 6; i++)
        {
            typesQuantities[i] = 0;
        }

        foreach (string line in lines)
        {
            Point point = new Point(line);
            typesQuantities[point.val]++;
            points.Add(point);
        }

        NormalizePoints();
        InstantiatePoints();
        KNearestNeighbors();
    }

    /// <summary>
    /// Czyœci wektor punktów i ustawia kolor kwadratu na domyœlny
    /// </summary>
    public void ClearPoints()
    {
        squareScript.SetColor(-1);
        points.Clear();
    }

    /// <summary>
    /// Metoda normalizuje punkty w <see cref="points"/>
    /// </summary>
    private void NormalizePoints()
    {
        float maxX, maxY;
        float minX, minY;
        Point firstPoint = points.First();
        minX = firstPoint.x;
        minY = firstPoint.y;
        maxX = firstPoint.x;
        maxY = firstPoint.y;

        //szukanie min i max wspó³rzêdnych x i y
        foreach(Point point in points)
        {
            if(point.x > maxX)
            {
                maxX = point.x;
            }
            else if(point.x < minX)
            {
                minX = point.x;
            }

            if(point.y > maxY)
            {
                maxY = point.y;
            }
            else if( point.y < minY)
            {
                minY = point.y;
            }
        }

        //skorygowana normalizacja x i y
        HashSet<Point> normalizedPoints = new HashSet<Point>();
        foreach(Point point in points)
        {
            Point normalized = new Point();
            normalized.x = (2 * (point.x - minX) / (maxX - minX)) - 1;
            normalized.y = (2 * (point.y - minY) / (maxY - minY)) - 1;


            //przeskalowanie znormalizowanego punktu do wymiarów p³ótna
            normalized.x = normalized.x * (rightEdge - leftEdge)/2 + areaTransform.position.x;
            normalized.y = normalized.y * (topEdge - bottomEdge)/2 + areaTransform.position.y;

            normalized.val = point.val;
            normalizedPoints.Add(normalized);
        }

        points = normalizedPoints;
    }

    /// <summary>
    /// Metoda drukuje punkty zbioru ucz¹cego na ekran
    /// </summary>
    private void InstantiatePoints()
    {
        foreach(Point point in points)
        {
            var newP = Instantiate(p, new Vector3(point.x, point.y, 0), new Quaternion());
            newP.name = $"Point {point.x} {point.y} {point.val}";
        }
    }

    /// <summary>
    /// Implementacja algorytmu k-nn
    /// </summary>
    public void KNearestNeighbors()
    {
        //wstêpne ustawienie pustego tekstu (dystansu) wszystkim punktom
        GameObject[] visiblePoints = GameObject.FindGameObjectsWithTag("Point");
        foreach (GameObject visiblePoint in visiblePoints)
        {
            visiblePoint.GetComponentInChildren<TextMeshPro>().text = "";
        }

        //algorytm zadzia³a, jeœli s¹ wprowadzone jakieœ obserwacje
        if (points.Count != 0 && points.Count > neighbors)
        {

            List<Tuple<Point, float>> pointsWithDistances = new List<Tuple<Point, float>>();
            //dla ka¿dego punktu ze zbioru ucz¹cego...
            foreach (Point point in points)
            {
                //...obliczanie odleg³oœci...
                float distance;
                if (isEuclideanMetric)
                {
                    //...euklidesowej
                    distance = (float)Math.Sqrt(Math.Pow(point.x - xTest, 2) + Math.Pow(point.y - yTest, 2));
                }
                else
                {
                    //...miejskiej
                    distance = Math.Abs(point.x - xTest) + Math.Abs(point.y - yTest);
                }

                pointsWithDistances.Add(new Tuple<Point, float>(point, distance));
            }

            //sortowanie s¹siadów rosn¹co wzglêdem dystansu
            pointsWithDistances.Sort(new MyTupleComparer());

            //przygotowanie tablicy do g³osowania
            int[] votes = new int[6];
            for (int i = 0; i < 6; i++)
            {
                votes[i] = 0;
            }

            //g³osowanie wœród najbli¿szych s¹siadów
            for (int i = 0; i < neighbors; i++)
            {
                votes[pointsWithDistances.ElementAt(i).Item1.val]++;
            }

            //szukanie, do którego typu nale¿y nowa obserwacja wraz ze sprawdzeniem czy nie wyst¹pi³ remis
            bool isRemis = false;
            int winner = 0;
            for (int i = 1; i < 6; i++)
            {
                if (votes[i] > winner)
                {
                    isRemis = false;
                    winner = i;
                }
                else if (votes[i] == winner)
                {
                    isRemis = true;
                }
            }

            //w przypadku remisu stosowane jest g³osowanie...
            if (isRemis)
            {
                //...proste
                if (isSimpleVoting)
                {
                    int mostNumerous = winner;
                    for(int i = 0; i < 6; i++)
                    {
                        if(i != winner)
                        {
                            if (votes[i] == votes[winner])
                            {
                                if (typesQuantities[i] > typesQuantities[winner])
                                {
                                    mostNumerous = i;
                                }
                            }
                        }
                    }
                    winner = mostNumerous;
                }
                //...wa¿one
                else
                {
                    float[] weights = new float[6];
                    for (int i = 0; i < 6; i++)
                    {
                        weights[i] = 0;
                        if (votes[i] != votes[winner])
                        {
                            votes[i] = -1;
                        }
                    }

                    for(int i = 0; i < neighbors; i++)
                    {
                        if (votes[pointsWithDistances.ElementAt(i).Item1.val] != -1)
                        {
                            float weight = (float)(1 / Math.Pow(pointsWithDistances.ElementAt(i).Item2, 2));
                            weights[pointsWithDistances.ElementAt(i).Item1.val] += weight;
                        }
                    }

                    winner = 0;
                    for(int i = 1; i < 6; i++)
                    {
                        if (weights[i] > weights[winner])
                        {
                            winner = i;
                        }
                    }

                }
            }

            //ustawienie koloru kwadracika w zale¿noœci do której kategorii nale¿y
            squareScript.SetColor(winner);
            
            //ustawienie widocznego dystansu s¹siadom, którzy brali udzia³ w g³osowaniu
            for(int i = 0; i < neighbors; i++)
            {
                Point point = pointsWithDistances.ElementAt(i).Item1;
                float distance = pointsWithDistances.ElementAt(i).Item2;
                GameObject visiblePoint = GameObject.Find($"Point {point.x} {point.y} {point.val}");
                visiblePoint.GetComponentInChildren<TextMeshPro>().text = "" + Math.Round(distance, 2);
            }
            
        }
        

    }


    /// <summary>
    /// Klasa pomocnicza implementuj¹ca porównywanie obiektów typu Tuple<Point, float>
    /// </summary>
    private class MyTupleComparer : IComparer<Tuple<Point, float>>
    {
        public int Compare(Tuple<Point, float> x, Tuple<Point, float> y)
        {
            if (x.Item2 <= y.Item2)
                return -1;
            else
                return 1;
        }
    }
}

