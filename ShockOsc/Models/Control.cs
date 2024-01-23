﻿using System.ComponentModel.DataAnnotations;

namespace OpenShock.ShockOsc.Models;

public class Control
{
    public required Guid Id { get; set; }
    public rftransmit? rftransmit { get; set; }
    public required ControlType Type { get; set; }
    [Range(1, 100)]
    public required byte Intensity { get; set; }
    [Range(300, 30000)]
    public required uint Duration { get; set; }
}