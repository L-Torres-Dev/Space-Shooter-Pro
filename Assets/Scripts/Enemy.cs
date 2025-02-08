using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 3.5f;
    [SerializeField] float respawnYPos = 8;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.Damage();
            Destroy(this.gameObject);
        }

        else if (other.TryGetComponent(out Laser laser))
        {
            Destroy(laser.gameObject);
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        transform.Translate(new Vector3(0, -speed, 0) * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            float x = Random.Range(-9, 9f);

            transform.position = new Vector3(x, respawnYPos, 0);
        }
    }
}
