using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using portal.mps.Data;
using portal.mps.Data.Repository;
using portal.mps.Models;
using portal.mps.Models.ViewModels;

namespace portal.mps.Services
{
    public class Utils : IUtils
    {
        private IConfiguration _config;
        private IHostingEnvironment _env;
        private UserManager<mpsUser> _manager;
        private mpsContext _ctx;

        public Utils(IConfiguration config,IHostingEnvironment env,UserManager<mpsUser> manager,mpsContext ctx)
        {
            _config = config;
            _env = env;
            _manager = manager;
            _ctx = ctx;
        }

        public string getCurrentAcademicYear()
        {
             string academicYear = _config["academicYear:academicYearValue"];
             return academicYear;
        }
        public DateTime getCurrentStartYearDate()
        {
             string academicYear = _config["academicYear:startDate"];
             DateTime stdt = new DateTime();
             DateTime.TryParse(academicYear,out stdt);
             return stdt;
        }
        public DateTime getCurrentEndYearDate()
        {
             string academicYear = _config["academicYear:endDate"];
             DateTime enddt = new DateTime();
             DateTime.TryParse(academicYear,out enddt);
             return enddt;
        }

        public string getSalaryDeductions()
        {
            string pf = _config["salaryDeductions:pf"];
            string profTax = _config["salaryDeductions:profTax"];
            return String.Concat(pf,"~",profTax);//,"-Pf=",pf.Replace("P","%"),", prof.Tax=",profTax.Replace("R","Rs. "));
        }

        public byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
        }

        public byte[] printBill(string billType, printModel bill ,out string strAttachment)
        {
          //MemoryStream workStream = new MemoryStream();  
          StringBuilder status = new StringBuilder("");  
          DateTime dTime = DateTime.Now;  
          //file name to be created   
          string filename = string.Format("BillPdf" + dTime.ToString("yyyyMMdd-HHmmss") + ".pdf");  
          Document doc = new Document(PageSize.A4);  
          doc.SetMargins(0f,0f, 9f,0f);  
          //Create PDF Table  
          using(MemoryStream workStream = new MemoryStream()){
            var writer = PdfWriter.GetInstance(doc, workStream  );
            //delete bills
            DirectoryInfo d = new DirectoryInfo(String.Concat(_env.WebRootPath,"\\Bills\\"));
            if(!d.Exists){
              d.Create();
            }
            FileInfo[] Files = d.GetFiles("*.pdf"); //Getting Excel files
            foreach(FileInfo f in Files )
            {
                if(f.Exists){f.Delete();}
            }
            //file will created in this path  
            strAttachment = String.Concat(_env.WebRootPath,"\\Bills\\",filename);  
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;  
            doc.Open();  
            //Add Content to PDF  
            doc.Add(makeBill(bill));  

            // var htmlWorker = new iTextSharp.text.html.simpleparser.HtmlWorker(doc);
            // var sr = new StringReader(makeBillHtml(bill));
            // //Parse the HTML
            // htmlWorker.Parse(sr);
                

            // Closing the document  
            doc.Close();  

            byte[] byteInfo = workStream.ToArray(); 
            return byteInfo;
          }
        }
        
