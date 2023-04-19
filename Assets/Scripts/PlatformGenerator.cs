using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public Material[] materials;
    public int numberOfStartingTiles = 3;
    public int numberOfPlatformsVisible = 5;
    public float platformLength = 10f;
    public float platformSpacing = 2f;
    public Transform player;
    public int tilesBehindPlayer = 3;

    private Queue<GameObject> floorTiles;
    private Queue<GameObject> leftWallTiles;
    private Queue<GameObject> rightWallTiles;
    private Queue<GameObject> ceilingTiles;
    private float nextPlatformPosition;

    // [HideInInspector]
    public bool gameOver = false;
    static bool gameStarted = false;
    public float score = 0;

    private bool topGap = false;
    private bool bottomGap = false;
    private bool leftGap = false;
    private bool rightGap = false;

    private bool isGapActive;


    void Start()
    {
        floorTiles = new Queue<GameObject>();
        leftWallTiles = new Queue<GameObject>();
        rightWallTiles = new Queue<GameObject>();
        ceilingTiles = new Queue<GameObject>();

        for (int i = 0; i < numberOfStartingTiles + tilesBehindPlayer; i++)

        {
            // Use the same prefab for the first "x" tiles, where x is numberOfStartingTiles.
            int prefabIndex = 0;
            SpawnLeftWall(prefabIndex);
            SpawnRightWall(prefabIndex);
            SpawnCeiling(prefabIndex);
            SpawnFloor(prefabIndex);
        }
    }

    void Update()
    {
        if (Random.Range(0, 100) == 0) // Increase the range to reduce the chance of a gap
        {
            StartGapCoroutine(GetRandomDirection());
        }
        if (player.position.z + platformLength / 2 + platformSpacing + numberOfPlatformsVisible * platformLength > nextPlatformPosition)
        {
            if (leftGap)
            {
                SpawnLeftWall(1);
            } 
            else
            {
                SpawnLeftWall(Random.Range(0, tilePrefabs.Length));
            }
            if (rightGap)
            {
                SpawnRightWall(1);
            }
            else
            {
                SpawnRightWall(Random.Range(0, tilePrefabs.Length));
            }
            if (topGap)
            {
                SpawnCeiling(1);
            }
            else
            {
                SpawnCeiling(Random.Range(0, tilePrefabs.Length));
            }
            if (bottomGap)
            {
                SpawnFloor(1);
            }
            else
            {
                SpawnFloor(Random.Range(0, tilePrefabs.Length));
            }
        }
        if (player.position.z - floorTiles.Peek().transform.position.z > (tilesBehindPlayer) * (platformLength + platformSpacing))
        {
            DestroyOldestTile(floorTiles);
            DestroyOldestTile(leftWallTiles);
            DestroyOldestTile(rightWallTiles);
            DestroyOldestTile(ceilingTiles);
        }

        if (gameOver || !gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (gameOver)
                {
                    //Restart current scene
                    Scene scene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(scene.name);
                }
                else
                {
                    //Start the game
                    gameStarted = true;
                }
            }
        }
    }

    private void SpawnFloor(int prefabIndex)
    {
        GameObject newTile = Instantiate(tilePrefabs[prefabIndex], new Vector3(0, 0, nextPlatformPosition), Quaternion.identity);
        ChangeChildrenCubeMaterials(newTile.transform, materials[0]);
        floorTiles.Enqueue(newTile);
        nextPlatformPosition += platformLength + platformSpacing;
    }

    private void SpawnLeftWall(int prefabIndex)
    {
        GameObject newTile = Instantiate(tilePrefabs[prefabIndex], new Vector3(-(16 / 2), 8, nextPlatformPosition - platformSpacing), Quaternion.Euler(0, 0, -90));
        ChangeChildrenCubeMaterials(newTile.transform, materials[1]);
        leftWallTiles.Enqueue(newTile);
    }

    private void SpawnRightWall(int prefabIndex)
    {
        GameObject newTile = Instantiate(tilePrefabs[prefabIndex], new Vector3(16 / 2, 8, nextPlatformPosition - platformSpacing), Quaternion.Euler(0, 0, 90));
        ChangeChildrenCubeMaterials(newTile.transform, materials[2]);
        rightWallTiles.Enqueue(newTile);
    }

    private void SpawnCeiling(int prefabIndex)
    {
        GameObject newTile = Instantiate(tilePrefabs[prefabIndex], new Vector3(0, 16, nextPlatformPosition - platformSpacing), Quaternion.Euler(0, 0, 180));
        ChangeChildrenCubeMaterials(newTile.transform, materials[3]);
        ceilingTiles.Enqueue(newTile);
    }

    private void DestroyOldestTile(Queue<GameObject> tileQueue)
    {
        GameObject oldestTile = tileQueue.Dequeue();
        Destroy(oldestTile);
    }

    private void ChangeChildrenCubeMaterials(Transform parent, Material newMaterial)
    {
        if (newMaterial == null)
        {
            Debug.LogWarning("New Material is not assigned. Please assign a material in the inspector.");
            return;
        }

        foreach (Transform child in parent)
        {
            if (child.CompareTag("Cube"))
            {
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.material = newMaterial;
                }
            }

            if (child.childCount > 0)
            {
                ChangeChildrenCubeMaterials(child, newMaterial);
            }
        }
    }

    enum WallDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    WallDirection GetRandomDirection()
    {
        return (WallDirection)Random.Range(0, 4);
    }

    private void StartGapCoroutine(WallDirection direction)
    {
        if (!isGapActive)
        {
            isGapActive = true;
            StartCoroutine(WallGapCoroutine(direction));
        }
    }


    private IEnumerator WallGapCoroutine(WallDirection direction)
    {
        switch (direction)
        {
            case WallDirection.Top:
                topGap = true;
                break;
            case WallDirection.Bottom:
                bottomGap = true;
                break;
            case WallDirection.Left:
                leftGap = true;
                break;
            case WallDirection.Right:
                rightGap = true;
                break;
        }

        yield return new WaitForSeconds(2f);

        switch (direction)
        {
            case WallDirection.Top:
                topGap = false;
                break;
            case WallDirection.Bottom:
                bottomGap = false;
                break;
            case WallDirection.Left:
                leftGap = false;
                break;
            case WallDirection.Right:
                rightGap = false;
                break;
        }

        isGapActive = false;
    }


    void OnGUI()
    {
        if (gameOver)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), "Game Over\nYour score is: " + ((int)score) + "\nPress 'Space' to restart");
        }
        else
        {
            if (!gameStarted)
            {
                GUI.color = Color.red;
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), "Press 'Space' to start");
            }
        }


        GUI.color = Color.green;
        GUI.Label(new Rect(5, 5, 200, 25), "Score: " + ((int)score));
    }
}
