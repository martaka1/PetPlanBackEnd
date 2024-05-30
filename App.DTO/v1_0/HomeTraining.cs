using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1_0;

public class HomeTraining
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public ICollection<ExcerciseInTraining>? Excercises { get; set; }
    public ICollection<HomeTrainingRating>?HomeTrainingRatings { get; set; }
}