using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] ShieldUI _shieldUI;
    [SerializeField] private GameObject _playerShield;

    private int _shieldStrength;

    private void Awake()
    {
        _shieldStrength = 0;
        _shieldUI.VisualizeShields(0);
    }

    public void AddShields()
    {
        _shieldStrength = 3;
        _shieldUI.VisualizeShields(_shieldStrength);
        _playerShield.gameObject.SetActive(true);
    }

    public void Damage()
    {
        _shieldStrength -= 1;
        if(_shieldStrength <= 0)
            _playerShield.gameObject.SetActive(false);

        _shieldUI.VisualizeShields(_shieldStrength);

    }

    public int ShieldStrength => _shieldStrength;
}
