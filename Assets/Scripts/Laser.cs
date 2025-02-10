using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float _laserSpeed = 8;
    [SerializeField] float _laserDeathPosition = 8;

    void Update()
    {
        transform.Translate(Vector3.up *_laserSpeed * Time.deltaTime);

        if(transform.position.y > _laserDeathPosition)
        {
            if(transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
            
    }
}
