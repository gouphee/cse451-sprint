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

        if (mainCamera.WorldToViewportPoint(spawnedFloorTiles[0].endPoint.position).z < -25)
        {
            
            //Move the tile to the front if it's behind the Camera
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, tiles.Length);
            PlatformTile tileTmp = Instantiate(tiles[randomNumber], floorStartPoint.position, Quaternion.identity) as PlatformTile;
            PlatformTile tileTmp2 = spawnedFloorTiles[0];

            spawnedFloorTiles.RemoveAt(0);
            tileTmp.transform.position = spawnedFloorTiles[spawnedFloorTiles.Count - 1].endPoint.position - tileTmp2.startPoint.localPosition;
            //tileTmp.ActivateRandomObstacle();
            spawnedFloorTiles.Add(tileTmp);
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