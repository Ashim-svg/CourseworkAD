using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseworkAD.Model;
using CourseworkAD.Models;

namespace CourseworkAD.Models
{
    public class AppData
    {
        public List<User> Users { get; set; } = new();
        
        public List<Transaction> Transactions { get; set; } = new();

    }
}
