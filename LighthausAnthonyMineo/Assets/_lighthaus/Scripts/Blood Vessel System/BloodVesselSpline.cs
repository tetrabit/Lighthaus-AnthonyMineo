using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineComputer))]
public class BloodVesselSpline : MonoBehaviour
{
    [SerializeField]
    private SplineComputer _spline; 

    public SplineComputer Spline { get => _spline; set => _spline = value; }

    private void Awake()
    {
        Init();
    }

    private void OnValidate()
    {
        Init();
    }

    private void Init()
    {
        _spline = GetComponent<SplineComputer>();
    }
}
