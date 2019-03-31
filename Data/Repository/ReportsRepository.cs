using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using portal.mps.Models;
using portal.mps.Models.ViewModels;
using portal.mps.Services;

namespace portal.mps.Data.Repository
{
    public class ReportsRepository : IReportsRepository
    {
        private mpsContext _ctx;
        private ILogger _logger;
        private IUtils _utils;

        public ReportsRepository(mpsContext ctx, ILogger<ReportsRepository> logger, IUtils utils)
        {
            _ctx =ctx;
            _logger = logger;
            _utils = utils;
        }

        public IEnumerable<ExpenseReport> getExpenses()
        {
            var x = _ctx.Expenses
                .Include(e => e.ExpenseHeader)
                .ToList();
            var expenseList = new List<ExpenseReport>();
            foreach (var expense in x)
            {
                string name="";
                if(expense.StudentSlabLinkedId>0)
                {
                    var n=_ctx.StudentSlabs.Include(s => s.Student).ThenInclude(i => i.StudentUser).Where(s => s.Id == expense.StudentSlabLinkedId).Single();
                    name = string.Concat(n.Student.StudentUser.FirstName," ",n.Student.StudentUser.LastName);
                }
                var expenseVM = new ExpenseReport
                {
                    ExpenseId = expense.Id,
                    ExpenseName = expense.ExpenseHeader.ExpenseHeaderName,
                    ExpenseDate = expense.ExpenseDate,
                    ExpenseDescription = expense.ExpenseDesc,
                    ExpenseLinkedTo = name                    
                };
                expenseList.Add(expenseVM);
            }
            return expenseList;
        }
        public IEnumerable getExpensesByYearDashBoard(int year)
        {
            var query = _ctx.Expenses.Include(e => e.ExpenseHeader)
            .Where(a => a.ExpenseDate.Year == year)
            .GroupBy(a => a.ExpenseDate.Month)
            .ToList();
            return query;
        }
        public IEnumerable getPaymentsByYearDashBoard(int year)
        {
            var query = _ctx.StudentFees
            .Where(a => a.PaidDate.Value.Year == year && a.PaidFees>0)
            .GroupBy(a => a.PaidDate.Value.Month)
            .ToList();
            return query;
        }


        public IEnumerable getMonthlyExpensesDashBoard(int year,int month)
        {
            var query = _ctx.Expenses
            .Where(a => a.ExpenseDate.Month == month && a.ExpenseDate.Year == year)
            .GroupBy(a => a.ExpenseDate.Day)
            .ToList();
            return query;
        }
        public IEnumerable getMonthlyPaymentsDashBoard(int year,int month)
        {
            var query = _ctx.StudentFees
            .Where(a => a.PaidDate.Value.Month == month && a.PaidDate.Value.Year == year && a.PaidFees>0)
            .GroupBy(a => a.PaidDate.Value.Day)
            .ToList();
            return query;
        }

        public decimal[] getExpensesAndPaymentsByYearDashBoard(int year)
        {
            var ex = _ctx.Expenses.Where(a => a.ExpenseDate.Year == year).ToList();
            decimal v1 = ex.Sum(p => p.ExpenseAmount);
            var sf = _ctx.StudentFees
            .Where(a => a.PaidDate.Value.Year == year && a.PaidFees>0).ToList();
            decimal v2 = sf.Sum(p => p.PaidFees);
            decimal[] d = {v1,v2};
            return d;
        }

        public decimal[] getExpensesAndPaymentsByMonthlyDashBoard(int year, int month)
        {
            var ex = _ctx.Expenses.Where(a => a.ExpenseDate.Year == year && a.ExpenseDate.Month == month).ToList();
            decimal v1 = ex.Sum(p => p.ExpenseAmount);
            var sf = _ctx.StudentFees
            .Where(a => a.PaidDate.Value.Year == year && a.PaidDate.Value.Month == month &&a.PaidFees>0).ToList();
            decimal v2 = sf.Sum(p => p.PaidFees);
            decimal[] d = {v1,v2};
            return d;
        }
        public IEnumerable<StaffReport> getStaff()
        {
            var s = _ctx.Staffs
               .Include(a => a.StaffUser)
               .Include(i => i.CreatedBy)
               .ToList();
            var staffList = new List<StaffReport>();

            foreach (var staff in s)
            {
                var staffVM = new StaffReport
                {
                    isActive = staff.isActive,
                    isTeacher = staff.isTeacher,
                    StaffId = staff.StaffId,
                    Address1 = staff.StaffUser.Address1,
                    Phone = staff.StaffUser.PhoneNumber,
                    FirstName = staff.StaffUser.FirstName,
                    MiddleName = staff.StaffUser.MiddleName,
                    LastName = staff.StaffUser.LastName,
                    DOB = staff.StaffUser.DOB,
                    Email = staff.StaffUser.Email,
                    UserName = staff.StaffUser.UserName,
                    JoiningDate = staff.JoiningDate,
                    StaffCreatedBy = staff.CreatedBy.UserName
                };
                staffList.Add(staffVM);
            }
            return staffList;
        }

