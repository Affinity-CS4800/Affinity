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
        public string UId { get; set; }
        public ICollection<GraphID> GraphIds { get; set; }
    }
    public class GraphID
    {
        public int ID { get; set; }
        public string graphID { get; set; }
    }


}
