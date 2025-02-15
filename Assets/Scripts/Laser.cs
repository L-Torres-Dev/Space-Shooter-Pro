using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float _laserSpeed = 8;
    [SerializeField] float _laserDeathPosition = 8;
    [SerializeField] float _angle = 90;

    Vector3 direction = Vector3.up;

    void Update()
    {
        transform.position += (direction * _laserSpeed * Time.deltaTime);

        if(transform.position.y > _laserDeathPosition || transform.position.y < -_laserDeathPosition)
        {
            if(transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }            
    }

    public void Rotate()
    {
        float offsetAngle = _angle - 90;
        float angle = Mathf.Deg2Rad * _angle;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        direction = new Vector3(x, y, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, offsetAngle));
    }

    public void ReverseSpeed()
    {
        _laserSpeed *= -1;
    }
}
