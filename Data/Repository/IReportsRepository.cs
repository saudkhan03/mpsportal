using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using portal.mps.Models;
using portal.mps.Models.ViewModels;

namespace portal.mps.Data.Repository
{
    public interface IReportsRepository
    {
        IEnumerable<StudentReport> getStudents();
        IEnumerable<StaffReport> getStaff();
        IEnumerable<ExpenseReport> getExpenses();
        IEnumerable<StudentFeeReport> getStudentFee();
        IEnumerable<StudentFeeDetailReport> getStudentFeeDetail(int studentslabid);
        IEnumerable<StudentFeeExpenseDetailReport> getStudentFeeExpenseDetail(int studentslabid);
        IEnumerable<StaffSalaryReport> getStaffSalary();
        IEnumerable getExpensesByYearDashBoard(int year);
        IEnumerable getMonthlyExpensesDashBoard(int year,int month);
        IEnumerable getPaymentsByYearDashBoard(int year);
        IEnumerable getMonthlyPaymentsDashBoard(int year, int month);
        decimal[] getExpensesAndPaymentsByYearDashBoard(int year);
        decimal[] getExpensesAndPaymentsByMonthlyDashBoard(int year, int month);
        IEnumerable<StudentAcademicsReport> getStudentAcademics();
    }
}