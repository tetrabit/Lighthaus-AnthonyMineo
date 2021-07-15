using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CellVelocityUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _cellName;
    [SerializeField]
    TextMeshProUGUI _velocity;

    public TextMeshProUGUI CellName { get => _cellName; set => _cellName = value; }
    public TextMeshProUGUI Velocity { get => _velocity; set => _velocity = value; }

    public void UpdateCellVelocity(string name ,Vector3 velocity)
    {
        _cellName.text = name + ":";
        _velocity.text = velocity.ToString("0.0000");
    }
}
