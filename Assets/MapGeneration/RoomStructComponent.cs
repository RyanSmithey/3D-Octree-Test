using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomStructComponent : MonoBehaviour
{
    // Start is called before the first frame update
    public RoomStructure RoomInfo;

    public bool Enabled = true;
    public bool GenerateConnections = false;
    public bool GenTex = false;
    public int NumRooms = 10;

    public Texture2D Tex;

    private GameObject TestObj;

    // Update is called once per frame

    void OnDrawGizmos()
    {
        if (Enabled)
        {
            List<byte> NewRooms = new List<byte>();
            for (byte i = 0; i < NumRooms; i++)
            {
                NewRooms.Add((byte)Mathf.Ceil(Random.Range(1, 11)));//(byte)(i + 1));
            }
            RoomInfo = new RoomStructure();
            RoomInfo.Initialize(NewRooms);

            TestObj = new GameObject();

            Enabled = false;
        }

        if (GenerateConnections)
        {
            RoomInfo.CafeteriaRoom.AddAllRoomConnections();

            GenerateConnections = false;
        }

        if (GenTex)
        {
            Tex = GenerateTexture();

            Sprite sprite = Sprite.Create(Tex, new Rect(0, 0, Tex.width, Tex.height), new Vector2(50, 50), 1f, 0);
            //This takes 5ms which is fairly long but workable

            TestObj.name = "TestObj";
            TestObj.AddComponent<CanvasRenderer>();
            TestObj.AddComponent<Image>();
            TestObj.GetComponent<Image>().sprite = sprite;

            GenTex = false;
        }



        foreach (int[] Dimensions in RoomInfo.CafeteriaRoom.GetRoomDimensions())
        {
            Gizmos.DrawWireCube(new Vector3(Dimensions[0], Dimensions[1], 0), new Vector3(Dimensions[2], Dimensions[3], 0) * 2);
            Gizmos.color = new Color(1, 1, 1, 1);
        }

        foreach (int[] Conneciton in RoomInfo.CafeteriaRoom.GetRoomConnections())
        {
            //Debug.Log(Conneciton[4]);
            int MaxDepth = 10;
            Gizmos.color = new Color(1.0f - Conneciton[4] / MaxDepth, (float)Conneciton[4] / MaxDepth, 0, 1);
            Gizmos.DrawLine(new Vector3(Conneciton[0], Conneciton[1], 0), new Vector3(Conneciton[2], Conneciton[3], 0));
        }
    }


    Texture2D GenerateTexture()
    {
        int[] Dimensions = RoomInfo.GetTotalSize();

        Texture2D MapTexture = new Texture2D(Mathf.Abs(Dimensions[0]) + Dimensions[2] + 1, Mathf.Abs(Dimensions[1]) + Dimensions[3] + 1);
        
        for (int i = Dimensions[0] - 1; i <= Dimensions[2]; i++)
        {
            for (int j = Dimensions[1] - 1; j <= Dimensions[3]; j++)
            {
                byte value = 0;
                if (RoomInfo.CafeteriaRoom.RoomArea.TryGetValue(new int[] { i, j }, out value))
                {
                    if (value == 255)
                    {
                        MapTexture.SetPixel(i - Dimensions[0], j - Dimensions[1], new Color(0.0f, 1.0f, 0.0f, 1.0f));
                    }
                    else
                    {
                        MapTexture.SetPixel(i - Dimensions[0], j - Dimensions[1], new Color(0.0f, 0.0f, 1.0f, 1.0f));
                    }
                }
                else
                {
                    MapTexture.SetPixel(i - Dimensions[0], j - Dimensions[1], new Color(0.0f, 0.0f, 0.0f, 1.0f));
                }
            }
        }
        MapTexture.filterMode = FilterMode.Point;

        MapTexture.Apply();

        return MapTexture;
    }


}