        /*
        Function to return a table with exact measurements
         */
        private PdfPTable makeBill(printModel bill)
        {
          var borderColor = new BaseColor(9,42,114);
          var borderwidth=0.5f;
          var blankHeight = 3f;
          PdfPTable table = new PdfPTable(6);
          table.TotalWidth = 455f;
          table.LockedWidth = true;

          #region top row
            string logoPath = _env.WebRootPath+"/images/top.png";
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleToFit(448f,90f);
            logo.Alignment = Element.ALIGN_CENTER;
            Paragraph p = new Paragraph();
            p.Add(new Chunk(logo, 0, 0));

            PdfPCell Cell1 = new PdfPCell(new Phrase(0f,""));
            Cell1.Colspan = 6;
            Cell1.HorizontalAlignment = 0;
            Cell1.VerticalAlignment = 1;
            //Cell1.Rowspan=5;
            Cell1.BorderColor = borderColor;
            Cell1.BorderWidth = borderwidth;
            Cell1.BorderWidthBottom = 0.0f;
            Cell1.AddElement(p);
            table.AddCell(Cell1);
          #endregion
          
          #region 2nd row (Bill No. & Date)

          PdfPCell cb1 = new PdfPCell(new Phrase(5f, "No.", new Font(Font.TIMES_ROMAN, 12f,0, borderColor)));
          cb1.Colspan = 1;
          cb1.HorizontalAlignment = 2;
          cb1.BorderColor = borderColor;
          cb1.BorderWidth = borderwidth;
          cb1.BorderWidthBottom = 0.0f;
          cb1.BorderWidthTop = 0.0f;
          cb1.BorderWidthRight = 0.0f;
          table.AddCell(cb1);

          PdfPCell cb2 = new PdfPCell(new Phrase(1f, bill.BillNumber.ToString(), new Font(Font.TIMES_ROMAN, 12f,0, new BaseColor(255,0,0))));
          cb2.Colspan = 1;
          cb2.HorizontalAlignment = 0;
          cb2.BorderColor = borderColor;
          cb2.BorderWidth = borderwidth;
          cb2.BorderWidthBottom = 0.0f;
          cb2.BorderWidthTop = 0.0f;
          cb2.BorderWidthLeft = 0.0f;
          cb2.BorderWidthRight = 0.0f;
          table.AddCell(cb2);

          PdfPCell cb3 = new PdfPCell(new Phrase(1f, "FEE", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          cb3.Colspan = 1;
          cb3.HorizontalAlignment = 2;
          cb3.BorderColor = borderColor;
          cb3.BorderWidth = borderwidth;
          cb3.BorderWidthBottom = 0.0f;
          cb3.BorderWidthTop = 0.0f;
          cb3.BorderWidthLeft = 0.0f;
          cb3.BorderWidthRight = 0.0f;
          table.AddCell(cb3);

          PdfPCell cb4 = new PdfPCell(new Phrase(1f, " RECEIPT", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          cb4.Colspan = 1;
          cb4.HorizontalAlignment = 0;
          cb4.BorderColor = borderColor;
          cb4.BorderWidth = borderwidth;
          cb4.BorderWidthBottom = 0.0f;
          cb4.BorderWidthTop = 0.0f;
          cb4.BorderWidthLeft = 0.0f;
          cb4.BorderWidthRight = 0.0f;
          table.AddCell(cb4);

          PdfPCell cb5 = new PdfPCell(new Phrase(1f, " Date: ", new Font(Font.TIMES_ROMAN, 10f,0, borderColor)));
          cb5.Colspan = 1;
          cb5.HorizontalAlignment = 2;
          cb5.BorderColor = borderColor;
          cb5.BorderWidth = borderwidth;
          cb5.BorderWidthBottom = 0.0f;
          cb5.BorderWidthTop = 0.0f;
          cb5.BorderWidthLeft = 0.0f;
          cb5.BorderWidthRight = 0.0f;
          table.AddCell(cb5);

          PdfPCell cb6 = new PdfPCell(new Phrase(1f, bill.paidDate.ToString(), new Font(Font.TIMES_ROMAN, 10f,0, borderColor)));
          cb6.Colspan = 1;
          cb6.HorizontalAlignment = 0;
          cb6.BorderColor = borderColor;
          cb6.BorderWidth = borderwidth;
          cb6.BorderWidthBottom = 0.0f;
          cb6.BorderWidthTop = 0.0f;
          cb6.BorderWidthLeft = 0.0f;
          table.AddCell(cb6);

          #endregion

          PdfPCell Cellblank1 = new PdfPCell(new Phrase(17f, " ", new Font(Font.TIMES_ROMAN, blankHeight,0, borderColor)));
          Cellblank1.Colspan = 6;
          Cellblank1.HorizontalAlignment = 1;
          Cellblank1.BorderColor = borderColor;
          Cellblank1.BorderWidth = 0.0f;
          Cellblank1.BorderWidthLeft= borderwidth;
          Cellblank1.BorderWidthRight= borderwidth;
          table.AddCell(Cellblank1);

          #region 3rd row (name)
          PdfPCell Cell3 = new PdfPCell(new Phrase(0f, "     Recieved with thanks to the credit of..............", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell3.Colspan = 4;
          Cell3.HorizontalAlignment = 0;
          Cell3.BorderColor = borderColor;
          Cell3.BorderWidth = 0.0f;
          Cell3.BorderWidthLeft= borderwidth;
          table.AddCell(Cell3);

          PdfPCell Cell31 = new PdfPCell(new Phrase(1f, bill.name, new Font(Font.TIMES_ROMAN, 13f,5, borderColor)));
          Cell31.Colspan = 2;
          Cell31.HorizontalAlignment = 0;
          Cell31.BorderColor = borderColor;
          Cell31.BorderWidth = 0.0f;
          Cell31.BorderWidthRight = borderwidth;
          table.AddCell(Cell31);
          #endregion
            
          // PdfPCell Cellblank2 = new PdfPCell(new Phrase(17f, "", new Font(Font.TIMES_ROMAN, blankHeight,0, borderColor)));
          // Cellblank2.Colspan = 6;
          // Cellblank2.HorizontalAlignment = 1;
          // Cellblank2.BorderColor = borderColor;
          // Cellblank2.BorderWidth = 0.0f;
          // Cellblank2.BorderWidthLeft= borderwidth;
          // Cellblank2.BorderWidthRight= borderwidth;
          table.AddCell(Cellblank1);

          #region 4th row (roll, class pay by)

          PdfPCell Cell4 = new PdfPCell(new Phrase(0f,string.Concat("     Roll No...........",bill.rollNo,".....","           Class.........",bill.grade,"......","            By..........",bill.paymentType,"....."), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell4.Colspan = 6;
          Cell4.HorizontalAlignment = 0;
          Cell4.BorderColor = borderColor;
          Cell4.BorderWidth = 0.0f;
          Cell4.BorderWidthLeft= borderwidth;
          Cell4.BorderWidthRight= borderwidth;
          table.AddCell(Cell4);

          // PdfPCell Cell41 = new PdfPCell(new Phrase(1f, bill.rollNo, new Font(Font.TIMES_ROMAN, 13f,1, borderColor)));
          // Cell41.Colspan = 1;
          // Cell41.HorizontalAlignment = 0;
          // Cell41.BorderWidth = 0.0f;
          // table.AddCell(Cell41);

          // PdfPCell Cell42 = new PdfPCell(new Phrase(1f, "Class.....", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          // Cell42.Colspan = 1;
          // Cell42.HorizontalAlignment = 2;
          // Cell42.BorderWidth = 0.0f;
          // table.AddCell(Cell42);

          // PdfPCell Cell43 = new PdfPCell(new Phrase(1f, bill.grade, new Font(Font.TIMES_ROMAN, 13f,1, borderColor)));
          // Cell43.Colspan = 1;
          // Cell43.HorizontalAlignment = 0;
          // Cell43.BorderWidth = 0.0f;
          // table.AddCell(Cell43);

          // PdfPCell Cell44 = new PdfPCell(new Phrase(1f, string.Concat("Class.....",bill.grade,"    By...",bill.paymentType), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          // Cell44.Colspan = 3;
          // Cell44.HorizontalAlignment = 0;
          // Cell44.BorderColor = borderColor;
          // Cell44.BorderWidth = 0.0f;
          // Cell44.BorderWidthRight= borderwidth;
          // table.AddCell(Cell44);

          #endregion
          
          // PdfPCell Cellblank3 = new PdfPCell(new Phrase(17f, "", new Font(Font.TIMES_ROMAN, blankHeight,0, borderColor)));
          // Cellblank3.Colspan = 6;
          // Cellblank3.HorizontalAlignment = 1;
          // Cellblank3.BorderColor = borderColor;
          // Cellblank3.BorderWidth = 0.0f;
          // Cellblank3.BorderWidthLeft= borderwidth;
          // Cellblank3.BorderWidthRight= borderwidth;
          table.AddCell(Cellblank1);

          #region payment detail
          if(bill.paymentType!="Cash")
          {
            StringBuilder sb = new StringBuilder();
            sb.Append("     ");
            //sb.Append(bill.paymentType);
            sb.Append("No.....");
            sb.Append(bill.chequeOrDdNo);
            sb.Append("    Dated.....");
            sb.Append(bill.chequeOrDdDate);
            sb.Append("    Drawn on......");
            sb.Append(bill.chequeOrDdDrawnDate);
            sb.Append("    ");
            PdfPCell Cell5 = new PdfPCell(new Phrase(0f,sb.ToString(), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
            Cell5.Colspan = 6;
            Cell5.HorizontalAlignment = 0;
            Cell5.BorderColor = borderColor;
            Cell5.BorderWidth = 0.0f;
            Cell5.BorderWidthLeft= borderwidth;
            Cell5.BorderWidthRight= borderwidth;
            table.AddCell(Cell5);

            PdfPCell Cellblank4 = new PdfPCell(new Phrase(17f, "", new Font(Font.TIMES_ROMAN, blankHeight,0, borderColor)));
            Cellblank4.Colspan = 6;
            Cellblank4.HorizontalAlignment = 1;
            Cellblank4.BorderColor = borderColor;
            Cellblank4.BorderWidth = 0.0f;
            Cellblank4.BorderWidthLeft= borderwidth;
            Cellblank4.BorderWidthRight= borderwidth;
            table.AddCell(Cellblank4);
          }
          #endregion
            
          #region row 6 (bill amount)
          PdfPCell Cell6 = new PdfPCell(new Phrase(0f,"     For Rupees...................", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell6.Colspan = 3;
          Cell6.HorizontalAlignment = 0;
          Cell6.BorderColor = borderColor;
          Cell6.BorderWidth = 0.0f;
          Cell6.BorderWidthLeft= borderwidth;
          table.AddCell(Cell6);

          PdfPCell Cell61 = new PdfPCell(new Phrase(0f,bill.paidFees.ToString(), new Font(Font.TIMES_ROMAN, 13f,5, borderColor)));
          Cell61.Colspan = 3;
          Cell61.HorizontalAlignment = 0;
          Cell61.BorderColor = borderColor;
          Cell61.BorderWidth = 0.0f;
          Cell61.BorderWidthRight= borderwidth;
          table.AddCell(Cell61);

          #endregion
            
          // PdfPCell Cellblank5 = new PdfPCell(new Phrase(17f, "", new Font(Font.TIMES_ROMAN, blankHeight,0, borderColor)));
          // Cellblank5.Colspan = 6;
          // Cellblank5.HorizontalAlignment = 1;
          // Cellblank5.BorderColor = borderColor;
          // Cellblank5.BorderWidth = 0.0f;
          // Cellblank5.BorderWidthLeft= borderwidth;
          // Cellblank5.BorderWidthRight= borderwidth;
          // table.AddCell(Cellblank5);

          PdfPCell Cellblank6 = new PdfPCell(new Phrase(17f, " ", new Font(Font.TIMES_ROMAN, 19f,0, borderColor)));
          Cellblank6.Colspan = 6;
          Cellblank6.HorizontalAlignment = 1;
          Cellblank6.BorderColor = borderColor;
          Cellblank6.BorderWidth = 0.0f;
          Cellblank6.BorderWidthLeft= borderwidth;
          Cellblank6.BorderWidthRight= borderwidth;
          table.AddCell(Cellblank6);

          #region row 7 (particulars heading)
          PdfPCell Cell7 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell7.Colspan = 1;
          Cell7.HorizontalAlignment = 0;
          Cell7.BorderColor = borderColor;
          Cell7.BorderWidth = 0.0f;
          Cell7.BorderWidthLeft= borderwidth;
          table.AddCell(Cell7);

          PdfPCell Cell71 = new PdfPCell(new Phrase(0f,"Particulars", new Font(Font.TIMES_ROMAN, 13f,1, borderColor)));
          Cell71.Colspan = 2;
          Cell71.HorizontalAlignment = 1;
          Cell71.BorderColor = borderColor;
          Cell71.BorderWidth = borderwidth;
          //Cell71.BorderWidthLeft= borderwidth;
          table.AddCell(Cell71);

          PdfPCell Cell72 = new PdfPCell(new Phrase(0f,"Rs.", new Font(Font.TIMES_ROMAN, 13f,1, borderColor)));
          Cell72.Colspan = 2;
          Cell72.HorizontalAlignment = 1;
          Cell72.BorderColor = borderColor;
          Cell72.BorderWidth = borderwidth;
          //Cell71.BorderWidthLeft= borderwidth;
          table.AddCell(Cell72);

          PdfPCell Cell73 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell73.Colspan = 1;
          Cell73.HorizontalAlignment = 0;
          Cell73.BorderColor = borderColor;
          Cell73.BorderWidth = 0.0f;
          Cell73.BorderWidthRight= borderwidth;
          table.AddCell(Cell73);
          #endregion
          
          #region row 8 (Admission fee)
          PdfPCell Cell8 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell8.Colspan = 1;
          Cell8.HorizontalAlignment = 0;
          Cell8.BorderColor = borderColor;
          Cell8.BorderWidth = 0.0f;
          Cell8.BorderWidthLeft= borderwidth;
          table.AddCell(Cell8);

          PdfPCell Cell81 = new PdfPCell(new Phrase(0f,"Admission fee", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell81.Colspan = 2;
          Cell81.HorizontalAlignment = 1;
          Cell81.BorderColor = borderColor;
          Cell81.BorderWidth = borderwidth;
          Cell81.BorderWidthTop= 0.0f;
          table.AddCell(Cell81);

          PdfPCell Cell82 = new PdfPCell(new Phrase(0f,bill.admissionFee.ToString(), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell82.Colspan = 2;
          Cell82.HorizontalAlignment = 1;
          Cell82.BorderColor = borderColor;
          Cell82.BorderWidth = borderwidth;
          Cell82.BorderWidthTop= 0.0f;
          table.AddCell(Cell82);

          PdfPCell Cell83 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell83.Colspan = 1;
          Cell83.HorizontalAlignment = 0;
          Cell83.BorderColor = borderColor;
          Cell83.BorderWidth = 0.0f;
          Cell83.BorderWidthRight= borderwidth;
          table.AddCell(Cell83);
          #endregion

          #region row 9 Caution Deposit
          PdfPCell Cell9 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell9.Colspan = 1;
          Cell9.HorizontalAlignment = 0;
          Cell9.BorderColor = borderColor;
          Cell9.BorderWidth = 0.0f;
          Cell9.BorderWidthLeft= borderwidth;
          table.AddCell(Cell9);

          PdfPCell Cell91 = new PdfPCell(new Phrase(0f,"Caution Deposit", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell91.Colspan = 2;
          Cell91.HorizontalAlignment = 1;
          Cell91.BorderColor = borderColor;
          Cell91.BorderWidth = borderwidth;
          Cell91.BorderWidthTop= 0.0f;
          table.AddCell(Cell91);

          PdfPCell Cell92 = new PdfPCell(new Phrase(0f,bill.cautionDeposit.ToString(), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell92.Colspan = 2;
          Cell92.HorizontalAlignment = 1;
          Cell92.BorderColor = borderColor;
          Cell92.BorderWidth = borderwidth;
          Cell92.BorderWidthTop= 0.0f;
          table.AddCell(Cell92);

          PdfPCell Cell93 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell93.Colspan = 1;
          Cell93.HorizontalAlignment = 0;
          Cell93.BorderColor = borderColor;
          Cell93.BorderWidth = 0.0f;
          Cell93.BorderWidthRight= borderwidth;
          table.AddCell(Cell93);

          #endregion

          #region row 10 Tution fee
          PdfPCell Cell10 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell10.Colspan = 1;
          Cell10.HorizontalAlignment = 0;
          Cell10.BorderColor = borderColor;
          Cell10.BorderWidth = 0.0f;
          Cell10.BorderWidthLeft= borderwidth;
          table.AddCell(Cell10);

          PdfPCell Cell101 = new PdfPCell(new Phrase(0f,"Tution fee", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell101.Colspan = 2;
          Cell101.HorizontalAlignment = 1;
          Cell101.BorderColor = borderColor;
          Cell101.BorderWidth = borderwidth;
          Cell101.BorderWidthTop= 0.0f;
          table.AddCell(Cell101);

          PdfPCell Cell102 = new PdfPCell(new Phrase(0f,bill.tutionFee.ToString(), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell102.Colspan = 2;
          Cell102.HorizontalAlignment = 1;
          Cell102.BorderColor = borderColor;
          Cell102.BorderWidth = borderwidth;
          Cell102.BorderWidthTop= 0.0f;
          table.AddCell(Cell102);

          PdfPCell Cell103 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell103.Colspan = 1;
          Cell103.HorizontalAlignment = 0;
          Cell103.BorderColor = borderColor;
          Cell103.BorderWidth = 0.0f;
          Cell103.BorderWidthRight= borderwidth;
          table.AddCell(Cell103);

          #endregion

          #region row 11 Boarding/Hostel Expenses
          PdfPCell Cell11 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell11.Colspan = 1;
          Cell11.HorizontalAlignment = 0;
          Cell11.BorderColor = borderColor;
          Cell11.BorderWidth = 0.0f;
          Cell11.BorderWidthLeft= borderwidth;
          table.AddCell(Cell11);

          PdfPCell Cell111 = new PdfPCell(new Phrase(0f,"Boarding/Hostel Expenses", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell111.Colspan = 2;
          Cell111.HorizontalAlignment = 1;
          Cell111.BorderColor = borderColor;
          Cell111.BorderWidth = borderwidth;
          Cell111.BorderWidthTop= 0.0f;
          table.AddCell(Cell111);

          PdfPCell Cell112 = new PdfPCell(new Phrase(0f,bill.hostelExpenses.ToString(), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell112.Colspan = 2;
          Cell112.HorizontalAlignment = 1;
          Cell112.BorderColor = borderColor;
          Cell112.BorderWidth = borderwidth;
          Cell112.BorderWidthTop= 0.0f;
          table.AddCell(Cell112);

          PdfPCell Cell113 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell113.Colspan = 1;
          Cell113.HorizontalAlignment = 0;
          Cell113.BorderColor = borderColor;
          Cell113.BorderWidth = 0.0f;
          Cell113.BorderWidthRight= borderwidth;
          table.AddCell(Cell113);

          #endregion

          #region row 12 Others
          PdfPCell Cell12 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell12.Colspan = 1;
          Cell12.HorizontalAlignment = 0;
          Cell12.BorderColor = borderColor;
          Cell12.BorderWidth = 0.0f;
          Cell12.BorderWidthLeft= borderwidth;
          table.AddCell(Cell12);

          PdfPCell Cell121 = new PdfPCell(new Phrase(0f,"Others", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell121.Colspan = 2;
          Cell121.HorizontalAlignment = 1;
          Cell121.BorderColor = borderColor;
          Cell121.BorderWidth = borderwidth;
          Cell121.BorderWidthTop= 0.0f;
          table.AddCell(Cell121);

          PdfPCell Cell122 = new PdfPCell(new Phrase(0f,bill.others.ToString(), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell122.Colspan = 2;
          Cell122.HorizontalAlignment = 1;
          Cell122.BorderColor = borderColor;
          Cell122.BorderWidth = borderwidth;
          Cell122.BorderWidthTop= 0.0f;
          table.AddCell(Cell122);

          PdfPCell Cell123 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell123.Colspan = 1;
          Cell123.HorizontalAlignment = 0;
          Cell123.BorderColor = borderColor;
          Cell123.BorderWidth = 0.0f;
          Cell123.BorderWidthRight= borderwidth;
          table.AddCell(Cell123);

          #endregion

          #region row 13 Total
          PdfPCell Cell13 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell13.Colspan = 1;
          Cell13.HorizontalAlignment = 0;
          Cell13.BorderColor = borderColor;
          Cell13.BorderWidth = 0.0f;
          Cell13.BorderWidthLeft= borderwidth;
          table.AddCell(Cell13);

          PdfPCell Cell131 = new PdfPCell(new Phrase(0f,"Total", new Font(Font.TIMES_ROMAN, 13f,1, borderColor)));
          Cell131.Colspan = 2;
          Cell131.HorizontalAlignment = 1;
          Cell131.BorderColor = borderColor;
          Cell131.BorderWidth = borderwidth;
          Cell131.BorderWidthTop= 0.0f;
          table.AddCell(Cell131);

          PdfPCell Cell132 = new PdfPCell(new Phrase(0f,bill.total.ToString(), new Font(Font.TIMES_ROMAN, 13f,1, borderColor)));
          Cell132.Colspan = 2;
          Cell132.HorizontalAlignment = 1;
          Cell132.BorderColor = borderColor;
          Cell132.BorderWidth = borderwidth;
          Cell132.BorderWidthTop= 0.0f;
          table.AddCell(Cell132);

          PdfPCell Cell133 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell133.Colspan = 1;
          Cell133.HorizontalAlignment = 0;
          Cell133.BorderColor = borderColor;
          Cell133.BorderWidth = 0.0f;
          Cell133.BorderWidthRight= borderwidth;
          table.AddCell(Cell133);

          #endregion
          
          PdfPCell Cellblank7 = new PdfPCell(new Phrase(17f, " ", new Font(Font.TIMES_ROMAN, 49f,0, borderColor)));
          Cellblank7.Colspan = 6;
          Cellblank7.HorizontalAlignment = 1;
          Cellblank7.BorderColor = borderColor;
          Cellblank7.BorderWidth = 0.0f;
          Cellblank7.BorderWidthLeft= borderwidth;
          Cellblank7.BorderWidthRight= borderwidth;
          table.AddCell(Cellblank7);

          #region row 14 cashier, sign
          PdfPCell Cell14 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell14.Colspan = 1;
          Cell14.HorizontalAlignment = 0;
          Cell14.BorderColor = borderColor;
          Cell14.BorderWidth = 0.0f;
          Cell14.BorderWidthLeft= borderwidth;
          table.AddCell(Cell14);

          PdfPCell Cell141 = new PdfPCell(new Phrase(0f,"Cashier", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell141.Colspan = 1;
          Cell141.HorizontalAlignment = 1;
          Cell141.BorderWidth = 0.0f;
          table.AddCell(Cell141);

          PdfPCell Cell142 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell142.Colspan = 2;
          Cell142.HorizontalAlignment = 0;
          Cell142.BorderWidth = 0.0f;
          table.AddCell(Cell142);

          PdfPCell Cell143 = new PdfPCell(new Phrase(0f,"Signature", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell143.Colspan = 1;
          Cell143.HorizontalAlignment = 0;
          Cell143.BorderWidth = 0.0f;
          table.AddCell(Cell143);

          PdfPCell Cell144 = new PdfPCell(new Phrase(0f,"", new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
          Cell144.Colspan = 1;
          Cell144.HorizontalAlignment = 0;
          Cell144.BorderColor = borderColor;
          Cell144.BorderWidth = 0.0f;
          Cell144.BorderWidthRight= borderwidth;
          table.AddCell(Cell144);

          #endregion
          
          PdfPCell Cellblank8 = new PdfPCell(new Phrase(17f, " ", new Font(Font.TIMES_ROMAN, 19f,0, borderColor)));
          Cellblank8.Colspan = 6;
          Cellblank8.HorizontalAlignment = 1;
          Cellblank8.BorderColor = borderColor;
          Cellblank8.BorderWidth = borderwidth;
          Cellblank8.BorderWidthTop= 0.0f;
          table.AddCell(Cellblank8);
          
          return table;
        }

        /*
        Function to return a HTML Biull format, that'll be converted to PDF
         */
        private string makeBillHtml(printModel bill)
        {
          string logoPath = string.Concat("<imh src='",_env.WebRootPath+"/images/logo_t.png","' />");
            //string html= @"<p>This <em>is </em><span style=""font-size:200%;text-decoration: underline;"">some</span> <strong>sample <em> text</em></strong><span style=""color: red;"">!!!</span></p>";
            string html = @"<table><tr><td style=""text-align:left"">
            <table style=""color:#092A72;"" cellpadding=""3"" cellspacing=""0"" width=""74.3%"">
            <tr>
              <td>
                <div>@img</div>
                <div style=""text-align:center"">An ICSE &amp; ISC School</div>
              </td>
              <td colspan=""2"">
                <table style=""width:100%;color:#092A72"">
                  <tr>
                    <td  style=""font-size:22px;font-family:serif;padding-left: 6px;"">MYSORE PUBLIC SCHOOL</td>
                  </tr>
                  <tr>
                    <td style=""font-size:10px;font-family:serif;padding-left: 6px;"">[Day/Boarding, affiliated to the Council of the Indian School Certificate Examinations, New Delhi.]</td>
                  </tr>
                  <tr>
                    <td style=""font-size:10px;font-family:serif;"">
                        Rayanakere Post, H D Kote Road, Mysuru-570008, Karanataka, India
                    </td>
                  </tr>
                  <tr>
                    <td style=""font-size:12px;font-family:serif;padding-left: 6px;"">
                        Ph. 0821-2597917, 2597223, 2597224, 2597957
                    </td> 
                  </tr>
                </table>

              </td>
            </tr>
            <tr>
              <td>
                <div style=""font-size:12px;font-family:serif;padding-left:6px"">No.<span style=""font-size:18px;color:red;font-family:serif;"">@billno</span></div>
              </td>
              <td >
                <span style=""font-size:18px;font-family:serif;padding-left:13px"">FEE RECIEPT</span>
              </td>
              <td>
                <span style=""font-size:12px;font-family:serif;margin-top:-12px;padding-left:210px"">Date...<span  style=""font-weight:600;"">@date</span>.........</span>
              </td>
            </tr>
            <tr>
              <td colspan=""3"" style=""padding-top:6px;padding-left:6px;letter-spacing:1px"">
                <div>Recieved with thanks to the credit of......<span style=""font-weight:600"">@name</span>.....</div>
              </td>
            </tr>
             <tr>
              <td colspan=""3"" style=""padding-top:6px;padding-left:6px;letter-spacing:1px"">
                <div>Roll No.......<span style=""font-weight:600"">@rollno</span>.....
                Class...<span style=""font-weight:600"">@grade</span>....By <span style=""font-weight:600"">@paymenttype</span>, No..<span style=""font-weight:600"">@ddcheno</span>
                </div>
              </td>
            </tr>
            <tr>
              <td colspan=""3"" style=""padding-top:6px;padding-left:6px;letter-spacing:1px"">
                <div class=""col-md-12"">...<span></span>...Dated...<span style=""font-weight:600"">@ddchdate</span>....Drawn on...<span style=""font-weight:600"">@ddchdrawdate</span>.........</div>
              </td>
            </tr>
            <tr>
              <td colspan=""3"" style=""padding-top:6px;padding-left:6px;letter-spacing:1px"">
                <div class=""col-md-12"">For rupees........<span style=""width:230px;font-weight:600;text-align:center"">@amount</span>..........</div>
              </td>
            </tr>
            <tr>
            <td colspan=""3"" style=""text-align:center;padding-top:6px"" >
              <table width=""90%"" cellpadding=""2"" cellspacing=""0"" style=""font-weight:600;padding-left:6px;padding-top:10px;color:#092A72;letter-spacing:1px"" border=""1"">
                <tr>
                  <td style=""text-align:center;padding:4px;width:35%;border-left:1px solid #092A72;border-top:1px solid #092A72;"">Particulars</td>
                  <td style=""text-align:center;border:1px solid #092A72;border-bottom:none"">Rs.</td>
                </tr>
                 <tr>
                  <td style=""text-align:left;padding:2px;border-left:1px solid #092A72;border-top:1px solid #092A72;"">Admission fee</td>
                  <td style=""text-align:center;border:1px solid #092A72;border-bottom:none"">@admission</td>
                </tr>
                <tr>
                  <td style=""text-align:left;padding:2px;border-left:1px solid #092A72;border-top:1px solid #092A72;"">Caution Deposit</td>
                  <td style=""text-align:center;border:1px solid #092A72;border-bottom:none"">@cautiondep</td>
                </tr>
                 <tr>
                  <td style=""text-align:left;padding:2px;border-left:1px solid #092A72;border-top:1px solid #092A72;"">Tution fee</td>
                  <td style=""text-align:center;border:1px solid #092A72;border-bottom:none"">@tution</td>
                </tr>
                <tr>
                  <td style=""text-align:left;padding:2px;border-left:1px solid #092A72;border-top:1px solid #092A72;"">Boarding/Hostel Expenses</td>
                  <td style=""text-align:center;border:1px solid #092A72;border-bottom:none"">@hostel</td>
                </tr>
                <tr>
                  <td style=""text-align:left;padding:2px;border-left:1px solid #092A72;border-top:1px solid #092A72;"">Others</td>
                  <td style=""text-align:center;border:1px solid #092A72;border-bottom:none"">@others</td>
                </tr>
                <tr >
                  <td style=""text-align:left;padding:2px;border-left:1px solid #092A72;border-top:1px solid #092A72;border-bottom:1px solid #092A72"">Total</td>
                  <td style=""text-align:center;border:1px solid #092A72;"">@total</td>
                </tr>
              </table>
            </td>
            </tr>
            <tr style=""text-align:center"">
                <td style=""padding-top:32px;"">Cashier</td>
                <td style=""padding-top:32px;"">Signature</td>
            </tr>
            </table>
            </td></tr></table>";
            html = html.Replace("@img",logoPath);
            html = html.Replace("@billno",bill.BillNumber.ToString());
            html = html.Replace("@date",bill.paidDate);
            html = html.Replace("@name",bill.name);
            html = html.Replace("@rollno",bill.rollNo);
            html = html.Replace("@grade",bill.grade);
            html = html.Replace("@paymenttype",bill.paymentType);
            html = html.Replace("@ddcheno",bill.chequeOrDdNo);
            html = html.Replace("@ddchdate",bill.chequeOrDdDate);
            html = html.Replace("@ddchdrawdate",bill.chequeOrDdDrawnDate);
            html = html.Replace("@amount",bill.paidFees.ToString());
            html = html.Replace("@admission",bill.admissionFee.ToString());
            html = html.Replace("@cautiondep",bill.cautionDeposit.ToString());
            html = html.Replace("@tution",bill.tutionFee.ToString());
            html = html.Replace("@hostel",bill.hostelExpenses.ToString());
            html = html.Replace("@others",bill.others.ToString());
            html = html.Replace("@total",bill.total.ToString());
            return html;
        }

        public async Task<mpsUser> getUserFromUserNameAsync(string username)
        {
          return await Task.FromResult<mpsUser>(await _ctx.Users.Include(a => a.UserPic).Where(a => a.UserName==username).SingleAsync());
        }

        public async Task<string> GetRolesAsync(mpsUser u)
        {
          //string role="";
          IList<string> r = await _manager.GetRolesAsync(u);
          return r.SingleOrDefault();
        }

        public byte[] printMatrix(matrixPrintModel matrix, out string strAttachment)
        {
            //MemoryStream workStream = new MemoryStream();  
          StringBuilder status = new StringBuilder("");  
          DateTime dTime = DateTime.Now;  
          //file name to be created   
          string filename = string.Format("MarksCard" + dTime.ToString("yyyyMMdd-HHmmss") + ".pdf");  
          Document doc = new Document(PageSize.A4);  
          doc.SetMargins(0f,0f, 7f,0f);  
          //Create PDF Table  
          using(MemoryStream workStream = new MemoryStream()){
            var writer = PdfWriter.GetInstance(doc, workStream  );
            DirectoryInfo d = new DirectoryInfo(_env.WebRootPath);
            FileInfo[] Files = d.GetFiles("*.pdf"); //Getting Excel files
            foreach(FileInfo f in Files )
            {
                if(f.Exists){f.Delete();}
            }
            //file will created in this path  
            strAttachment = String.Concat(_env.WebRootPath,"\\",filename);  
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;  
            doc.Open();  
            //Add Content to PDF  
            doc.Add(makeMatrix(matrix));  

            // var htmlWorker = new iTextSharp.text.html.simpleparser.HtmlWorker(doc);
            // var sr = new StringReader(makeBillHtml(bill));
            // //Parse the HTML
            // htmlWorker.Parse(sr);
                

            // Closing the document  
            doc.Close();  

            byte[] byteInfo = workStream.ToArray(); 
            return byteInfo;
          }
        }

        private PdfPTable makeMatrix(matrixPrintModel matrix)
        {
          string acad = getCurrentAcademicYear();
          StudentSlab s = getStudentSlabByIdAndYear(matrix.matrix[0].StudentId,acad,true);
          
          var borderColor = new BaseColor(9,42,114);
          var borderwidth=0.5f;
         // var blankHeight = 3f;
         var headerFontSize = 7f;
         var textFontSize = 10f;
         var footerFontSize = 9f;
          PdfPTable table = new PdfPTable(14);
          table.TotalWidth = 559f;
          table.LockedWidth = true;

          #region top row
            string logoPath = _env.WebRootPath+"/images/top.png";
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleToFit(407f,90f);
            logo.Alignment = Element.ALIGN_CENTER;
            Paragraph p = new Paragraph();
            p.Add(new Chunk(logo, 43, 0));

            PdfPCell Cell1 = new PdfPCell(new Phrase(0f,""));
            Cell1.Colspan = 14;
            Cell1.HorizontalAlignment = 1;
            Cell1.VerticalAlignment = 1;
            //Cell1.Rowspan=5;
            //Cell1.BorderColor = borderColor;
            //Cell1.BorderWidth = borderwidth;
            Cell1.BorderWidthTop = 0.0f;
            Cell1.BorderWidthLeft = 0.0f;
            Cell1.BorderWidthRight = 0.0f;
            Cell1.BorderWidthBottom = 0.0f;
            Cell1.AddElement(p);
            table.AddCell(Cell1);

            //header and name
            PdfPCell Cell2 = new PdfPCell(new Phrase(33f, string.Concat("STUDENT'S PERFORMANCE REPORT FOR THE YEAR 20",acad), new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
            Cell2.Colspan = 14;
            Cell2.HorizontalAlignment = 1;
            Cell2.VerticalAlignment = 1;
            //Cell1.Rowspan=5;
            //Cell1.BorderColor = borderColor;
            //Cell1.BorderWidth = borderwidth;
            Cell2.BorderWidthTop = 0.0f;
            Cell2.BorderWidthLeft = 0.0f;
            Cell2.BorderWidthRight = 0.0f;
            Cell2.BorderWidthBottom = 0.0f;
            table.AddCell(Cell2);

            string ss=s.Slab.Grade.Split('-')[0].TrimEnd();
            if(Convert.ToInt16(ss)<=6){
              ss="(Stds. I - VI)";
            }
            else{
              ss="(Stds. V11 - XII)";
            }
            PdfPCell Cell3 = new PdfPCell(new Phrase(33f, ss , new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
            Cell3.Colspan = 14;
            Cell3.HorizontalAlignment = Element.ALIGN_CENTER;
            Cell3.VerticalAlignment = 1;
            //Cell1.Rowspan=5;
            //Cell1.BorderColor = borderColor;
            //Cell1.BorderWidth = borderwidth;
            Cell3.BorderWidthTop = 0.0f;
            Cell3.BorderWidthLeft = 0.0f;
            Cell3.BorderWidthRight = 0.0f;
            Cell3.BorderWidthBottom = 0.0f;
            table.AddCell(Cell3);

            PdfPCell Cellblank1 = new PdfPCell(new Phrase(17f, " ", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
            Cellblank1.Colspan = 14;
            Cellblank1.HorizontalAlignment = 1;
            Cellblank1.BorderColor = borderColor;
            Cellblank1.BorderWidth = 0.0f;
          // Cellblank1.BorderWidthLeft= borderwidth;
          // Cellblank1.BorderWidthRight= borderwidth;
            table.AddCell(Cellblank1);

            string sTopName=string.Concat("Name of Student ...............",s.Student.StudentUser.FirstName," ",s.Student.StudentUser.LastName,".................Roll No:",s.Student.RollNumber,"...........Sex: ",s.Student.StudentUser.Gender,"............Class: ",getRoman(s.Slab.Grade));

            PdfPCell Cell4 = new PdfPCell(new Phrase(0f, sTopName, new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
            Cell4.Colspan = 14;
            Cell4.HorizontalAlignment = 1;
            Cell4.VerticalAlignment = 1;
            //Ce4l1.Rowspan=5;
            //Ce4l1.BorderColor = borderColor;
            //Ce4l1.BorderWidth = borderwidth;
            Cell4.BorderWidthTop = 0.0f;
            Cell4.BorderWidthLeft = 0.0f;
            Cell4.BorderWidthRight = 0.0f;
            Cell4.BorderWidthBottom = 0.0f;
            table.AddCell(Cell4);

             PdfPCell Cellblank2 = new PdfPCell(new Phrase(17f, " ", new Font(Font.TIMES_ROMAN, 8f,0, borderColor)));
            Cellblank2.Colspan = 14;
            Cellblank2.HorizontalAlignment = 1;
            Cellblank2.BorderColor = borderColor;
            Cellblank2.BorderWidth = 0.0f;
          // Cellblank1.BorderWidthLeft= borderwidth;
          // Cellblank1.BorderWidthRight= borderwidth;
            table.AddCell(Cellblank2);
            
          #endregion
          
          #region head row
            PdfPCell Cellh1 = new PdfPCell(new Phrase(0f, "Subject", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
            Cellh1.Colspan = 3;
            Cellh1.HorizontalAlignment = 1;
            Cellh1.VerticalAlignment = Element.ALIGN_CENTER;
            Cellh1.BorderColor = borderColor;
            Cellh1.BorderWidth = borderwidth;
            //Cellh1.BorderWidthTop = 0.0f;
            //Cellh1.BorderWidthLeft = 0.0f;
            //Cellh1.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh1);

            PdfPCell Cellh2 = new PdfPCell(new Phrase(0f, "First Mid Term Examination (20)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh2.Colspan = 1;
            Cellh2.HorizontalAlignment = 1;
            Cellh2.VerticalAlignment = 1;
            Cellh2.BorderColor = borderColor;
            Cellh2.BorderWidth = borderwidth;
            //Cel2h1.BorderWidthTop = 0.0f;
            Cellh2.BorderWidthLeft = 0.0f;
            //Cellh2.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh2);

            PdfPCell Cellh3 = new PdfPCell(new Phrase(0f, "Second Mid Term Examination (20)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh3.Colspan = 1;
            Cellh3.HorizontalAlignment = 1;
            Cellh3.VerticalAlignment = 1;
            Cellh3.BorderColor = borderColor;
            Cellh3.BorderWidth = borderwidth;
            //Cel3h1.BorderWidthTop = 0.0f;
            Cellh3.BorderWidthLeft = 0.0f;
            //Cellh3.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh3);

            PdfPCell Cellh4 = new PdfPCell(new Phrase(0f, "First Terminal Examination (60)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh4.Colspan = 1;
            Cellh4.HorizontalAlignment = 1;
            Cellh4.VerticalAlignment = 1;
            Cellh4.BorderColor = borderColor;
            Cellh4.BorderWidth = borderwidth;
            //Cel4h1.BorderWidthTop = 0.0f;
            Cellh4.BorderWidthLeft = 0.0f;
            //Cellh4.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh4);

            PdfPCell Cellh5 = new PdfPCell(new Phrase(0f, "Total                  (100)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh5.Colspan = 1;
            Cellh5.HorizontalAlignment = 1;
            Cellh5.VerticalAlignment = 1;
            Cellh5.BorderColor = borderColor;
            Cellh5.BorderWidth = borderwidth;
            //Cel5h1.BorderWidthTop = 0.0f;
            Cellh5.BorderWidthLeft = 0.0f;
            //Cellh5.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh5);

            PdfPCell Cellh6 = new PdfPCell(new Phrase(0f, "Third Mid Term Examination (20)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh6.Colspan = 1;
            Cellh6.HorizontalAlignment = 1;
            Cellh6.VerticalAlignment = 1;
            Cellh6.BorderColor = borderColor;
            Cellh6.BorderWidth = borderwidth;
            //Cel6h1.BorderWidthTop = 0.0f;
            Cellh6.BorderWidthLeft = 0.0f;
            //Cellh6.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh6);

            PdfPCell Cellh7 = new PdfPCell(new Phrase(0f, "Fourth Mid Term Examination (20)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh7.Colspan = 1;
            Cellh7.HorizontalAlignment = 1;
            Cellh7.VerticalAlignment = 1;
            Cellh7.BorderColor = borderColor;
            Cellh7.BorderWidth = borderwidth;
            //Cel7h1.BorderWidthTop = 0.0f;
            Cellh7.BorderWidthLeft = 0.0f;
            //Cellh7.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh7);
            
            PdfPCell Cellh8 = new PdfPCell(new Phrase(0f, "Second Terminal Examination (60)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh8.Colspan = 1;
            Cellh8.HorizontalAlignment = 1;
            Cellh8.VerticalAlignment = 1;
            Cellh8.BorderColor = borderColor;
            Cellh8.BorderWidth = borderwidth;
            //Cel8h1.BorderWidthTop = 0.0f;
            Cellh8.BorderWidthLeft = 0.0f;
            //Cellh8.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh8);

            PdfPCell Cellh9 = new PdfPCell(new Phrase(0f, "Total                (100)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh9.Colspan = 1;
            Cellh9.HorizontalAlignment = 1;
            Cellh9.VerticalAlignment = 1;
            Cellh9.BorderColor = borderColor;
            Cellh9.BorderWidth = borderwidth;
            //Cel9h1.BorderWidthTop = 0.0f;
            Cellh9.BorderWidthLeft = 0.0f;
            //Cellh9.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh9);

            PdfPCell Cellh10 = new PdfPCell(new Phrase(0f, "Grand Total            (100)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh10.Colspan = 1;
            Cellh10.HorizontalAlignment = 1;
            Cellh10.VerticalAlignment = 1;
            Cellh10.BorderColor = borderColor;
            Cellh10.BorderWidth = borderwidth;
            //Cel10h1.BorderWidthTop = 0.0f;
            Cellh10.BorderWidthLeft = 0.0f;
            //Cellh10.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh10);

            PdfPCell Cellh11 = new PdfPCell(new Phrase(0f, "Final Average              (%)", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh11.Colspan = 1;
            Cellh11.HorizontalAlignment = 1;
            Cellh11.VerticalAlignment = 1;
            Cellh11.BorderColor = borderColor;
            Cellh11.BorderWidth = borderwidth;
            //Cel11h1.BorderWidthTop = 0.0f;
            Cellh11.BorderWidthLeft = 0.0f;
            //Cellh11.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh11);

            PdfPCell Cellh12 = new PdfPCell(new Phrase(0f, "Median Marks in the Class", new Font(Font.TIMES_ROMAN, headerFontSize,0, borderColor)));
            Cellh12.Colspan = 1;
            Cellh12.HorizontalAlignment = 1;
            Cellh12.VerticalAlignment = 1;
            Cellh12.BorderColor = borderColor;
            Cellh12.BorderWidth = borderwidth;
            //Cel12h1.BorderWidthTop = 0.0f;
            Cellh12.BorderWidthLeft = 0.0f;
            //Cellc12.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellh12);
          #endregion
          
          foreach(var row in matrix.matrix){
            PdfPCell Cellc1 = new PdfPCell(new Phrase(0f, row.Subject, new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
            Cellc1.Colspan = 3;
            Cellc1.HorizontalAlignment = 0;
            Cellc1.VerticalAlignment = Element.ALIGN_CENTER;
            Cellc1.BorderColor = borderColor;
            Cellc1.BorderWidth = borderwidth;
            Cellc1.BorderWidthTop = 0.0f;
            Cellc1.PaddingBottom = 5f;
            Cellc1.PaddingLeft=3f;
            //Cellh1.BorderWidthLeft = 0.0f;
            //Cellh1.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellc1);

            #region column 1
            PdfPCell Cellc2 = null;
            if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
              Cellc2 = new PdfPCell(new Phrase(0f, matrix.exam1Tot, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
              Cellc2 = new PdfPCell(new Phrase(0f, matrix.exam1Per, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            else{
              Cellc2 = new PdfPCell(new Phrase(0f, row.Exam1=="-"||row.Exam1=="0"?"":row.Exam1, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            if(!row.Subject.Equals("Remarks")){
              Cellc2.Colspan = 1;
            }
            else{
              Cellc2.Colspan = 4;              
            }
            Cellc2.HorizontalAlignment = 1;
            Cellc2.VerticalAlignment = 1;
            Cellc2.BorderColor = borderColor;
            Cellc2.BorderWidth = borderwidth;
            Cellc2.BorderWidthTop = 0.0f;
            Cellc2.BorderWidthLeft = 0.0f;
            //Cellc2.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellc2);
            #endregion
            
            #region column 2
            if(!row.Subject.Equals("Remarks")){
              PdfPCell Cellc3 = null;
              if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
                Cellc3 = new PdfPCell(new Phrase(0f, matrix.exam2Tot, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
                Cellc3 = new PdfPCell(new Phrase(0f, matrix.exam2Per, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else
              {
                Cellc3 = new PdfPCell(new Phrase(0f, row.Exam2=="-"||row.Exam2=="0"?"":row.Exam2, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              Cellc3.Colspan = 1;
              Cellc3.HorizontalAlignment = 1;
              Cellc3.VerticalAlignment = 1;
              Cellc3.BorderColor = borderColor;
              Cellc3.BorderWidth = borderwidth;
              Cellc3.BorderWidthTop = 0.0f;
              Cellc3.BorderWidthLeft = 0.0f;
              //Cellc3.BorderWidthRight = 0.0f;
              //Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc3);
            }
            #endregion

            #region column 3
            if(!row.Subject.Equals("Remarks")){
              PdfPCell Cellc4 = null;
              if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
                Cellc4 = new PdfPCell(new Phrase(0f, matrix.exam3Tot, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
                Cellc4 = new PdfPCell(new Phrase(0f, matrix.exam3Per, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else{
                Cellc4 = new PdfPCell(new Phrase(0f, row.Exam3=="-"||row.Exam3=="0"?"":row.Exam3, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              Cellc4.Colspan = 1;
              Cellc4.HorizontalAlignment = 1;
              Cellc4.VerticalAlignment = 1;
              Cellc4.BorderColor = borderColor;
              Cellc4.BorderWidth = borderwidth;
              Cellc4.BorderWidthTop = 0.0f;
              Cellc4.BorderWidthLeft = 0.0f;
              //Cellc4.BorderWidthRight = 0.0f;
              //Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc4);
            }
            #endregion
            
            #region column 4
            if(!row.Subject.Equals("Remarks")){
              string sum = getSum(row.Exam1,row.Exam2,row.Exam3);
              PdfPCell Cellc5 = null;
              if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
                 Cellc5 = new PdfPCell(new Phrase(0f, matrix.Tot1, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
                  Cellc5 = new PdfPCell(new Phrase(0f, matrix.perTot1, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else{
                Cellc5 = new PdfPCell(new Phrase(0f, sum=="-"||sum=="0"?"":sum, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              Cellc5.Colspan = 1;
              Cellc5.HorizontalAlignment = 1;
              Cellc5.VerticalAlignment = 1;
              Cellc5.BorderColor = borderColor;
              Cellc5.BorderWidth = borderwidth;
              Cellc5.BorderWidthTop = 0.0f;
              Cellc5.BorderWidthLeft = 0.0f;
              //Cellc5.BorderWidthRight = 0.0f;
              //Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc5);
            }
            #endregion

            #region column 5
            PdfPCell Cellc6 = null;
            if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
              Cellc6 = new PdfPCell(new Phrase(0f, matrix.exam4Tot, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
              Cellc6 = new PdfPCell(new Phrase(0f, matrix.exam4Per, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            else{
              Cellc6 = new PdfPCell(new Phrase(0f, row.Exam4=="-"||row.Exam4=="0"?"":row.Exam4, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            if(!row.Subject.Equals("Remarks")){
              Cellc6.Colspan = 1;
            }
            else{
              Cellc6.Colspan = 4;              
            }
            Cellc6.HorizontalAlignment = 1;
            Cellc6.VerticalAlignment = 1;
            Cellc6.BorderColor = borderColor;
            Cellc6.BorderWidth = borderwidth;
            Cellc6.BorderWidthTop = 0.0f;
            Cellc6.BorderWidthLeft = 0.0f;
            //Cellc6.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellc6);

            #endregion
            
            #region column 6
            if(!row.Subject.Equals("Remarks")){
              PdfPCell Cellc7=null;
              if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
                Cellc7 = new PdfPCell(new Phrase(0f, matrix.exam5Tot, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
                Cellc7 = new PdfPCell(new Phrase(0f, matrix.exam5Per, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else{
                Cellc7 = new PdfPCell(new Phrase(0f, row.Exam5=="-"||row.Exam5=="0"?"":row.Exam5, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              Cellc7.Colspan = 1;
              Cellc7.HorizontalAlignment = 1;
              Cellc7.VerticalAlignment = 1;
              Cellc7.BorderColor = borderColor;
              Cellc7.BorderWidth = borderwidth;
              Cellc7.BorderWidthTop = 0.0f;
              Cellc7.BorderWidthLeft = 0.0f;
              //Cellc7.BorderWidthRight = 0.0f;
              //Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc7);
            }
            #endregion
            
            #region column 7
            if(!row.Subject.Equals("Remarks")){
              PdfPCell Cellc8 = null;
              if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
                Cellc8 = new PdfPCell(new Phrase(0f, matrix.exam6Tot, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
                Cellc8 = new PdfPCell(new Phrase(0f, matrix.exam6Per, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else{
                Cellc8 = new PdfPCell(new Phrase(0f, row.Exam6=="-"||row.Exam6=="0"?"":row.Exam6, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              Cellc8.Colspan = 1;
              Cellc8.HorizontalAlignment = 1;
              Cellc8.VerticalAlignment = 1;
              Cellc8.BorderColor = borderColor;
              Cellc8.BorderWidth = borderwidth;
              Cellc8.BorderWidthTop = 0.0f;
              Cellc8.BorderWidthLeft = 0.0f;
              //Cellc8.BorderWidthRight = 0.0f;
              //Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc8);
            }
  	        #endregion
            
            #region column 8
            if(!row.Subject.Equals("Remarks")){
              PdfPCell Cellc9 = null;
              string sum = getSum(row.Exam4,row.Exam5,row.Exam6);
              if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
                 Cellc9 = new PdfPCell(new Phrase(0f, matrix.Tot2, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
                Cellc9 = new PdfPCell(new Phrase(0f, matrix.perTot2, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else{
                Cellc9 = new PdfPCell(new Phrase(0f, sum=="-"||sum=="0"?"":sum, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              Cellc9.Colspan = 1;
              Cellc9.HorizontalAlignment = 1;
              Cellc9.VerticalAlignment = 1;
              Cellc9.BorderColor = borderColor;
              Cellc9.BorderWidth = borderwidth;
              Cellc9.BorderWidthTop = 0.0f;
              Cellc9.BorderWidthLeft = 0.0f;
              //Cellc9.BorderWidthRight = 0.0f;
              //Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc9);
            }
            #endregion

            #region column 9
            PdfPCell Cellc10 = null;
            //string sum1 = getSum(row.Exam1,row.Exam2,row.Exam3);
            //string sum2 = getSum(row.Exam4,row.Exam5,row.Exam6);
            string sum1 = getSum(getSum(row.Exam1,row.Exam2,row.Exam3),getSum(row.Exam4,row.Exam5,row.Exam6),"0");
            if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
              Cellc10 = new PdfPCell(new Phrase(0f, matrix.Tot3, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
              Cellc10 = new PdfPCell(new Phrase(0f, matrix.perTot3, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            else{
              Cellc10 = new PdfPCell(new Phrase(0f, sum1=="-"||sum1=="0"?"":sum1, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
            }
            if(!row.Subject.Equals("Remarks")){
              Cellc10.Colspan = 1;
            }
            else{
              Cellc10.Colspan = 4;              
            }
            Cellc10.HorizontalAlignment = 1;
            Cellc10.VerticalAlignment = 1;
            Cellc10.BorderColor = borderColor;
            Cellc10.BorderWidth = borderwidth;
            Cellc10.BorderWidthTop = 0.0f;
            Cellc10.BorderWidthLeft = 0.0f;
            //Cellc10.BorderWidthRight = 0.0f;
            //Cellh1.BorderWidthBottom = 0.0f;
            table.AddCell(Cellc10);
            #endregion

            #region column10
            if(!row.Subject.Equals("Remarks")){
              PdfPCell Cellc11 = null;
              int per=0; string v="";
              if(Int32.TryParse(sum1,out per)){ per = per / 2; v=per.ToString();  }
              if(row.Subject.Equals("Total") && row.Type.Equals("Value")){
                Cellc11 = new PdfPCell(new Phrase(0f, matrix.finalAvg, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else if(row.Subject.Equals("Percentage") && row.Type.Equals("Value")){
                Cellc11 = new PdfPCell(new Phrase(0f, matrix.perFinalAvg, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              else{
                Cellc11 = new PdfPCell(new Phrase(0f, v=="-"||v=="0"?"":v, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              }
              Cellc11.Colspan = 1;
              Cellc11.HorizontalAlignment = 1;
              Cellc11.VerticalAlignment = 1;
              Cellc11.BorderColor = borderColor;
              Cellc11.BorderWidth = borderwidth;
              Cellc11.BorderWidthTop = 0.0f;
              Cellc11.BorderWidthLeft = 0.0f;
              //Cellc11.BorderWidthRight = 0.0f;
              //Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc11);
            }
            #endregion

            #region column11
            if(!row.Subject.Equals("Remarks")){
              PdfPCell Cellc12 = new PdfPCell(new Phrase(0f, row.Median, new Font(Font.TIMES_ROMAN, textFontSize,0, borderColor)));
              Cellc12.Colspan = 1;
              Cellc12.HorizontalAlignment = 1;
              Cellc12.VerticalAlignment = 1;
              Cellc12.BorderColor = borderColor;
              Cellc12.BorderWidth = borderwidth;
              Cellc12.BorderWidthTop = 0.0f;
              Cellc12.BorderWidthLeft = 0.0f;
              //Cellc12.BorderWidthRight = 0.0f;
              //Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc12);
            }
            #endregion

          }

          PdfPCell Cellblank3 = new PdfPCell(new Phrase(17f, " ", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
          Cellblank3.Colspan = 14;
          Cellblank3.HorizontalAlignment = 1;
          Cellblank3.BorderWidth = 0.0f;
          table.AddCell(Cellblank3);

          PdfPCell Cellf1 = new PdfPCell(new Phrase(0f, "For TL, G.K,Value Edu., Art & Craft, Music, Dance, Yoga & Physical Education, only grades will be given as follows", new Font(Font.TIMES_ROMAN, footerFontSize,0, borderColor)));
          Cellf1.Colspan = 14;
          Cellf1.HorizontalAlignment = 1;
          Cellf1.VerticalAlignment = Element.ALIGN_CENTER;
          Cellf1.BorderWidth = 0.0f;
          table.AddCell(Cellf1);

          PdfPCell Cellf2 = new PdfPCell(new Phrase(0f, "A=Excellent, B=Very Good, C=Good, D=Satisfactory, E=Failed", new Font(Font.TIMES_ROMAN, footerFontSize,0, borderColor)));
          Cellf2.Colspan = 14;
          Cellf2.HorizontalAlignment = 1;
          Cellf2.VerticalAlignment = Element.ALIGN_CENTER;
          Cellf2.BorderWidth = 0.0f;
          table.AddCell(Cellf2);

          PdfPCell Cellf3 = new PdfPCell(new Phrase(0f, "Median Mark = Average of the highest and lowest marks in the class", new Font(Font.TIMES_ROMAN, footerFontSize,0, borderColor)));
          Cellf3.Colspan = 14;
          Cellf3.HorizontalAlignment = 1;
          Cellf3.VerticalAlignment = Element.ALIGN_CENTER;
          Cellf3.BorderWidth = 0.0f;
          table.AddCell(Cellf3);

          PdfPCell Cellf4 = new PdfPCell(new Phrase(0f, "There is no correction or overwriting in this document", new Font(Font.TIMES_ROMAN, footerFontSize,0, borderColor)));
          Cellf4.Colspan = 14;
          Cellf4.HorizontalAlignment = 1;
          Cellf4.VerticalAlignment = Element.ALIGN_CENTER;
          Cellf4.BorderWidth = 0.0f;
          table.AddCell(Cellf4);

          return table;
        }

        private string getSum(string v1, string v2, string v3)
        {
            string res="";
            try{
              int i=0,j=0,k=0;
              Int32.TryParse(v1,out i);
              Int32.TryParse(v2,out j);
              Int32.TryParse(v3,out k);
              res = (i+j+k).ToString();
            }
            catch
            {
              
            }
            return res;
        }

        public string getRoman(string grade)
        {
          string r=grade;
            switch(grade.Split('-')[0].TrimEnd())
            {
              case "1":r=r.Replace("1","I");break;
              case "2":r=r.Replace("2","II");break;
              case "3":r=r.Replace("3","III");break;
              case "4":r=r.Replace("4","IV");break;
              case "5":r=r.Replace("5","V");break;
              case "6":r=r.Replace("6","VI");break;
              case "7":r=r.Replace("7","VII");break;
              case "8":r=r.Replace("8","VIII");break;
              case "9":r=r.Replace("9","IX");break;
              case "10":r=r.Replace("10","X");break;
              case "11":r=r.Replace("11","XI");break;
              case "12":r=r.Replace("12","XII");break;
            }
            return r;
        }

        private StudentSlab getStudentSlabByIdAndYear(string id, string academicyear, bool includeStudent)
        {
          if(includeStudent)
          {
              var studentslab = _ctx.StudentSlabs
                              .Include(s => s.Slab)
                              .Include(s => s.Student).ThenInclude(s => s.StudentUser)
                              .SingleOrDefault(s => s.StudentId == id && s.AcademicYear==academicyear);
              return studentslab;
          }
          else
          {
              var studentslab = _ctx.StudentSlabs
                              .Include(s => s.Slab)
                              .SingleOrDefault(s => s.StudentId == id && s.AcademicYear==academicyear);
              return studentslab;
          }
        }

        public byte[] printCharacterMatrix(characterPrintModel model, out string strAttachment)
        {
              //MemoryStream workStream = new MemoryStream();  
          StringBuilder status = new StringBuilder("");  
          DateTime dTime = DateTime.Now;  
          //file name to be created   
          string filename = string.Format("CharacterCertificate" + dTime.ToString("yyyyMMdd-HHmmss") + ".pdf");  
          Document doc = new Document(PageSize.A4);  
          doc.SetMargins(0f,0f, 59f,0f);  
          //Create PDF Table  
          using(MemoryStream workStream = new MemoryStream()){
            var writer = PdfWriter.GetInstance(doc, workStream  );
            DirectoryInfo d = new DirectoryInfo(_env.WebRootPath);
            FileInfo[] Files = d.GetFiles("*.pdf"); //Getting pdf files
            foreach(FileInfo f in Files )
            {
                if(f.Exists){f.Delete();}
            }
            //file will created in this path  
            strAttachment = String.Concat(_env.WebRootPath,"\\",filename);  
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;  
            doc.Open();  
            //Add Content to PDF  
            doc.Add(makeCharacterMatrix(model));  

            // Closing the document  
            doc.Close();  

            byte[] byteInfo = workStream.ToArray(); 
            return byteInfo;
          }
        }

        private PdfPTable makeCharacterMatrix(characterPrintModel model)
        {
          var borderColor = new BaseColor(9,42,114);
           var borderwidth=0.5f;
          // // var blankHeight = 3f;
          // var headerFontSize = 7f;
          // var textFontSize = 10f;
          // var footerFontSize = 9f;
          PdfPTable table = new PdfPTable(16);
          table.TotalWidth = 503f;
          table.LockedWidth = true;

          string checkPath = _env.WebRootPath+"/images/check.png";
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(checkPath);
            logo.ScaleToFit(13f,10f);
            logo.Alignment = Element.ALIGN_CENTER;
            Paragraph ch = new Paragraph();
            ch.Add(new Chunk(logo, 7, 1));

            //header and name
            PdfPCell Cell2 = new PdfPCell(new Phrase(33f, "CHARACTER & PERSONALITY PROFILE", new Font(Font.TIMES_ROMAN, 17f,0, borderColor)));
            Cell2.Colspan = 16;
            Cell2.HorizontalAlignment = 1;
            Cell2.VerticalAlignment = 1;
            //Cell1.Rowspan=5;
            //Cell1.BorderColor = borderColor;
            //Cell1.BorderWidth = borderwidth;
            Cell2.BorderWidthTop = 0.0f;
            Cell2.BorderWidthLeft = 0.0f;
            Cell2.BorderWidthRight = 0.0f;
            Cell2.BorderWidthBottom = 0.0f;
            table.AddCell(Cell2);

            PdfPCell Cell3 = new PdfPCell(new Phrase(33f, " ", new Font(Font.TIMES_ROMAN, 17f,0, borderColor)));
            Cell3.Colspan = 16;
            Cell3.HorizontalAlignment = 1;
            Cell3.VerticalAlignment = 1;
            //Ce3l1.Rowspan=5;
            //Ce3l1.BorderColor = borderColor;
            //Ce3l1.BorderWidth = borderwidth;
            Cell3.BorderWidthTop = 0.0f;
            Cell3.BorderWidthLeft = 0.0f;
            Cell3.BorderWidthRight = 0.0f;
            Cell3.BorderWidthBottom = 0.0f;
            table.AddCell(Cell3);

            #region header
              PdfPCell Cellh1 = new PdfPCell(new Phrase(0f, " ", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh1.Colspan = 4;
              Cellh1.HorizontalAlignment = 0;
              Cellh1.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh1.BorderColor = borderColor;
              Cellh1.BorderWidth = borderwidth;
              //Cellh1.BorderWidthTop = 0.0f;
              Cellh1.PaddingBottom = 5f;
              Cellh1.PaddingLeft=3f;
              //Cellh1.BorderWidthLeft = 0.0f;
              Cellh1.BorderWidthRight = 0.0f;
              Cellh1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh1);

              PdfPCell Cellh2 = new PdfPCell(new Phrase(0f, "Grades", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh2.Colspan = 4;
              Cellh2.HorizontalAlignment = 1;
              Cellh2.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh2.BorderColor = borderColor;
              Cellh2.BorderWidth = borderwidth;
              //Cel2h1.BorderWidthTop = 0.0f;
              Cellh2.PaddingBottom = 5f;
              Cellh2.PaddingLeft=3f;
              //Cel2h1.BorderWidthLeft = 0.0f;
              Cellh2.BorderWidthRight = 0.0f;
              Cellh2.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh2);

              PdfPCell Cellh3 = new PdfPCell(new Phrase(0f, " ", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh3.Colspan = 4;
              Cellh3.HorizontalAlignment = 0;
              Cellh3.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh3.BorderColor = borderColor;
              Cellh3.BorderWidth = borderwidth;
              //Cel3h1.BorderWidthTop = 0.0f;
              Cellh3.PaddingBottom = 5f;
              Cellh3.PaddingLeft=3f;
              //Cel3h1.BorderWidthLeft = 0.0f;
              Cellh3.BorderWidthRight = 0.0f;
              Cellh3.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh3);

              PdfPCell Cellh4 = new PdfPCell(new Phrase(0f, "Grades", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh4.Colspan = 4;
              Cellh4.HorizontalAlignment = 1;
              Cellh4.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh4.BorderColor = borderColor;
              Cellh4.BorderWidth = borderwidth;
              //Cel4h1.BorderWidthTop = 0.0f;
              Cellh4.PaddingBottom = 5f;
              Cellh4.PaddingLeft=3f;
              //Cel4h1.BorderWidthLeft = 0.0f;
              //Cellh4.BorderWidthRight = 0.0f;
              Cellh4.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh4);

              PdfPCell Cellh5= new PdfPCell(new Phrase(0f, "Social Habits", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh5.Colspan = 4;
              Cellh5.HorizontalAlignment = 1;
              Cellh5.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh5.BorderColor = borderColor;
              Cellh5.BorderWidth = borderwidth;
              //Cel5h1.BorderWidthTop = 0.0f;
              Cellh5.PaddingBottom = 5f;
              Cellh5.PaddingLeft=3f;
              //Cel5h1.BorderWidthLeft = 0.0f;
              Cellh5.BorderWidthRight = 0.0f;
              Cellh5.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh5);

              PdfPCell Cellh6= new PdfPCell(new Phrase(0f, "A", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh6.Colspan = 1;
              Cellh6.HorizontalAlignment = 1;
              Cellh6.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh6.BorderColor = borderColor;
              Cellh6.BorderWidth = borderwidth;
              //Cel6h1.BorderWidthTop = 0.0f;
              //Cel6h5.PaddingBottom = 5f;
              //Cel6h5.PaddingLeft=3f;
              //Cel6h1.BorderWidthLeft = 0.0f;
              Cellh6.BorderWidthRight = 0.0f;
              Cellh6.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh6);

              PdfPCell Cellh7= new PdfPCell(new Phrase(0f, "B", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh7.Colspan = 1;
              Cellh7.HorizontalAlignment = 1;
              Cellh7.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh7.BorderColor = borderColor;
              Cellh7.BorderWidth = borderwidth;
              //Cel7h1.BorderWidthTop = 0.0f;
              //Cel7h5.PaddingBottom = 5f;
              //Cel7h5.PaddingLeft=3f;
              //Cel7h1.BorderWidthLeft = 0.0f;
              Cellh7.BorderWidthRight = 0.0f;
              Cellh7.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh7);

              PdfPCell Cellh8= new PdfPCell(new Phrase(0f, "C", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh8.Colspan = 1;
              Cellh8.HorizontalAlignment = 1;
              Cellh8.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh8.BorderColor = borderColor;
              Cellh8.BorderWidth = borderwidth;
              //Cel8h1.BorderWidthTop = 0.0f;
              //Cel8h5.PaddingBottom = 5f;
              //Cel8h5.PaddingLeft=3f;
              //Cel8h1.BorderWidthLeft = 0.0f;
              Cellh8.BorderWidthRight = 0.0f;
              Cellh8.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh8);

              PdfPCell Cellh9= new PdfPCell(new Phrase(0f, "D", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh9.Colspan = 1;
              Cellh9.HorizontalAlignment = 1;
              Cellh9.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh9.BorderColor = borderColor;
              Cellh9.BorderWidth = borderwidth;
              //Cel9h1.BorderWidthTop = 0.0f;
              //Cel9h5.PaddingBottom = 5f;
              //Cel9h5.PaddingLeft=3f;
              //Cel9h1.BorderWidthLeft = 0.0f;
              Cellh9.BorderWidthRight = 0.0f;
              Cellh9.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh9);

              PdfPCell Cellh10= new PdfPCell(new Phrase(0f, "Residential Habits", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh10.Colspan = 4;
              Cellh10.HorizontalAlignment = 1;
              Cellh10.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh10.BorderColor = borderColor;
              Cellh10.BorderWidth = borderwidth;
              //Cel10h1.BorderWidthTop = 0.0f;
              //Cel10h5.PaddingBottom = 5f;
              //Cel10h5.PaddingLeft=3f;
              //Cel10h1.BorderWidthLeft = 0.0f;
              Cellh10.BorderWidthRight = 0.0f;
              Cellh10.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh10);

              PdfPCell Cellh11= new PdfPCell(new Phrase(0f, "A", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh11.Colspan = 1;
              Cellh11.HorizontalAlignment = 1;
              Cellh11.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh11.BorderColor = borderColor;
              Cellh11.BorderWidth = borderwidth;
              //Cel11h1.BorderWidthTop = 0.0f;
              //Cel11h5.PaddingBottom = 5f;
              //Cel11h5.PaddingLeft=3f;
              //Cel11h1.BorderWidthLeft = 0.0f;
              Cellh11.BorderWidthRight = 0.0f;
              Cellh11.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh11);

              PdfPCell Cellh12= new PdfPCell(new Phrase(0f, "B", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh12.Colspan = 1;
              Cellh12.HorizontalAlignment = 1;
              Cellh12.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh12.BorderColor = borderColor;
              Cellh12.BorderWidth = borderwidth;
              //Cel12h1.BorderWidthTop = 0.0f;
              //Cel12h5.PaddingBottom = 5f;
              //Cel12h5.PaddingLeft=3f;
              //Cel12h1.BorderWidthLeft = 0.0f;
              Cellh12.BorderWidthRight = 0.0f;
              Cellh12.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh12);

              PdfPCell Cellh13= new PdfPCell(new Phrase(0f, "C", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh13.Colspan = 1;
              Cellh13.HorizontalAlignment = 1;
              Cellh13.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh13.BorderColor = borderColor;
              Cellh13.BorderWidth = borderwidth;
              //Cel13h1.BorderWidthTop = 0.0f;
              //Cel13h5.PaddingBottom = 5f;
              //Cel13h5.PaddingLeft=3f;
              //Cel13h1.BorderWidthLeft = 0.0f;
              Cellh13.BorderWidthRight = 0.0f;
              Cellh13.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh13);

              PdfPCell Cellh14= new PdfPCell(new Phrase(0f, "D", new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellh14.Colspan = 1;
              Cellh14.HorizontalAlignment = 1;
              Cellh14.VerticalAlignment = Element.ALIGN_CENTER;
              Cellh14.BorderColor = borderColor;
              Cellh14.BorderWidth = borderwidth;
              //Cel14h1.BorderWidthTop = 0.0f;
              //Cel14h5.PaddingBottom = 5f;
              //Cel14h5.PaddingLeft=3f;
              //Cel14h1.BorderWidthLeft = 0.0f;
              //Cellh14.BorderWidthRight = 0.0f;
              Cellh14.BorderWidthBottom = 0.0f;
              table.AddCell(Cellh14);
            #endregion
            int count=model.characterMatrix.Count;
            foreach(var c in model.characterMatrix){
              PdfPCell Cellc1 = new PdfPCell(new Phrase(0f, c.social, new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellc1.Colspan = 4;
              Cellc1.HorizontalAlignment = 0;
              Cellc1.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc1.BorderColor = borderColor;
              Cellc1.BorderWidth = borderwidth;
              //Cellc1.BorderWidthTop = 0.0f;
              Cellc1.PaddingBottom = 5f;
              Cellc1.PaddingLeft=3f;
              //Cellh1.BorderWidthLeft = 0.0f;
              Cellc1.BorderWidthRight = 0.0f;
              if(count>1)
                Cellc1.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc1);


              PdfPCell Cellc2 = new PdfPCell(new Phrase(0f,""));
              Cellc2.Colspan = 1;
              Cellc2.HorizontalAlignment = 1;
              Cellc2.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc2.BorderColor = borderColor;
              Cellc2.BorderWidth = borderwidth;
              //Cellc2.BorderWidthTop = 0.0f;
              Cellc2.PaddingBottom = 5f;
              Cellc2.PaddingLeft=3f;
              //Cellh1.BorderWidthLeft = 0.0f;
              Cellc2.BorderWidthRight = 0.0f;
              if(count>1)
                Cellc2.BorderWidthBottom = 0.0f;
              if(c.sg=="A")
                Cellc2.AddElement(ch);
              table.AddCell(Cellc2);

              PdfPCell Cellc3 = new PdfPCell(new Phrase(0f, ""));
              Cellc3.Colspan = 1;
              Cellc3.HorizontalAlignment = 1;
              Cellc3.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc3.BorderColor = borderColor;
              Cellc3.BorderWidth = borderwidth;
              //Cel3c2.BorderWidthTop = 0.0f;
              //Cellc3.PaddingBottom = 5f;
              //Cellc3.PaddingLeft=3f;
              //Cel3h1.BorderWidthLeft = 0.0f;
              Cellc3.BorderWidthRight = 0.0f;
              if(count>1)
                Cellc3.BorderWidthBottom = 0.0f;
              if(c.sg=="B")
                Cellc3.AddElement(ch);
              table.AddCell(Cellc3);

              PdfPCell Cellc4 = new PdfPCell(new Phrase(0f,""));
              Cellc4.Colspan = 1;
              Cellc4.HorizontalAlignment = 1;
              Cellc4.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc4.BorderColor = borderColor;
              Cellc4.BorderWidth = borderwidth;
              //Cel4c2.BorderWidthTop = 0.0f;
              //Cellc4.PaddingBottom = 5f;
              //Cellc4.PaddingLeft=3f;
              //Cel4h1.BorderWidthLeft = 0.0f;
              Cellc4.BorderWidthRight = 0.0f;
              if(count>1)
                Cellc4.BorderWidthBottom = 0.0f;
              if(c.sg=="C")
                Cellc4.AddElement(ch);
              table.AddCell(Cellc4);

              PdfPCell Cellc5 = new PdfPCell(new Phrase(0f, ""));
              Cellc5.Colspan = 1;
              Cellc5.HorizontalAlignment = 1;
              Cellc5.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc5.BorderColor = borderColor;
              Cellc5.BorderWidth = borderwidth;
              //Cel5c2.BorderWidthTop = 0.0f;
              //Cellc5.PaddingBottom = 5f;
              //Cellc5.PaddingLeft=3f;
              //Cel5h1.BorderWidthLeft = 0.0f;
              Cellc5.BorderWidthRight = 0.0f;
              if(count>1)
                Cellc5.BorderWidthBottom = 0.0f;
              if(c.sg=="D")
                Cellc5.AddElement(ch);
              table.AddCell(Cellc5);

              PdfPCell Cellc6 = new PdfPCell(new Phrase(0f, c.residential, new Font(Font.TIMES_ROMAN, 11f,0, borderColor)));
              Cellc6.Colspan = 4;
              Cellc6.HorizontalAlignment = 0;
              Cellc6.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc6.BorderColor = borderColor;
              Cellc6.BorderWidth = borderwidth;
              //Cel6c2.BorderWidthTop = 0.0f;
              //Cel6c5.PaddingBottom = 5f;
              //Cel6c5.PaddingLeft=3f;
              //Cel6h1.BorderWidthLeft = 0.0f;
              Cellc6.BorderWidthRight = 0.0f;
              if(count>1)
                Cellc6.BorderWidthBottom = 0.0f;
              table.AddCell(Cellc6);

              PdfPCell Cellc7 = new PdfPCell(new Phrase(0f, ""));
              Cellc7.Colspan = 1;
              Cellc7.HorizontalAlignment = 1;
              Cellc7.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc7.BorderColor = borderColor;
              Cellc7.BorderWidth = borderwidth;
              //Cel7c2.BorderWidthTop = 0.0f;
              //Cel7c5.PaddingBottom = 5f;
              //Cel7c5.PaddingLeft=3f;
              //Cel7h1.BorderWidthLeft = 0.0f;
              Cellc7.BorderWidthRight = 0.0f;
              if(count>1)
                Cellc7.BorderWidthBottom = 0.0f;
              if(c.rg=="A")
                Cellc7.AddElement(ch);
              table.AddCell(Cellc7);

              PdfPCell Cellc8 = new PdfPCell(new Phrase(0f, ""));
              Cellc8.Colspan = 1;
              Cellc8.HorizontalAlignment = 1;
              Cellc8.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc8.BorderColor = borderColor;
              Cellc8.BorderWidth = borderwidth;
              //Cel8c2.BorderWidthTop = 0.0f;
              //Cel8c5.PaddingBottom = 5f;
              //Cel8c5.PaddingLeft=3f;
              //Cel8h1.BorderWidthLeft = 0.0f;
              Cellc8.BorderWidthRight = 0.0f;
              if(count>1)
                Cellc8.BorderWidthBottom = 0.0f;
              if(c.rg=="B")
                Cellc8.AddElement(ch);
              table.AddCell(Cellc8);

              PdfPCell Cellc9 = new PdfPCell(new Phrase(0f, ""));
              Cellc9.Colspan = 1;
              Cellc9.HorizontalAlignment = 1;
              Cellc9.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc9.BorderColor = borderColor;
              Cellc9.BorderWidth = borderwidth;
              //Cel9c2.BorderWidthTop = 0.0f;
              //Cel9c5.PaddingBottom = 5f;
              //Cel9c5.PaddingLeft=3f;
              //Cel9h1.BorderWidthLeft = 0.0f;
              Cellc9.BorderWidthRight = 0.0f;
              if(count>1)              
                Cellc9.BorderWidthBottom = 0.0f;
              if(c.rg=="C")
                Cellc9.AddElement(ch);
              table.AddCell(Cellc9);

              PdfPCell Cellc10 = new PdfPCell(new Phrase(0f, ""));
              Cellc10.Colspan = 1;
              Cellc10.HorizontalAlignment = 1;
              Cellc10.VerticalAlignment = Element.ALIGN_CENTER;
              Cellc10.BorderColor = borderColor;
              Cellc10.BorderWidth = borderwidth;
              //Cel10Cellc10c2.BorderWidthTop = 0.0f;
              //Cel10Cellc10c5.PaddingBottom = 5f;
              //Cel10Cellc10c5.PaddingLeft=3f;
              //Cel10Cellc10h1.BorderWidthLeft = 0.0f;
              //Cellc10.BorderWidthRight = 0.0f;
              if(count>1)              
                Cellc10.BorderWidthBottom = 0.0f;
              if(c.rg=="D")
                Cellc10.AddElement(ch);
              table.AddCell(Cellc10);

              count--;
            }

            table.AddCell(Cell3); //space

            PdfPCell Cell4 = new PdfPCell(new Phrase(33f, "A = EXCELLENT, B = VERY GOOD, C = FAIR, D = NEEDS IMPROVEMENT", new Font(Font.TIMES_ROMAN, 10f,0, borderColor)));
            Cell4.Colspan = 16;
            Cell4.HorizontalAlignment = 1;
            Cell4.VerticalAlignment = 1;
            //Ce4l1.Rowspan=5;
            //Ce4l1.BorderColor = borderColor;
            //Ce4l1.BorderWidth = borderwidth;
            Cell4.BorderWidthTop = 0.0f;
            Cell4.BorderWidthLeft = 0.0f;
            Cell4.BorderWidthRight = 0.0f;
            Cell4.BorderWidthBottom = 0.0f;
            table.AddCell(Cell4);

            table.AddCell(Cell3); //space

            PdfPCell Cell5 = new PdfPCell(new Phrase(33f, " ", new Font(Font.TIMES_ROMAN, 17f,0, borderColor)));
            Cell5.Colspan = 1;
            Cell5.HorizontalAlignment = 1;
            Cell5.VerticalAlignment = 1;
            //Ce5l1.Rowspan=5;
            //Ce5l1.BorderColor = borderColor;
            //Ce5l1.BorderWidth = borderwidth;
            Cell5.BorderWidthTop = 0.0f;
            Cell5.BorderWidthLeft = 0.0f;
            Cell5.BorderWidthRight = 0.0f;
            Cell5.BorderWidthBottom = 0.0f;
            table.AddCell(Cell5);

            PdfPCell Cell6 = new PdfPCell(new Phrase(33f, "_____________________", new Font(Font.TIMES_ROMAN, 17f,0, borderColor)));
            Cell6.Colspan = 6;
            Cell6.HorizontalAlignment = 1;
            Cell6.VerticalAlignment = 1;
            //Ce6l1.Rowspan=5;
            //Ce6l1.BorderColor = borderColor;
            //Ce6l1.BorderWidth = borderwidth;
            Cell6.BorderWidthTop = 0.0f;
            Cell6.BorderWidthLeft = 0.0f;
            Cell6.BorderWidthRight = 0.0f;
            Cell6.BorderWidthBottom = 0.0f;
            table.AddCell(Cell6);

            PdfPCell Cell7 = new PdfPCell(new Phrase(33f, " ", new Font(Font.TIMES_ROMAN, 17f,0, borderColor)));
            Cell7.Colspan = 2;
            Cell7.HorizontalAlignment = 1;
            Cell7.VerticalAlignment = 1;
            //Ce7l1.Rowspan=5;
            //Ce7l1.BorderColor = borderColor;
            //Ce7l1.BorderWidth = borderwidth;
            Cell7.BorderWidthTop = 0.0f;
            Cell7.BorderWidthLeft = 0.0f;
            Cell7.BorderWidthRight = 0.0f;
            Cell7.BorderWidthBottom = 0.0f;
            table.AddCell(Cell7);

            PdfPCell Cell8 = new PdfPCell(new Phrase(33f, "_____________________", new Font(Font.TIMES_ROMAN, 17f,0, borderColor)));
            Cell8.Colspan = 6;
            Cell8.HorizontalAlignment = 1;
            Cell8.VerticalAlignment = 1;
            //Ce8l1.Rowspan=5;
            //Ce8l1.BorderColor = borderColor;
            //Ce8l1.BorderWidth = borderwidth;
            Cell8.BorderWidthTop = 0.0f;
            Cell8.BorderWidthLeft = 0.0f;
            Cell8.BorderWidthRight = 0.0f;
            Cell8.BorderWidthBottom = 0.0f;
            table.AddCell(Cell8);

            PdfPCell Cell9 = new PdfPCell(new Phrase(33f, " ", new Font(Font.TIMES_ROMAN, 17f,0, borderColor)));
            Cell9.Colspan = 1;
            Cell9.HorizontalAlignment = 1;
            Cell9.VerticalAlignment = 1;
            //Ce9l1.Rowspan=5;
            //Ce9l1.BorderColor = borderColor;
            //Ce9l1.BorderWidth = borderwidth;
            Cell9.BorderWidthTop = 0.0f;
            Cell9.BorderWidthLeft = 0.0f;
            Cell9.BorderWidthRight = 0.0f;
            Cell9.BorderWidthBottom = 0.0f;
            table.AddCell(Cell9);

            PdfPCell Cell5t = new PdfPCell(new Phrase(33f, " ", new Font(Font.TIMES_ROMAN, 12f,0, borderColor)));
            Cell5t.Colspan = 1;
            Cell5t.HorizontalAlignment = 1;
            Cell5t.VerticalAlignment = 1;
            //Ce5tl1.Rowspan=5;
            //Ce5tl1.BorderColor = borderColor;
            //Ce5tl1.BorderWidth = borderwidth;
            Cell5t.BorderWidthTop = 0.0f;
            Cell5t.BorderWidthLeft = 0.0f;
            Cell5t.BorderWidthRight = 0.0f;
            Cell5t.BorderWidthBottom = 0.0f;
            table.AddCell(Cell5t);

            PdfPCell Cell6t = new PdfPCell(new Phrase(33f, "Name of Class Teacher", new Font(Font.TIMES_ROMAN, 12f,0, borderColor)));
            Cell6t.Colspan = 6;
            Cell6t.HorizontalAlignment = 1;
            Cell6t.VerticalAlignment = 1;
            //Ce6tl1.Rowspan=5;
            //Ce6tl1.BorderColor = borderColor;
            //Ce6tl1.BorderWidth = borderwidth;
            Cell6t.BorderWidthTop = 0.0f;
            Cell6t.BorderWidthLeft = 0.0f;
            Cell6t.BorderWidthRight = 0.0f;
            Cell6t.BorderWidthBottom = 0.0f;
            table.AddCell(Cell6t);

            PdfPCell Cell7t = new PdfPCell(new Phrase(33f, " ", new Font(Font.TIMES_ROMAN, 12f,0, borderColor)));
            Cell7t.Colspan = 2;
            Cell7t.HorizontalAlignment = 1;
            Cell7t.VerticalAlignment = 1;
            //Ce7tl1.Rowspan=5;
            //Ce7tl1.BorderColor = borderColor;
            //Ce7tl1.BorderWidth = borderwidth;
            Cell7t.BorderWidthTop = 0.0f;
            Cell7t.BorderWidthLeft = 0.0f;
            Cell7t.BorderWidthRight = 0.0f;
            Cell7t.BorderWidthBottom = 0.0f;
            table.AddCell(Cell7t);

            PdfPCell Cell8t = new PdfPCell(new Phrase(33f, "Signature of the  Class Teacher", new Font(Font.TIMES_ROMAN, 12f,0, borderColor)));
            Cell8t.Colspan = 6;
            Cell8t.HorizontalAlignment = 1;
            Cell8t.VerticalAlignment = 1;
            //Ce8tl1.Rowspan=5;
            //Ce8tl1.BorderColor = borderColor;
            //Ce8tl1.BorderWidth = borderwidth;
            Cell8t.BorderWidthTop = 0.0f;
            Cell8t.BorderWidthLeft = 0.0f;
            Cell8t.BorderWidthRight = 0.0f;
            Cell8t.BorderWidthBottom = 0.0f;
            table.AddCell(Cell8t);

            PdfPCell Cell9t = new PdfPCell(new Phrase(33f, " ", new Font(Font.TIMES_ROMAN, 12f,0, borderColor)));
            Cell9t.Colspan = 1;
            Cell9t.HorizontalAlignment = 1;
            Cell9t.VerticalAlignment = 1;
            //Ce9tl1.Rowspan=5;
            //Ce9tl1.BorderColor = borderColor;
            //Ce9tl1.BorderWidth = borderwidth;
            Cell9t.BorderWidthTop = 0.0f;
            Cell9t.BorderWidthLeft = 0.0f;
            Cell9t.BorderWidthRight = 0.0f;
            Cell9t.BorderWidthBottom = 0.0f;
            table.AddCell(Cell9t);

            table.AddCell(Cell3); //spaces
            table.AddCell(Cell3);

            string p = string.Concat(model.promoted,string.IsNullOrEmpty(model.notready)?" ":string.Concat(" ",model.notready));
            PdfPCell Cell10 = new PdfPCell(new Phrase(33f, p, new Font(Font.TIMES_ROMAN, 13f,0, borderColor)));
            Cell10.Colspan = 16;
            Cell10.HorizontalAlignment = 1;
            Cell10.VerticalAlignment = 1;
            //Ce10l1.Rowspan=5;
            //Ce10l1.BorderColor = borderColor;
            //Ce10l1.BorderWidth = borderwidth;
            Cell10.BorderWidthTop = 0.0f;
            Cell10.BorderWidthLeft = 0.0f;
            Cell10.BorderWidthRight = 0.0f;
            Cell10.BorderWidthBottom = 0.0f;
            table.AddCell(Cell10);

            table.AddCell(Cell3); //spaces
            table.AddCell(Cell3);

            PdfPCell Cell11 = new PdfPCell(new Phrase(33f, "_____________________", new Font(Font.TIMES_ROMAN, 17f,0, borderColor)));
            Cell11.Colspan = 16;
            Cell11.HorizontalAlignment = 1;
            Cell11.VerticalAlignment = 1;
            //Ce11l1.Rowspan=5;
            //Ce11l1.BorderColor = borderColor;
            //Ce11l1.BorderWidth = borderwidth;
            Cell11.BorderWidthTop = 0.0f;
            Cell11.BorderWidthLeft = 0.0f;
            Cell11.BorderWidthRight = 0.0f;
            Cell11.BorderWidthBottom = 0.0f;
            table.AddCell(Cell11);

             PdfPCell Cell12 = new PdfPCell(new Phrase(33f, "Signature of the Principal", new Font(Font.TIMES_ROMAN, 12f,0, borderColor)));
            Cell12.Colspan = 16;
            Cell12.HorizontalAlignment = 1;
            Cell12.VerticalAlignment = 1;
            //Ce12l1.Rowspan=5;
            //Ce12l1.BorderColor = borderColor;
            //Ce12l1.BorderWidth = borderwidth;
            Cell12.BorderWidthTop = 0.0f;
            Cell12.BorderWidthLeft = 0.0f;
            Cell12.BorderWidthRight = 0.0f;
            Cell12.BorderWidthBottom = 0.0f;
            table.AddCell(Cell12);

            table.AddCell(Cell3); //spaces

            PdfPCell Cell13 = new PdfPCell(new Phrase(33f, "There is no correction or overwriting in this document", new Font(Font.TIMES_ROMAN, 12f,0, borderColor)));
            Cell13.Colspan = 16;
            Cell13.HorizontalAlignment = 1;
            Cell13.VerticalAlignment = 1;
            //Ce13l1.Rowspan=5;
            //Ce13l1.BorderColor = borderColor;
            //Ce13l1.BorderWidth = borderwidth;
            Cell13.BorderWidthTop = 0.0f;
            Cell13.BorderWidthLeft = 0.0f;
            Cell13.BorderWidthRight = 0.0f;
            Cell13.BorderWidthBottom = 0.0f;
            table.AddCell(Cell13);
          return table;
        }

        public async Task<StudentSlab> getStudentFromUserAsync(mpsUser u)
        {
            return await Task.FromResult<StudentSlab>( await _ctx.StudentSlabs.Include(p => p.Slab).Where(p => p.Student.StudentUserId == u.Id ).FirstOrDefaultAsync());
        }

        public async Task<Staff> getStaffFromUserAsync(mpsUser u)
        {
            return await Task.FromResult<Staff>( await _ctx.Staffs.Where(p => p.StaffUser.Id == u.Id).FirstOrDefaultAsync());
        }

        public string download(ImgDoc attachment, out string exep)
        {
          exep="";
            string path = String.Concat(_env.WebRootPath,@"\temp\",attachment.name,".",attachment.fileExtension);
            DirectoryInfo d = new DirectoryInfo(String.Concat(_env.WebRootPath,@"\temp"));
            FileInfo[] Files = d.GetFiles("*.*"); //Getting All files
            foreach(FileInfo f in Files )
            {
                if(!f.Extension.Equals("txt")){f.Delete();}
            }
            try{
              System.IO.File.WriteAllBytes(path, Convert.FromBase64String(attachment.content));
            }
            catch(Exception ex){
              exep=ex.Message;
            }
            string url=String.Concat( "/temp/",attachment.name,".",attachment.fileExtension);
            return url;
        }
    }
}
