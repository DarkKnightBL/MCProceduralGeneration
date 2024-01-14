using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    [SerializeField] GameObject grass, stone;
    [SerializeField] GameObject decorationObject;
    [SerializeField] GameObject playerObject;
    [SerializeField] GameObject player;

    [SerializeField] int worldSizeX = 20;
    [SerializeField] int worldSizeZ = 20;
    [SerializeField] int noiseHeight = 3;
    [SerializeField] int decorationCount = 3;
    [SerializeField] float gridOffset = 1.1f;
    [SerializeField] float detailScale = 1.1f;

    [SerializeField] Vector3 startPos;

    [SerializeField] Hashtable blockContainer = new Hashtable();
    [SerializeField] List<Vector3> blockPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate player at initial position
        player = Instantiate(playerObject, new Vector3(0f, 10f, 0f), Quaternion.identity);
        startPos = player.transform.position;

        // Generate initial grid of grass blocks
        GenerateInitialGrid();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for player movement and update grid accordingly
        UpdateGridOnPlayerMove();
    }

    // Generate the initial grid of grass blocks
    void GenerateInitialGrid()
    {
        
        for (int x = -worldSizeX; x < worldSizeX; x++)
        {
            for (int z = -worldSizeZ; z < worldSizeZ; z++)
            {
                // Use Perlin noise to determine the height of the block
                int worldSizeY = Mathf.FloorToInt(GenerateNoise(x, z, detailScale) * noiseHeight);
                Vector3 pos = new Vector3(x * 1 + startPos.x, worldSizeY, z * 1 + startPos.z);

                // Instantiate grass block at the calculated position
                GameObject grassBlock = Instantiate(grass, pos, Quaternion.identity) as GameObject;

                // Add the block to the container and positions list
                blockContainer.Add(pos, grassBlock);
                blockPositions.Add(grassBlock.transform.position);

                // Set the parent of the block to this object
                grassBlock.transform.SetParent(this.transform);

            }
        }
    }

    // Update the grid when the player moves
    void UpdateGridOnPlayerMove()
    {
        if (Mathf.Abs(xPlayerMove) >= 1 || Mathf.Abs(zPlayerMove) >= 1)
        {
            for (int x = -worldSizeX; x < worldSizeX; x++)
            {
                for (int z = -worldSizeZ; z < worldSizeZ; z++)
                {
                    // Use Perlin noise to determine the height of the block
                    int worldSizeY = Mathf.FloorToInt(GenerateNoise(x + xPlayerPosition, z + zPlayerPosition, detailScale) * noiseHeight);
                    Vector3 pos = new Vector3(x * 1 + xPlayerPosition, worldSizeY, z * 1 + zPlayerPosition);

                    // Check if the block at this position is not already in the container
                    if (!blockContainer.ContainsKey(pos))
                    {
                        // Instantiate grass block at the new position
                        GameObject grassBlock = Instantiate(grass, pos, Quaternion.identity) as GameObject;

                        // Add the block to the container and positions list
                        blockContainer.Add(pos, grassBlock);
                        blockPositions.Add(grassBlock.transform.position);

                        // Set the parent of the block to this object
                        grassBlock.transform.SetParent(this.transform);
                    }
                }
            }
        }
    }

    // Spawn decoration objects on the grid
    private void SpawnDecoration(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject toPlace = Instantiate(decorationObject, DecorationLocation(), Quaternion.identity);
            toPlace.transform.SetParent(this.transform);
        }
    }

    // Get a random position from the block positions list for decoration placement
    private Vector3 DecorationLocation()
    {
        int randomIndex = Random.Range(0, blockPositions.Count);

        Vector3 newPosition = new Vector3(
            blockPositions[randomIndex].x,
            blockPositions[randomIndex].y + 0.5f,
            blockPositions[randomIndex].z
        );

        // Remove the chosen position from the list to avoid placing multiple decorations in the same location
        blockPositions.RemoveAt(randomIndex);
        return newPosition;
    }

    // Properties to get player movement information
    public int xPlayerMove
    {
        get { return (int)(player.transform.position.x - startPos.x); }
    }

    public int zPlayerMove
    {
        get { return (int)(player.transform.position.z - startPos.z); }
    }

    // Properties to get player position information
    private int xPlayerPosition
    {
        get { return (int)(Mathf.Floor(player.transform.position.x)); }
    }

    private int zPlayerPosition
    {
        get { return (int)(Mathf.Floor(player.transform.position.z)); }
    }

    // Generate Perlin noise based on x, z, and detail scale
    private float GenerateNoise(int x, int z, float detailScale)
    {
        float xNoise = (x + this.transform.position.x) / detailScale;
        float zNoise = (z + this.transform.position.y) / detailScale;

        return Mathf.PerlinNoise(xNoise, zNoise);
    }
}
