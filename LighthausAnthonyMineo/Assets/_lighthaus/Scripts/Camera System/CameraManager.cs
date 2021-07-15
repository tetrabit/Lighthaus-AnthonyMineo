using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CameraManager : MonoBehaviour
{
    private List<Camera> _cameras = new List<Camera>();
    private int _currentCamera = 0;

    [Inject]
    private void Construct(OrbitCamera orbitCamera, SplineCamera splineCamera)
    {
        _cameras.Add(orbitCamera.Camera);
        _cameras.Add(splineCamera.Camera);
    }

    void Start()
    {
        UpdateCamera();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            _currentCamera++;
            UpdateCamera();
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _currentCamera--;
            UpdateCamera();
        }
    }

    private void UpdateCamera()
    {
        if (_currentCamera > _cameras.Count() - 1)
            _currentCamera = 0;
        else if (_currentCamera < 0)
            _currentCamera = _cameras.Count() - 1;

        for (int i = 0; i < _cameras.Count(); i++)
        {
            if (i != _currentCamera)
                _cameras[i].enabled = false;
            else
                _cameras[i].enabled = true;
        }
    }    
}
