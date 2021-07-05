using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    private List<Camera> _cameras = new List<Camera>();
    private int _currentCamera = 0;

    private void Awake()
    {
        _cameras = FindObjectsOfType<Camera>().ToList();
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
        if (_currentCamera < 0 || _currentCamera > _cameras.Count() - 1)
            _currentCamera = 0;

        for (int i = 0; i < _cameras.Count(); i++)
        {
            if (i != _currentCamera)
                _cameras[i].enabled = false;
            else
                _cameras[i].enabled = true;
        }
    }    
}
