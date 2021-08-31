using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public byte[,] GenMapInfo(int Depth = 0)
    {
        int Xsize = (int)Mathf.Pow(4, Depth);

        byte[,] MapInformation = new byte[Xsize, Xsize];

        for (int i = 0; i < Xsize; i++)
        {
            for (int j = 0; j < Xsize; j++)
            {
                byte newinfo = 0;

                

                if (Mathf.PerlinNoise(i, j) > .5f)
                {
                    newinfo = 4;
                }
                
                MapInformation[i, j] = newinfo;
            }
        }

        

        return MapInformation;
    }
}
