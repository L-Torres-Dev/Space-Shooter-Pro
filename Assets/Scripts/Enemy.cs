using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed = 3.5f;
    [SerializeField] float _respawnYPos = 8;
    [SerializeField] int _scoreReward = 10;
    [SerializeField] Animator _anim;
    [SerializeField] Collider2D _collider;

    private bool _isDead;
    UIManager _manager;
    private void Awake()
    {
        _manager = FindObjectOfType<UIManager>();
    }
    void Update()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (_isDead) return;
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
            Destroy();
        }

        else if (other.TryGetComponent(out Laser laser))
        {
            _manager.UpdateScore(_scoreReward);
            Destroy(laser.gameObject);
            Destroy();            
        }
    }

    public void Destroy()
    {
        _isDead = true;
        _collider.enabled = false;
        _anim.SetTrigger("OnEnemyDeath");
    }
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
