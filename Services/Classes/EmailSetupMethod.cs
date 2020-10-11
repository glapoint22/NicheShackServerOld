using DataAccess.Models;
using System;
using System.Threading.Tasks;

namespace Services.Classes
{
    public class EmailSetupMethod
    {
        public Func<NicheShackContext, object, Task> Func { get; set; }
        public object Args { get; set; }
    }
}
