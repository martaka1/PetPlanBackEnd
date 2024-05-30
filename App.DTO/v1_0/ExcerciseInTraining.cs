namespace App.DTO.v1_0;

public class ExcerciseInTraining
{
    public Guid Id { get; set; }
    public Guid HomeTrainingId { get; set; }
    public HomeTraining? HomeTraining { get; set; }
    public Guid ExcerciseId { get; set; }
    public Excercise? Excercise { get; set; }
}