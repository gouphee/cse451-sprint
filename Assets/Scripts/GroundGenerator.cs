using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GroundGenerator : MonoBehaviour
{
    public Camera mainCamera;
    public Transform floorStartPoint; //Point from where ground tiles will start
    public Transform ceilingStartPoint;
    public Transform leftWallStartPoint;
    public Transform rightWallStartPoint;
    public PlatformTile[] tiles;
    public float movingSpeed = 12;
    public int tilesToPreSpawn = 1; //How many tiles should be pre-spawned
    public int tilesWithoutObstacles = 3; //How many tiles at the beginning should not have obstacles, good for warm-up

    List<Vector3> spawnPoints = new List<Vector3>();

    List<PlatformTile> spawnedFloorTiles = new List<PlatformTile>();
    List<PlatformTile> spawnedCeilingTiles = new List<PlatformTile>();
    List<PlatformTile> spawnedLeftWallTiles = new List<PlatformTile>();
    List<PlatformTile> spawnedRightWallTiles = new List<PlatformTile>();

    int nextTileToActivate = -1;
    [HideInInspector]
    public bool gameOver = false;
    static bool gameStarted = false;
    float score = 0;

    public static GroundGenerator instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        Vector3 floorSpawnPosition = floorStartPoint.position;
        Vector3 ceilingSpawnPosition = ceilingStartPoint.position;
        Vector3 leftWallSpawnPosition = leftWallStartPoint.position;
        Vector3 rightWallSpawnPosition = rightWallStartPoint.position;

        spawnPoints.Add(floorSpawnPosition);
        spawnPoints.Add(ceilingSpawnPosition); 
        spawnPoints.Add(leftWallSpawnPosition);
        spawnPoints.Add(rightWallSpawnPosition);

        
        int tilesWithNoObstaclesTmp = tilesWithoutObstacles;
        for (int i = 0; i < tilesToPreSpawn; i++)
        {
            floorSpawnPosition -= tiles[0].startPoint.localPosition;
            ceilingSpawnPosition -= tiles[0].startPoint.localPosition;
            leftWallSpawnPosition -= tiles[0].startPoint.localPosition;
            rightWallSpawnPosition -= tiles[0].startPoint.localPosition;
            

            PlatformTile spawnedFloorTile = Instantiate(tiles[0], floorSpawnPosition, Quaternion.identity) as PlatformTile;
            PlatformTile spawnedCeilingTile = Instantiate(tiles[0], ceilingSpawnPosition, Quaternion.identity) as PlatformTile;
            PlatformTile spawnedLeftWallTile = Instantiate(tiles[0], rightWallSpawnPosition, Quaternion.identity) as PlatformTile;
            PlatformTile spawnedRightWallTile = Instantiate(tiles[0], leftWallSpawnPosition, Quaternion.identity) as PlatformTile;

            spawnedLeftWallTile.transform.Rotate(0f, 0f, 90f);
            spawnedRightWallTile.transform.Rotate(0f, 0f, 90f);

            if (tilesWithNoObstaclesTmp > 0)
            {
                //spawnedTile.DeactivateAllObstacles();
                tilesWithNoObstaclesTmp--;
            }
            else
            {
                //spawnedTile.ActivateRandomObstacle();
            }

            floorSpawnPosition = spawnedFloorTile.endPoint.position;
            ceilingSpawnPosition = spawnedCeilingTile.endPoint.position;
            leftWallSpawnPosition = spawnedLeftWallTile.endPoint.position;
            rightWallSpawnPosition = spawnedRightWallTile.endPoint.position;

            spawnedFloorTile.transform.SetParent(transform);
            spawnedCeilingTile.transform.SetParent(transform);
            spawnedLeftWallTile.transform.SetParent(transform);
            spawnedRightWallTile.transform.SetParent(transform);

            spawnedFloorTiles.Add(spawnedFloorTile);
            spawnedCeilingTiles.Add(spawnedCeilingTile);
            spawnedLeftWallTiles.Add(spawnedLeftWallTile);
            spawnedRightWallTiles.Add(spawnedRightWallTile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object upward in world space x unit/second.
        //Increase speed the higher score we get
        if (!gameOver && gameStarted)
        {
            transform.Translate(-spawnedFloorTiles[0].transform.forward * Time.deltaTime * (movingSpeed + (score / 500)), Space.World);
            score += Time.deltaTime * movingSpeed;
        }

        if (mainCamera.WorldToViewportPoint(spawnedFloorTiles[0].endPoint.position).z < -5)
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, tiles.Length);
            PlatformTile tileTmp = Instantiate(tiles[randomNumber], floorStartPoint.position, Quaternion.identity) as PlatformTile;
            Vector3 prevPos = spawnedFloorTiles[0].startPoint.localPosition;

            // Removes tile in spawned floor tiles and adds new one at the position of 
            Destroy(spawnedFloorTiles[0].gameObject);
            spawnedFloorTiles.RemoveAt(0);
            tileTmp.transform.position = spawnedFloorTiles[spawnedFloorTiles.Count - 1].endPoint.position - prevPos;
            //tileTmp.ActivateRandomObstacle();
            tileTmp.transform.SetParent(transform);
            spawnedFloorTiles.Add(tileTmp);

            random = new System.Random();
            randomNumber = random.Next(0, tiles.Length);
            tileTmp = Instantiate(tiles[randomNumber], ceilingStartPoint.position, Quaternion.identity) as PlatformTile;
            prevPos = spawnedCeilingTiles[0].startPoint.localPosition;

            // Removes tile in spawned floor tiles and adds new one at the position of 
            Destroy(spawnedCeilingTiles[0].gameObject);
            spawnedCeilingTiles.RemoveAt(0);
            tileTmp.transform.position = spawnedCeilingTiles[spawnedCeilingTiles.Count - 1].endPoint.position - prevPos;
            //tileTmp.ActivateRandomObstacle();
            tileTmp.transform.SetParent(transform);
            spawnedCeilingTiles.Add(tileTmp);

            random = new System.Random();
            randomNumber = random.Next(0, tiles.Length);
            tileTmp = Instantiate(tiles[randomNumber], leftWallStartPoint.position, Quaternion.identity) as PlatformTile;
            prevPos = spawnedLeftWallTiles[0].startPoint.localPosition;

            // Removes tile in spawned floor tiles and adds new one at the position of 
            Destroy(spawnedLeftWallTiles[0].gameObject);
            spawnedLeftWallTiles.RemoveAt(0);
            tileTmp.transform.Rotate(0f, 0f, 90f);
            tileTmp.transform.position = spawnedLeftWallTiles[spawnedLeftWallTiles.Count - 1].endPoint.position - prevPos;
            //tileTmp.ActivateRandomObstacle();
            tileTmp.transform.SetParent(transform);
            spawnedLeftWallTiles.Add(tileTmp);

            random = new System.Random();
            randomNumber = random.Next(0, tiles.Length);
            tileTmp = Instantiate(tiles[randomNumber], rightWallStartPoint.position, Quaternion.identity) as PlatformTile;
            prevPos = spawnedRightWallTiles[0].startPoint.localPosition;

            // Removes tile in spawned floor tiles and adds new one at the position of 
            Destroy(spawnedRightWallTiles[0].gameObject);
            spawnedRightWallTiles.RemoveAt(0);
            tileTmp.transform.Rotate(0f, 0f, 90f);

            tileTmp.transform.position = spawnedRightWallTiles[spawnedRightWallTiles.Count - 1].endPoint.position - prevPos;
            //tileTmp.ActivateRandomObstacle();
            tileTmp.transform.SetParent(transform);
            spawnedRightWallTiles.Add(tileTmp);
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