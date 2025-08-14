
using OOPS_Abstraction;

string role = "accountant";

switch(role)
{
    case "admin":
        IAdmin objAdmin = new MyAbstraction();
        objAdmin.f1();
        objAdmin.f2();
        objAdmin.f3();
        break;
    case "accountant":
        IAccountant objAcc = new MyAbstraction();
        objAcc.f1();
        objAcc.f2();
        //objAcc.f3();-----------Accountant is not authorised to use f3 
        break;
    case "Sg":
        ISG objsg = new MyAbstraction();
        objsg.f1();
        // objsg.f2();-------------Security Guard is not authorised to use f2
        // objsg.f3();-------------Security Guard is not authorised to use f3
        break;

    default:
        Console.WriteLine("Invalid Role");
        break;

}
Console.ReadKey();