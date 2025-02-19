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
    
    Transform _player, _enemy, _target;
    bool _homingOnPlayer;

    float _homingRadius = 10;
    float _turnSpeed = 23;
    float _speed = 5;

    int _playerMask = 0b_0000_0000_0000_0000_0000_0010_0000_0000;
    int _enemyMask = 0b_0000_0000_0000_0000_0000_0100_0000_0000;

    public void SetOnPlayer(bool homingOnPlayer)
    {
        Transform player = GameManager.Instance.PlayerTransform;
        if (homingOnPlayer && player)
        {
            _turnSpeed = 23;
            _speed = 5;
            this._player = player.transform;
            _target = _player;
            _homingOnPlayer = true;
        }
        else
        {
            _homingOnPlayer = false;
            _turnSpeed = 40;
            _speed = 7;
            FindClosestEnemy();            

            tag = "Untagged";
            _target = _enemy;
        }
    }

    public void Destroy()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        FindClosestEnemy();
        if (_target)
        {
            Vector2 direction = GetDirection(_target.position, transform.position);
            
            if(transform.position.y > -4f)
                Rotate(direction, _turnSpeed);

            Vector3 moveDir = GetVector2FromAngle(transform.rotation.eulerAngles.z + 90);

            transform.position += moveDir * _speed * Time.deltaTime;
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

    private void FindClosestEnemy()
    {
        if (_homingOnPlayer) return;

        Collider2D[] enemyHitAll = Physics2D.OverlapCircleAll(transform.position, _homingRadius, _enemyMask);

        Collider2D enemyHit = null;
        float sqrdDistance = float.MaxValue;
        Vector3 hitPosition = Vector3.zero;
        Vector3 myPosition = transform.position;
        foreach (var hit in enemyHitAll)
        {
            hitPosition = hit.gameObject.transform.position;
            if (myPosition.y > hitPosition.y)
            {
               continue;
            }

            float current = SqrdDistance(hit.gameObject.transform.position, myPosition);

            if (current < sqrdDistance)
            {
                sqrdDistance = current;
                enemyHit = hit;
            }                
        }

        if (enemyHit == null) return;
        bool isHit = enemyHit;
        bool sameEnemy = enemyHit.gameObject.transform == _target;
        if (!sameEnemy)
        {
            _enemy = enemyHit.gameObject.transform;
            _target = _enemy.gameObject.transform;
        }
            
    }

    private float SqrdDistance(Vector3 a, Vector3 b)
    {
        float num = a.x - b.x;
        float num2 = a.y - b.y;
        float num3 = a.z - b.z;
        return num * num + num2 * num2 + num3 * num3;
    }
}