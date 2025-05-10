using System;
using UnityEngine;

public class Diagram : MonoBehaviour
{
    [SerializeField] private Material matRespectful;
    [SerializeField] private Material matDisrespectful;
    [SerializeField] private Material matAll;
    [SerializeField] private Material matValue;
    [SerializeField] private Material matMaxValue;
    [SerializeField] private Material matBackGround;
    [SerializeField] private int CountVertices;
    [SerializeField] private float[] figureRespectfulProcent, figureDisrespectfulProcent;
    [SerializeField] private GameObject prefabFigure;
    [SerializeField] private GameObject prefabLine;
    [SerializeField] private GameObject prefabCircel;
    [SerializeField] private Color[] colors;


    private void OnValidate() {
        //CreateFigure(CountVertices, figureRespectfulProcent, figureDisrespectfulProcent);
    }
    private void CreateFigure(int countVertices, float[] figureRespectfulProcent, float[] figureDisrespectfulProcent) {
        Vector3 linePosition = new Vector3(transform.position.x, transform.position.y);
        Debug.Log(linePosition);
        DestoroyChildObject(this.gameObject);
        GameObject backGround = gameObject;

        GameObject figureRespectful = Instantiate(prefabFigure, this.transform);
        figureRespectful.transform.localPosition = new(0, 0, -0.2f);
        GameObject figureDisrespectful = Instantiate(prefabFigure, this.transform);
        figureDisrespectful.transform.localPosition = new(0, 0, -0.3f);
        GameObject figureAll = Instantiate(prefabFigure, this.transform);
        figureAll.transform.localPosition = new(0, 0, -0.15f);

        GameObject maxValue = Instantiate(prefabFigure, this.transform);
        maxValue.transform.localPosition = new(0, 0, -0.1f);
        GameObject halfValue = Instantiate(prefabFigure, this.transform);
        halfValue.transform.localPosition = new(0, 0, -0.1f);
        GameObject quarterValue = Instantiate(prefabFigure, this.transform);
        quarterValue.transform.localPosition = new(0, 0, -0.1f);

        int[] triangles = Triangles(CountVertices);
        int[] trianglesContour = TrianglesContour(CountVertices);
        Vector3[] vertices = Vertices(fillingArray(countVertices, 1f));
        for (int i = 0; i < countVertices; i++)
        {
            GameObject Circle = Instantiate(prefabCircel, this.transform);
            Circle.GetComponent<SpriteRenderer>().color = colors[i];
            Circle.transform.localPosition = vertices[i + 1];
            GameObject Line = Instantiate(prefabLine, this.transform);
            LineRenderer lineRenderer = Line.GetComponent<LineRenderer>();
            lineRenderer.endColor = colors[i];
            lineRenderer.SetPosition(1, vertices[i + 1] + linePosition);
            lineRenderer.SetPosition(0, linePosition);
        }
        for (int i = 0; i < countVertices; i++)
        {
            GameObject Line = Instantiate(prefabLine, this.transform);
            LineRenderer lineRenderer = Line.GetComponent<LineRenderer>();
            lineRenderer.startColor = colors[i];
            lineRenderer.SetPosition(0, vertices[i + 1] + linePosition);

            if (i == countVertices - 1)
            {
                lineRenderer.endColor = colors[0];
                lineRenderer.SetPosition(1, vertices[1] + linePosition);
            }
            else
            {
                lineRenderer.endColor = colors[i + 1];
                lineRenderer.SetPosition(1, vertices[i + 2] + linePosition);
            }
        }
        FilingMesh(backGround, matBackGround, vertices, triangles);
        //FilingMesh(maxValue, matMaxValue, Vertices(fillingArray(countVertices, 1f), true), trianglesContour);
        FilingMesh(halfValue, matValue, Vertices(fillingArray(countVertices, 0.5f), true), trianglesContour);
        FilingMesh(quarterValue, matValue, Vertices(fillingArray(countVertices, 0.25f), true), trianglesContour);
        FilingMesh(figureRespectful, matRespectful, Vertices(figureRespectfulProcent), triangles);
        FilingMesh(figureDisrespectful, matDisrespectful, Vertices(figureDisrespectfulProcent), triangles);
        FilingMesh(figureAll, matAll, Vertices(SumArray(figureRespectfulProcent, figureDisrespectfulProcent)), triangles);


    }
    private float[] fillingArray(int countElement, float number) {
        float[] array = new float[countElement];
        for (int i = 0; i < countElement; i++)
        {
            array[i] = number;
        }
        return array;
    }
    private float[] SumArray(float[] figureRespectfulProcent, float[] figureDisrespectfulProcent) {
        float[] array = new float[figureRespectfulProcent.Length];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = figureRespectfulProcent[i] + figureDisrespectfulProcent[i];
        }
        return array;
    }

    private void FilingMesh(GameObject gameObject, Material material, Vector3[] vertices, int[] triangles) {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = material;
    }

    private Vector3[] Vertices(float[] procent, bool isContour = false) {
        float vertecsDublicateProcent = 0.95f;
        int countVertices = procent.Length;
        int countureVertecsCount = isContour ? countVertices : 1;

        Vector3[] vectors = new Vector3[countVertices + countureVertecsCount];
        if (!isContour)
            vectors[0] = new Vector2(0, 0);

        float AngleRadianVertices = (2 * 3.14f) / countVertices;
        float AngleRadianPart = 3.14f / 2f;
        float totalAngle = 0;
        int part = 0;
        bool partBool = false;
        for (int i = 1, k = 0; k < countVertices; i++, k++)
        {
            if (isContour && i != k) i = k;
            vectors[i] = new();
            if (totalAngle > AngleRadianPart)
            {
                totalAngle -= AngleRadianPart;
                part++;
                partBool = !partBool;
            }
            vectors[i].y = MathF.Round(partBool ? MathF.Cos(AngleRadianPart - totalAngle) : MathF.Cos(totalAngle), 2) * procent[k];
            vectors[i].x = MathF.Round(partBool ? MathF.Cos(totalAngle) : MathF.Cos(AngleRadianPart - totalAngle), 2) * procent[k];

            if (part == 0 || part == 1)
                vectors[i].x *= -1;
            if (part == 1 || part == 2)
                vectors[i].y *= -1;
            if (isContour) vectors[countVertices + i] = vectors[i] * vertecsDublicateProcent;

            totalAngle += AngleRadianVertices;
        }
        return vectors;
    }

    private int[] Triangles(int countVertices) {
        int[] triangles = new int[countVertices * 3];

        for (int i = 0; i < countVertices; i++)
        {
            triangles[3 * i + 0] = 0;
            triangles[3 * i + 1] = i + 1;
            if (3 * i + 2 == triangles.Length - 1)
                triangles[3 * i + 2] = 1;
            else
                triangles[3 * i + 2] = i + 2;
        }

        return triangles;
    }
    private int[] TrianglesContour(int countVertices) {
        int[] triangles = new int[countVertices * 2 * 3];

        for (int i = 0; i < countVertices; i++)
        {
            triangles[6 * i + 0] = i;
            triangles[6 * i + 1] = countVertices + i;
            triangles[6 * i + 3] = i;
            if (i == countVertices - 1)
            {
                triangles[6 * i + 2] = countVertices + 0;
                triangles[6 * i + 4] = 0;
                triangles[6 * i + 5] = countVertices + 0;
            }
            else
            {
                triangles[6 * i + 2] = countVertices + i + 1;
                triangles[6 * i + 4] = i + 1;
                triangles[6 * i + 5] = countVertices + i + 1;
            }
        }

        return triangles;
    }
    public static void DestoroyChildObject(GameObject parent, int indexStart = 0, int indexEnd = int.MaxValue) {
        int personCellCount = parent.transform.childCount;
        if (indexEnd > personCellCount) indexEnd = personCellCount - 1;
        for (int i = indexEnd; i >= indexStart; i--)
        {
            if (parent.transform.childCount <= indexStart)
            {
                Debug.LogError("����������� parentPerson.childCount �� ���������");
                return;
            }
            GameObject chiled = parent.transform.GetChild(i).gameObject;
            chiled.transform.SetParent(null);
            Destroy(chiled);
        }
    }
}
