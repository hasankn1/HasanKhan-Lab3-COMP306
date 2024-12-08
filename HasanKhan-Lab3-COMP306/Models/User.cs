using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HasanKhan_Lab3_COMP306.Models;

public class User
{
    [Required(ErrorMessage = "An Email is required")]
    public string UserEmail { get; set; } = null!;

    [Required(ErrorMessage = "A Password is required")]
    public string Password { get; set; } = null!;
}
