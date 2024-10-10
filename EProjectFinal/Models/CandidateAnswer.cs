using System;
using System.Collections.Generic;

namespace EProjectFinal.Models;

public partial class CandidateAnswer
{
    public int AnswerId { get; set; }

    public int CandidateId { get; set; }

    public int TestId { get; set; }

    public int QuestionId { get; set; }

    public string SelectedAnswer { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public int MarksAwarded { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
