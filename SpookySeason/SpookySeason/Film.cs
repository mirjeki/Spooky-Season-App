using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpookySeason
{
    public class Film
    {
        public string Title { get; set; }
        public bool Watched { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
        public string WatchedDate { get; set; } = DateTime.Now.ToShortDateString();
    }
}
