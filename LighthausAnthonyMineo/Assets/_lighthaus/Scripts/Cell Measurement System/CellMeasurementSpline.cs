using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class CellMeasurementSpline : MonoBehaviour
{
    private SplineProjector _splineProjector;

    public SplineProjector SplineProjector { get => _splineProjector; set => _splineProjector = value; }

    private void Awake()
    {
        _splineProjector = GetComponent<SplineProjector>();
    }
}
