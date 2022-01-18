using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLine : MonoBehaviour
{
    public Vector3[] points = new[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 5f, 10f), new Vector3(10f, 10f, 10f), new Vector3(10f, 15f, 0f),
                                        new Vector3(0f, 20f, 0f), new Vector3(0f, 25f, 10f), new Vector3(10f, 30f, 10f), new Vector3(10f, 35f, 0f),
                                        new Vector3(0f, 40f, 0f), new Vector3(0f, 45f, 10f), new Vector3(10f, 50f, 10f), new Vector3(10f, 55f, 0f)};
    public Color c1 = Color.white;
    public Color c2 = Color.red;

    public float speed = 100.0f;

    private double[,] B = new double[4, 4] { { -1, 3, -3, 1 },
                                    { 3, -6, 3, 0 },
                                    { -3, 0, 3, 0 },
                                    { 1, 4, 1, 0 }};

    private double[,] B_tang = new double[3, 4] { { -1, 3, -3, 1 },
                                                { 2, -4, 2, 0 },
                                                { -1, 0, 1, 0 }};

    private double[,] B_dder = new double[2, 4] { { -1, 3, -3, 1 }, { 1, -2, 2, 0 } };

    private List<Vector3> curvePoints = new List<Vector3>();
    private List<Vector3> tangentPoints = new List<Vector3>();
    private List<Vector3> rotationOs = new List<Vector3>();
    private List<double> angles = new List<double>();
    private List<Vector3> dderPoints = new List<Vector3>();
    private List<Vector3> drawPoints = new List<Vector3>();
    private int index;
    void Start()
    {
        index = 0;
        for (int i = 0; i < points.Length - 3; i++)
        {
            double[,] R = new double[4, 3] { { points[i].x, points[i].y, points[i].z }, { points[i+1].x, points[i+1].y, points[i+1].z },
                                            { points[i+2].x, points[i+2].y, points[i+2].z }, { points[i+3].x, points[i+3].y, points[i+3].z } };

            for (int k = 0; k < 100; k += 2)
            {
                float t = k / 100f;

                double[,] T = new double[1, 4] { { Mathf.Pow(t, 3) / 6f, Mathf.Pow(t, 2) / 6f, t / 6f, 1 / 6f } };
                double[,] T_tang = new double[1, 3] { { Mathf.Pow(t, 2)/2f, t/2f, 1/2f } };
                double[,] T_dder = new double[1, 2] { { t, 1 } };

                double[,] pt = Multiply(Multiply(T, B), R);
                double[,] pt_tang = Multiply(Multiply(T_tang, B_tang), R);
                double[,] pt_dder = Multiply(Multiply(T_dder, B_dder), R);

                Vector3 p = new Vector3((float)pt[0, 0], (float)pt[0, 1], (float)pt[0, 2]);
                Vector3 p_tang = new Vector3((float)pt_tang[0, 0], (float)pt_tang[0, 1], (float)pt_tang[0, 2]);
                Vector3 p_dder = new Vector3((float)pt_dder[0, 0], (float)pt_dder[0, 1], (float)pt_dder[0, 2]);
                //printVector(p);
                p_dder.z = (norma(p_dder) == 0f) ? 1f : p_dder.z;
                curvePoints.Add(p);
                tangentPoints.Add(p_tang);
                dderPoints.Add(p_dder);
                if( i==4 && k == 50)
                {
                    drawPoints.Add(p + p_tang);
                }
            }
        }
        drawBSpline(curvePoints, drawPoints);
        transform.position = curvePoints.ToArray()[0];
        Debug.Log("Trenutna orijentacija aviona je " + transform.GetChild(0).rotation.ToString());

        Vector3 current = new Vector3(0f, 0f, 1f);
        foreach (Vector3 t in tangentPoints)
        {
            Vector3 v = t.normalized;
            Vector3 os = Vector3.Cross(current, v);
            double se = Vector3.Dot(current, v);
            Debug.Log("Vektorski produkt vektora (0,0,1) i " + v.ToString() + " iznosi " + os.ToString() + ".");
            Debug.Log("Skalarni produkt vektora (0,0,1) i " + v.ToString() + " iznosi " + se.ToString() + ".");

            double cos = se / (double)(norma(current) * norma(v));
            Debug.Log("Acos iznosi " + cos.ToString());
            cos = (cos > 1) ? 1 : cos;
            cos = (cos < -1) ? -1 : cos;
            Debug.Log("Odnosno acos iznosi " + cos.ToString());
            float angle = Mathf.Acos((float)cos) * 180 / Mathf.PI;
            Debug.Log("Na kraju kut u stupnjevima iznosi " + angle.ToString());
            angles.Add(angle);
            rotationOs.Add(os);
        }

    }

    private void drawBSpline(List<Vector3> curvePoints, List<Vector3> drawPoints)
    {
        LineRenderer bspline = gameObject.AddComponent<LineRenderer>();
        bspline.material = new Material(Shader.Find("Sprites/Default"));
        bspline.widthMultiplier = 0.1f;
        bspline.positionCount = 452;
        bspline.SetPositions(curvePoints.ToArray());
        bspline.SetPositions(drawPoints.ToArray());
    }

    void Update()
    {
        float step =  speed * Time.deltaTime;
        //Quaternion currentRotation = transform.rotation;
        //Debug.Log("Trenutna rotacija je:" + current.ToString());
        if (index < 449)
        {

            //Vector3 dder = dderPoints.ToArray()[index];
            Vector3 os = rotationOs.ToArray()[index];
            double angle = angles.ToArray()[index];
            //Vector3 w = tangentPoints.ToArray()[index];
            //Vector3 u = Vector3.Cross(w, dder);
            //Vector3 v = Vector3.Cross(w, u);
            //double[,] R = new double[3, 3] { { w.x, u.x, v.x }, { w.y, u.y, v.y }, { w.z, u.z, v.z } };

            Vector3 nextPosition = curvePoints.ToArray()[index];
            //Quaternion rotacija = transform.rotation;
            //double[,] finalrotata = Multiply(new double[1, 3] { { rotacija.y, rotacija.z, rotacija.x } }, R);
            //rotacija.y = (float) finalrotata[0, 0];
            //rotacija.z = (float)finalrotata[0, 1]; 
            //rotacija.w = (float)finalrotata[0, 2];
            //Debug.Log("Finalna rotacija je " + rotacija.ToString());
            if (transform.position == nextPosition)
            {
                if (index == 0)
                    transform.Rotate(os, (float)angle);
                else
                {
                    //Debug.Log("Index je jednak " + index + ", os je " + os.ToString() + ", a razlika kuta je " + (angle - angles.ToArray()[index - 1]).ToString());
                    transform.Rotate(os, (float)(angle - angles.ToArray()[index - 1]));
                }
                index++;
            }

            transform.position = Vector3.MoveTowards(transform.position, curvePoints.ToArray()[index], step);
        }
    }

    //private void drawBSpline(List<Vector3> curvePoints)
    //{
    //    LineRenderer bspline = gameObject.AddComponent<LineRenderer>();
    //    bspline.material = new Material(Shader.Find("Sprites/Default"));
    //    bspline.widthMultiplier = 0.1f;
    //    bspline.positionCount = 450;

    //    bspline.SetPositions(curvePoints.ToArray());
    //}

    private double norma(Vector3 v)
    {
        return (double)Mathf.Sqrt((v.x * v.x + v.y * v.y + v.z * v.z));
    }

    public static double[,] Multiply(double[,] matrix1, double[,] matrix2)
    {
        // cahing matrix lengths for better performance  
        var matrix1Rows = matrix1.GetLength(0);
        var matrix1Cols = matrix1.GetLength(1);
        var matrix2Rows = matrix2.GetLength(0);
        var matrix2Cols = matrix2.GetLength(1);

        // checking if product is defined  
        if (matrix1Cols != matrix2Rows)
            throw new System.Exception
              ("Product is undefined. n columns of first matrix must equal to n rows of second matrix");

        // creating the final product matrix  
        double[,] product = new double[matrix1Rows, matrix2Cols];

        // looping through matrix 1 rows  
        for (int matrix1_row = 0; matrix1_row < matrix1Rows; matrix1_row++)
        {
            // for each matrix 1 row, loop through matrix 2 columns  
            for (int matrix2_col = 0; matrix2_col < matrix2Cols; matrix2_col++)
            {
                // loop through matrix 1 columns to calculate the dot product  
                for (int matrix1_col = 0; matrix1_col < matrix1Cols; matrix1_col++)
                {
                    product[matrix1_row, matrix2_col] +=
                      matrix1[matrix1_row, matrix1_col] *
                      matrix2[matrix1_col, matrix2_col];
                }
            }
        }

        return product;
    }

    private void printVector(Vector3 p)
    {
        Debug.Log("Vektor ima vrijednosti: " + p.x + ", " + p.y + ", " + p.z);
    }

    private void printMatrix(double[,] r)
    {
        for (int i = 0; i < r.GetLength(0); i++)
        {
            for (int j = 0; j < r.GetLength(1); j++)
            {
                Debug.Log(r[i, j]);
            }
            Debug.Log("next");
        }
    }
}
