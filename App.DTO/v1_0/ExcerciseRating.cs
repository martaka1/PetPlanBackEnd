using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using AppUser = App.DTO.v1_0.Identity.AppUser;

namespace App.DTO.v1_0;

public class ExcerciseRating
{
    public Guid Id { get; set; }
    [MaxLength(10240)] 
    public string Comment { get; set; } = default!;
    public int Rating { get; set; }
    
    public Guid ExcerciseId { get; set; }
    public Excercise? Excercise { get; set; }

    public DateTime PostedAt { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;
}