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
    public class OrderForExcelForDistributorController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [HttpPost]
        public Result<SuccessResponse> Post(OrderSummery rm)
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

                    List<Sp_OrderSummeryReportInExcelRangeWiseForDistributor1_1_Result> result = db.Sp_OrderSummeryReportInExcelRangeWiseForDistributor1_1(FromDate, newDate, 6, rm.SaleOfficerID).ToList();


                    string SoName = "";
                    List<SaleOfficer> SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).ToList();
                    foreach (var SOS in SO)
                    {
                        SoName = SOS.Name;
                    }
                    string RangeName = "";
                    List<MainCategory> range = db.MainCategories.Where(u => u.MainCategID == 6).ToList();
                    foreach (var SOS in range)
                    {
                        RangeName = SOS.MainCategDesc;
                    }


                    ReportParameter[] prm = new ReportParameter[4];

                    prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                    prm[1] = new ReportParameter("SOName", SoName);
                    prm[2] = new ReportParameter("DateTo", DateTO);
                    prm[3] = new ReportParameter("DateFrom", FromTO);
                    ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\Report1.rdlc");
                    
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
                            string fname = "DIS" + DateTime.Now.ToString("ddMMyyyyHHss");
                            System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                            HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                            d.data = "http://panda.workforcepk.com/" + "\\PDF\\" + fname + ".pdf";
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

                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");

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




