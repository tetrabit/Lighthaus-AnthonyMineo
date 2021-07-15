public class ActiveBloodVesselSignal
{
    private BloodVessel _bloodVessel;

    public BloodVessel BloodVessel { get => _bloodVessel; set => _bloodVessel = value; }

    public ActiveBloodVesselSignal(BloodVessel bloodVessel)
    {
        _bloodVessel = bloodVessel;
    }
}