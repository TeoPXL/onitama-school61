using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Onitama.Core.UserAggregate;

// No need to change this class (for the minimal requirements)
public class User : IdentityUser<Guid>
{
    [Required]
    public string WarriorName { get; set; }
}