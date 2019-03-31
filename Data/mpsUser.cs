using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using portal.mps.Models;
using portal.mps.Models.ViewModels;

namespace portal.mps.Data
{
    public class mpsUser : IdentityUser
    {
        [Required]
        [StringLength(32)]
        public string FirstName { get; set; }

        [StringLength(32)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(32)]
        public string LastName { get; set; }

        [StringLength(12)]
        public string Gender { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public string Address1 { get; set; }
        
        public string Address2 { get; set; }

        public int? UserPicId { get; set; }
        public ImgDoc UserPic { get; set; }
        
        public async Task<mpsUserResult> CreatempsUserAsync(UserManager<mpsUser> _manager,mpsUser u, string pwd, string role)
        {
            //string fname, string mname, string lname, string email, string username, string phoneno,DateTime dob, string add1, string add2, string pwd, string role
            IEnumerable exep = null;
            string newId = "";
            // var u = new mpsUser {
            //     FirstName=fname,
            //     MiddleName=mname,
            //     LastName = lname,
            //     Email = email,
            //     UserName=username,
            //     PhoneNumber = pwd,
            //     DOB=dob,
            //     Address1=add1,
            //     Address2 = add2
            // };
            try{
                var result = await _manager.CreateAsync(u, pwd);
                if (result.Succeeded)
                {
                    newId = u.Id;
                    await _manager.AddToRoleAsync(u,role);
                }
                else{
                    exep=result.Errors;
                }
            }
            catch(Exception ex){
                throw  ex;
            }
            var res = new mpsUserResult{
                newUserId = newId,
                errors = exep
            };

            return res;
        }
    }
}