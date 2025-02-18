using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] GameObject explosion;

    Transform player;
    bool homingOnPlayer;

    LayerMask mask;
    int playerMask = 0b_0000_0000_0000_0000_0000_0010_0000_0000;
    int enemyMask = 0b_0000_0000_0000_0000_0000_0100_0000_0000;
    private void Awake()
    {
        mask = LayerMask.NameToLayer("Player");
        
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 50, playerMask);

        if(hit.TryGetComponent(out Player player))
        {
            this.player = player.transform;
            homingOnPlayer = true;
            print($"Player to missile");
        }
    }

    public void Destroy()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (homingOnPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime);
        }
    }
}
