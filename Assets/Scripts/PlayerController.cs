using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    public float speed = 5.0f;
    public bool hasPowerup;
    private float powerupStrength = 15.0f;
    public GameObject powerupIndicator;

    [Header("Powerup Settings")]
    public GameObject powerupPrefab;  // Assign your Powerup prefab in the Inspector
    private float powerupSpawnRange = 9.0f; // same as your enemy spawn range
    private float respawnDelay = 5.0f; // seconds before new powerup spawns

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);

            // Start countdown for powerup duration
            StartCoroutine(PowerupCountdownRoutine());

            // Start respawn timer for new powerup
            StartCoroutine(RespawnPowerupRoutine());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);

            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            Debug.Log("Collided with: " + collision.gameObject.name + " with powerup set to " + hasPowerup);

            // Change enemy color to green when hit with radiation- I mean, powerup!!
            Renderer enemyRenderer = collision.gameObject.GetComponent<Renderer>();
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = Color.green;
            }
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator RespawnPowerupRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        Vector3 spawnPos = GenerateSpawnPosition();
        Instantiate(powerupPrefab, spawnPos, powerupPrefab.transform.rotation);
        Debug.Log("New powerup respawned at " + spawnPos);
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-powerupSpawnRange, powerupSpawnRange);
        float spawnPosZ = Random.Range(-powerupSpawnRange, powerupSpawnRange);
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}

