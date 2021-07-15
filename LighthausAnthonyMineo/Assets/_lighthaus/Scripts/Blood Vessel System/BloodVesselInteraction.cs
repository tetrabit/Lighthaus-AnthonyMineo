using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Zenject;

public class BloodVesselInteraction : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private GameObject _projectionTarget;
    private Ray _ray = new Ray();
    private RaycastHit _hit = new RaycastHit();
    private LayerMask _layerMask;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Awake()
    {
        _layerMask = 1 << LayerMask.NameToLayer("VelocityMeasurement");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(_ray, out _hit, Mathf.Infinity,_layerMask))
            {
                _projectionTarget.transform.position = _hit.point;
                _signalBus.Fire(new ActiveBloodVesselSignal(_hit.transform.GetComponentInParent<BloodVessel>()));
            }
        }
    }
}
