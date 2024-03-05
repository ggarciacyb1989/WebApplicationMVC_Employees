using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplicationMVC_Employees.Models;
using WebApplicationMVC_Employees.Services;

//Added
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Employees.Services;

/* Aqui en lugar de crear una DAL class, incorporamos su funcionalidad
   a la EmployeeService class con su IEmployeeService interface. */

namespace WebApplicationMVC_Employees.Controllers
{
    public class EmployeeController : Controller
    {
        #region Fields
        private readonly IEmployeeService _employeeService;
        #endregion

        #region Ctor
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        #endregion

        #region Methods
      public IActionResult Index()
        {
            List<Employee> empsList = new List<Employee>();
            empsList = _employeeService.GetAllEmployees();
            return View(empsList);
        } 

        public IActionResult AddOrEdit(int id = 0)
        {
            //Creating a List of registered departments to populate a ddl.
            List<SelectListItem> registeredDepartments = new List<SelectListItem>();
            registeredDepartments = _employeeService.LoadDepartments();
            Employee model;
            if (id == 0)
            {
                model = new Employee();
                model.DepartmentsList = registeredDepartments;
                return View(model);
            }
            else
            {
                model = _employeeService.GetEmployee(id);
                model.DepartmentsList = registeredDepartments;
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult AddOrEdit([Bind("Id,Department,Cedula,FName,LName,Sex,Edad,Salario,FechaReg")] Employee employee)
        {
            /* At this point none of the [Required] Properties in the Model can be set to null.
               Otherwise, ModelState.IsValid == false. */
            if (ModelState.IsValid)
            {
                string? department = employee.Department;
                string? cedula = employee.Cedula;
                string? fname = employee.FName;
                string? lname = employee.LName;
                string? sex = employee.Sex;
                string age = employee.Edad.ToString();
                string salary = employee.Salario.ToString();
                string? regDate = employee.FechaReg;
                
                if (employee.Id == 0)
                {
                    var wasAdded = _employeeService.InsertEmployee(department, cedula, fname, lname, sex, age, salary, regDate);
                    if (wasAdded == false)
                        return View("OperationFailed", "addCode");
                }
                else
                {
                    string id = employee.Id.ToString();
                    var wasUpdated = _employeeService.UpdateEmployee(id, department, cedula, fname, lname, sex, age, salary, regDate);
                    if (wasUpdated == false)
                        return View("OperationFailed", "updateCode");
                }

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        public IActionResult Delete(int id)
        {
            var wasDeleted = _employeeService.DeleteEmployee(id);
            if(wasDeleted == false)
                return View("OperationFailed", "deleteCode");

            return RedirectToAction(nameof(Index));
        } 
    #endregion
    }

    

    
}