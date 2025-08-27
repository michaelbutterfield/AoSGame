using Godot;
using System;

public partial class InputManager : Node
{
    [Signal]
    public delegate void UnitSelectedEventHandler(Unit unit);

    [Signal]
    public delegate void UnitDeselectedEventHandler();

    [Signal]
    public delegate void PositionSelectedEventHandler(Vector3 position);

    [Signal]
    public delegate void DiceRollRequestedEventHandler(int count, int sides);

    [Signal]
    public delegate void CommandPointsToggleRequestedEventHandler();

    [Signal]
    public delegate void UnitAbilitiesToggleRequestedEventHandler();

    [Signal]
    public delegate void RadiusIndicatorsToggleRequestedEventHandler();

    [Signal]
    public delegate void RadiusIndicatorsTypeToggleRequestedEventHandler(string indicatorType);

    public static InputManager Instance { get; private set; }

    private Camera3D _camera;
    private Unit _selectedUnit;
    private Vector3 _lastMousePosition;
    private bool _isDragging = false;
    private bool _isMeasuring = false;
    private Vector3 _measureStartPoint;

    // Camera controls
    private float _cameraSpeed = 10.0f;
    private float _cameraZoomSpeed = 5.0f;
    private float _cameraRotationSpeed = 2.0f;
    private float _minZoom = 5.0f;
    private float _maxZoom = 50.0f;

    public override void _Ready()
    {
        Instance = this;
        SetupCamera();
    }

    private void SetupCamera()
    {
        _camera = GetNode<Camera3D>("/root/MainScene/Camera3D");
        if (_camera == null)
        {
            GD.PrintErr("InputManager: Camera not found!");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            HandleMouseButton(mouseButton);
        }
        else if (@event is InputEventMouseMotion mouseMotion)
        {
            HandleMouseMotion(mouseMotion);
        }
        else if (@event is InputEventKey key)
        {
            HandleKeyInput(key);
        }
    }

    public override void _Process(double delta)
    {
        HandleCameraMovement(delta);
        HandleCameraZoom(delta);
    }

    private void HandleMouseButton(InputEventMouseButton mouseButton)
    {
        if (mouseButton.ButtonIndex == MouseButton.Left)
        {
            if (mouseButton.Pressed)
            {
                HandleLeftClick();
            }
        }
        else if (mouseButton.ButtonIndex == MouseButton.Right)
        {
            if (mouseButton.Pressed)
            {
                HandleRightClick();
            }
        }
        else if (mouseButton.ButtonIndex == MouseButton.WheelUp)
        {
            HandleZoomIn();
        }
        else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
        {
            HandleZoomOut();
        }
    }

