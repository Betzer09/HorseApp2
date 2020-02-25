using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    public class SireResponse
    {
        public long sireServerId { get; set; }
        public string name { get; set; }
        public string horseType { get; set; }
        public string createdOn { get; set; }
        public string updatedOn { get; set; }
    }
}