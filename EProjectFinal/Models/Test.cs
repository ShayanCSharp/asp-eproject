using System;
using System.Collections.Generic;

namespace EProjectFinal.Models;

public partial class Test
{
    public int TestId { get; set; }

    public string TestName { get; set; } = null!;

    public int TotalQuestions { get; set; }

    public int MaxMarks { get; set; }

    public int TimeLimit { get; set; }

    public virtual ICollection<CandidateAnswer> CandidateAnswers { get; set; } = new List<CandidateAnswer>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
