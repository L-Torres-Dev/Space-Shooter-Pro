using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3;
    [SerializeField] private float _verticalBound = -7.5f;

    [SerializeField] private int PowerID;

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
            switch (PowerID)
            {
                case 0:
                    player.TripleShotPowerUp();
                    break;
                case 1:
                    player.SpeedPowerUP();
                    break;
                case 2:
                    player.ShieldPowerUp();
                    break;
                default:
                    print($"Power Up doesn't exist");
                    break;
            }

            Destroy(this.gameObject);
        }
    }
}
