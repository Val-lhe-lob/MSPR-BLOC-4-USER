using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MSPR_BLOC_4_USER.Models;

[Table("User")]
[Index("Username", Name = "UQ__User__536C85E493EAE937", IsUnique = true)]
[Index("Email", Name = "UQ__User__A9D10534E96C7099", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string AccountType { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastModifiedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