        public IEnumerable<StaffSalaryReport> getStaffSalary()
        {
            //var staffs = _ctx.StaffSalarys.Include(s => s.Staff).ThenInclude(s => s.StaffUser).ToList();
            var staffSalaryList =  from s in _ctx.StaffSalarys
                        join sp in _ctx.StaffPayments on s.StaffId equals sp.StaffId
                        select new StaffSalaryReport
                        {
                            StaffId = s.StaffId,
                            isActive = s.Staff.isActive,
                            FirstName = s.Staff.StaffUser.FirstName,
                            MiddleName = s.Staff.StaffUser.MiddleName,
                            LastName = s.Staff.StaffUser.LastName,
                            JoiningDate = s.Staff.JoiningDate,
                            SalarySetDate = s.SalarySetDate,
                            Salary = s.Salary,
                            PaidDate = sp.PaidDate==null?"":sp.PaidDate.ToString("dd-MM-yyyy"),
                            SalaryAmountPaid = sp.AmountPaid,
                        };
            //var staffPayments = _ctx.StaffPayments.ToList();
            //var staffSalaryList = new List<StaffSalaryReport>();
            // foreach(var s in staffs){
            //     string pd ="";
            //     decimal ap=0;
            //     foreach(var sp in staffPayments)
            //     {
            //         if(s.StaffId == sp.StaffId){
            //             pd = sp.PaidDate==null?"":sp.PaidDate.ToString("dd-MM-yyyy");
            //             ap =sp.AmountPaid;
            //             var staffVM = new StaffSalaryReport{
            //                 StaffId = s.StaffId,
            //                 isActive = s.Staff.isActive,
            //                 FirstName = s.Staff.StaffUser.FirstName,
            //                 MiddleName = s.Staff.StaffUser.MiddleName,
            //                 LastName = s.Staff.StaffUser.LastName,
            //                 JoiningDate = s.Staff.JoiningDate,
            //                 SalarySetDate = s.SalarySetDate,
            //                 Salary = s.Salary,
            //                 PaidDate = pd,
            //                 SalaryAmountPaid = ap,
            //             };
            //             staffSalaryList.Add(staffVM);
            //         }
                    
            //     }
                
            // }
            return staffSalaryList;
        }

        public IEnumerable<StudentFeeReport> getStudentFee()
        {
            var s = _ctx.StudentSlabs
               .Include(a => a.Slab)
               .Include(a => a.Student).ThenInclude(a => a.StudentUser)
               
               //.Where(f => f.PaidFees==0)
               .ToList();
            // var query = from c in _ctx.StudentFees
            //             join ssl in _ctx.StudentSlabs on  c.StudentSlabId equals ssl.Id
            //             join sl in _ctx.Slabs on ssl.SlabId equals sl.Id
            //             join st in _ctx.Students on ssl.StudentId equals st.StudentId
            //             group c by c.StudentSlabId into g
            //             select new
            //             { 
            //                 slabid = g.Single().StudentSlab.Id,
            //                 grade = g.Single().StudentSlab.Slab.Grade,
            //                 slabName = g.Single().StudentSlab.Slab.SlabName,
            //                 Sum = g.Sum(c => c.PaidFees)
            //             };
            var e = _ctx.Expenses
                    .Where(x => x.StudentSlabLinkedId>0)
                    .ToList();
            var f = _ctx.StudentFees.ToList();
            var studentList = new List<StudentFeeReport>();
            foreach (var studSlab in s)
            {
                var exp = from x in e
                            where x.StudentSlabLinkedId == studSlab.Id
                            group x by x.StudentSlabLinkedId into g
                            select new{
                                Sum = g.Sum(x => x.ExpenseAmount)
                            };
                decimal expAmount = 0;
                if(exp.Count()>0){
                    expAmount = exp.Single().Sum;
                }
                var paidsum = from x in f
                                where x.StudentSlabId == studSlab.Id
                                group x by x.StudentSlabId into g
                                select new{
                                    Sum = g.Sum(x => x.PaidFees)
                                };
                var studentVM = new StudentFeeReport
                {
                    Name = string.Concat(studSlab.Student.StudentUser.FirstName," ",studSlab.Student.StudentUser.LastName),
                    RollNumber = studSlab.Student.RollNumber,
                    isActive = studSlab.Student.isActive,
                    SlabName = studSlab.Slab.SlabName,
                    Grade = studSlab.Slab.Grade,
                    StudentSlabId = studSlab.Id,
                    Fees = studSlab.Slab.TotalFee,
                    TotalFees = studSlab.Slab.TotalFee + expAmount,
                    AcademicYear = studSlab.AcademicYear,
                    //PaidDate = student.PaidDate,
                    PaidFees = paidsum.Single().Sum,
                    FeesStartDate = studSlab.FeesStartDate,
                    FeesEndDate = studSlab.FeesEndDate,
                    //Expenses = _ctx.Expenses.Include("ExpenseHeader").Where(e => e.StudentSlabLinkedId == student.StudentSlab.Id).ToList()
                    ExpenseAmount = expAmount
                };
                studentList.Add(studentVM);
            }
            return studentList;
        }

