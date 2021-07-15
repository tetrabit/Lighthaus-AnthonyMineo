public class TelemetrySignal 
{
    private bool _active;

    public bool Active { get => _active; set => _active = value; }

    public TelemetrySignal(bool active)
    {
        _active = active;
    }
}