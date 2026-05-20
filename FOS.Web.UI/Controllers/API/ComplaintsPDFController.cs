using FOS.DataLayer;
using FOS.Web.UI.Common;
using iTextSharp.text;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class ComplaintsPDFController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [HttpPost]
        public Result<SuccessResponse> Post(OrderSummery1 rm)
        {
            string StickerPic = "";
            string ComplaintPic = "";
            // Retailer retailerObj = new Retailer();
            try
            {
                Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

                DateTime Todate = DateTime.Parse(rm.DateTo);
                DateTime newDate = Todate.AddDays(1);
                DateTime FromDate = DateTime.Parse(rm.DateFrom);

                string DateTO = Todate.ToString("dd-MM-yyyy");
                string FromTO = FromDate.ToString("dd-MM-yyyy");

                try
                {
                    // Create separate lists for orders and follow-ups
                    List<UnifiedComplaintResult> unifiedResults = new List<UnifiedComplaintResult>();

                    using (var context = new FOSDataModel())
                    {

                        //var resu = db.usp_GetComplaintDetailsUnified(FromDate, newDate, rm.SaleOfficerID, rm.ComplaintId).ToList();

                        var command = context.Database.Connection.CreateCommand();
                        command.CommandText = "usp_GetComplaintDetailsUnified";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@DateFrom", FromDate));
                        command.Parameters.Add(new SqlParameter("@DateTo", newDate));
                        command.Parameters.Add(new SqlParameter("@SOID", rm.SaleOfficerID));
                        command.Parameters.Add(new SqlParameter("@ComplaintID", rm.ComplaintId));
                        context.Database.Connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var result = new UnifiedComplaintResult
                                {
                                    ComplaintID = Convert.ToInt32(reader["ComplaintID"]),
                                    SONAME = reader["SONAME"] as string,
                                    CustomerName = reader["CustomerName"] as string,
                                    Phone1 = reader["Phone1"] as string,
                                    DealerName = reader["DealerName"] as string,
                                    DealerPhoneNo = reader["DealerPhoneNo"] as string,
                                    ProductNature = reader["ProductNature"] as string,
                                    ComplaintNumber = reader["ComplaintNumber"] as string,
                                    ComplaintDate = reader["ComplaintDate"] as DateTime? ?? DateTime.Now,
                                    ProductbatchNo = reader["ProductbatchNo"] as string,
                                    ProductDescription = reader["ProductDescription"] as string,
                                    ShakingTime = reader["ShakingTime"] as string,
                                    ColorCode = reader["ColorCode"] as string,
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    SubstrateColor = reader["SubstrateColor"] as string,
                                    TinningRatio = reader["TinningRatio"] as string,
                                    NoOfCoatsApplied = reader["NoOfCoatsApplied"] as string,
                                    TimeDurationWithinCoats = reader["TimeDurationWithinCoats"] as string,
                                    ThinningSolvent = reader["ThinningSolvent"] as string,
                                    AppliedTool = reader["AppliedTool"] as string,
                                    Remarks = reader["Remarks"] as string,
                                    Type = reader["Type"] as string,
                                    AttributeName = reader["AttributeName"] as string,
                                    StickerPicture = reader["StickerPicture"] as string,
                                    ComplaintPicture = reader["ComplaintPicture"] as string,
                                    IsChecked = Convert.ToBoolean(reader["IsChecked"])
                                };
                                unifiedResults.Add(result);
                            }
                        }
                    }

                    var pictureData = unifiedResults.Select(x => new { x.StickerPicture, x.ComplaintPicture }).FirstOrDefault();

                    var CurrentPath = HttpContext.Current.Request.Url.Authority + "/";
                    var path1 = "http://" + CurrentPath;
                    if (!string.IsNullOrEmpty(pictureData.StickerPicture))
                    {
                        StickerPic = path1 + pictureData.StickerPicture;
                    }
                    else
                    {
                        StickerPic = path1 + "/Images/NoImage.jpg";
                    }
                    if (!string.IsNullOrEmpty(pictureData.ComplaintPicture))
                    {
                        ComplaintPic = path1 + pictureData.ComplaintPicture;
                    }
                    else
                    {
                        ComplaintPic = path1 + "/Images/NoImage.jpg";
                    }

                    List<Sp_GetComplaintVerifiedorNorbyQualtiy_Result> result1 = db.Sp_GetComplaintVerifiedorNorbyQualtiy(rm.ComplaintId).ToList();

                    foreach (var item in result1)
                    {
                        if (string.IsNullOrEmpty(item.QualityImageUrl))
                        {
                            item.QualityImageUrl = path1 + "/Images/NoImage.jpg";
                        }
                        else
                        {
                            item.QualityImageUrl = path1 + item.QualityImageUrl;
                        }
                    }

                    if (unifiedResults.Count > 0)
                    {
                        string SoName = "";
                        var SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).FirstOrDefault();
                        SoName = SO?.Name ?? "";

                        ReportParameter[] prm = new ReportParameter[7];
                        prm[0] = new ReportParameter("DistributorName", "Test");
                        prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[2] = new ReportParameter("SOName", SoName);
                        prm[3] = new ReportParameter("DateTo", DateTO);
                        prm[4] = new ReportParameter("DateFrom", FromTO);
                        prm[5] = new ReportParameter("StickerPic", StickerPic);
                        prm[6] = new ReportParameter("ComplaintPic", ComplaintPic);

                        ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\SOComplaints.rdlc");
                        ReportViewer1.EnableExternalImages = true;

                        // Pass both datasets to the report
                        ReportDataSource ds1 = new ReportDataSource("DataSet1", unifiedResults);
                        ReportDataSource ds2 = new ReportDataSource("DataSet2", result1);


                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(ds1);
                        ReportViewer1.DataSources.Add(ds2);

                        ReportViewer1.Refresh();
                        Warning[] warnings;
                        string[] streamids;
                        string mimeType;
                        string encoding;
                        string extension;
                        byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                                out encoding, out extension, out streamids, out warnings);

                        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        using (MemoryStream memoryStream = new MemoryStream(bytes))
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            memoryStream.Close();

                            SuccessResponse d = new SuccessResponse();
                            string fname = "Complaints" + DateTime.Now.ToString("ddMMyyyyHHss");
                            System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                            HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                            //d.data = "http://116.58.33.11:90/" + "\\PDF\\" + fname + ".pdf";
                            //d.data = "http://localhost:4442/" + "\\PDF\\" + fname + ".pdf";
                            var path = HttpContext.Current.Request.Url.Authority + "/";
                            var FinalPath = "http://" + path;
                            d.data = FinalPath + "\\PDF\\" + fname + ".pdf";
                            return new Result<SuccessResponse>
                            {
                                Data = d,
                                Message = "Downloaded",
                                ResultType = ResultType.Success,
                                Exception = null,
                                ValidationErrors = null
                            };
                        }
                    }
                    else
                    {
                        return new Result<SuccessResponse>
                        {
                            Data = null,
                            Message = "No data Present today for this Distributor",
                            ResultType = ResultType.Success,
                            Exception = null,
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = ex.InnerException?.Message ?? ex.Message,
                        ResultType = ResultType.Success,
                        Exception = null,
                    };
                }
            }

            catch (Exception ex)
            {
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Something Went Wrong",
                    ResultType = ResultType.Failure,
                    Exception = null,

                };

            }

        }
    }
}

