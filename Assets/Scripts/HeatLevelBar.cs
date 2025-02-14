using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatLevelBar : MonoBehaviour
{
    [SerializeField] RectTransform rectTrans;
    [SerializeField] Image bar;
    [SerializeField] Thrusters _playerThrusters;

    Color ogColor;
    Vector3 scale;

    private void Start()
    {
        scale = new Vector3(1,1,1);
        ogColor = bar.color;
    }
    private void Update()
    {
        scale.x = _playerThrusters.HeatLevels;
        rectTrans.localScale = scale;

        if (_playerThrusters.Overheated)
        {
            bar.color = Color.red;
        }
        else
        {
            bar.color = ogColor;
        }
    }
}
