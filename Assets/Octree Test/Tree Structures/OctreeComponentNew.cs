using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeComponentNew : MonoBehaviour
{
    public int Depth = 0;
    public float Size = 5f;

    public GameObject[] Points;
    public int LOD = 0;

    public bool Generate = false;

    void OnDrawGizmos()
    {
        OctreeNew<bool> OCT = new OctreeNew<bool>();
        OCT.Initialize(gameObject.transform.position, Depth, Size);
        foreach (GameObject x in Points)
        {
            OCT.InsertGlobalData(x.transform.position, true);
        }


        DrawNodes(OCT.node);
    }

    private void DrawNodes(OctreeNew<bool>.Node FirstNode)
    {
        //if (FirstNode.Subnodes != null)
        //{
        //    for (int i = 0; i < 8; i++)
        //    {
        //        DrawNodes(FirstNode.Subnodes[i]);
        //    }
        //}

        //Gizmos.color = new Color(1, 1, 1, 1);
        //Gizmos.DrawWireCube(FirstNode.Position, Vector3.one * FirstNode.NodeSize);

        foreach (OctreeNew<bool>.Node ASDF in FirstNode.GetLeafNodes(LOD))
        {
            Gizmos.DrawWireCube(ASDF.Position, Vector3.one * ASDF.NodeSize);
        }

        Gizmos.color = new Color(1, 1, 1, 1);
    }
}
