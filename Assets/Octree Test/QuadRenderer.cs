using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadRenderer : MonoBehaviour
{
    public QuadtreeComponentNew QuadC;
    private float StepSize = 0;

    public Mesh FinalMesh;
    public MeshFilter filter;


    // Start is called before the first frame update
    void Start()
    {
        FinalMesh = new Mesh();
        filter.mesh = FinalMesh;
        StepSize = QuadC.Size / Mathf.Pow(2, QuadC.Depth);

        CreateMesh();
    }


    public void CreateMesh()
    {
        List<Vector3> Verts = new List<Vector3>();
        List<int> Tris = new List<int>();
        List<Vector2> UVS = new List<Vector2>();
        List<Color> colors = new List<Color>(); 

        int VertIndex = 0;
        bool BuildSide;

        Vector3 SidePosition;
        Vector3 DeltaSide;

        foreach (QuadtreeNew<byte>.Node IndividualNode in QuadC.Quad.node.GetLeafNodes())
        {
            
            if (IndividualNode.Data != 0)
            {
                float NStepSize = (IndividualNode.NodeSize/2);
                float NSize = IndividualNode.NodeSize;

                Vector3 InitialPos = new Vector3(IndividualNode.Position.x - NStepSize, IndividualNode.Position.y - NStepSize, 0);
                Verts.Add(InitialPos);
                Verts.Add(InitialPos + Vector3.up * NSize);
                Verts.Add(InitialPos + Vector3.right * NSize);
                Verts.Add(InitialPos + new Vector3(1, 1, 0) * NSize);

                Tris.Add((int)(VertIndex + 0));
                Tris.Add((int)(VertIndex + 1));
                Tris.Add((int)(VertIndex + 2));

                Tris.Add((int)(VertIndex + 3));
                Tris.Add((int)(VertIndex + 2));
                Tris.Add((int)(VertIndex + 1));

                UVS.Add((Verts[VertIndex + 0] / QuadC.Size) + Vector3.one * .5f);
                UVS.Add((Verts[VertIndex + 1] / QuadC.Size) + Vector3.one * .5f);
                UVS.Add((Verts[VertIndex + 2] / QuadC.Size) + Vector3.one * .5f);
                UVS.Add((Verts[VertIndex + 3] / QuadC.Size) + Vector3.one * .5f);

                colors.Add(new Color(IndividualNode.Data - 1, 0, 0));
                colors.Add(new Color(IndividualNode.Data - 1, 0, 0));
                colors.Add(new Color(IndividualNode.Data - 1, 0, 0));
                colors.Add(new Color(IndividualNode.Data - 1, 0, 0));

                VertIndex += 4;

                for (int i = 0; i < 4; i++)
                {

                    BuildSide = false;
                    foreach (byte Side in IndividualNode.ReturnLeafSides(i))
                    {
                        if (Side == 0)
                        {
                            BuildSide = true;
                        }
                    }
                    if (true)
                    {
                        SidePosition = Vector3.zero;
                        Vector3 Origin = new Vector3(IndividualNode.Position.x, IndividualNode.Position.y, 0);
                        if ((i & 2) == 2)
                        {
                            SidePosition += new Vector3(IndividualNode.NodeSize * 0.5f, IndividualNode.NodeSize * 0.5f, 0);
                            DeltaSide = new Vector3(0, IndividualNode.NodeSize, 0);
                        }
                        else
                        {
                            SidePosition += new Vector3(IndividualNode.NodeSize * 0.5f, -IndividualNode.NodeSize * 0.5f, 0);
                            DeltaSide = new Vector3(IndividualNode.NodeSize, 0, 0);
                        }
                        if ((i & 1) != 1)
                        {
                            SidePosition *= -1f;
                        }
                        else
                        {
                            DeltaSide *= -1f;
                        }


                        Verts.Add(SidePosition + Origin);
                        Verts.Add(SidePosition + Origin + Vector3.forward);
                        Verts.Add(SidePosition + Origin + DeltaSide);
                        Verts.Add(SidePosition + Origin + DeltaSide + Vector3.forward);

                        Tris.Add((int)(VertIndex + 0));
                        Tris.Add((int)(VertIndex + 1));
                        Tris.Add((int)(VertIndex + 2));

                        Tris.Add((int)(VertIndex + 3));
                        Tris.Add((int)(VertIndex + 2));
                        Tris.Add((int)(VertIndex + 1));

                        UVS.Add(new Vector2(0, 0));
                        UVS.Add(new Vector2(0, .25f));
                        UVS.Add(new Vector2(IndividualNode.NodeSize / QuadC.Size, 0));
                        UVS.Add(new Vector2(IndividualNode.NodeSize / QuadC.Size, .25f));

                        colors.Add(new Color(1, 0, 0));
                        colors.Add(new Color(1, 0, 0));
                        colors.Add(new Color(1, 0, 0));
                        colors.Add(new Color(1, 0, 0));

                        VertIndex += 4;
                    }
                }
            }

        }

        FinalMesh.SetVertices(Verts);
        FinalMesh.triangles = Tris.ToArray();
        FinalMesh.uv = UVS.ToArray();
        FinalMesh.SetColors(colors);

        FinalMesh.RecalculateNormals();
    }


}
