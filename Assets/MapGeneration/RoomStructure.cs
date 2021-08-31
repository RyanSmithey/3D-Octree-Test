using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Roomscalings
{
    Cafeteria = 0,
    Admin = 1,
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

public class RoomStructure
{
    public static int[,] RoomSizes;
    
    public Room CafeteriaRoom;

    public void Initialize(List<byte> RoomsToAdd)
    {
        RoomSizes = new int[12, 2];
        RoomSizes[0, 0] = 12;
        RoomSizes[0, 1] = 12;
        RoomSizes[1, 0] = 5;
        RoomSizes[1, 1] = 4;
        RoomSizes[2, 0] = 5;
        RoomSizes[2, 1] = 5;
        RoomSizes[3, 0] = 3;
        RoomSizes[3, 1] = 2;
        RoomSizes[4, 0] = 4;
        RoomSizes[4, 1] = 5;
        RoomSizes[5, 0] = 5;
        RoomSizes[5, 1] = 5;
        RoomSizes[6, 0] = 6;
        RoomSizes[6, 1] = 4;
        RoomSizes[7, 0] = 6;
        RoomSizes[7, 1] = 10;
        RoomSizes[8, 0] = 6;
        RoomSizes[8, 1] = 5;
        RoomSizes[9, 0] = 5;
        RoomSizes[9, 1] = 5;
        RoomSizes[10, 0] = 4;
        RoomSizes[10, 1] = 2;
        RoomSizes[11, 0] = 5;
        RoomSizes[11, 1] = 10;

        CafeteriaRoom = new Room();
        CafeteriaRoom.RoomType = 0;
        CafeteriaRoom.depth = 0;
        int[] loc = new int[2];
        loc[0] = 0;
        loc[1] = 0;
        CafeteriaRoom.RoomLocation = loc;
        CafeteriaRoom.SetKeyRoom();
        
        CreateRooms(RoomsToAdd);
    }

    public void CreateRooms(List<byte> RoomsToAdd)
    {
        foreach(byte NewRoom in RoomsToAdd)
        {
            this.CafeteriaRoom.AddRoom(NewRoom);
        }
    }

    public int[] GetTotalSize()
    {
        int[] final = new int[4];
        for (int i = 0; i < 4; i++) { final[i] = 0; }


        foreach (KeyValuePair<int[], byte> kvp in this.CafeteriaRoom.RoomArea)
        {
            for (int i = 0; i < 2; i++)
            {
                if (kvp.Key[i] < final[i]) { final[i] = kvp.Key[i]; }
                if (kvp.Key[i] > final[i + 2]) { final[i + 2] = kvp.Key[i]; }
            }
        }

        return final;
    }

    public class Room
    {
        public int[] RoomLocation;
        public int depth;
        public byte RoomType;
        public List<Room> SubRooms;
        public Room KeyRoom;
        public int WallWidth = 2;

        public int WallWidthVariability = 2;

        public Dictionary<int[], byte> RoomArea;
        
        public void AddRoom(byte RoomIndex)
        {
            int[] Location = new int[2];
            if (Random.Range(0.0f, 1.0f) > 0.5f)
            {
                Location[0] = (int)Random.Range(0.0f, KeyRoom.WallWidthVariability + 1) + RoomSizes[this.RoomType, 0] + RoomSizes[RoomIndex, 0] + this.KeyRoom.WallWidth;
                Location[1] = (int)Random.Range(-1 - RoomSizes[this.RoomType, 1], RoomSizes[this.RoomType, 1]);
                if (Random.Range(0.0f, 1.0f) > 0.5f)
                {
                    Location[0] *= -1;
                }
            }
            else
            {
                Location[1] = (int)Random.Range(0.0f, WallWidthVariability + 1) + RoomSizes[this.RoomType, 1] + RoomSizes[RoomIndex, 1] + this.KeyRoom.WallWidth;
                Location[0] = (int)Mathf.Ceil(Random.Range(-1 - RoomSizes[this.RoomType, 0], RoomSizes[this.RoomType, 0]));
                if (Random.Range(0.0f, 1.0f) > 0.5f)
                {
                    Location[1] *= -1;
                }
            }

            Location[0] += this.RoomLocation[0];
            Location[1] += this.RoomLocation[1];

            bool SpaceOcupied = false;
            for (int i = Location[0] - RoomSizes[RoomIndex, 0]; i <= Location[0] + RoomSizes[RoomIndex, 0]; i++)
            {
                for (int j = Location[1] - RoomSizes[RoomIndex, 1]; j <= Location[1] + RoomSizes[RoomIndex, 1]; j++)
                {
                    if (KeyRoom.RoomArea.ContainsKey(new int[] { i, j}))
                    {
                        SpaceOcupied = true;
                    }
                }
            }
            
            if (!SpaceOcupied)
            {
                if (this.SubRooms == null)
                {
                    this.SubRooms = new List<Room>();
                }

                Room PotentialRoom = new Room();
                PotentialRoom.depth = this.depth + 1;
                PotentialRoom.RoomLocation = Location;
                PotentialRoom.RoomType = RoomIndex;
                PotentialRoom.KeyRoom = this.KeyRoom;
                PotentialRoom.AddToRoomArea(RoomIndex);
                SubRooms.Add(PotentialRoom);
            }
            else
            {
                if (this.SubRooms == null)
                {
                    this.KeyRoom.AddRoom(RoomIndex);
                }
                else
                {
                    int NextRoom = (int)Mathf.Ceil(Random.Range(-0.999f, this.SubRooms.Count - 1));
                    this.SubRooms[NextRoom].AddRoom(RoomIndex);
                }
            }
        }

        public void SetKeyRoom()
        {
            this.RoomArea = new Dictionary<int[], byte>(new MyEqualityComparer());

            this.KeyRoom = this;

            this.AddToRoomArea(0);
        }

        public IEnumerable<int[]> GetRoomDimensions()
        {
            yield return new int[] { this.RoomLocation[0], this.RoomLocation[1], RoomSizes[this.RoomType, 0], RoomSizes[this.RoomType, 1] };

            if (this.SubRooms != null)
            {
                foreach (Room IndividualSubroom in this.SubRooms)
                {
                    foreach (int[] Dimensions in IndividualSubroom.GetRoomDimensions())
                    {
                        yield return Dimensions;
                    }
                }
            }

        }

        public IEnumerable<int[]> GetRoomConnections()
        {
            if (this.SubRooms != null)
            {
                foreach (Room IndividualSubroom in this.SubRooms)
                {

                    yield return new int[] { this.RoomLocation[0], this.RoomLocation[1], IndividualSubroom.RoomLocation[0], IndividualSubroom.RoomLocation[1], this.depth };


                    foreach (int[] Connection in IndividualSubroom.GetRoomConnections())
                    {
                        yield return Connection;
                    }
                }
            }

        }

        public void AddConnection(Room EndRoom)
        {
            bool FinishStage1 = false;
            int[] Index = new int[] { this.RoomLocation[0], this.RoomLocation[1] };
            void Xset(int StartLoc)
            {
                int[] SwapIndex = new int[] { Index[0], EndRoom.RoomLocation[0] };
                if (Index[0] > EndRoom.RoomLocation[0])
                {
                    SwapIndex[0] = EndRoom.RoomLocation[0];
                    SwapIndex[1] = Index[0];
                }


                for (int i = SwapIndex[0]; i < SwapIndex[1]; i++)
                {
                    byte value = 0;
                    if (this.KeyRoom.RoomArea.TryGetValue(new int[] { i, StartLoc }, out value))//Should Replace/insert
                    {
                        if (value == 255)
                        {
                            this.KeyRoom.RoomArea[new int[] { i, StartLoc }] = (byte)254;
                        }
                    }
                    else
                    {
                        this.KeyRoom.RoomArea.Add(new int[] { i, StartLoc }, (byte)254);
                    }
                }
            }
            void Yset(int StartLoc)
            {
                int[] SwapIndex = new int[] { Index[1], EndRoom.RoomLocation[1] };
                if (Index[1] > EndRoom.RoomLocation[1])
                {
                    SwapIndex[0] = EndRoom.RoomLocation[1];
                    SwapIndex[1] = Index[1];
                }

                for (int j = SwapIndex[0]; j < SwapIndex[1]; j++)
                {
                    byte value = 0;
                    if (this.KeyRoom.RoomArea.TryGetValue(new int[] { StartLoc, j }, out value))//Should Replace/insert
                    {
                        if (value == 255)
                        {
                            this.KeyRoom.RoomArea[new int[] { StartLoc, j }] = (byte)254;
                        }
                    }
                    else
                    {
                        this.KeyRoom.RoomArea.Add(new int[] { StartLoc, j }, (byte)254);
                    }
                }
            }
            
            if (EndRoom.RoomLocation[0] - this.RoomLocation[0] <= RoomSizes[this.RoomType, 0])
            {
                Yset(EndRoom.RoomLocation[0]);
                Xset(this.RoomLocation[1]);
            }
            else
            {
                Yset(this.RoomLocation[0]);
                Xset(EndRoom.RoomLocation[1]);
            }
        }

        public void AddAllRoomConnections()
        {
            foreach( Room ConnectedRoom in this.SubRooms)
            {
                if (ConnectedRoom.SubRooms != null)
                {
                    ConnectedRoom.AddAllRoomConnections();
                }
                AddConnection(ConnectedRoom);
            }
        }

        public void AddToRoomArea(byte RoomIndex)
        {
            for (int i = this.RoomLocation[0] - RoomSizes[RoomIndex, 0] - 1; i <= this.RoomLocation[0] + RoomSizes[RoomIndex, 0]; i++)
            {
                for (int j = this.RoomLocation[1] - RoomSizes[RoomIndex, 1] - 1; j <= this.RoomLocation[1] + RoomSizes[RoomIndex, 1]; j++)
                {
                    int[] temp = new int[] { i, j };
                    if (!this.KeyRoom.RoomArea.ContainsKey(new int[] { i, j }))
                    {
                        if (i == this.RoomLocation[0] - RoomSizes[RoomIndex, 0] - 1 || i == this.RoomLocation[0] + RoomSizes[RoomIndex, 0] ||
                            j == this.RoomLocation[1] - RoomSizes[RoomIndex, 1] - 1 || j == this.RoomLocation[1] + RoomSizes[RoomIndex, 1])
                        {
                            this.KeyRoom.RoomArea.Add(temp, 255);
                        }
                        else
                        {
                            this.KeyRoom.RoomArea.Add(temp, RoomIndex);
                        }
                    }
                }
            }
        }
    }

    

}
public class MyEqualityComparer : IEqualityComparer<int[]>
{
    public bool Equals(int[] x, int[] y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(int[] obj)
    {
        int result = 17;
        for (int i = 0; i < obj.Length; i++)
        {
            unchecked
            {
                result = result * 23 + obj[i];
            }
        }
        return result;
    }

    public static int[] AddArrays(int[] a, int[] b)
    {
        int[] newArray = new int[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            newArray[i] = a[i] + b[i];
        }
        return newArray;
    }
}
