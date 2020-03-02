using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    //Request object for testing database connection
    //row refers to the row in the database table 'tblName' that you want to see exists
    //For example:
    //send in row = 2, RowExists will check if row with id 2 is in tblName
    public class RowExistsRequestDTO
    {
        public int row { get; set; }
    }
}