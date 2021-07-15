using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using System;

public class CellTelemetryManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _averageVelocity;
    [SerializeField]
    private TextMeshProUGUI _averageSpeed;
    [SerializeField]
    private TextMeshProUGUI _totalCellsPassed;
    [SerializeField]
    private GameObject _cellVelocitiesContainer;
    [SerializeField]
    private GameObject _cellVelocityUIPrefab;
    [SerializeField]
    private List<CellVelocityUI> _cellVelocityUIs = new List<CellVelocityUI>();
    [SerializeField, Range(1, 20)]
    private int _velocitiesPoolAmount = 10;
    private CellMeasurement _cellMeasurement;

    [Inject]
    private void Construct(CellMeasurement cellMeasurement)
    {
        _cellMeasurement = cellMeasurement;
    }

    private void Awake()
    {
        PoolUIElements();

        //update average velocity ui only if the value changes checked once an update loop
        Observable.EveryUpdate()
            .Select(x => _cellMeasurement.AverageVelocity)
            .DistinctUntilChanged()
            .Subscribe(x => _averageVelocity.text = x.ToString("0.0000"))
            .AddTo(gameObject);

        //update average speed ui only if the value changes checked once an update loop
        Observable.EveryUpdate()
            .Select(x => _cellMeasurement.AverageSpeed)
            .DistinctUntilChanged()
            .Subscribe(x => _averageSpeed.text = $"{x.ToString("0.0000")} m/s")
            .AddTo(gameObject);

        //update total cells passed ui only if the value changes checked once an update loop
        Observable.EveryUpdate()
            .Select(x => _cellMeasurement.CellCount)
            .DistinctUntilChanged()
            .Subscribe(x => _totalCellsPassed.text = x.ToString())
            .AddTo(gameObject);

        //update ui every 250 millisecionds (partly done to allow values to be readable, partly done for performance reasons)
        Observable.Interval(TimeSpan.FromMilliseconds(250))
            .Subscribe(_ => VelocitiesUpdate())
            .AddTo(gameObject);
    }

    private void VelocitiesUpdate()
    {
        int availableDataCount = Mathf.Min(_cellMeasurement.Velocities.Count, _cellMeasurement.ParticlesToCalculate.Count);
        for (int i = 0; i < _cellVelocityUIs.Count; i++)
        {
            if (i >= Mathf.Min(_cellVelocityUIs.Count, availableDataCount))
                _cellVelocityUIs[i].UpdateCellVelocity("no cell", Vector3.zero);
            else
                _cellVelocityUIs[i].UpdateCellVelocity(_cellMeasurement.ParticlesToCalculate.ElementAt(i).Key.ToString(), _cellMeasurement.Velocities[i]);
        }
    }

    private void PoolUIElements()
    {
        for(int i = 0; i < _velocitiesPoolAmount; i++)
        {
            GameObject velocityUI = Instantiate(_cellVelocityUIPrefab, _cellVelocitiesContainer.transform);
            _cellVelocityUIs.Add(velocityUI.GetComponent<CellVelocityUI>());
        }
    }
}
