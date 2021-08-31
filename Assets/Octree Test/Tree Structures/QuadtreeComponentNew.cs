using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


public class QuadtreeComponentNew : MonoBehaviour
{
    public int Depth = 0;
    public float Size = 5f;

    public int LOD = 0;
    public QuadtreeNew<byte> Quad;

    public float PerlinScale = .03f;

    private bool InPlay = false;

    private QuadRenderer Renderer;
    private QuadCollider Collider;

    void Awake()
    {
        Renderer = gameObject.GetComponent<QuadRenderer>();
        Collider = gameObject.GetComponent<QuadCollider>();

        InPlay = true;
        //byte[,] TotalMapData = GenMapInfo(Depth);

        Quad = new QuadtreeNew<byte>();
        Quad.Initialize(gameObject.transform.position, Depth, Size);
        Vector2 ArrayOrigin = new Vector2(Quad.Size * 0.5f * -1, Quad.Size * 0.5f * -1);

        SetMapData();
    }

    public void UpdateMap()
    {
        //Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();
        Collider.GenerateColliders();
        //stopWatch.Stop();
        //UnityEngine.Debug.Log("GenerateColliders" + stopWatch.ElapsedMilliseconds);

        
        Renderer.CreateMesh();
        
    }



    public void SetMapData()
    {
        float StepSize = Size / Mathf.Pow(2, Depth);

        float offset = 102.658f;

        for (float i = -Size; i < Size; i += StepSize)
        {
            for (float j = -Size; j < Size; j += StepSize)
            {
                byte newinfo = 0;

                if (Mathf.PerlinNoise((i + offset) * PerlinScale, (j + offset) * PerlinScale) > 0.5f)
                {
                    newinfo = 4;
                }

                Quad.InsertGlobalData(new Vector2(i, j), newinfo);

            }
        }
    }

    void OnDrawGizmos()
    {
        if (InPlay)
        {
            DrawNodes(Quad.node);
        }
    }

    private void DrawNodes(QuadtreeNew<byte>.Node FirstNode)
    {
        if (FirstNode.Subnodes != null)
        {
            for (int i = 0; i < 4; i++)
            {
                DrawNodes(FirstNode.Subnodes[i]);
            }
        }

        Gizmos.DrawWireCube(FirstNode.Position, new Vector3(1, 1, 0) * FirstNode.NodeSize);
        Gizmos.color = new Color(1, 1, 1, 1);
    }


    public void InsertBulk(Vector2 ArrayOrigin, byte[,] Insert)
    {
        float StepSize = Size / Mathf.Pow(2, Depth);


        for (float i = ArrayOrigin.x, I = 0; i < ArrayOrigin.x + StepSize * Insert.Length; i += StepSize, I++)
        {
            for (float j = ArrayOrigin.y, J = 0; j < ArrayOrigin.y + StepSize * Insert.Length; j += StepSize, J++)
            {
                Quad.node.InsertData(new Vector2(i + (StepSize / 2), j + (StepSize / 2)), Insert[(int)I, (int)J]);
            }
        }
        UpdateMap();
    }
}
