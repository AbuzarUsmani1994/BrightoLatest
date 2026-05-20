using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class ComplaintProgressController : ApiController
    {
        public IHttpActionResult Get(int ComplaintID)
        {
            try
            {
                List<object> updatesList = new List<object>();

                string ComplaintNumber = "";
                DateTime LaunchedTime = DateTime.MinValue;
                string SaleOfficerName = "";
                string CustomerName = "";
                string DealerName = "";

                using (var context = new FOSDataModel())
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "usp_GetComplaintFullDetails";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ComplaintID", ComplaintID));

                    context.Database.Connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (updatesList.Count == 0)
                            {
                                ComplaintNumber = reader["ComplaintNumber"]?.ToString();
                                LaunchedTime = Convert.ToDateTime(reader["LaunchedTime"]);
                                SaleOfficerName = reader["SaleOfficerName"]?.ToString();
                                CustomerName = reader["CustomerName"]?.ToString();
                                DealerName = reader["DealerName"]?.ToString();
                            }

                            updatesList.Add(new
                            {
                                LastUpdatedAt = Convert.ToDateTime(reader["LastUpdatedAt"]),
                                ProgressStatusName = reader["ProgressStatusName"]?.ToString(),
                                Picture = reader["Picture"]?.ToString(),
                                Audio = reader["Audio"]?.ToString(),
                                Video = reader["Video"]?.ToString(),
                                ComplaintStatusName = reader["ComplaintStatusName"]?.ToString()
                            });
                        }
                    }

                    context.Database.Connection.Close();
                }

                if (updatesList.Count > 0)
                {
                    return Ok(new
                    {
                        ComplaintID = ComplaintID,
                        ComplaintNumber = ComplaintNumber,
                        LaunchedTime = LaunchedTime,
                        SaleOfficerName = SaleOfficerName,
                        CustomerName = CustomerName,
                        DealerName = DealerName,
                        Updates = updatesList
                    });
                }
                else
                {
                    return Ok(new
                    {
                        ComplaintID = ComplaintID,
                        Updates = new object[0],
                        Message = "No complaint updates found for the given ComplaintID"
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ComplaintProgressController GET API Failed");
                return InternalServerError(ex);
            }
        }
    }
}
