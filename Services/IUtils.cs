using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using portal.mps.Data;
using portal.mps.Models;
using portal.mps.Models.ViewModels;

namespace portal.mps.Services
{
    public interface IUtils
    {
        string getCurrentAcademicYear();
        DateTime getCurrentStartYearDate();
        DateTime getCurrentEndYearDate();
        string getSalaryDeductions();
        byte[] ReadImageFile(string imageLocation);
        byte[] printBill(string billType, printModel bill, out string filename);
        Task<mpsUser> getUserFromUserNameAsync(string username);
        Task<string> GetRolesAsync(mpsUser u);
        byte[] printMatrix(matrixPrintModel matrix, out string filename);
        string getRoman(string grade);
        byte[] printCharacterMatrix(characterPrintModel model, out string filename);
        Task<StudentSlab> getStudentFromUserAsync(mpsUser u);
        Task<Staff> getStaffFromUserAsync(mpsUser u);
        string download(ImgDoc attachment, out string exep);
    }
}
