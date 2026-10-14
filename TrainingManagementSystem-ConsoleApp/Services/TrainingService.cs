using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Services;

public sealed class TrainingService
{
    private readonly List<Course> courses;

    public TrainingService(List<Course> courses)
    {
        this.courses = courses;
    }

    public IReadOnlyList<Course> Courses => courses;

    public Course? FindByCode(string courseCode)
    {
        for (int i = 0; i < courses.Count; i++)
        {
            if (string.Equals(courses[i].CourseCode, courseCode, StringComparison.OrdinalIgnoreCase))
            {
                return courses[i];
            }
        }

        return null;
    }

    public List<Course> Search(string searchText)
    {
        List<Course> results = new List<Course>();

        for (int i = 0; i < courses.Count; i++)
        {
            Course course = courses[i];
            if (course.CourseCode.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                course.CourseName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(course);
            }
        }

        return results;
    }

    public void SortByFee(bool ascending)
    {
        courses.Sort((first, second) => first.Fee.CompareTo(second.Fee));
        if (!ascending)
        {
            courses.Reverse();
        }
    }
}
