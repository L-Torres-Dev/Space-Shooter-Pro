using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3;
    [SerializeField] private float _verticalBound = -7.5f;

    [SerializeField] private int _powerID;

    void Update()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        if (transform.position.y < _verticalBound)
        {
            print($"Destroying at MY pos: {transform.position.y}; bound: {_verticalBound}");
            Destroy(this.gameObject);
        }
            
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
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
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            if(_powerID == 0)
            {
                player.TripleShotPowerUp();
            }
            else if (_powerID == 1)
            {
                player.SpeedPowerUP();
            }
            else if (_powerID == 2)
            {
                player.ShieldPowerUp();
            }
            else
            {
                print("Power Up doesn't exist");
            }

            switch (_powerID)
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
