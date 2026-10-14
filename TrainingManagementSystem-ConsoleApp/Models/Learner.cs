namespace TrainingManagementSystem.Models;

public class Learner
{
    public string LearnerId { get; set; }

    public string Name { get; set; }

    public string ContactNumber { get; set; }

    public string Email { get; set; }


    public Learner(string learnerId, string name, string number, string email)
    {
        LearnerId = learnerId;
        Name = name;
        ContactNumber = number;
        Email = email;
    }
}