public class SuccessResponses
{
    public string data { get; set; }
}
public class OrderSummery1
{
    public OrderSummery1()
    {
        CompititorInformation = new List<CompititorInfoModel>();
    }
    public int RetailerID { get; set; }

    public int ComplaintId { get; set; }
    public string ShopName { get; set; }
    public string DateFrom { get; set; }
    public string DateTo { get; set; }
    public int SegmentTypeID { get; set; }

    public int RangeID { get; set; }
    public int SaleOfficerID { get; set; }
    public string Email { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }

    public List<CompititorInfoModel> CompititorInformation { get; set; }



}

public class CompititorInfoModel1
{
    public int SaleOfficerID { get; set; }
    public int RetailerID { get; set; }

    public int SylabusID { get; set; }


}

public class UnifiedComplaintResult
{
    public int ComplaintID { get; set; }
    public string SONAME { get; set; }
    public string CustomerName { get; set; }
    public string Phone1 { get; set; }
    public string DealerName { get; set; }
    public string DealerPhoneNo { get; set; }
    public string ProductNature { get; set; }
    public string ComplaintNumber { get; set; }
    public DateTime ComplaintDate { get; set; }
    public string ProductbatchNo { get; set; }
    public string ProductDescription { get; set; }
    public string ProductCategory { get; set; }
    public string ShakingTime { get; set; }
    public string ColorCode { get; set; }
    public int Quantity { get; set; }
    public string SubstrateColor { get; set; }
    public string TinningRatio { get; set; }
    public string NoOfCoatsApplied { get; set; }
    public string TimeDurationWithinCoats { get; set; }
    public string ThinningSolvent { get; set; }
    public string AppliedTool { get; set; }
    public string Remarks { get; set; }
    public string Type { get; set; }
    public string AttributeName { get; set; }
    public string StickerPicture { get; set; }
    public string ComplaintPicture { get; set; }
    public bool IsChecked { get; set; }
}
