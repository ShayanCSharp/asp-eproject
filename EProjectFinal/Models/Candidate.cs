using System;
using System.Collections.Generic;

namespace EProjectFinal.Models;

public partial class Candidate
{
    public int CandidateId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string EducationDetails { get; set; } = null!;

    public string WorkExperience { get; set; } = null!;

    public bool? IsTestCompleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CandidateAnswer> CandidateAnswers { get; set; } = new List<CandidateAnswer>();

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
