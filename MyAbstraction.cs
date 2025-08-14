using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPS_Abstraction
{
    interface ISG
    {
        void f1();

    }
    interface IAdmin
    {
        void f1();
        void f2();
        void f3();

    }
    interface IAccountant
    {
        void f1();
        void f2();

        
    }
    internal class MyAbstraction:ISG,IAdmin, IAccountant
    {
      
        public void f1()
        {
            Console.WriteLine("This is f1");
        }

        public void f2()
        {
            Console.WriteLine("This is f2");
        }

        public void f3()
        {
            Console.WriteLine("This is f3");
        }
    }
}
