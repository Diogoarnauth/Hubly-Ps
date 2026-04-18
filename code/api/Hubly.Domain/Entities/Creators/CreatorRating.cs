namespace Hubly.api.Domain.Entities;

public class CreatorRating
{
    public int Id { get; set; }
    public int EvaluatorId { get; set; }
    public int TargetCreatorId { get; set; }
    public int RatingValue { get; set; }
    public DateTime RatedAt { get; set; }

    public User Evaluator { get; set; }
    public Creator TargetCreator { get; set; }
}