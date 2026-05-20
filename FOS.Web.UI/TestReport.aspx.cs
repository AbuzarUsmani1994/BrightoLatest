using FOS.DataLayer;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FOS.Web.UI
{
    public partial class TestReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PrintDailyItemReport();
            }

        }
        public void PrintDailyItemReport()
        {
            string from = "1 Mar 2019";
            DateTime dtFrom = DateTime.Parse(from.ToString());
            string To = "26 Mar 2019";
            DateTime dtTo = DateTime.Parse(To.ToString());

            FOSDataModel db = new FOSDataModel();

            //ADD New Retailer 
            List<Sp_OrderSummeryReportInExcel_Result> result = db.Sp_OrderSummeryReportInExcel(dtFrom, dtTo, 101).ToList();

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
        
            ReportViewer1.LocalReport.ReportPath = HttpContext.Current.Server.MapPath("\\Reports\\TestReport.rdlc");
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
            string dealername = "";
            List<Dealer> dealer = db.Dealers.Where(u => u.ID == 101).ToList();
            foreach (var deal in dealer)
            {
                dealername = deal.Name;
            }
            ReportParameter[] prm = new ReportParameter[2];
            prm[0] = new ReportParameter("DistributorName", dealername);
            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
          
           


            ReportViewer1.LocalReport.SetParameters(prm);



            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(dt1);
        //    Warning[] warnings;
        //    string[] streamIds;
        //    string mimeType = string.Empty;
        //    string encoding = string.Empty;
        //string extension = string.Empty;
         
        //    ReportViewer1.LocalReport.Refresh();
        //    byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        //    // byte[] bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        //    // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.          
        //    // System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Buffer = true;
        //    Response.Clear();
        //    Response.ContentType = mimeType;
        //    Response.AddHeader("content-disposition", "attachment; filename= filename" + "." + extension);
        //    Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
        //    Response.Flush(); // send it to the client to download  
        //    Response.End();

        }
    }
}