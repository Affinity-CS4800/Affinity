using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Affinity.Models
{
    public class User
    {
        public int ID { get; set; }
        //Firebase ID
        public string UID { get; set; }
        //Unique GraphID used to get vertex/edges
        public string GraphID { get; set; }
    }
}
