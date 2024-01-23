using OpenShock.ShockOsc.OscChangeTracker;

namespace OpenShock.ShockOsc.Models;

public class Shocker
{
    public DateTime LastActive { get; set; }
    public DateTime LastExecuted { get; set; }
    public DateTime LastVibration { get; set; }
    public uint LastDuration { get; set; }
    public byte LastIntensity { get; set; }
    public float LastStretchValue { get; set; }
    public bool IsGrabbed { get; set; }

    public ChangeTrackedOscParam<bool> ParamActive { get; }
    public ChangeTrackedOscParam<bool> ParamCooldown { get; }
    public ChangeTrackedOscParam<float> ParamCooldownPercentage { get; }
    public ChangeTrackedOscParam<float> ParamIntensity { get; }


    public Guid Id { get; }
    public rftransmit? rftransmit { get; }
    public string Name { get; }
    public TriggerMethod TriggerMethod { get; set; }

    public Shocker(Guid id, string name)
    {
        Id = id;        // Guid.Empty = _All
        Name = name;

        ParamActive = new ChangeTrackedOscParam<bool>(Name, "_Active", false);
        ParamCooldown = new ChangeTrackedOscParam<bool>(Name, "_Cooldown", false);
        ParamCooldownPercentage = new ChangeTrackedOscParam<float>(Name, "_CooldownPercentage", 0f);
        ParamIntensity = new ChangeTrackedOscParam<float>(Name, "_Intensity", 0f);
    }

    public Shocker(rftransmit _rftransmit, string name)
    {
        Id = Guid.Empty;
        rftransmit = _rftransmit;
        Name = name;

        ParamActive = new ChangeTrackedOscParam<bool>(Name, "_Active", false);
        ParamCooldown = new ChangeTrackedOscParam<bool>(Name, "_Cooldown", false);
        ParamCooldownPercentage = new ChangeTrackedOscParam<float>(Name, "_CooldownPercentage", 0f);
        ParamIntensity = new ChangeTrackedOscParam<float>(Name, "_Intensity", 0f);
    }

    public void Reset()
    {
        IsGrabbed = false;
        LastStretchValue = 0;
    }
}