        public IEnumerable<StudentFeeDetailReport> getStudentFeeDetail(int studentslabid)
        {
            var f = _ctx.StudentFees
            .Where(c => c.StudentSlabId == studentslabid)
            .ToList();
            var studentList = new List<StudentFeeDetailReport>();
            foreach (var student in f)
            {
                // mpsUser u = student.Student.StudentUser;
                var studentVM = new StudentFeeDetailReport
                {
                    PaidFees = student.PaidFees,
                    PaidDate = student.PaidDate
                };
                if(studentVM.PaidFees>0)
                    studentList.Add(studentVM);
            }
            return studentList;
        }

        public IEnumerable<StudentFeeExpenseDetailReport> getStudentFeeExpenseDetail(int studentslabid)
        {
            var f = _ctx.Expenses.Include(e => e.ExpenseHeader)
            .Where(c => c.StudentSlabLinkedId == studentslabid)
            .ToList();
            var studentList = new List<StudentFeeExpenseDetailReport>();
            foreach (var e in f)
            {
                // mpsUser u = student.Student.StudentUser;
                var studentVM = new StudentFeeExpenseDetailReport
                {
                    ExpenseName = e.ExpenseHeader.ExpenseHeaderName,
                    ExpenseDate = e.ExpenseDate,
                    ExpenseAmount = e.ExpenseAmount,
                    ExpenseDesc = e.ExpenseDesc
                };
                studentList.Add(studentVM);
            }
            return studentList;
        }

        public IEnumerable<StudentReport> getStudents()
        {
            var s = _ctx.StudentSlabs
                .Include(a => a.Slab)
                .Include(z => z.Student.CreatedBy)
                .Include(a => a.Student).ThenInclude(i => i.StudentUser)
                .ToList();
            var studentList = new List<StudentReport>();
            
            foreach (var student in s)
            {
                // mpsUser u = student.Student.StudentUser;
                var studentVM = new StudentReport
                {
                    isActive = student.Student.isActive,
                    StudentId = student.Student.StudentId,
                    Address1 = student.Student.StudentUser.Address1,
                    Address2 = student.Student.StudentUser.Address2,
                    FirstName = student.Student.StudentUser.FirstName,
                    MiddleName = student.Student.StudentUser.MiddleName,
                    LastName = student.Student.StudentUser.LastName,
                    DOB = student.Student.StudentUser.DOB,
                    Email = student.Student.StudentUser.Email,
                    UserName = student.Student.StudentUser.UserName,
                    TotalFees = student.Slab.TotalFee,
                    AdmissionDate = student.Student.AdmissionDate,
                    // TotalPaidFees = student.TotalPaidFees,
                   // PaidDate = student.PaidDate.Value.ToShortDateString(),
                    // FeesStartDate = student.FeesStartDate.ToShortDateString(),
                    // FeesEndDate = student.FeesEndDate.ToShortDateString(),
                    AcademicYear = student.AcademicYear,
                    SlabName = student.Slab.SlabName,
                    Grade = student.Slab.Grade,
                    Phone = student.Student.StudentUser.PhoneNumber,
                    StudentCreatedBy = student.CreatedBy.UserName
                };
                studentList.Add(studentVM);
            }
            return studentList;
        }

        public IEnumerable<StudentAcademicsReport> getStudentAcademics()
        {
            throw new NotImplementedException();
        }
    }
}