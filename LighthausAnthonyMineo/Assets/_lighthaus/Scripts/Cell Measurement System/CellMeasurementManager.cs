using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Zenject;
using UniRx;

public class CellMeasurementManager : MonoBehaviour
{
    private CellMeasurement _averageCellVelocity;
    private CellMeasurementSpline _averageCellVelocitySpline;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus, CellMeasurement averageCellVelocity, CellMeasurementSpline averageCellVelocitySpline)
    {
        _signalBus = signalBus;
        _averageCellVelocity = averageCellVelocity;
        _averageCellVelocitySpline = averageCellVelocitySpline;
    }

    private void Awake()
    {
        _signalBus.GetStream<ActiveBloodVesselSignal>()
            .Subscribe(x => UpdateActiveBloodVessel(x.BloodVessel.ParticleManager.ParticleSystem, x.BloodVessel.SplineComputer))
            .AddTo(gameObject);
    }

    private void UpdateActiveBloodVessel(ParticleSystem particleSystem, SplineComputer spline)
    {
        _averageCellVelocity.UpdateActiveBloodVessel(particleSystem);
        _averageCellVelocitySpline.SplineProjector.spline = spline;
    }
}
