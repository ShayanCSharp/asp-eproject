using System;
using System.Collections.Generic;

namespace EProjectFinal.Models;

public partial class Manager
{
    public int ManagerId { get; set; }

    public string FullName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
