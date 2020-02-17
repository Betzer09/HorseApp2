using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HorseApp2.Controllers
{
    public class HorseController : ApiController
    {
        //WORKS WITH ROW IN URL
        /*
        [HttpPost]
        [Route("RowExists/{row}")]
        public bool Exists(int row)
        {
            int exists = 0;
            string storedProcedureName = "usp_RowExists";
            string parameterSequence = "@row";

            SqlParameter[] sqlParameter =
            {
                    new SqlParameter { ParameterName = "@row", SqlDbType  = SqlDbType.BigInt, Value = row },

            };

            using (var context = new HorseDatabaseEntities())
            {
                exists = context.Database.SqlQuery<int>(storedProcedureName + " " + parameterSequence, sqlParameter).FirstOrDefault();
            }

            if(exists > 0)
            {
                return true;
            }

            return false;
        }
        */

            //WORKS WITH ROW IN HEADERS
        [HttpGet]
        [Route("RowExists")]
        public bool Exists()
        {
            int row = 0;
            var request = this.Request;
            var headers = request.Headers;

            if (headers.Contains("row"));
            {
                row = int.Parse(headers.GetValues("row").FirstOrDefault());
            }


            int exists = 0;
            string storedProcedureName = "usp_RowExists";
            string parameterSequence = "@row";

            SqlParameter[] sqlParameter =
            {
                    new SqlParameter { ParameterName = "@row", SqlDbType  = SqlDbType.BigInt, Value = row },

            };

            using (var context = new HorseDatabaseEntities())
            {
                exists = context.Database.SqlQuery<int>(storedProcedureName + " " + parameterSequence, sqlParameter).FirstOrDefault();
            }

            if (exists > 0)
            {
                return true;
            }

            return false;
        }
    }
}
