using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    public class SireResponse
    {
        public long SireServerId { get; set; }
        public string Name { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
    }
}