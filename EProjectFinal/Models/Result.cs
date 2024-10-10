using System;
using System.Collections.Generic;

namespace EProjectFinal.Models;

public partial class Result
{
    public int ResultId { get; set; }

    public int CandidateId { get; set; }

    public int TestId { get; set; }

    public int TotalMarks { get; set; }

    public bool IsPassed { get; set; }

    public DateTime? TestDate { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
