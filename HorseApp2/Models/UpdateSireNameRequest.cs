using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    public class UpdateSireNameRequest
    {
        public string oldName { get; set; }
        public string newName { get; set; }
    }
}