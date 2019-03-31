using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using portal.mps.Models;
using portal.mps.Models.ViewModels;

namespace portal.mps.Data.Repository
{
    public interface IAdminRepository
    {
        IEnumerable getStudentSlabs();
        IEnumerable getStudentList(bool activeOnly); //gets the id,names and class of students, for display only
        IEnumerable getStudentGrades();
        IEnumerable getStudentTotalFees();
        Task<Slab> getCalculatedSlabAsync(string slabName, string grade, decimal totalFee);
        Task<bool> SaveStudentAsync(StudentForm model, string newId, string pathToPic, mpsUser _loggedinuser);
        Student getStudentById(string id);
        Task<bool> updateStudentAsync(StudentForm student, mpsUser _loggedinuser);
        Decimal getFee(string slabName, string grade);
        StudentSlab getStudentSlabByIdAndYear(string id, string academicyear,bool includeStudent);
        Task<bool> SaveStaffAsync(StaffForm model, string newId, string pathToPic, mpsUser _loggedinuser);
        IEnumerable getStaffList(); //gets the id,names and class of students, for display only
        Staff getStaffById(string id);
        Task<bool> updateStaffAsync(StaffForm staff, mpsUser _loggedinuser);
        IEnumerable<ExpenseHeader> getExpenseHeaders();
        Task<bool> addExpenseHeaderAsync(ExpenseHeaderForm expense, mpsUser loggedinuser);
        Task<bool> updateExpenseHeaderAsync(ExpenseHeaderForm expense, mpsUser loggedinuser);
        IEnumerable getSlabList();
        Slab getSlabById(int id);
        Task<bool> updateSlabAsync(SlabForm slab, mpsUser loggedinuser);
        IEnumerable<deductions> getSalaryDeductions();
        IEnumerable getDeductionList();
        Deduction getDeductionById(int id);
        IEnumerable getUserDetails(string id);
        IEnumerable getStudentListPwd();
        IEnumerable getStaffListPwd();
        IEnumerable getStudentListMail();
        IEnumerable getStaffListMail();
        Task<bool> addDeductionAsync(DeductionsForm deduction, mpsUser loggedinuser);
        string getNewRollNo();
        Task<bool> updateDeductionAsync(DeductionsForm deduction, mpsUser loggedinuser);
        Task<bool> deleteDeductionAsync(DeductionsForm deduction, mpsUser loggedinuser);
        IEnumerable getStudentPhoneNos();
        IEnumerable getStaffPhoneNos();
    }
}