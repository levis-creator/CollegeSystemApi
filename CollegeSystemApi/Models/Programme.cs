using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models;

public class Programme : BaseEntity
{
    public string ProgrammeName { get; set; } = string.Empty;
    public string ProgrammeCode { get; set; } = string.Empty;
}
