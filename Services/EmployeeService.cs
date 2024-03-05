using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationMVC_Employees.Models;

//Added
using Microsoft.AspNetCore.Mvc.Rendering;

//Next namespaces are related with the use of Raw ADO.NET
using System.Data;
using System.Data.SqlClient;
using MVC_Employees.Services;

namespace WebApplicationMVC_Employees.Services
{
public class EmployeeService : IEmployeeService
    {
        /* I created this EmployeesDb Database using SQl Server Object Explorer; which comes integrated
           with VStudio 2019. So, this MVC project was built under a DatabaseFirst approach paradigm, but 
           without using Entity Framework. So, no Scaffold-DbContext command from the Package Manager(PM)
           was executed because no DbContext class was needed. Instead, Raw ADO.NET was used to interact
           with the Database. 
           Note1: 
           To work with Raw ADO.NET, we installed the System.Data.SqlClient Nuget Package. 
           Note2: 
           Hay que agregar un '\' despues del (localdb) al valor de la Connection string property de la EmployeesDb. 
           Su valor original es:
           Data Source=(localdb)\ProjectsV13;Initial Catalog=EmployeesDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
        */

        private const string strConnection = "Data Source=(localdb)\\ProjectsV13;Initial Catalog=EmployeesDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        /* We need to declare the CreateInputParameter method as static in order to call it without the necessity
           of creating an instance of its class(EmployeeService). */
        private static SqlParameter CreateInputParameter(string parname, SqlDbType partype, object parvalue)
        {
            SqlParameter Parameter = new SqlParameter(parname, partype);
            if (parvalue == null)
            {
                Parameter.IsNullable = true;
                Parameter.Value = DBNull.Value;
                return Parameter;
            }
            Parameter.Value = parvalue;
            return Parameter;
        } //del CreateInputParameter.

