﻿using System.ComponentModel.DataAnnotations;

namespace Onitama.Api.Models.Input;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}