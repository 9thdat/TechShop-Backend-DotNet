﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PhoneShopManagementBackend.Models;

[Table("customer_validate_email")]
public partial class CustomerValidateEmail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; }

    [Column("validate_key_hash")]
    [StringLength(64)]
    public string ValidateKeyHash { get; set; }

    [Column("validate_key_expires_at", TypeName = "datetime")]
    public DateTime? ValidateKeyExpiresAt { get; set; }
}