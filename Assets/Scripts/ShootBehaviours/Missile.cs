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

    float _turnSpeed = 23;
    float _speed = 5;

    int _playerMask = 0b_0000_0000_0000_0000_0000_0010_0000_0000;
    int _enemyMask = 0b_0000_0000_0000_0000_0000_0100_0000_0000;

    public void SetOnPlayer(bool homingOnPlayer)
    {
        print($"Setting Player");
        Transform player = GameManager.Instance.PlayerTransform;
        if (homingOnPlayer)
        {
            _turnSpeed = 23;
            _speed = 5;
            this._player = player.transform;
            _target = _player;
            _homingOnPlayer = homingOnPlayer;
        }
        else
        {
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
        if (!_homingOnPlayer)
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
        Collider2D[] enemyHitAll = Physics2D.OverlapCircleAll(transform.position, 50, _enemyMask);

        Collider2D enemyHit = null;
        float sqrdDistance = float.MaxValue;
        foreach (var hit in enemyHitAll)
        {
            float current = GetDelta(hit.gameObject.transform.position, transform.position).sqrMagnitude;

            if (current < sqrdDistance)
                enemyHit = hit;
        }

        if (enemyHit)
        {
            print($"Enemy Sighted: {enemyHit.gameObject.transform.position}");

            _enemy = enemyHit.gameObject.transform;
        }
    }
}
