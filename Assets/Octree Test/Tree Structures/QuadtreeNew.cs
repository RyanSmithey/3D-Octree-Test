using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuadtreeIndex
{
    LeftBottom = 0,  //00
    LeftTop = 1,     //01
    RightBottom = 2, //10
    RightTop = 3,    //11
}

public enum QuadtreeSideIndex
{
    Bottom = 0,//00
    Top = 1,   //01
    Left = 2,  //10
    Right = 3, //11
}

public class QuadtreeNew<TType>
{
    public float Size;
    public int Depth;
    public Node node;

    public void Initialize(Vector2 position, int depth, float size)
    {
        this.node = new QuadtreeNew<TType>.Node();
        this.Size = size;
        this.Depth = depth;
        this.node.Position = position;
        this.node.Depth = depth;
        this.node.NodeSize = size;
    }

    public void InsertGlobalData(Vector2 Position, TType Value)
    {
        node.InsertData(Position, Value);
    }


    public class Node
    {
        public Vector2 Position;
        public float NodeSize;
        public int Depth;
        public TType Data;
        public Node[] Subnodes;
        public Node Root;

        public TType GetInfoAtPosition(Vector2 Ipos)
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

        public void InsertData(Vector2 Position, TType Value)
        {

            if (this.Depth == 0)
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

                for (int i = 0; i < 4; i++) //Evaluate along all subnodes
                {
                    if (Convert.ToString(RootNode.Subnodes[i].Data) != Convert.ToString(this.Data) || RootNode.Subnodes[i].Subnodes != null) //Evaluate if nodes can be optimized
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
                Subnodes = new Node[4];

                for (int i = 0; i < 4; i++)
                {
                    Vector2 NewPos = Position;

                    if ((i & 2) == 2) { NewPos.x += NodeSize * 0.25f; }
                    else { NewPos.x -= NodeSize * 0.25f; }

                    if ((i & 1) == 1) { NewPos.y += NodeSize * 0.25f; }
                    else { NewPos.y -= NodeSize * 0.25f; }

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
                for (int i = 0; i < 4; i++)
                {
                    foreach (var IndividualNodes in this.Subnodes[i].GetLeafNodes(LOD))
                    {
                        yield return IndividualNodes;
                    }
                }
            }
        }

        public IEnumerable<TType> GetLeafSides(int Side)
        {
            Vector2 SearchPos;
            if ((Side & 2) == 2)
            {
                SearchPos = new Vector2(this.NodeSize, 0);
            }
            else
            {
                SearchPos = new Vector2(0, this.NodeSize);
            }
            if ((Side & 1) != 1)
            {
                SearchPos *= -1f;
            }

            return this.GetSurfaceNode().GetNodeOfPosition(SearchPos + this.Position, this.Depth).ReturnLeafSides(Side);
        }

        public IEnumerable<TType> ReturnLeafSides(int Side) // This is inverted to prevent issues with GetLeafSides(side)
        {
            if (this.Subnodes == null)
            {
                yield return this.Data;
            }
            else
            {
                int StartValue = 0;
                int IncrementValue = 0;

                if ((Side & 2) == 2)
                {
                    IncrementValue = 1;
                }
                else
                {
                    IncrementValue = 2;
                }
                if ((Side & 1) == 1)
                {
                    StartValue = 0;
                }
                else
                {
                    StartValue = 2 * (1 / IncrementValue);
                }
                for (int i = StartValue, j = 0; j < 4; i += IncrementValue, j++)
                {
                    foreach (TType FinalData in this.Subnodes[i].ReturnLeafSides(Side)) { yield return FinalData; }
                }
            }
        }

        public Node GetNodeOfPosition(Vector2 LookupPosition, int MaxDepth = 0)
        {
            if (this.Depth == MaxDepth || this.Subnodes == null)
            {
                return this;
            }
            else
            {
                return this.Subnodes[GetIndexOfPosition(LookupPosition, this.Position)].GetNodeOfPosition(LookupPosition);
            }
        }

        public Node GetSurfaceNode()
        {
            if (this.Root != null)
            {
                return this.Root.GetSurfaceNode();
            }
            else
            {
                return this;
            }

        }
    }

    private static int GetIndexOfPosition(Vector2 loopkuPosition, Vector2 nodePosition)
    {
        int index = 0;
        // |= is the same as += in this case. It is used
        index |= loopkuPosition.x > nodePosition.x ? 2 : 0;
        index |= loopkuPosition.y > nodePosition.y ? 1 : 0;

        return index;
    }



    //public IEnumerable<Node> GetAllLeafNodes() //A more global version of GetLeafNodes();
    //{
    //    return this.node.GetLeafNodes();
    //}
}
