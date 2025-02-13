using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed = 3.5f;
    [SerializeField] float _respawnYPos = 8;
    [SerializeField] int _scoreReward = 10;
    [SerializeField] Animator _anim;
    [SerializeField] Collider2D _collider;
    [SerializeField] Laser _laserPrefab;
    

    private AudioSource _explosionAudioSource;
    AudioSource _laserAudioSource;
    private bool _isDead;
    UIManager _manager;
    private void Awake()
    {
        _manager = FindObjectOfType<UIManager>();
        StartCoroutine(CO_Shoot());

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
        if (other.CompareTag("Enemy Laser")) return;

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
        StopAllCoroutines();
        _isDead = true;
        _collider.enabled = false;
        _explosionAudioSource.Play();
        _anim.SetTrigger("OnEnemyDeath");
    }
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
    public void SetExplosionAudio(AudioSource audioSource)
    {
        _explosionAudioSource = audioSource;
    }
    public void SetLaserAudio(AudioSource audioSource)
    {
        _laserAudioSource = audioSource;
    }

    private IEnumerator CO_Shoot()
    {
        float fireRate = 5;

        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            Laser laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);

            _laserAudioSource.Play();
            laser.tag = "Enemy Laser";
            laser.ReverseSpeed();
            fireRate = Random.Range(2, 5);
        }        
    }
}
