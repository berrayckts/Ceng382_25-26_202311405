using System.Collections.Generic;

namespace Week3;

public sealed class StudentGrade
{
    public string Name { get; set; } = string.Empty;
    public int Midterm { get; set; }
    public int Final { get; set; }
    public double Average { get; set; }
    public string LetterGrade { get; set; } = "FF";
}

public static class GradeCalculator
{
    public static double CalculateAverage(int midterm, int finalExam)
    {
        return (midterm * 0.40) + (finalExam * 0.60);
    }

    public static string GetLetterGrade(double average)
    {
        if (average >= 90) return "AA";
        if (average >= 85) return "BA";
        if (average >= 80) return "BB";
        if (average >= 75) return "CB";
        if (average >= 70) return "CC";
        if (average >= 60) return "DC";
        if (average >= 50) return "DD";
        return "FF";
    }

    public static List<StudentGrade> BuildSampleStudents()
    {
        return new List<StudentGrade>
        {
            new() { Name = "Ayse", Midterm = 70, Final = 80 },
            new() { Name = "Mehmet", Midterm = 55, Final = 60 },
            new() { Name = "Zeynep", Midterm = 90, Final = 95 }
        };
    }
}
