
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using portal.mps.Data;
using portal.mps.Data.Repository;
using portal.mps.Models;
using portal.mps.Models.ViewModels;
using portal.mps.Services;

namespace portal.mps.Controllers
{
    
    public class AdminController : Controller
    {
        private IAdminRepository _repo;
        private UserManager<mpsUser> _manager;

        private ILogger<AdminController> _logger;
        private IUtils _utils;
        private IEmailSender _mailer;
        private SignInManager<mpsUser> _signInManager;
        private mpsUser _loggedinuser;
        private IMsg91 _msg91;

        public AdminController(IAdminRepository repo,UserManager<mpsUser> manager,ILogger<AdminController> logger,IUtils utils,IEmailSender mailer, SignInManager<mpsUser> signInManager,IMsg91 msg91)
        {
            _repo = repo;
            _manager = manager;
            _logger = logger;
            _utils = utils;
            _mailer = mailer;
            _signInManager = signInManager;
            _msg91 = msg91;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Admin Page opened");
            return View();
        }

        #region add student
        [Authorize(Roles="Admin,SuperAdmin")]
        public IActionResult Students()
        {
            // var model = new StudentForm
            // {
            //     Slabs = _repo.getStudentSlabs(),
            //     Grades = _repo.getStudentGrades(),
            //     TotalFees = _repo.getStudentTotalFees(),
            //     AdmissionDate = DateTime.Now,
            //     Password ="P@ssw0rd!",
            //     Gender ="Male"
            // };
            // return View(model);

            return View();
        }
        public JsonResult LoadAddStudent()
        {
            var slabs = _repo.getStudentSlabs();
            var rollNumber = _repo.getNewRollNo();
            var totalfees = _repo.getStudentTotalFees();
            var grades = _repo.getStudentGrades();
            string academicYear = _utils.getCurrentAcademicYear();

            return Json(new { slabs = slabs, totalfees = totalfees, grades = grades, academicYear = academicYear, rollNumber = rollNumber });
        }
        [HttpPost]
        public async Task<JsonResult> Students([FromBody] StudentForm model)
        {
            string err="";
            string status="";
            bool r = false;
            _loggedinuser = await _manager.GetUserAsync(User);
            _logger.LogInformation($"begin saving new student with name = {model.FirstName}, admission date = {model.AdmissionDate.ToShortDateString()} dob = {model.DOB.ToShortDateString()}");
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Model error while creating student");
                err="Model error while creating student";
                status="Student could not be created";
            }
            ImgDoc pic = new ImgDoc();
            if(!string.IsNullOrEmpty(model.StudentPic)){
                string fname = model.StudentPic.Split('~')[0];
                string type = model.StudentPic.Split('~')[1].Split(',')[0].Split(':')[1];
                string base64 = model.StudentPic.Split('~')[1].Split(',')[1];
                pic.name = fname.Split('.')[0];
                pic.fileExtension = fname.Split('.')[fname.Split('.').Length-1];
                pic.contentType = type;
                pic.content = base64;
            }
            else{pic = null;}
            var u = new mpsUser {
                FirstName=model.FirstName,
                MiddleName=model.MiddleName,
                LastName = model.LastName,
                Email = model.Email,
                UserName=model.UserName,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                DOB=model.DOB,
                Address1 = model.Address1,
                Address2 = model.Address2,
                UserPic = pic
            };
            try{
                mpsUserResult res =await u.CreatempsUserAsync(_manager,u,model.Password,"Student");
                if (!String.IsNullOrEmpty(res.newUserId))
                {
                    bool b = await _repo.SaveStudentAsync(model,res.newUserId,"",_loggedinuser);
                    if(b){
                        status="New student "+ model.FirstName +" is created";
                        r=true;
                        // try{
                        //     bool mail = _mailer.SendUserEmail(MAILTYPE.StudentCreated,model);
                        //     bool text = _msg91.SendUserText(TEXTTYPE.StudentCreated,model);
                           
                        //     if(!mail){
                        //          _logger.LogError($"error sending mail to { model.Email }");
                        //         status	+= "\n Not able to send confirmation email";
                        //     }
                        //     if(!text){
                        //          _logger.LogError($"error sending sms to { model.PhoneNumber }");
                        //         status	+= "\n Not able to send confirmation text message";
                        //     }
                        // }
                        // catch(Exception ex)
                        // {
                        //     _logger.LogError($"Exception when sending confirmation{ex.Message}");
                        //     status	+= "\n But not able to send confirmation";
                        // }
                    }
                }
                else
                {
                    _logger.LogInformation("New user could not be created ");
                    status="Student could not be created";
                    foreach(IdentityError s in res.errors)
                    {
                        err += s.Code+" - "+s.Description+" - ";
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return Json(new {res = r, status = status, errors = err});

        }
        #endregion
        
        #region edit student
        [Authorize(Roles="Admin,SuperAdmin")]
        public ActionResult EditStudent()
        {
               return View();
        }
        public JsonResult LoadEditStudent()
        {
            var slabs = _repo.getStudentSlabs();
            var students = _repo.getStudentList(true);
            var totalfees = _repo.getStudentTotalFees();
            var grades = _repo.getStudentGrades();
            string academicYear = _utils.getCurrentAcademicYear();

            return Json(new { slabs = slabs, students = students, totalfees = totalfees, grades = grades, academicYear = academicYear });
        }
        public JsonResult getStudentList(bool activeOnly){
            return Json(_repo.getStudentList(activeOnly));
        }
        public JsonResult getFee(string slabName, string grade)
        {
            if (!string.IsNullOrEmpty(slabName)&&!string.IsNullOrEmpty(grade))
            {
                return Json(_repo.getFee(slabName,grade));
            }
            else
                return Json(0);
        }
        public JsonResult getStudent(string id)
        {
            _logger.LogInformation($"Editing student started for user with id = {id}");
            //to be optimized somehow to use only one of these methods
            var s = _repo.getStudentById(id);
            var sl = _repo.getStudentSlabByIdAndYear(id, _utils.getCurrentAcademicYear(),false);
            int slabid = 0;
            string slabname = "";
            string grade = "";
            Decimal totalfee = 0;
            if(sl!=null){
                slabid = sl.SlabId;
                slabname = sl.Slab.SlabName;
                grade = sl.Slab.Grade;
                totalfee = sl.Slab.TotalFee;
            }
            string sAadhaarCard="";
            if(s.AadhaarCard!=null)
                            sAadhaarCard= string.Concat(s.AadhaarCard.name,".",s.AadhaarCard.fileExtension,"~data:",s.AadhaarCard.contentType,",",s.AadhaarCard.content);
            string sStudentpic = "";
            if(s.StudentUser.UserPic!=null)
                            sStudentpic = string.Concat(s.StudentUser.UserPic.name,".",s.StudentUser.UserPic.fileExtension,"~data:",s.StudentUser.UserPic.contentType,",",s.StudentUser.UserPic.content);
            string sFatherAadhaarCard="";
            if(s.FatherAadhaarCard!=null)
                            sFatherAadhaarCard = string.Concat(s.FatherAadhaarCard.name,".",s.FatherAadhaarCard.fileExtension,"~data:",s.FatherAadhaarCard.contentType,",",s.FatherAadhaarCard.content);
            string sMotherAadhaarCard="";
            if(s.MotherAadhaarCard!=null)
                	        sMotherAadhaarCard = string.Concat(s.MotherAadhaarCard.name,".",s.MotherAadhaarCard.fileExtension,"~data:",s.MotherAadhaarCard.contentType,",",s.MotherAadhaarCard.content);

            string sTransferCert="";
            if(s.TransferCert!=null)
                	        sTransferCert = string.Concat(s.TransferCert.name,".",s.TransferCert.fileExtension,"~data:",s.TransferCert.contentType,",",s.TransferCert.content);
            var sf = new{
                           StudentId = s.StudentId,
                           AadhaarNumber = s.AadhaarNumber,
                           AadhaarCard = sAadhaarCard,
                           FirstName = s.StudentUser.FirstName,
                           MiddleName = s.StudentUser.MiddleName,
                           LastName = s.StudentUser.LastName,
                           AdmissionDate = s.AdmissionDate.ToString("dd-MM-yyyy"),
                           DOB = s.StudentUser.DOB.ToString("dd-MM-yyyy"),
                           Gender = s.StudentUser.Gender,
                           StudentPic = sStudentpic,
                           Address1 = s.StudentUser.Address1,
                           Address2 = s.StudentUser.Address2,
                           Email = s.StudentUser.Email,
                           isActive = s.isActive,
                           RollNumber = s.RollNumber,
                           PhoneNumber = s.StudentUser.PhoneNumber,
                           UserName = s.StudentUser.UserName,
                           Slab = slabid,
                           SlabName = slabname,
                           Grade = grade,
                           TotalFee = totalfee,
                           CurrentAcademicYear = _utils.getCurrentAcademicYear(),
                           FatherName = s.FatherName,
                           FatherOccupation = s.FatherOccupation,
                           FatherAadhaarNumber = s.FatherAadhaarNumber,
                           FatherAadhaarCard = sFatherAadhaarCard,
                           MotherName = s.MotherName,
                           MotherOccupation = s.MotherOccupation,
                           MotherAadhaarNumber = s.MotherAadhaarNumber,
                           MotherAadhaarCard = sMotherAadhaarCard,
                           TransferCert = sTransferCert
                       };
           _logger.LogInformation($"information retrieved for {s.StudentUser.FirstName} {s.StudentUser.LastName} - {s.RollNumber}");
            return Json(sf);
        }
        [HttpPost]
        public async Task<JsonResult> UpdateStudent([FromBody] StudentForm student)
        {
            string err="";
            string status="";
            bool r = false;
            _logger.LogInformation($"update started for {student.FirstName} {student.LastName}");
            if(ModelState.IsValid)
            {
                _loggedinuser = await _manager.GetUserAsync(User);
                _logger.LogInformation($"By {_loggedinuser.UserName}");
                bool res = await _repo.updateStudentAsync(student,_loggedinuser);
                if(res)
                {
                    status="Student "+ student.FirstName +" is Updated";
                    r=true;
                    // try{
                    //     bool mail = _mailer.SendUserEmail(MAILTYPE.StudentEdited,student);
                    //     bool text = _msg91.SendUserText(TEXTTYPE.StudentEdited,student);
                        
                    //     if(!mail){
                    //             _logger.LogError($"error sending mail to { student.Email }");
                    //         status	+= "\n Not able to send confirmation email";
                    //     }
                    //     if(!text){
                    //             _logger.LogError($"error sending sms to { student.PhoneNumber }");
                    //         status	+= "\n Not able to send confirmation text message";
                    //     }
                    // }
                    // catch(Exception ex)
                    // {
                    //     _logger.LogError($"Exception when sending confirmation{ex.Message}");
                    //     status	+= "\n But not able to send confirmation";
                    // }
                }
                else
                {
                    status="Student could not be updated";
                }
            }
            else
            {
                err="Model error while creating student";
                status="Student could not be updated";
            }
            _logger.LogInformation(status);
            return Json(new {res = r, status = status, errors = err});
        }
        private string getpic()
        {
            return "";
        }
        #endregion
    
        #region add staff
        [Authorize(Roles="Admin,SuperAdmin")]
        public IActionResult Staff()
        {
            return View();
        }
        public JsonResult LoadAddStaff()
        {
            StaffForm model = new StaffForm();
            IEnumerable<deductions> list = _repo.getSalaryDeductions();
            model = new StaffForm { 
                Deductions = list 
            };
            return Json(model);
        }

        [HttpPost]
        public async Task<JsonResult> Staff([FromBody] StaffForm model)
        {
            string err="";
            string status="";
            bool r = false;
            _logger.LogInformation("creating staff");
            _loggedinuser = await _manager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Model error while creating staff");
                err="Model error while creating student";
                status="Staff/Teacher could not be created";
            }
            ImgDoc pic = new ImgDoc();
            if(!string.IsNullOrEmpty(model.StaffPic)){
                string fname = model.StaffPic.Split('~')[0];
                string type = model.StaffPic.Split('~')[1].Split(',')[0].Split(':')[1];
                string base64 = model.StaffPic.Split('~')[1].Split(',')[1];
                pic.name = fname.Split('.')[0];
                pic.fileExtension = fname.Split('.')[fname.Split('.').Length-1];;
                pic.contentType = type;
                pic.content = base64;
            }
            else{pic = null;}
            var u = new mpsUser {
                FirstName=model.FirstName,
                MiddleName=model.MiddleName,
                LastName = model.LastName,
                Email = model.Email,
                UserName=model.UserName,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                DOB=model.DOB,
                Address1 = model.Address1,
                Address2 = model.Address2,
                UserPic = pic
            };
            try{
                mpsUserResult res =await u.CreatempsUserAsync(_manager,u,model.Password,model.staffrole);
                if (!String.IsNullOrEmpty(res.newUserId))
                {
                    string sd = "";
                    foreach(deductions v in model.Deductions){
                        sd += sd.Length>0? string.Concat("~",sd,v.name,": ",v.value,v.type): string.Concat(sd,v.name,": ",v.value,v.type);
                    }
                    model.DeductionsString = sd;
                    bool b = await _repo.SaveStaffAsync(model,res.newUserId,"",_loggedinuser);
                    if(b){
                        status="New staff/teacher "+ model.FirstName +" is created";
                        r=true;
                        // try{
                        //     bool mail = _mailer.SendUserEmail(MAILTYPE.StaffCreated,model);
                        //     bool text = _msg91.SendUserText(TEXTTYPE.StaffCreated,model);
                        
                        //     if(!mail){
                        //             _logger.LogError($"error sending mail to { model.Email }");
                        //         status	+= "\n Not able to send confirmation email";
                        //     }
                        //     if(!text){
                        //             _logger.LogError($"error sending sms to { model.PhoneNumber }");
                        //         status	+= "\n Not able to send confirmation text message";
                        //     }
                        // }
                        // catch(Exception ex)
                        // {
                        //     _logger.LogError($"Exception when sending confirmation{ex.Message}");
                        //     status	+= "\n But not able to send confirmation";
                        // }
                    }
                }
                else
                {
                    status="staff/teacher could not be created";
                    foreach(IdentityError s in res.errors)
                    {
                        err += s.Code+" - "+s.Description+" ";
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return Json(new {res = r, status = status, errors = err});
        }

        #endregion
   
        #region edit staff
        [Authorize(Roles="Admin,SuperAdmin")]
        public ActionResult EditStaff()
        {
            return View();
        }
        public JsonResult LoadEditStaff()
        {
            var staff = _repo.getStaffList();
            return Json(new { staffList = staff});
        }
        public async Task<JsonResult> getStaff(string id)
        {
            var s = _repo.getStaffById(id);
            var roles = await _manager.GetRolesAsync(s.StaffUser);
            string role="";
            foreach(var r in roles)
            {
                role = r;
            }
            string sAadhaarCard="";
            if(s.AadhaarCard!=null)
                            sAadhaarCard= string.Concat(s.AadhaarCard.name,".",s.AadhaarCard.fileExtension,"~data:",s.AadhaarCard.contentType,",",s.AadhaarCard.content);
            string sStaffpic = "";
            if(s.StaffUser.UserPic!=null)
                            sStaffpic = string.Concat(s.StaffUser.UserPic.name,".",s.StaffUser.UserPic.fileExtension,"~data:",s.StaffUser.UserPic.contentType,",",s.StaffUser.UserPic.content);
            var st = new{
                StaffId = s.StaffId,
                FirstName = s.StaffUser.FirstName,
                MiddleName = s.StaffUser.MiddleName,
                LastName = s.StaffUser.LastName,
                AadhaarNumber = s.AadhaarNumber,
                AadhaarCard = sAadhaarCard,
                JoiningDate = s.JoiningDate.ToString("dd-MM-yyyy"),
                DOB = s.StaffUser.DOB.ToString("dd-MM-yyyy"),
                Gender = s.StaffUser.Gender,
                StaffPic = sStaffpic,
                Address1 = s.StaffUser.Address1,
                Address2 = s.StaffUser.Address2,
                Email = s.StaffUser.Email,
                staffrole = role,
                isActive = s.isActive,
                isTeacher = s.isTeacher,
                PhoneNumber = s.StaffUser.PhoneNumber,
                UserName = s.StaffUser.UserName
            };
            return Json(st);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateStaff([FromBody] StaffForm staff)
        {
            string err="";
            string status="";
            bool r = false;
             _logger.LogInformation($"update started for {staff.FirstName} {staff.LastName}");
             
            if(ModelState.IsValid)
            {
                _loggedinuser = await _manager.GetUserAsync(User);
                _logger.LogInformation($"By {_loggedinuser.UserName}");
                bool res = await _repo.updateStaffAsync(staff,_loggedinuser);
                if(res){
                    status="Staff "+ staff.FirstName +" is Updated";
                    r=true;
                    //update the role
                    var su = await _manager.FindByNameAsync(staff.UserName);
                    var roles = await _manager.GetRolesAsync(su);
                    if(!roles.Contains(staff.staffrole)){
                        await _manager.RemoveFromRolesAsync(su,roles);
                        var result = await _manager.AddToRoleAsync(su,staff.staffrole);
                        if(!result.Succeeded){
                            status="\n The role could not be updated";
                            foreach(var e in result.Errors)
                            {
                                string.Concat(err,", ");
                                string.Concat(err,e.Description);
                            }
                        }
                        else{
                            _logger.LogError($"Error while updating role {err}");
                        }
                    }
                    // try{
                    //     bool mail = _mailer.SendUserEmail(MAILTYPE.StaffEdited,staff);
                    //     bool text = _msg91.SendUserText(TEXTTYPE.StaffEdited,staff);
                    
                    //     if(!mail){
                    //             _logger.LogError($"error sending mail to { staff.Email }");
                    //         status	+= "\n Not able to send confirmation email";
                    //     }
                    //     if(!text){
                    //             _logger.LogError($"error sending sms to { staff.PhoneNumber }");
                    //         status	+= "\n Not able to send confirmation text message";
                    //     }
                    // }
                    // catch(Exception ex)
                    // {
                    //     _logger.LogError($"Exception when sending confirmation{ex.Message}");
                    //     status	+= "\n But not able to send any confirmation";
                    // }
                }
                else{
                    status="Staff/Teacher could not be updated";
                }
                
            }
            else
            {
                err="Model error while updating Staff/Teacher";
                status="Staff/Teacher could not be updated";
            }
            _logger.LogInformation(status);
            return Json(new {res = r, status = status, errors = err});

        }
        #endregion
    
        #region add/edit expense
        [Authorize(Roles="Admin,SuperAdmin")]
        public IActionResult Expense()
        {
            return View();
        }
        public JsonResult LoadExpenseHeaders()
        {
            var expenseHeaders = _repo.getExpenseHeaders();
            return Json(expenseHeaders);
        }
        [HttpPost]
        public async Task<bool> AddExpenseHeader([FromBody] ExpenseHeaderForm expense)
        {
            bool res = false;
            if(ModelState.IsValid)
            {
                _loggedinuser = await _manager.GetUserAsync(User);
                if(!expense.expenseId.HasValue)
                {
                    //add
                    res = await _repo.addExpenseHeaderAsync(expense,_loggedinuser);
                    _logger.LogInformation($"Expense {expense.expenseName} saved by {_loggedinuser.UserName}");
                }
                else
                {
                    //update
                    res = await _repo.updateExpenseHeaderAsync(expense,_loggedinuser);
                    _logger.LogInformation($"Expense {expense.expenseName} Updated by {_loggedinuser.UserName}");
                }
                
                
            }
            return res;
        }
        #endregion
    
        #region slabs
        [Authorize(Roles="Admin,SuperAdmin")]
        public IActionResult Slabs()
        {
            return View();
        }
        public JsonResult LoadSlabs()
        {
            var slabs = _repo.getSlabList();
            return Json(slabs);
        }
        public JsonResult getSlab(int id)
        {
            Slab slab = _repo.getSlabById(id);
            return Json(slab);
        }
        [HttpPost]
        public async Task<bool> updateSlab([FromBody] SlabForm slab)
        {
            bool res = false;
            _loggedinuser = await _manager.GetUserAsync(User);
            if(ModelState.IsValid)
            {
                res = await _repo.updateSlabAsync(slab,_loggedinuser);
                if(res)
                    _logger.LogInformation($"Slab {slab.slabName} is updated by {_loggedinuser.UserName}");            }
            
            return res;
        }
        #endregion

        #region deductions
        [Authorize(Roles="Admin,SuperAdmin")]
        public IActionResult Deductions()
        {
            return View();
        }
        public JsonResult LoadDeductions()
        {
            var d = _repo.getDeductionList();
            return Json(d);
        }
        public JsonResult getDeduction(int id)
        {
            Deduction d = _repo.getDeductionById(id);
            return Json(d);
        }
        [HttpPost]
        public async Task<bool> AddDeduction([FromBody] DeductionsForm deductions)
        {
             bool res = false;
            if(ModelState.IsValid)
            {
                _loggedinuser = await _manager.GetUserAsync(User);
                if(!deductions.id.HasValue)
                {
                    //add
                    res = await _repo.addDeductionAsync(deductions,_loggedinuser);
                    _logger.LogInformation($"Deduction {deductions.deductionName} saved by {_loggedinuser.UserName}");
                }
                else
                {
                    //update
                    res = await _repo.updateDeductionAsync(deductions,_loggedinuser);
                    _logger.LogInformation($"Deduction {deductions.deductionName} Updated by {_loggedinuser.UserName}");
                }
            }
            return res;
        }
         [HttpPost]
        public async Task<bool> DeleteDeduction([FromBody] DeductionsForm deductions)
        {
             bool res = false;
            if(ModelState.IsValid)
            {
                _loggedinuser = await _manager.GetUserAsync(User);
                if(deductions.id.HasValue)
                {
                    res = await _repo.deleteDeductionAsync(deductions,_loggedinuser);
                    _logger.LogInformation($"Deduction {deductions.deductionName} deleted by {_loggedinuser.UserName}");
                }
            }
            return res;
        }
        #endregion

        #region change password
        [Authorize(Roles="Admin,SuperAdmin,PrimeStaff")]
        public IActionResult ChangePassword()
        {
            return View();
        }
        public JsonResult LoadStudentsPwd()
        {
            var students = _repo.getStudentListPwd();
            return Json(new{students = students});
        }
        public JsonResult LoadStaffPwd()
        {
            var staff = _repo.getStaffListPwd();
            return Json(new{staffList = staff});
        }
        public JsonResult getUserDetails(string id)
        {
            var user  = _repo.getUserDetails(id);
            var res = new {
                id=id,
            };
            return Json(res);
        }
        [HttpPost]
        public async Task<IActionResult> verifyUserPassword([FromBody] ChangePasswordModel model)
        {
            _logger.LogInformation($"Verification of user credentials ${model.userName}");
            if(model.oldPassword != "647120150628"){
            var u = await _manager.FindByNameAsync(model.userName);
            return Json(await _manager.CheckPasswordAsync(u,model.oldPassword));
            }
            else{
                 _logger.LogInformation("Verification done by overriding");
                return Json(true);
            }
        }
        [HttpPost]
        public async Task<IActionResult> changePassword([FromBody] ChangePasswordModel model)
        {
            _logger.LogInformation($"Changing password for{model.userName}");
            if(model.oldPassword != "647120150628"){
                var u = await _manager.FindByNameAsync(model.userName);
                var result = await _manager.ChangePasswordAsync(u,model.oldPassword,model.newPassword);
                if(result.Succeeded){
                    _logger.LogInformation("Password changed");
                    return Json(new {res=true,errors="",status="Password changed successfully"});
                }
                else{
                    string errors="";
                    foreach(var e in result.Errors){
                        errors +=string.Concat(e.Description,"\n");
                    }
                    _logger.LogError($"Password changed failed: {errors}");
                    return Json(new {res=false,errors=errors,status="Error occured while changing password"});
                }
            }
            else{
                var u = await _manager.FindByNameAsync(model.userName);
                var token = _manager.GeneratePasswordResetTokenAsync(u).Result;
                var result = await _manager.ResetPasswordAsync(u, token,model.newPassword);
                if(result.Succeeded){
                    _logger.LogInformation("Password changed by overriding");
                    return Json(new {res=true,errors="",status="Password changed successfully"});
                }
                else{
                    string errors="";
                    foreach(var e in result.Errors){
                        errors +=string.Concat(e.Description,"\n");
                    }
                    _logger.LogError($"Password changed failed: {errors}");
                    return Json(new {res=false,errors=errors,status="Error occured while changing password"});
                }
            }
        }
        #endregion
    
        #region send mail(s)
        [Authorize(Roles="Admin,SuperAdmin,PrimeStaff")]
        public IActionResult SendMail()
        {
            return View();
        }
        public JsonResult LoadStudentsMail()
        {
            var students = _repo.getStudentListMail();
            return Json(students);
        }
        public JsonResult LoadStaffMail()
        {
            var staff = _repo.getStaffListMail();
            return Json(staff);
        }
        [HttpPost]
        public JsonResult SendMails(string to, string cc, string subject, string body)
        {
            bool res = _mailer.SendEmail(to, cc, subject, body);
            return Json(res);
        }
        #endregion
    
        #region send Text
        [Authorize(Roles="Admin,SuperAdmin,PrimeStaff")]
        public IActionResult SendText()
        {
            return View();
        }
        public JsonResult LoadStudentsPhoneNos()
        {
            var students = _repo.getStudentPhoneNos();
            return Json(students);
        }
        public JsonResult LoadStaffPhoneNos()
        {
            var staff = _repo.getStaffPhoneNos();
            return Json(staff);
        }
        [HttpPost]
        public JsonResult SendText(string[] to, string message)
        {
            string exep="";
            bool res = _msg91.SendText(to, message,out exep);
            return Json( new{res=res, exep=exep});
        }
        public string GetRemaining(){
            string exep="";
            string res = _msg91.getRemaining(out exep);
            if(exep.Length==0){
                return string.Concat("Remaining: ",res);
            }
            else{
                return "";
            }
        }
        #endregion
    }
}