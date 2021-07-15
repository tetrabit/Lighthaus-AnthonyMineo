using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

public class VelocityVisualizer : MonoBehaviour
{
    private GameObject _arrow;
    private SignalBus _signalBus;
    private CellMeasurement _averageCellVelocity;

    [Inject]
    private void Construct(SignalBus signalBus, CellMeasurement averageCellVelocity)
    {
        _signalBus = signalBus;
        _averageCellVelocity = averageCellVelocity;
    }

    void Awake()
    {
        _arrow = this.gameObject;
        _signalBus.GetStream<TelemetrySignal>()
            .Subscribe(x => _arrow.SetActive(x.Active))
            .AddTo(gameObject);
    }

    void Update()
    {
        Vector3 direction = Quaternion.FromToRotation(Vector3.up, _averageCellVelocity.AverageVelocity.normalized).eulerAngles;
        _arrow.transform.eulerAngles = new Vector3(direction.x, direction.y, direction.z);
    }
}
