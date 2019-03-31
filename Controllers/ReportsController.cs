
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using OfficeOpenXml;
using portal.mps.Data;
using portal.mps.Data.Repository;
using portal.mps.Models.ViewModels;
using portal.mps.Services;

namespace portal.mps.Controllers
{
    [Authorize(Roles="Accountant,Admin,SuperAdmin")]
    public class ReportsController : Controller
    {
        private IReportsRepository _repo;

        private ILogger<ReportsController> _logger;
        private IUtils _utils;
        private IHostingEnvironment _env;

        //private mpsUser _loggedinuser;

        public ReportsController(IReportsRepository repo,ILogger<ReportsController> logger,IUtils utils, IHostingEnvironment env)
        {
            _repo = repo;
            _logger = logger;
            _utils = utils;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region students
        public IActionResult Students()
        {
            return View();
        }
        public JsonResult LoadStudents()
        {
            var s = _repo.getStudents();
            return Json(s);
        }
        [HttpPost]
        public JsonResult ExportStudents(string T, IEnumerable<StudentReport> data)
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = string.Concat(T,DateTime.Now.ToString("dd-MM-yyyy_hhmm"),".xls");
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            DirectoryInfo d = new DirectoryInfo(sWebRootFolder);
            FileInfo[] Files = d.GetFiles("*.xls"); //Getting Excel files
            foreach(FileInfo f in Files )
            {
                if(f.Exists){f.Delete();}
            }
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Students");
                worksheet.DefaultColWidth = 20;
                //First add the headers
                worksheet.Cells[1, 1].Value = "Active";
                worksheet.Cells[1, 2].Value = "Admission Date";
                worksheet.Cells[1, 3].Value = "Name";
                worksheet.Cells[1, 4].Value = "Class";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Phone";
                worksheet.Cells[1, 7].Value = "DOB";
                worksheet.Cells[1, 8].Value = "Email";
                worksheet.Cells[1, 9].Value = "mps Username";
                worksheet.Cells[1, 10].Value = "Slab";
                worksheet.Cells[1, 11].Value = "Fees";
                worksheet.Cells[1, 12].Value = "Academic year";
                worksheet.Cells[1, 13].Value = "Created by";
                
                int cell=2;
                foreach(var v in data){
                    worksheet.Cells[cell, 1].Value = v.isActive;
                    worksheet.Cells[cell, 2].Value = v.AdmissionDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 3].Value = string.Concat(v.FirstName," ",v.LastName);
                    worksheet.Cells[cell, 4].Value = v.Grade;
                    worksheet.Cells[cell, 5].Value = v.Address1;
                    worksheet.Cells[cell, 6].Value = v.Phone;
                    worksheet.Cells[cell, 7].Value = v.DOB.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 8].Value = v.Email;
                    worksheet.Cells[cell, 9].Value = v.UserName;
                    worksheet.Cells[cell, 10].Value = v.SlabName;
                    worksheet.Cells[cell, 11].Value = v.TotalFees;
                    worksheet.Cells[cell, 12].Value = v.AcademicYear;
                    worksheet.Cells[cell, 13].Value = v.StudentCreatedBy;
                    cell++;
                }
                package.Save(); //Save the workbook.
            }
            return Json(URL);
        }
        #endregion
        
        #region student academics
         public IActionResult StudentAcademics()
        {
            return View();
        }
        public JsonResult LoadStudentAcademics()
        {
            var s = _repo.getStudentAcademics();
            return Json(s);
        }
        #endregion
        
        #region studentfees
        public IActionResult StudentFee()
        {
            return View();
        }
        public JsonResult LoadStudentFee()
        {
            IEnumerable<StudentFeeReport> s = _repo.getStudentFee();
            return Json(s);
        }
        public JsonResult LoadStudentFeeDetail(int studentslabid)
        {
            IEnumerable<StudentFeeDetailReport> s = _repo.getStudentFeeDetail(studentslabid);
            return  Json(s);
        }
        public JsonResult LoadStudentFeeExpenseDetail(int studentslabid)
        {
            IEnumerable<StudentFeeExpenseDetailReport> s = _repo.getStudentFeeExpenseDetail(studentslabid);
            return  Json(s);
        }
        [HttpPost]
        public JsonResult ExportStudentFees(string T, IEnumerable<StudentFeeReport> data)
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = string.Concat(T,DateTime.Now.ToString("dd-MM-yyyy_hhmm"),".xls");
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            DirectoryInfo d = new DirectoryInfo(sWebRootFolder);
            FileInfo[] Files = d.GetFiles("*.xls"); //Getting Excel files
            foreach(FileInfo f in Files )
            {
                if(f.Exists){f.Delete();}
            }
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("StudentFees");
                worksheet.DefaultColWidth = 20;
                //First add the headers
                worksheet.Cells[1, 1].Value = "Active";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "RollNumber";
                worksheet.Cells[1, 4].Value = "Class";
                worksheet.Cells[1, 5].Value = "Slab";
                worksheet.Cells[1, 6].Value = "Academic year";
                worksheet.Cells[1, 7].Value = "FeeStartDate";
                worksheet.Cells[1, 8].Value = "FeeEndDate";
                worksheet.Cells[1, 9].Value = "Fees";
                worksheet.Cells[1, 10].Value = "Total Fee";
                worksheet.Cells[1, 11].Value = "Expense Amount";
                worksheet.Cells[1, 12].Value = "Paid Fees";
                
                int cell=2;
                foreach(var v in data){
                    worksheet.Cells[cell, 1].Value = v.isActive;
                    worksheet.Cells[cell, 2].Value = v.Name;
                    worksheet.Cells[cell, 3].Value = v.RollNumber;
                    worksheet.Cells[cell, 4].Value = v.Grade;
                    worksheet.Cells[cell, 5].Value = v.SlabName;
                    worksheet.Cells[cell, 6].Value = v.AcademicYear;
                    worksheet.Cells[cell, 7].Value = v.FeesStartDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 8].Value = v.FeesEndDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 9].Value = v.Fees;
                    worksheet.Cells[cell, 10].Value = v.TotalFees;
                    worksheet.Cells[cell, 11].Value = v.ExpenseAmount;
                    worksheet.Cells[cell, 12].Value = v.PaidFees;
                    cell++;
                }
                package.Save(); //Save the workbook.
            }
            return Json(URL);
        }
        #endregion

        #region staff
        public IActionResult Staff()
        {
            return View();
        }
        public JsonResult LoadStaff()
        {
            var s = _repo.getStaff();
            return Json(s);
        }

        [HttpPost]
        public JsonResult ExportStaff(string T, IEnumerable<StaffReport> data)
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = string.Concat(T,DateTime.Now.ToString("dd-MM-yyyy_hhmm"),".xls");
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            DirectoryInfo d = new DirectoryInfo(sWebRootFolder);
            FileInfo[] Files = d.GetFiles("*.xls"); //Getting Excel files
            foreach(FileInfo f in Files )
            {
                if(f.Exists){f.Delete();}
            }
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Staff");
                worksheet.DefaultColWidth = 20;
                //First add the headers
                worksheet.Cells[1, 1].Value = "Active";
                worksheet.Cells[1, 2].Value = "Teacher";
                worksheet.Cells[1, 3].Value = "Joining Date";
                worksheet.Cells[1, 4].Value = "Name";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Phone";
                worksheet.Cells[1, 7].Value = "DOB";
                worksheet.Cells[1, 8].Value = "Email";
                worksheet.Cells[1, 9].Value = "mps Username";
                worksheet.Cells[1, 10].Value = "CreatedBy";
                
                int cell=2;
                foreach(var v in data){
                    worksheet.Cells[cell, 1].Value = v.isActive;
                    worksheet.Cells[cell, 2].Value = v.isTeacher;
                    worksheet.Cells[cell, 3].Value = v.JoiningDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 4].Value = string.Concat(v.FirstName," ",v.LastName);
                    worksheet.Cells[cell, 5].Value = v.Address1;
                    worksheet.Cells[cell, 6].Value = v.Phone;
                    worksheet.Cells[cell, 7].Value = v.DOB.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 8].Value = v.Email;
                    worksheet.Cells[cell, 9].Value = v.UserName;
                    worksheet.Cells[cell, 10].Value = v.StaffCreatedBy;
                    cell++;
                }
                package.Save(); //Save the workbook.
            }
            //var result = PhysicalFile(Path.Combine(sWebRootFolder, sFileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            // Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
            // {
            //     FileName = file.Name
            // }.ToString();

            return Json(URL);
        }
        #endregion

        #region staffSalary
        public IActionResult StaffSalary()
        {
            return View();
        }
        public JsonResult LoadStaffSalary()
        {
            var s = _repo.getStaffSalary();
            return Json(s);
        }
        [HttpPost]
        public JsonResult ExportStaffSalary(string T, IEnumerable<StaffSalaryReport> data)
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = string.Concat(T,DateTime.Now.ToString("dd-MM-yyyy_hhmm"),".xls");
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            DirectoryInfo d = new DirectoryInfo(sWebRootFolder);
            FileInfo[] Files = d.GetFiles("*.xls"); //Getting Excel files
            foreach(FileInfo f in Files )
            {
                if(f.Exists){f.Delete();}
            }
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("StaffSalary");
                worksheet.DefaultColWidth = 20;
                //First add the headers
                worksheet.Cells[1, 1].Value = "Joining Date";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Salary Set Date";
                worksheet.Cells[1, 4].Value = "Salary (in Rs)";
                worksheet.Cells[1, 5].Value = "Paid on";
                worksheet.Cells[1, 6].Value = "Amount Paid";
                
                int cell=2;
                foreach(var v in data){
                    worksheet.Cells[cell, 1].Value = v.JoiningDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 2].Value = string.Concat(v.FirstName," ",v.LastName);
                    worksheet.Cells[cell, 3].Value = v.SalarySetDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 4].Value = v.Salary;
                    worksheet.Cells[cell, 5].Value = v.PaidDate;
                    worksheet.Cells[cell, 6].Value = v.SalaryAmountPaid;
                    cell++;
                }
                package.Save(); //Save the workbook.
            }
            return Json(URL);
        }
        #endregion
        
        #region expenses
        public IActionResult Expenses()
        {
            return View();
        }
        public IActionResult LoadExpenses()
        {
            var x = _repo.getExpenses();
            return Json(x);
        }
        [HttpPost]
        public JsonResult ExportExpenses(string T, IEnumerable<ExpenseReport> data)
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = string.Concat(T,DateTime.Now.ToString("dd-MM-yyyy_hhmm"),".xls");
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            DirectoryInfo d = new DirectoryInfo(sWebRootFolder);
            FileInfo[] Files = d.GetFiles("*.xls"); //Getting Excel files
            foreach(FileInfo f in Files )
            {
                if(f.Exists){f.Delete();}
            }
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Expenses");
                worksheet.DefaultColWidth = 20;
                //First add the headers
                worksheet.Cells[1, 1].Value = "Expense Name";
                worksheet.Cells[1, 2].Value = "Expense Description";
                worksheet.Cells[1, 3].Value = "Expense Date";
                worksheet.Cells[1, 4].Value = "Linked To (student)";
                
                int cell=2;
                foreach(var v in data){
                    worksheet.Cells[cell, 1].Value = v.ExpenseName;
                    worksheet.Cells[cell, 2].Value = v.ExpenseDescription;
                    worksheet.Cells[cell, 3].Value = v.ExpenseDate.ToString("dd-MM-yyyy");
                    worksheet.Cells[cell, 4].Value = v.ExpenseLinkedTo;
                    cell++;
                }
                package.Save(); //Save the workbook.
            }
            return Json(URL);
        }

        #endregion
        
        public IActionResult ExpensesDashBoard()
        {
            return View();
        }
        public JsonResult getExpensesForDashBoard(int year)
        {
            var x = _repo.getExpensesByYearDashBoard(year);
            var y = _repo.getPaymentsByYearDashBoard(year);
            return Json(new{expenses = x, fees = y});
        }
        public JsonResult getMonthlyExpensesForDashBoard(int year,int month)
        {
            var x = _repo.getMonthlyExpensesDashBoard(year,month);
            var y = _repo.getMonthlyPaymentsDashBoard(year,month);
            return Json(new{expenses = x, fees = y});
        }
        public JsonResult getConsolidatedDashBoard(int year){
            decimal[] x = _repo.getExpensesAndPaymentsByYearDashBoard(year);
            return Json(new{expenses = x[0], fees = x[1]});
        }
        public JsonResult getConsolidatedMonthlyDashBoard(int year, int month){
            decimal[] x = _repo.getExpensesAndPaymentsByMonthlyDashBoard(year,month);
            return Json(new{expenses = x[0], fees = x[1]});
        }
    }
}