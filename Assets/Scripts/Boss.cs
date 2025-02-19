using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] GameObject _multiShotPrefab;
    [SerializeField] Missile _missilePrefab;
    [SerializeField] Transform _missileLauncher1;
    [SerializeField] Transform _missileLauncher2;
    [SerializeField] GameObject _shield;
    [SerializeField] Collider2D _collider;
    [SerializeField] Animator _anim;
    [SerializeField] float yPosition = 3.95f;
    [SerializeField] int health = 10;
    [SerializeField] float speed = 3;
    [SerializeField] float xPos = 8.9f;

    float newOffset = 22;
    AudioSource _laserAudioSource, _explosionAudioSource;

    int _shieldStrength;

    private void Start()
    {
        StartCoroutine(CO_MoveDownWard());
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy Laser")) return;

        if (collision.TryGetComponent(out Laser laser))
        {
            if (_shieldStrength > 0)
            {
                _shieldStrength--;
                Destroy(laser.gameObject);

                if (_shieldStrength < 1)
                    _shield.gameObject.SetActive(false);
                return;
            }
            health--;
            Destroy(laser.gameObject);
            
        }
        else if (collision.TryGetComponent(out Missile missile))
        {
            if (_shieldStrength > 0)
            {
                _shieldStrength--;
                Destroy(missile.gameObject);

                if (_shieldStrength < 1)
                    _shield.gameObject.SetActive(false);
                return;
            }
            missile.Destroy();
            health--;
        }

        if (health <= 0)
        {
            Destroy();
        }
    }
    private IEnumerator CO_MoveDownWard()
    {
        while (transform.position.y > yPosition)
        {
            transform.Translate(Vector3.down * (2 * Time.deltaTime));

            if(transform.position.y < yPosition)
                transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);


            yield return null;
        }

        StartCoroutine(CO_Shoot());
        StartCoroutine(CO_ShootMissiles());
        StartCoroutine(CO_MoveHorizontal());
        StartCoroutine(CO_Shield());
    }

    private IEnumerator CO_MoveHorizontal()
    {
        var wait = new WaitForSeconds(.75f);

        while (true)
        {
            while(transform.position.x < xPos)
            {
                transform.Translate(Vector3.right * (speed * Time.deltaTime));
                yield return null;
            }

            yield return wait;

            while (transform.position.x > -xPos)
            {
                transform.Translate(Vector3.left * (speed * Time.deltaTime));
                yield return null;
            }

            yield return wait;
        }
    }

    private IEnumerator CO_Shoot()
    {
        float fireRate = 2;
        

        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            for (int i = 0; i < 4; i++)
            {
                var multiShot = Instantiate(_multiShotPrefab, transform.position, Quaternion.identity);

                _laserAudioSource.Play();
                for (int j = 0; j < multiShot.transform.childCount; j++)
                {
                    Laser laser = multiShot.transform.GetChild(j).GetComponent<Laser>();

                    laser.RotateOpposite(newOffset);
                    laser.tag = "Enemy Laser";
                }
                yield return new WaitForSeconds(.5f);
            }
        }
    }

    private IEnumerator CO_ShootMissiles()
    {
        float missileFireRate = 3;
        while (true)
        {
            yield return new WaitForSeconds(missileFireRate);

            Transform currentTransform;
            for (int i = 0; i < 3; i++)
            {
                currentTransform = i % 2 == 0 ? _missileLauncher1 : _missileLauncher2;

                float angle = currentTransform.position.x > 0 ? -180 : 180;

                Missile missile = Instantiate(_missilePrefab, currentTransform.position, Quaternion.Euler(0, 0, 180));

                missile.SetOnPlayer(true);
            }
        }
    }

    private IEnumerator CO_Shield()
    {
        while (true)
        {
            if(_shieldStrength == 0)
            {
                yield return new WaitForSeconds(3);

                if (_shieldStrength == 0)
                    ForceShieldOn();
            }
            yield return null;
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void ForceShieldOn()
    {
        _shield.gameObject.SetActive(true);
        _shieldStrength = 3;
    }
    public void SetExplosionAudio(AudioSource audioSource)
    {
        _explosionAudioSource = audioSource;
    }
    public void SetLaserAudio(AudioSource audioSource)
    {
        _laserAudioSource = audioSource;
    }

    public void Destroy()
    {
        StopAllCoroutines();
        _collider.enabled = false;
        _explosionAudioSource.Play();
        _anim.SetTrigger("OnEnemyDeath");
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
