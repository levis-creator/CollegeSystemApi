using CollegeSystemApi.Data;
using CollegeSystemApi.Models;
using CollegeSystemApi.Services.Interfaces;

namespace CollegeSystemApi.Services;

public class DepartmentService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    : GenericServices<Department>(context, httpContextAccessor), IDepartmentService;
