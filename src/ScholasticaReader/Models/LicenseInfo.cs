using System;

namespace ScholasticaReader.Models;

public class LicenseInfo
{
    public int Id { get; set; }
    public string HardwareId { get; set; } = string.Empty;
    public string ActivationCode { get; set; } = string.Empty;
    public DateTime ActivatedOn { get; set; }
    public bool IsActive { get; set; }
}