        public List<Employee> GetAllEmployees()
        {
            List<Employee> emps = new List<Employee>();
            SqlConnection cnn = new SqlConnection(strConnection);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "EmployeesReport_SP";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            try
            {
                cnn.Open();
                da.Fill(ds);
                cnn.Close();
            }
            catch (Exception)
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
                cnn.Dispose();
                cmd.Dispose();
                throw;
            } // del catch

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                emps.Add(new Employee
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Department = Convert.ToString(dr["Departamento"]),
                    Cedula = Convert.ToString(dr["Cedula"]),
                    FName = Convert.ToString(dr["Nombre"]),
                    LName = Convert.ToString(dr["Apellido"]),
                    Sex = Convert.ToString(dr["Sexo"]),
                    Edad = Convert.ToByte(dr["Edad"]),
                    Salario = Convert.ToDouble(dr["Salario"]),
                    FechaReg = Convert.ToString(dr["FechaReg"])
                });
            }

            return emps;
        } //del GetAllEmployees.

        public Employee GetEmployee(int empId)
        {
            Employee emp = new Employee();
            SqlConnection cnn = new SqlConnection(strConnection);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetEmployee_SP";

            cmd.Parameters.Add(CreateInputParameter("@EmployeeId", SqlDbType.Int, empId));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            try
            {
                cnn.Open();
                da.Fill(ds);
                cnn.Close();
            }
            catch (Exception e)
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
                cnn.Dispose();
                cmd.Dispose();
                throw e;
            } // del catch

            emp.Id = Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]);
            emp.Cedula = Convert.ToString(ds.Tables[0].Rows[0]["Cedula"]);
            emp.FName = Convert.ToString(ds.Tables[0].Rows[0]["Nombre"]);
            emp.LName = Convert.ToString(ds.Tables[0].Rows[0]["Apellido"]);
            emp.Sex = Convert.ToString(ds.Tables[0].Rows[0]["Sexo"]);
            emp.Edad = Convert.ToByte(ds.Tables[0].Rows[0]["Edad"]);
            emp.Department = Convert.ToString(ds.Tables[0].Rows[0]["Departamento"]);
            emp.Salario = Convert.ToDouble(ds.Tables[0].Rows[0]["Salario"]);
            emp.FechaReg = Convert.ToString(ds.Tables[0].Rows[0]["FechaReg"]);

            return emp;
        } // del GetEmployee. 

        public bool InsertEmployee(string?  department,
                                           string? cedula,
                                           string? firstName,
                                           string? lastName,
                                           string? sex,
                                           string age,
                                           string salary,
                                           string? regDate)
        {
            int PK_V_Err;
            SqlConnection cnn = new SqlConnection(strConnection);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertEmployee_SP";

            //Adding parameters
            cmd.Parameters.Add(CreateInputParameter("@Department", SqlDbType.VarChar, department));
            cmd.Parameters.Add(CreateInputParameter("@Cedula", SqlDbType.VarChar, cedula));
            cmd.Parameters.Add(CreateInputParameter("@FirstName", SqlDbType.VarChar, firstName));
            cmd.Parameters.Add(CreateInputParameter("@LastName", SqlDbType.VarChar, lastName));
            cmd.Parameters.Add(CreateInputParameter("@Sex", SqlDbType.VarChar, sex));
            cmd.Parameters.Add(CreateInputParameter("@Age", SqlDbType.TinyInt, Convert.ToByte(age)));
            cmd.Parameters.Add(CreateInputParameter("@Salary", SqlDbType.SmallMoney, Convert.ToDouble(salary)));
            cmd.Parameters.Add(CreateInputParameter("@RegDate", SqlDbType.VarChar, regDate));

            cmd.Parameters.Add("@PK_Viol_Err", SqlDbType.Int);
            cmd.Parameters["@PK_Viol_Err"].Direction = ParameterDirection.Output;

            try
            {
                cnn.Open();
                cmd.ExecuteNonQuery();
                PK_V_Err = Convert.ToInt32(cmd.Parameters["@PK_Viol_Err"].Value.ToString());
            }
            catch (Exception e)
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
                cnn.Dispose();
                cmd.Dispose();
                throw e;
            } // del catch

            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
            cnn.Dispose();
            cmd.Dispose();

            if (PK_V_Err == 0)
                return true;
            else
                return false;
        } // del InsertEmployee.

        public bool UpdateEmployee(string id,
                                   string? department,
                                   string? cedula,
                                   string? firstName,
                                   string? lastName,
                                   string? sex,
                                   string age,
                                   string salary,
                                   string? regDate)
        {
            int PK_V_Err;
            SqlConnection cnn = new SqlConnection(strConnection);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateEmployee_SP";

            //Adding parameters
            cmd.Parameters.Add(CreateInputParameter("@Id", SqlDbType.Int, Convert.ToInt32(id)));
            cmd.Parameters.Add(CreateInputParameter("@Department", SqlDbType.VarChar, department));
            cmd.Parameters.Add(CreateInputParameter("@Cedula", SqlDbType.VarChar, cedula));
            cmd.Parameters.Add(CreateInputParameter("@FirstName", SqlDbType.VarChar, firstName));
            cmd.Parameters.Add(CreateInputParameter("@LastName", SqlDbType.VarChar, lastName));
            cmd.Parameters.Add(CreateInputParameter("@Sex", SqlDbType.VarChar, sex));
            cmd.Parameters.Add(CreateInputParameter("@Age", SqlDbType.TinyInt, Convert.ToByte(age)));
            cmd.Parameters.Add(CreateInputParameter("@Salary", SqlDbType.SmallMoney, Convert.ToDouble(salary)));
            cmd.Parameters.Add(CreateInputParameter("@RegDate", SqlDbType.VarChar, regDate));

            cmd.Parameters.Add("@PK_Viol_Err", SqlDbType.Int);
            cmd.Parameters["@PK_Viol_Err"].Direction = ParameterDirection.Output;

            try
            {
                cnn.Open();
                cmd.ExecuteNonQuery();
                PK_V_Err = Convert.ToInt32(cmd.Parameters["@PK_Viol_Err"].Value.ToString());
            }
            catch (Exception e)
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
                cnn.Dispose();
                cmd.Dispose();
                throw e;
            } // del catch

            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
            cnn.Dispose();
            cmd.Dispose();

            if (PK_V_Err == 0)
                return true;
            else
                return false;
        } // del UpdateEmployee.


        public bool DeleteEmployee(int empId)
        {
            int PK_V_Err;
            SqlConnection cnn = new SqlConnection(strConnection);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeleteEmployee_SP";
            cmd.Parameters.Add(CreateInputParameter("@Id", SqlDbType.Int, empId));

            cmd.Parameters.Add("@PK_Viol_Err", SqlDbType.Int);
            cmd.Parameters["@PK_Viol_Err"].Direction = ParameterDirection.Output;

            try
            {
                cnn.Open();
                cmd.ExecuteNonQuery();
                PK_V_Err = Convert.ToInt32(cmd.Parameters["@PK_Viol_Err"].Value.ToString());
            }
            catch (Exception e)
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
                cnn.Dispose();
                cmd.Dispose();
                throw e;
            } // del catch

            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
            cnn.Dispose();
            cmd.Dispose();

            if (PK_V_Err == 0)
                return true;
            else
                return false;
        } // del DeleteEmployee.
        

        public List<SelectListItem> LoadDepartments()
        {
            List<SelectListItem> dpts = new List<SelectListItem>();
            dpts.Add(new SelectListItem { Text = "Select one Department", Value = "None" });
            dpts.Add(new SelectListItem { Text = "Contabilidad", Value = "Contabilidad" });
            dpts.Add(new SelectListItem { Text = "Informatica", Value = "Informatica" });
            dpts.Add(new SelectListItem { Text = "Recursos Humanos", Value = "Recursos Humanos" });

            return dpts;
        } //del LoadDepartments.
    }
}