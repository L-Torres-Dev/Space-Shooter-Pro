using UnityEngine;
using static Utility.VectorExtensions;
public class Missile : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] float angle;
    [SerializeField] float currentAngle;
    [SerializeField] float angleDiff;
    [SerializeField] Vector2 currentAngleV;
    [SerializeField] Vector2 directionGoal;
    
    Transform player;
    bool homingOnPlayer;

    int playerMask = 0b_0000_0000_0000_0000_0000_0010_0000_0000;
    int enemyMask = 0b_0000_0000_0000_0000_0000_0100_0000_0000;
    private void Start()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 50, playerMask);

        Transform player = GameManager.Instance.PlayerTransform;
        if (player != null)
        {
            this.player = player.transform;
            homingOnPlayer = true;
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
            Vector2 direction = GetDirection(player.position, transform.position);
            
            if(transform.position.y > -4f)
                Rotate(direction, 23);

            Vector3 moveDir = GetVector2FromAngle(transform.rotation.eulerAngles.z + 90);

            transform.position += moveDir * 5 * Time.deltaTime;

            
        }

        if (Mathf.Abs(transform.position.x) > 11f)
            Destroy(this.gameObject);
        if(Mathf.Abs(transform.position.y) > 6f)
            Destroy(this.gameObject);
    }

    private void Rotate(Vector2 direction)
    {
        float angle = direction.GetVector2Angle();
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void Rotate(Vector2 direction, float speed)
    {
        Quaternion goal = Quaternion.Euler(0, 0, direction.GetVector2Angle() - 90);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, Time.deltaTime * speed);

    }
}
