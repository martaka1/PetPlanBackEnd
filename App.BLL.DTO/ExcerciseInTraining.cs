using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class ExcerciseInTraining: IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid HomeTrainingId { get; set; }
    public HomeTraining? HomeTraining { get; set; }
    public Guid ExcerciseId { get; set; }
    public Excercise? Excercise { get; set; }
}