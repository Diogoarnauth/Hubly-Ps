
using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;
public class UserCreateInputModel
{

    [Required]
    public string Name { get; set; }        
    [Required]
    [EmailAddress]
    public string Email {get; set;}
        
    [Required]
    public string Password { get; set; }
}


