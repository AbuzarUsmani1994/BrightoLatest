using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class GetCompetitorActivityTypesController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var list = new List<object>();
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    "SELECT ID, Name FROM dbo.Tbl_CompetitorActivityTypes WHERE IsActive = 1 ORDER BY Name", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new
                            {
                                ID   = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"] as string
                            });
                        }
                    }
                }

                return Ok(new { Data = list, Message = "Success", ResultType = "Success" });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetCompetitorActivityTypes Failed");
                return Ok(new { Data = (object)null, Message = "Failed to load activity types", ResultType = "Exception" });
            }
        }
    }
}
