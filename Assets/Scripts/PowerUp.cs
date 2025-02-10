using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3;
    [SerializeField] private float _verticalBound = -7.5f;

    void Update()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        if (transform.position.y < _verticalBound)
        {
            print($"Destroying at MY pos: {transform.position.y}; bound: {_verticalBound}");
            Destroy(this.gameObject);
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Player player))
        {
            player.TripleShotPowerUp();
            Destroy(this.gameObject);
        }
    }
}
