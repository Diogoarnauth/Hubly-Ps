using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;
public class EditUserInputModel
{
    [Required]
    public string NewUsername { get; set; }        
}