using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum OctreeIndex
{
    LeftBottomBack = 0,  //000
    LeftBottomFront = 1, //001
    LeftTopBack = 2,     //010
    LeftTopFront = 3,    //011
    RightBottomBack = 4, //100
    RightBottomFront = 5,//101
    RightTopBack = 6,    //110
    RightTopFront = 7,    //111
}

public enum OctreeSideIndex
{
    Left = 0, //000
    Right = 1, //001
    Bottom = 2, //010
    Top = 3, //011
    Back = 4, //100
    Front = 5, //101
}

public class OctreeNew<TType>
{
    public float Size;
    public int Depth;
    public Node node;
    
    public void Initialize(Vector3 position, int depth, float size)
    {
        this.node = new OctreeNew<TType>.Node();
        this.Size = size;
        this.Depth = depth;
        this.node.Position = position;
        this.node.Depth = depth;
        this.node.NodeSize = size;
    }

    public void InsertGlobalData(Vector3 Position, TType Value)
    {
        node.InsertData(Position, Value);
    }
    

    public class Node
    {
        public Vector3 Position;
        public float NodeSize;
        public int Depth;
        TType Data;
        public Node[] Subnodes;
        public Node Root;

        public TType GetInfoAtPosition(Vector3 Ipos)
        {
            if (this.Subnodes == null) { return this.Data; }

            return this.Subnodes[GetIndexOfPosition(Ipos, this.Position)].GetInfoAtPosition(Ipos);
        }

        public void DenseFormat()
        {
            Subdivide();
            if (Subnodes != null)
            {
                foreach (Node x in Subnodes)
                {
                    x.DenseFormat();
                }
            }
        }

        public void InsertData(Vector3 Position, TType Value)
        {
            if (this.Depth == 0 || (this.Subnodes == null && Convert.ToString(this.Data) == Convert.ToString(Value)))
            {
                this.Data = Value;
                this.Optimize();
            }
            else if (this.Subnodes == null)
            {
                this.Subdivide();
                this.Subnodes[GetIndexOfPosition(Position, this.Position)].InsertData(Position, Value);
            }
            else
            {
                this.Subnodes[GetIndexOfPosition(Position, this.Position)].InsertData(Position, Value);
            }
        }

        public void Optimize()
        {
            this.Subnodes = null; //Assume this function will only be called if it is a leaf node or should be a leaf node
            if (this.Root != null)
            {
                bool finished = false;
                var RootNode = this.Root;

                for (int i = 0; i < 8; i++) //Evaluate along all subnodes
                {
                    if (Convert.ToString(RootNode.Subnodes[i].Data) != Convert.ToString(this.Data)|| RootNode.Subnodes[i].Subnodes != null) //Evaluate if nodes can be optimized
                    {
                        finished = true;
                    }
                }
                if (!finished)
                {
                    RootNode.Data = this.Data; //copy data to root cell
                    RootNode.Optimize();       //Traverse up tree
                }
            }
        }

        public void Subdivide()
        {
            if (this.Depth > 0) //Do not subdivide further if at max depth
            {
                Subnodes = new Node[8];
                
                for (int i = 0; i < 8; i++)
                {
                    Vector3 NewPos = Position;

                    if ((i & 4) == 4) { NewPos.x += NodeSize * 0.25f; }
                    else { NewPos.x -= NodeSize * 0.25f; }

                    if ((i & 2) == 2) { NewPos.y += NodeSize * 0.25f; }
                    else { NewPos.y -= NodeSize * 0.25f; }

                    if ((i & 1) == 1) { NewPos.z += NodeSize * 0.25f; }
                    else { NewPos.z -= NodeSize * 0.25f; }

                    Subnodes[i] = new Node();
                    Subnodes[i].Position = NewPos;
                    Subnodes[i].Depth = Depth - 1;
                    Subnodes[i].NodeSize = NodeSize * 0.5f;
                    Subnodes[i].Data = this.Data;
                    Subnodes[i].Root = this;
                }
            }
        }

        public IEnumerable<Node> GetLeafNodes(int LOD = 0)
        {
            if (this.Subnodes == null || this.Depth <= LOD)
            {
                yield return this;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    foreach (var IndividualNodes in this.Subnodes[i].GetLeafNodes(LOD))
                    {
                        yield return IndividualNodes;
                    }
                }
            }
        }

        //public IEnumerable<TType> GetLeafSides(int Side)
        //{
        //    if (this.Root == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        float maxDivision = NodeSize / Mathf.Pow(2, this.Depth);
        //        for (float i = -this.NodeSize + maxDivision; i < this.NodeSize + maxDivision; i += maxDivision)
        //        {
                    
                    
        //            yield return GetInfoAtPosition();
        //        }
        //    }
        //}
    }

    private static int GetIndexOfPosition(Vector3 loopkuPosition, Vector3 nodePosition)
    {
        int index = 0;
        // |= is the same as += in this case. It is used
        index |= loopkuPosition.x > nodePosition.x ? 4 : 0;
        index |= loopkuPosition.y > nodePosition.y ? 2 : 0;
        index |= loopkuPosition.z > nodePosition.z ? 1 : 0;

        return index;
    }



    //public IEnumerable<Node> GetAllLeafNodes() //A more global version of GetLeafNodes();
    //{
    //    return this.node.GetLeafNodes();
    //}
}
