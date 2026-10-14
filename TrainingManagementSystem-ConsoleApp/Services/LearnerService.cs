using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Services;

public sealed class LearnerService
{
    private readonly List<Learner> learners;

    public LearnerService(List<Learner> learners)
    {
        this.learners = learners;
    }

    public IReadOnlyList<Learner> Learners => learners;

    public Learner? FindById(string learnerId)
    {
        for (int i = 0; i < learners.Count; i++)
        {
            if (string.Equals(learners[i].LearnerId, learnerId, StringComparison.OrdinalIgnoreCase))
            {
                return learners[i];
            }
        }

        return null;
    }

    public bool Register(Learner learner)
    {
        if (FindById(learner.LearnerId) is not null)
        {
            return false;
        }

        learners.Add(learner);
        return true;
    }
}
