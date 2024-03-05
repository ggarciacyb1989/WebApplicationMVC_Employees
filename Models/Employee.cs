using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Added.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace WebApplicationMVC_Employees.Models
{
    public class Employee
    {
        #region Employee Basic Atributes
        public int Id { get; set; }
        [DisplayName("Departamento")]
        [Required]
        public string? Department { get; set; }
        public string? Cedula { get; set; }
        [DisplayName("Nombre")]
        [Required]
        public string? FName { get; set; }
        [DisplayName("Apellido")]
        [Required]
        public string? LName { get; set; }
        [DisplayName("Sexo")]
        [Required]
        public string? Sex { get; set; }
        [Required]
        public byte Edad { get; set; }
        public double Salario { get; set; }
        [DisplayName("Fecha")]
        [Required]
        public string? FechaReg { get; set; }
        #endregion

        #region Utilities
        public List<SelectListItem>? DepartmentsList { get; set; }
        #endregion
    }
}
