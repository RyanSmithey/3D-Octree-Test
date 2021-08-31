using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rooms
{
    Admin = 0,
    Cafeteria = 1,
    Weapons = 2,
    O2 = 3,
    Navegation = 4,
    Shields = 5,
    Communications = 6,
    Storage = 7,
    Electrical = 8,
    Engine = 9,
    Security = 10,
    Reactor = 11
}

//public enum RoomsS
//{
//    Admin = [5,4],
//    Cafeteria = [15, 15],
//    Weapons = [5,5],
//    O2 = [3,2],
//    Navegation = [4,5],
//    Shields = [5,5],
//    Communications = [6,4],
//    Storage = [6, 10],
//    Electrical = [6, 5],
//    Engine = [5,5],
//    Security = [4,2],
//    Reactor = [5,10]
//}

public class MapGenerator : MonoBehaviour
{
    public int MapSize = 20;
    public int Divisions = 8;
    
    public int[] Counts;

    private int[,] FinalMap;

    private float Voxelsize;

    private int[,] RoomSizes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
