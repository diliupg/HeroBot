using System.Collections;
using UnityEngine;

public class FallingObject : MonoBehaviour, IDamageble
{
    GameObject player;
    public GameObject explosion;
    private AudioManager audioManager;
    Rigidbody2D rb;
    BoxCollider2D boxCol;
    private SpriteRenderer spriteRenderer;

    bool falling;
    bool explode;

    public int minDamage;
    public int maxDamage;
    public int Health
    {
        get;
        set;
    }

    public float distanceToPlayer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag ( "Player" );
        rb = GetComponent<Rigidbody2D> ( );
        boxCol = GetComponent<BoxCollider2D> ( );
        rb.bodyType = RigidbodyType2D.Kinematic;
        audioManager = AudioManager.Instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer> ( );
    }

    void Update()
    {
        if ( Mathf.Abs(transform.position.x - player.transform.position.x) < distanceToPlayer && !falling)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = Random.Range( 4, 6);
            rb.mass = 1;
            falling = true;
        }
    }

    private void OnCollisionEnter2D ( Collision2D other )
    {
        audioManager.PlaySFX ( audioManager.BarrelHitGround );
        if ( other.gameObject.CompareTag ( "Player" ) && !explode )
        {
            IPlayerDamage hit = other.gameObject.GetComponent<IPlayerDamage>();

            if ( hit != null )
            {
                hit.Damage ( Random.Range ( minDamage, maxDamage), true );

                if(!explode)
                {
                    explode = true;
                    StartCoroutine ( SpawnExplosion ( ) );
                }

            }
        }
    }

    public void Damage ( )
    {
        StartCoroutine ( SpawnExplosion() );
    }

    public IEnumerator SpawnExplosion( )
    {
        spriteRenderer.enabled = false;
        boxCol.enabled = false;
        GameObject inst = Instantiate ( explosion, this.transform.position, this.transform.rotation );
        audioManager.BarrelBlowUp ( );
        yield return new WaitForSeconds ( 2f );
        Destroy ( inst );
        Destroy ( gameObject );
    }
}