    private void HandleLeftClick()
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        
        if (worldPosition != Vector3.Zero)
        {
            // Check if clicking on a unit
            Unit clickedUnit = GetUnitAtPosition(worldPosition);
            
            if (clickedUnit != null)
            {
                SelectUnit(clickedUnit);
            }
            else
            {
                // Clicked on empty space
                if (_selectedUnit != null)
                {
                    // Try to move selected unit
                    if (GameManager.Instance.CurrentTurnPhase == GameManager.TurnPhase.Movement)
                    {
                        MoveSelectedUnit(worldPosition);
                    }
                }
                
                DeselectUnit();
            }
        }
    }

    private void HandleRightClick()
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        
        if (worldPosition != Vector3.Zero)
        {
            if (_isMeasuring)
            {
                // End measurement
                EndMeasurement(worldPosition);
            }
            else
            {
                // Start measurement
                StartMeasurement(worldPosition);
            }
        }
    }

    private void HandleMouseMotion(InputEventMouseMotion mouseMotion)
    {
        Vector2 mouseDelta = mouseMotion.Relative;
        _lastMousePosition = mouseMotion.Position;

        // Camera panning with middle mouse button
        if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            PanCamera(mouseDelta);
        }

        // Update measurement line if measuring
        if (_isMeasuring)
        {
            UpdateMeasurementLine();
        }
    }

    private void HandleKeyInput(InputEventKey keyEvent)
    {
        if (!keyEvent.Pressed) return;

        switch (keyEvent.Keycode)
        {
            case Key.W:
                MoveCameraForward();
                break;
            case Key.S:
                MoveCameraBackward();
                break;
            case Key.A:
                MoveCameraLeft();
                break;
            case Key.D:
                MoveCameraRight();
                break;
            case Key.Q:
                RotateCameraLeft();
                break;
            case Key.E:
                RotateCameraRight();
                break;
            case Key.Z:
                ZoomCameraIn();
                break;
            case Key.X:
                ZoomCameraOut();
                break;
            case Key.Space:
                ToggleCommandPointTracker();
                break;
            case Key.P:
                ToggleCommandPointTracker();
                break;
            case Key.U:
                ToggleUnitAbilities();
                break;
            case Key.R:
                ToggleRadiusIndicators();
                break;
            case Key.B:
                ToggleRadiusIndicatorsType("Buff");
                break;
            case Key.V:
                ToggleRadiusIndicatorsType("Debuff");
                break;
            case Key.T:
                ToggleRadiusIndicatorsType("Terrain");
                break;
            case Key.Y:
                ToggleRadiusIndicatorsType("Aura");
                break;
        }
    }

    private void HandleCameraMovement(double delta)
    {
        Vector3 movement = Vector3.Zero;

        if (Input.IsActionPressed("ui_left"))
            movement.X -= 1;
        if (Input.IsActionPressed("ui_right"))
            movement.X += 1;
        if (Input.IsActionPressed("ui_up"))
            movement.Z -= 1;
        if (Input.IsActionPressed("ui_down"))
            movement.Z += 1;

        if (movement != Vector3.Zero)
        {
            movement = movement.Normalized() * _cameraSpeed * (float)delta;
            _camera.Translate(movement);
        }
    }

    private void HandleCameraZoom(double delta)
    {
        if (Input.IsActionPressed("camera_zoom_in"))
        {
            HandleZoomIn();
        }
        if (Input.IsActionPressed("camera_zoom_out"))
        {
            HandleZoomOut();
        }
    }

    private void HandleZoomIn()
    {
        if (_camera != null)
        {
            Vector3 zoomDirection = -_camera.Transform.Basis.Z;
            _camera.Translate(zoomDirection * _cameraZoomSpeed);
            
            // Limit zoom
            float distance = _camera.GlobalPosition.Length();
            if (distance < _minZoom)
            {
                _camera.GlobalPosition = _camera.GlobalPosition.Normalized() * _minZoom;
            }
        }
    }

    private void HandleZoomOut()
    {
        if (_camera != null)
        {
            Vector3 zoomDirection = _camera.Transform.Basis.Z;
            _camera.Translate(zoomDirection * _cameraZoomSpeed);
            
            // Limit zoom
            float distance = _camera.GlobalPosition.Length();
            if (distance > _maxZoom)
            {
                _camera.GlobalPosition = _camera.GlobalPosition.Normalized() * _maxZoom;
            }
        }
    }

    private void PanCamera(Vector2 delta)
    {
        if (_camera != null)
        {
            Vector3 panDirection = new Vector3(-delta.X, 0, -delta.Y) * 0.01f;
            _camera.Translate(panDirection);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (_camera == null) return Vector3.Zero;

        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 from = _camera.ProjectRayOrigin(mousePos);
        Vector3 to = from + _camera.ProjectRayNormal(mousePos) * 1000.0f;

        // Raycast against the board plane (Y = 0)
        float t = -from.Y / (to.Y - from.Y);
        if (t > 0)
        {
            return from + (to - from) * t;
        }

        return Vector3.Zero;
    }

    private Unit GetUnitAtPosition(Vector3 position)
    {
        // Simple distance-based unit detection
        foreach (Unit unit in GameManager.Instance.AllUnits)
        {
            float distance = position.DistanceTo(unit.Position);
            if (distance <= unit.BaseSize)
            {
                return unit;
            }
        }
        return null;
    }

    private void SelectUnit(Unit unit)
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.Deselect();
        }

        _selectedUnit = unit;
        unit.Select();
        EmitSignal(SignalName.UnitSelected, unit);
        
        GD.Print($"InputManager: Selected unit {unit.UnitName}");
    }

    private void DeselectUnit()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.Deselect();
            _selectedUnit = null;
            EmitSignal(SignalName.UnitDeselected);
        }
    }

    private void MoveSelectedUnit(Vector3 targetPosition)
    {
        if (_selectedUnit != null && _selectedUnit.CanMove())
        {
            if (_selectedUnit.MoveTo(targetPosition))
            {
                GD.Print($"InputManager: Moved {_selectedUnit.UnitName} to {targetPosition}");
                
                // Sync movement in multiplayer
                if (NetworkManager.Instance != null && NetworkManager.Instance.IsConnected)
                {
                    NetworkManager.Instance.MoveUnit(_selectedUnit.Id, targetPosition);
                }
            }
            else
            {
                GD.Print($"InputManager: Cannot move {_selectedUnit.UnitName} to {targetPosition}");
            }
        }
    }

    private void StartMeasurement(Vector3 startPoint)
    {
        _isMeasuring = true;
        _measureStartPoint = startPoint;
        GD.Print($"InputManager: Started measurement at {startPoint}");
    }

    private void EndMeasurement(Vector3 endPoint)
    {
        _isMeasuring = false;
        float distance = _measureStartPoint.DistanceTo(endPoint);
        float distanceInches = GameManager.Instance.ConvertUnitsToInches(distance);
        GD.Print($"InputManager: Measurement: {distanceInches:F1} inches");
    }

    private void UpdateMeasurementLine()
    {
        // Update visual measurement line
        Vector3 currentPoint = GetMouseWorldPosition();
        if (currentPoint != Vector3.Zero)
        {
            float distance = _measureStartPoint.DistanceTo(currentPoint);
            float distanceInches = GameManager.Instance.ConvertUnitsToInches(distance);
            // Update UI to show current measurement
        }
    }

    private void MoveCameraForward()
    {
        if (_camera != null)
        {
            Vector3 forward = -_camera.Transform.Basis.Z;
            _camera.Translate(forward * _cameraSpeed * 0.1f);
        }
    }

    private void MoveCameraBackward()
    {
        if (_camera != null)
        {
            Vector3 backward = _camera.Transform.Basis.Z;
            _camera.Translate(backward * _cameraSpeed * 0.1f);
        }
    }

    private void MoveCameraLeft()
    {
        if (_camera != null)
        {
            Vector3 left = -_camera.Transform.Basis.X;
            _camera.Translate(left * _cameraSpeed * 0.1f);
        }
    }

    private void MoveCameraRight()
    {
        if (_camera != null)
        {
            Vector3 right = _camera.Transform.Basis.X;
            _camera.Translate(right * _cameraSpeed * 0.1f);
        }
    }

    private void RotateCameraLeft()
    {
        if (_camera != null)
        {
            _camera.RotateY(_cameraRotationSpeed * 0.1f);
        }
    }

    private void RotateCameraRight()
    {
        if (_camera != null)
        {
            _camera.RotateY(-_cameraRotationSpeed * 0.1f);
        }
    }

    private void ZoomCameraIn()
    {
        HandleZoomIn();
    }

    private void ZoomCameraOut()
    {
        HandleZoomOut();
    }

    private void ToggleCommandPointTracker()
    {
        EmitSignal(SignalName.CommandPointsToggleRequested);
        GD.Print("InputManager: Command points toggle requested");
    }
    
    private void ToggleUnitAbilities()
    {
        EmitSignal(SignalName.UnitAbilitiesToggleRequested);
        GD.Print("InputManager: Unit abilities toggle requested");
    }

    private void ToggleRadiusIndicators()
    {
        EmitSignal(SignalName.RadiusIndicatorsToggleRequested);
        GD.Print("InputManager: Radius indicators toggle requested");
    }

    private void ToggleRadiusIndicatorsType(string indicatorType)
    {
        EmitSignal(SignalName.RadiusIndicatorsTypeToggleRequested, indicatorType);
        GD.Print($"InputManager: Radius indicators type toggle requested: {indicatorType}");
    }

    public Unit GetSelectedUnit()
    {
        return _selectedUnit;
    }

    public bool IsMeasuring()
    {
        return _isMeasuring;
    }

    public Vector3 GetMeasureStartPoint()
    {
        return _measureStartPoint;
    }
}
