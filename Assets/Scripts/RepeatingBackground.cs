using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingBackground : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToRepeat;
    [SerializeField, Range(2, 5)]
    private int timesToRepeat = 4;
    [SerializeField, Range(1f, 10f)]
    private float movementSpeed = 1f;
    [SerializeField]
    private Camera mainCamera;

    private readonly LinkedList<GameObject> tiles = new();
    private Plane[] cameraFrustrumPlanes;

    // Start is called before the first frame update
    private void Start()
    {
        // Set the list and spawn duplicates
        tiles.AddFirst(objectToRepeat);

        for(int i = 1; i < timesToRepeat; i++)
        {
            // Spawn
            GameObject newObj = GameObject.Instantiate(objectToRepeat, this.transform);
            newObj.transform.name = $"{objectToRepeat.transform.name} {i + 1}";

            // Correct position
            SnapToEnd(newObj, tiles.Last.Value);

            // Add to list
            tiles.AddLast(newObj);
        }

        cameraFrustrumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        Debug.Log("Background setup complete...");
    }

    // Update is called once per frame
    private void Update()
    {
        foreach(GameObject obj in tiles) 
        {
            obj.transform.position -= new Vector3(movementSpeed * Time.deltaTime, 0, 0);
        }

        if (GeometryUtility.TestPlanesAABB(cameraFrustrumPlanes, tiles.First.Value.transform.GetComponent<SpriteRenderer>().bounds) == false)
        {
            GameObject swapObj = tiles.First.Value;
            Debug.Log(swapObj.transform.name + " is no longer visible. Snapping to end...");
            tiles.RemoveFirst();

            // Correct position
            SnapToEnd(swapObj, tiles.Last.Value);

            tiles.AddLast(swapObj);
        }
    }
    
    private void SnapToEnd(GameObject objToSnap, GameObject objToSnapTo)
    {
        // Get position
        float lx = objToSnapTo.transform.position.x;
        float lw = objToSnapTo.transform.GetComponent<SpriteRenderer>().bounds.size.x;
        
        // Perform "snap"
        objToSnap.transform.position = new Vector3(lx + lw, objToSnap.transform.position.y, objToSnap.transform.position.z);
    }
}
