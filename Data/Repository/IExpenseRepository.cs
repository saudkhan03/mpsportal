using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using portal.mps.Models;
using portal.mps.Models.ViewModels;

namespace portal.mps.Data.Repository
{
    public interface IExpenseRepository
    {
        IEnumerable getStudentGrades();
        IEnumerable getStudentSlabs();
        Task<Slab> getCalculatedSlabAsync(string slabName, string grade);
        IEnumerable getStudentList(bool includeActive);
        StudentFeesForm getStudentById(string id);
        StudentFee getStudentFee(int intId);
        Task<StudentFee> addStudentFeeAsync(string id, string amount, string date, mpsUser _loggedinuser);
        IEnumerable<StudentFeesList> getStudentFeesByYear(string studentId, string academicYear, out Decimal TotalFee, out IEnumerable<StudentExpenseList> expenses);
        Task<bool> deleteFee(int id);
        Task<int> addStudentSlabAsync(string studentId, Slab slab, string academicYear, DateTime startDate, DateTime endDate, mpsUser _loggedinuser);
        Task<bool> addStudentNewAYFeeAsync(string academicYear, int studentSlabId, mpsUser _loggedinuser);
        IEnumerable getStaffList(bool includeActive);
        StaffPaymentForm getStaffById(string id);
        Task<StaffPayment> addStaffSalaryAsync(string staffId, string amount, string date, mpsUser _loggedinuser);
        IEnumerable<StaffPaymentList> getStaffPayments(string staffId);
        Task<bool> deleteSalaryAsync(int id);
        IEnumerable<ExpenseHeader> getExpenseHeaders();
        Task<StaffSalary> updateSalaryAsync(string staffId, string newSalaryAmount, string newSalaryDate, string deductionString, mpsUser loggedinuser);
        Task<bool> addExpenseAsync(ExpenseForm expense, mpsUser loggedinuser);
        IEnumerable getExpenseList();
        Expense getExpenseById(int id);
        Task<bool> updateExpenseAsync(ExpenseForm expense, mpsUser loggedinuser);
        bool saveBill(string billType, byte[] byteInfo, printModel bill);
        long getNewBillNumber();
        byte[] getStudentRecieptBill(int id, out string filename);
        IEnumerable getStudentListForExpense();
        IEnumerable<deductions> getSalaryDeductions();
        StaffPaymentForm getStaffSalaryDetails(string staffId, string paymentId);
        IEnumerable<StudentSlab> getStudentBySlabId(int studentSlabId);
    }
}