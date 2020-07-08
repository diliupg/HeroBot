using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public bool bossActive;
    public bool bossRight;
    public bool takeDamage;
    public bool waitingForRespawn;

    public float timeBetweenDrops;
    private float timeBetweenDropsStore;
    private float dropCount;

    public float waitForPlatforms;
    private float platformCount;

    public Transform leftPoint;
    public Transform rightPoint;
    public Transform dropSawSpawnPoint;

    public GameObject theBoss;
    public GameObject dropsaw;
    public GameObject rightPlatforms;
    public GameObject leftPlatforms;
    public GameObject levelExit;

    private CameraController cameraController;
    private LevelManager levelManager;

    public int startingHealth;
    private int currentHealth;

    void Start()
    {
        dropCount = timeBetweenDrops;
        timeBetweenDropsStore = timeBetweenDrops;
        platformCount = waitForPlatforms;
        theBoss.transform.position = rightPoint.position;
        bossRight = true;
        currentHealth = startingHealth;
        cameraController = FindObjectOfType<CameraController> ( );
        levelManager = FindObjectOfType<LevelManager> ( );
    }

    // Update is called once per frame
    void Update()
    {
        if(levelManager._respawnCoActive)
        {
            bossActive = false;
            waitingForRespawn = true;
        }

        if(waitingForRespawn && !levelManager._respawnCoActive)
        {
            theBoss.SetActive ( false );
            leftPlatforms.SetActive ( false );
            rightPlatforms.SetActive ( false );

            platformCount = waitForPlatforms;
            timeBetweenDrops = timeBetweenDropsStore;
            dropCount = timeBetweenDrops;

            theBoss.transform.position = rightPoint.position;
            bossRight = true;
            currentHealth = startingHealth;

            cameraController.followTarget = true;

            waitingForRespawn = false;
        }

        if(bossActive)
        {
            //cameraController.followTarget = false;
            cameraController.transform.position = Vector3.Lerp ( cameraController.transform.position, new Vector3(transform.position.x, cameraController.transform.position.y, cameraController.transform.position.z), cameraController.smoothing * Time.deltaTime );
            theBoss.SetActive ( true );

            if(dropCount > 0)
            {
                dropCount -= Time.deltaTime;
            }
            else
            {
                dropSawSpawnPoint.position = new Vector3 ( Random.Range ( leftPoint.position.x, rightPoint.position.x ), dropSawSpawnPoint.position.y, dropSawSpawnPoint.position.z );

                Instantiate ( dropsaw, dropSawSpawnPoint.position, dropSawSpawnPoint.rotation );

                dropCount = timeBetweenDrops;
            }

            if(bossRight == true )
            {
                if( platformCount > 0 )
                {
                    platformCount -= Time.deltaTime;
                }
                else
                {
                    rightPlatforms.SetActive ( true );
                }
            }
            else
            {
                if ( platformCount > 0 )
                {
                    platformCount -= Time.deltaTime;
                }
                else
                {
                    leftPlatforms.SetActive ( true );
                }
            }

            if(takeDamage)
            {
                currentHealth -= 1 ;

                if(currentHealth <= 0)
                {
                    levelExit.SetActive ( true );
                    gameObject.SetActive ( false );
                }

                if(bossRight)
                {
                    theBoss.transform.position = leftPoint.position;
                }
                else
                {
                    theBoss.transform.position = rightPoint.position;
                }

                bossRight = !bossRight;
                takeDamage = false;

                rightPlatforms.SetActive ( false );
                leftPlatforms.SetActive ( false );

                platformCount = waitForPlatforms;

                timeBetweenDrops = timeBetweenDrops / 2f;
            }

        }
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.CompareTag("Player"))
        {
            bossActive = true;
        }
    }
}
