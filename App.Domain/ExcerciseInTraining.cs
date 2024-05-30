using Base.Domain;

namespace Domain.App;

public class ExcerciseInTraining:BaseEntityId
{
    public Guid HomeTrainingId { get; set; }
    public HomeTraining? HomeTraining { get; set; }
    
    public Guid ExcerciseId { get; set; }
    public Excercise? Excercise { get; set; }
    
}