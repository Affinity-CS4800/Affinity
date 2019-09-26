using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Affinity.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(75)]
        public string LastName { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
