using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Interfaces
{
    public interface IItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
