using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SplineCamera : MonoBehaviour
{
    private Camera _camera;

	public Camera Camera
	{
		get
		{
			if (_camera == null)
				_camera = GetComponent<Camera>();
			return _camera;
		}
		set => _camera = value;
	}
}
