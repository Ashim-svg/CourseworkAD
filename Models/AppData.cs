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
        public List<User> Users { get; set; } = new List<User>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
