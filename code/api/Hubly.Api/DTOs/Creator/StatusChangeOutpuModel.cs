using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;

namespace Hubly.api.DTOs;

public class StatusChangeOutpuModel
{
    public string AvailabilityStatus { get; set; } = null!;
}