using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform TeleportTo;
    private AudioManager audioManager;
    private AudioSource audioSource;
    private void Start ( )
    {
        audioManager = AudioManager.Instance;
        audioSource = GetComponent<AudioSource> ( );
    }

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if ( collision.CompareTag ( "Player" ) )
        {
            audioManager.EnemActivitySound ( audioSource, audioManager.PlayerTeleport );
            collision.transform.position = TeleportTo.position;
        }
    }
}
