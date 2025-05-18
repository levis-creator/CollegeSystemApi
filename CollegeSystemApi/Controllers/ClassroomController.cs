using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeSystemApi.Controllers;
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class ClassroomController : GenericController<Classroom>
{
    public ClassroomController(IClassroomService genericService) : base(genericService)
    {
    }
}
