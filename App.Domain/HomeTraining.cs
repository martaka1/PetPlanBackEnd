

using Base.Domain;

namespace Domain.App;

public class HomeTraining: BaseEntityId
{
    public string? Name { get; set; }
    public ICollection<HomeTrainingRating>?HomeTrainingRatings { get; set; }
    public ICollection<ExcerciseInTraining>? Excercises { get; set; }
}