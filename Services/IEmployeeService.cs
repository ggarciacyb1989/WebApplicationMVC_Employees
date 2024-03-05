using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationMVC_Employees.Models;

//Added
using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem.

namespace MVC_Employees.Services
{
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployee(int empId);
        bool InsertEmployee(string? department, string? cedula, string? firstName, string? lastName, string? sex, string age, string salary, string? regDate);
        bool UpdateEmployee(string id, string? department, string? cedula, string? firstName, string? lastName, string? sex, string age, string salary, string? regDate);
        bool DeleteEmployee(int empId);
        List<SelectListItem> LoadDepartments();
    }
}