using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour {

	[SerializeField]
	private Transform _focus = default;

	[SerializeField, Range(1f, 20f)]
	private float _distance = 5f;

	[SerializeField, Min(0f)]
	private float _focusRadius = 5f;

	[SerializeField, Range(0f, 1f)]
	private float _focusCentering = 0.5f;

	[SerializeField, Range(1f, 360f)]
	private float _rotationSpeed = 90f;

	[SerializeField, Range(-89f, 89f)]
	private float _minVerticalAngle = -45f, maxVerticalAngle = 45f;

	[SerializeField, Min(0f)]
	private float _alignDelay = 5f;

	[SerializeField, Range(0f, 90f)]
	private float _alignSmoothRange = 45f;

	[SerializeField]
	private LayerMask _obstructionMask = -1;

	private Camera _camera;

	private Vector3 _focusPoint, previousFocusPoint;

	private Vector2 _orbitAngles = new Vector2(45f, 0f);

	private float _lastManualRotationTime;

	Vector3 CameraHalfExtends {
		get {
			Vector3 halfExtends;
			halfExtends.y =
				Camera.nearClipPlane *
				Mathf.Tan(0.5f * Mathf.Deg2Rad * Camera.fieldOfView);
			halfExtends.x = halfExtends.y * Camera.aspect;
			halfExtends.z = 0f;
			return halfExtends;
		}
	}

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

    void OnValidate () 
	{
		if (maxVerticalAngle < _minVerticalAngle) {
			maxVerticalAngle = _minVerticalAngle;
		}
	}

	void Awake () 
	{
		_focusPoint = _focus.position;
		transform.localRotation = Quaternion.Euler(_orbitAngles);
	}

	void LateUpdate () 
	{
		UpdateFocusPoint();
		Quaternion lookRotation;
		if (ManualRotation() || AutomaticRotation()) {
			ConstrainAngles();
			lookRotation = Quaternion.Euler(_orbitAngles);
		}
		else {
			lookRotation = transform.localRotation;
		}

		Vector3 lookDirection = lookRotation * Vector3.forward;
		Vector3 lookPosition = _focusPoint - lookDirection * _distance;

		Vector3 rectOffset = lookDirection * Camera.nearClipPlane;
		Vector3 rectPosition = lookPosition + rectOffset;
		Vector3 castFrom = _focus.position;
		Vector3 castLine = rectPosition - castFrom;
		float castDistance = castLine.magnitude;
		Vector3 castDirection = castLine / castDistance;

		if (Physics.BoxCast(
			castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
			lookRotation, castDistance, _obstructionMask
		)) {
			rectPosition = castFrom + castDirection * hit.distance;
			lookPosition = rectPosition - rectOffset;
		}

		_distance += Input.mouseScrollDelta.y;
		_distance = Mathf.Clamp(_distance, 6, 20);
		
		transform.SetPositionAndRotation(lookPosition, lookRotation);
	}

	void UpdateFocusPoint () 
	{
		previousFocusPoint = _focusPoint;
		Vector3 targetPoint = _focus.position;
		if (_focusRadius > 0f) {
			float distance = Vector3.Distance(targetPoint, _focusPoint);
			float t = 1f;
			if (distance > 0.01f && _focusCentering > 0f) {
				t = Mathf.Pow(1f - _focusCentering, Time.unscaledDeltaTime);
			}
			if (distance > _focusRadius) {
				t = Mathf.Min(t, _focusRadius / distance);
			}
			_focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
		}
		else {
			_focusPoint = targetPoint;
		}
	}

	bool ManualRotation () 
	{
		Vector2 input = new Vector2(
			Input.GetAxis("Vertical Camera"),
			Input.GetAxis("Horizontal Camera")
		);
		const float e = 0.001f;
		if (input.x < -e || input.x > e || input.y < -e || input.y > e) 
		{
			input.x = -input.x;
			if(Input.GetMouseButton(1))
            {
				_orbitAngles += _rotationSpeed * Time.unscaledDeltaTime * input;
				_lastManualRotationTime = Time.unscaledTime;
            }
			return true;
		}
		return false;
	}

	bool AutomaticRotation () 
	{
		if (Time.unscaledTime - _lastManualRotationTime < _alignDelay) {
			return false;
		}

		Vector2 movement = new Vector2(
			_focusPoint.x - previousFocusPoint.x,
			_focusPoint.z - previousFocusPoint.z
		);
		float movementDeltaSqr = movement.sqrMagnitude;
		if (movementDeltaSqr < 0.0001f) {
			return false;
		}

		float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
		float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(_orbitAngles.y, headingAngle));
		float rotationChange =
			_rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
		if (deltaAbs < _alignSmoothRange) {
			rotationChange *= deltaAbs / _alignSmoothRange;
		}
		else if (180f - deltaAbs < _alignSmoothRange) {
			rotationChange *= (180f - deltaAbs) / _alignSmoothRange;
		}
		_orbitAngles.y =
			Mathf.MoveTowardsAngle(_orbitAngles.y, headingAngle, rotationChange);
		return true;
	}

	void ConstrainAngles () 
	{
		_orbitAngles.x =
			Mathf.Clamp(_orbitAngles.x, _minVerticalAngle, maxVerticalAngle);

		if (_orbitAngles.y < 0f) {
			_orbitAngles.y += 360f;
		}
		else if (_orbitAngles.y >= 360f) {
			_orbitAngles.y -= 360f;
		}
	}

	static float GetAngle (Vector2 direction) {
		float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
		return direction.x < 0f ? 360f - angle : angle;
	}
}
