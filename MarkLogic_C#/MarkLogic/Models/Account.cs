using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarkLogic.Models
{
    public class Account
    {
        public string email { get; set; }
        public bool active { get; set; }
        public DateTime created_date { get; set; }
        public IList<string> roles { get; set; }
    }
}
