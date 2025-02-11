using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed = 3.5f;
    [SerializeField] float _respawnYPos = 8;
    [SerializeField] int _scoreReward = 10;

    UIManager _manager;
    private void Awake()
    {
        _manager = FindObjectOfType<UIManager>();
    }
    void Update()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < -6f)
        {
            float x = Random.Range(-9, 9f);

            transform.position = new Vector3(x, _respawnYPos, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.Damage();
            Destroy(this.gameObject);
        }

        else if (other.TryGetComponent(out Laser laser))
        {
            _manager.UpdateScore(_scoreReward);
            Destroy(laser.gameObject);
            Destroy(this.gameObject);            
        }
    }
}
