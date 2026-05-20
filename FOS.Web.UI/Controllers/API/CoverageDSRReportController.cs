using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Web.Http;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using System.Web;
using System.Net.Http.Headers;

namespace FOS.Web.UI.Controllers.API
{
    public class CoverageDSRReportController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [HttpPost]
        public Result<SuccessResponse> Post(CoverageSummery rm)
        {
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
                   

                        List<usp_GetCoverageReportForPDF_Result> result = db.usp_GetCoverageReportForPDF(rm.CustomerID,rm.CoverageID,rm.SOID,FromDate, newDate).ToList();


                        if (result.Count > 0)
                        {


                            string SoName = "";
                            var SO = db.SaleOfficers.Where(u => u.ID == rm.SOID).FirstOrDefault();

                            SoName = SO.Name;
                        string CustName = "";
                        var Cus = db.Retailers.Where(u => u.ID == rm.CustomerID).FirstOrDefault();

                        CustName = Cus.ShopName + " , " + Cus.Name;



                        ReportParameter[] prm = new ReportParameter[5];
                      
                            prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                            

                            prm[1] = new ReportParameter("DateTo", DateTO);
                            prm[2] = new ReportParameter("DateFrom", FromTO);
                            prm[3] = new ReportParameter("SOName", SoName);
                            prm[4] = new ReportParameter("CustomerName", CustName);

                        ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\CoverageDSRReport.rdlc");
                            ReportViewer1.EnableExternalImages = true;
                            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                            ReportViewer1.SetParameters(prm);
                            ReportViewer1.DataSources.Clear();
                            ReportViewer1.DataSources.Add(dt1);


                            ReportViewer1.Refresh();
                            Warning[] warnings;
                            string[] streamids;
                            string mimeType;
                            string encoding;
                            string extension;
                            byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                                    out encoding, out extension, out streamids, out warnings);
                            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                            // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                            using (MemoryStream memoryStream = new MemoryStream(bytes))
                            {

                                memoryStream.Seek(0, SeekOrigin.Begin);

                                memoryStream.Close();



                                SuccessResponse d = new SuccessResponse();
                                string fname = "CoverageDSR" + DateTime.Now.ToString("ddMMyyyyHHss");
                                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                                HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                                d.data = "http://116.58.33.11:81/" + "\\PDF\\" + fname + ".pdf";
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
                            Message = ex.InnerException.Message,
                            ResultType = ResultType.Success,
                            Exception = null,

                        };
                    }

              






                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "There is an issue with your internet. Kindly Try again",
                    ResultType = ResultType.Success,
                    Exception = null,

                };





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



public class SuccessResponse3
{
    public string data { get; set; }
}
public class CoverageSummery
{
    public int SOID { get; set; }
    public int CustomerID { get; set; }
    public int CoverageID { get; set; }
    
    public string DateFrom { get; set; }
    public string DateTo { get; set; }
   

  


}


