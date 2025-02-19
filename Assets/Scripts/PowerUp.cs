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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Player player))
        {
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
                case 3:
                    player.AmmoPickup();
                    break;
                case 4:
                    player.Heal();
                    break;
                case 5:
                    player.MultiShotPowerUp();
                    break;
                case 6:
                    player.FakeShot();
                    break;
                default:
                    print($"Power Up doesn't exist");
                    break;
            }

            Destroy(this.gameObject);
        }
        else if (collision.TryGetComponent(out Laser laser))
        {
            Destroy(laser.gameObject);
            Destroy(this.gameObject);
        }
        else if (collision.TryGetComponent(out Missile missile))
        {
            missile.Destroy();
            Destroy(this.gameObject);
        }        
    }
}
