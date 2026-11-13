
using Microsoft.AspNetCore.Identity;

namespace Smart_Community_Health_Monitoring_System.Models.Identity;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
