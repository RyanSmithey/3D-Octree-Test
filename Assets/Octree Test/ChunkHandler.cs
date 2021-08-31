//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ChunkHandler : MonoBehaviour
//{
//    public GameObject IndividualChunk;

//    public GameObject Player;

//    private float ChunkSize = 17;
//    private List<TValue> VisualizeChunks = new List<TValue>();

//    //SudoCode
//    //In the start
//    //Create a dictionary to store chunk values //This is necessary to access data later

//    public Dictionary<TKey, TValue> StoredChunkData = new Dictionary<int[2], TValue>();

//    public Dictionary<TKey, TValue> VisibleChunkData = new Dictionary<int[2], TValue>();


//    //Smaller temperary dictionary for currently loaded chunks
//    //Create initial surrounding chunks
//    //as the player moves generate chunks around the player and destroy chunks far enough away

//    //Find chunks that should be loaded [(X,Y)min, (X,Y)max]
//    private int[] FindNecessaryChunks()
//    {
//        int[] Final = new int[4];

//        Final[0] = Mathf.Ceil(Player.transform.position.x % ChunkSize) - 1;
//        Final[1] = Mathf.Ceil(Player.transform.position.y % ChunkSize) - 1;
//        Final[2] = Mathf.Ceil(Player.transform.position.x % ChunkSize) + 1;
//        Final[3] = Mathf.Ceil(Player.transform.position.y % ChunkSize) + 1;
//    }

//    private void LoadNecessaryChunks(int[] NecessaryChunks)
//    {
//        int[] Chunk = new int[2];
//        //Load necessaryChunks
//        for (int i = NecessaryChunks[0]; i <= NecessaryChunks[2]; i++)
//        {
//            for (int j = NecessaryChunks[1]; j <= NecessaryChunks[3]; j++)
//            {
//                Chunk[0] = i;
//                Chunk[1] = j;

//                if (VisibleChunkData[Chunk] == null)
//                {
//                    LoadChunk(Chunk);
//                }
//            }
//        }
//        //Deload necessaryChunks
//        foreach (KeyValuePair<int[], TValue> kvp in VisibleChunkData)
//        {
//            if (kvp.Key[0] < NecessaryChunks[0] || kvp.Key[0] > NecessaryChunks[2] || kvp.Key[1] < NecessaryChunks[1] || kvp.Key[1] > NecessaryChunks[3])
//            {
//                DeloadChunk(kvp.Key);
//            }
//        }
//    }

//    private void LoadChunk(int[] ChunkToLoad)
//    {
//        if (StoredChunkData[ChunkToLoad] == null)
//        {
//            StoredChunkData[ChunkToLoad] = GenerateChunk(ChunkToLoad);
//            VisibleChunkData[ChunkToLoad] = StoredChunkData[ChunkToLoad];
//        }
//        else
//        {
//            VisibleChunkData[ChunkToLoad] = StoredChunkData[ChunkToLoad];
//        }
//        VisualizeChunks.Add(VisibleChunkData[ChunkToLoad]);
//    }

//    private void DeloadChunk(int[] ChunkToDeload)
//    {




//    }

//    void Update()
//    {
//        LoadNecessaryChunks(FindNecessaryChunks());

//        foreach (TValue Chunk in VisualizeChunks)
//        {
//            VisualizeChunk(Chunk);
//        }


//    }


//    //DestroyChunkX (X)
//    //    Add it to the dictionary at location[2] = [x,y]
//    //    delete it from the scene

//    //LoadChunk(X,Y)
//    //    if Chunk X,Y exists in the dictionary load chunk (X,Y) into scene

//    //    otherwise generate the chunk normally

//    //Generate Chunk
//    //    If location X,Y is special
//    //        Load Special
//    //    otherwise
//    //        Load Perlin noise chunk



//}
