using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OOPS_Inheritance
{
    internal class Organization
    {
        int OrgID = 10;

        public void f1()
        {
            Console.WriteLine("OrgID is {0}",OrgID);
        }
    }

    class Department
    {
        int DeptID = 2;
        public void f1()
        {
            Console.WriteLine("Department ID is {0}", DeptID);
        }
    }

    class Employee
    {

        int EmpID = 3;
        public void f1()
        {
            Console.WriteLine("Employee ID is {0}", EmpID);
        }

    }
}
