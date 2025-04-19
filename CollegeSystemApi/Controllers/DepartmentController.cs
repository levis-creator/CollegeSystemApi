using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeSystemApi.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class DepartmentController : GenericController<Department>
{
    public DepartmentController(IDepartmentService genericService) : base(genericService)
    {
    }
}
