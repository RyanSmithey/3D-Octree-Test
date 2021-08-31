using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDig : MonoBehaviour
{
    public float DigRadius = 1;
    public float DigDistance = 1;
    public QuadtreeComponentNew Quad;

    private Vector3 Direction = Vector3.one;
    private RaycastHit hit;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!(Input.GetAxis("Aimx") + Input.GetAxis("Aimy") == 0))
        {
            Direction = new Vector3(Input.GetAxis("Aimx"), Input.GetAxis("Aimy"), 0);
        }

        if (Physics.Raycast(gameObject.transform.position, Direction, out hit, DigDistance))
        {

            byte[,] Digarray = new byte[1, 1];

            Digarray[0, 0] = 0;

            Quad.InsertBulk(hit.transform.position, Digarray);
        }
    }
}
