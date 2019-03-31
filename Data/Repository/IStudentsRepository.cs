
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using portal.mps.Models;
using portal.mps.Models.ViewModels;

namespace portal.mps.Data.Repository
{
    public interface IStudentsRepository
    {
        Task<IEnumerable> getStudentMatrix(string studentId, string grade, string academicYear, mpsUser loggedinuser);
        bool addAcademicEntity(AcademicEntity e);
        Task<bool> updateStudentMatrixAsync(List<StudentMatrixForm> sm, mpsUser loggedinuser);
        IEnumerable<AcademicEntity> getSubjects();
        bool updateAcademicEntity(AcademicEntity e);
        IList<Dictionary<string,string>> getStudentCharacterCert();
        IList<StudentMatrixForm> getStudentMatrixFromUserName(string username);
        IList<StudentMatrixForm> getStudentMatrixFromUserId(string studentId);
        IEnumerable getStudentList(bool activeOnly);
    }
}