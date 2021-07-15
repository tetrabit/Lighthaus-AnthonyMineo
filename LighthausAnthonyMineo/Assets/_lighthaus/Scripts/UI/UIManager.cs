using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _telemetry;
    private SignalBus _signalBus;
    private bool _active = false;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Awake()
    {
        _signalBus.GetStream<TelemetrySignal>()
            .Subscribe(x => _telemetry.SetActive(x.Active))
            .AddTo(gameObject);
    }

    void Start()
    {
        _signalBus.Fire(new TelemetrySignal(_active));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _active = !_active;
            _signalBus.Fire(new TelemetrySignal(_active));
        }
    }
}
