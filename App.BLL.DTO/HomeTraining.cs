using Base.Contracts.Domain;
using Domain.App;

namespace App.BLL.DTO;

public class HomeTraining: IDomainEntityId
{
    public Guid Id { get; set; }
    public string? Name { set; get; }
    public ICollection<ExcerciseInTraining?>? Excercises { get; set; }
    public ICollection<HomeTrainingRating>?HomeTrainingRatings { get; set; }

}