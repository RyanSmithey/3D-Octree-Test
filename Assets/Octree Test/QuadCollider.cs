using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class QuadCollider : MonoBehaviour
{
    public QuadtreeComponentNew QuadC;
    public bool running = false;

    private BoxCollider[] Colliders;
    // Update is called once per frame
    void Start()
    {
        GenerateColliders();
    }


    public void GenerateColliders()
    {
        if (Colliders != null) { Reset(); }
        
        if (running) { return ; }
        running = true;

        List<Vector2> Positions = new List<Vector2>();
        List<float> Scales = new List<float>();

        int len = 0;

        foreach (QuadtreeNew<byte>.Node IndividualNode in QuadC.Quad.node.GetLeafNodes())
        {
            if (IndividualNode.Data != 0)
            {
                Positions.Add(IndividualNode.Position);
                Scales.Add(IndividualNode.NodeSize);
                len += 1;
            }
        }

        Colliders = new BoxCollider[len];

        StartCoroutine(AssignColliders(len, Positions, Scales));
        running = false;
    }

    public void Reset()
    {
        foreach (BoxCollider i in Colliders)
        {
            Destroy(i);
        }
    }


    IEnumerator AssignColliders(int len, List<Vector2> pos, List<float> SCALE)
    {
        BoxCollider CurrentCollider;

        int Iterations = 5;

        for (int i = 0; i < len - Iterations; i += Iterations + 1)
        {
            for (int j = i; j < Mathf.Min((i + Iterations), len); j++)
            {
                CurrentCollider = gameObject.AddComponent<BoxCollider>();

                CurrentCollider.center = new Vector3(pos[j].x, pos[j].y, 0);
                CurrentCollider.size = new Vector3(SCALE[j], SCALE[j], 1);
                Colliders[i] = CurrentCollider;
            }
            yield return null;
        }
    }

}
