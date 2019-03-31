using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using portal.mps.Data;

namespace portal.mps.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private ILogger _logger;
        private IConfiguration _config;
        private IHostingEnvironment _env;

        public EmailSender(ILogger<EmailSender> logger,IConfiguration config,IHostingEnvironment env)
        {
            _logger = logger;
            _config = config;
            _env = env;
        }

        public bool SendEmail(string to, string cc, string subject, string message)
        {
            _logger.LogInformation($"sending mail to {to}");
            try{
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["emailSettings:fromAddress"]),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                foreach(string m in to.Split(';'))
                    mailMessage.To.Add(m);
                if(cc!=null)
                {
                    foreach(string c in cc.Split(';'))
                        mailMessage.CC.Add(c);
                }

                var smtpClient = new SmtpClient
                {
                    Credentials = new NetworkCredential(_config["emailSettings:fromAddress"],_config["emailSettings:fromPassword"]),
                    Host = _config["emailSettings:host"],
                    Port = 25
                };
                smtpClient.EnableSsl=Convert.ToBoolean(_config["emailSettings:ssl"]);
                smtpClient.Send(mailMessage);

                // using (var client = new SmtpClient(_config["emailSettings:host"],25)) {
                //     MailMessage msg = new MailMessage();

                //     msg.From = new MailAddress(_config["emailSettings:fromAddress"]);
                //     msg.To.Add(new MailAddress(email));
                //     msg.Body = message;
                //     msg.Subject = subject;
                //     client.UseDefaultCredentials = false;
                //     client.Credentials = new System.Net.NetworkCredential(_config["emailSettings:fromAddress"],_config["emailSettings:fromPassword"]);
                //     client.Send(msg);
                // }
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Error sending mail: {ex.Message}");
                return false;
            }
        }

        public bool SendUserEmail(MAILTYPE m, object model)
        {
            string mail="";
            bool res=false;
            try{
                string subject="",body="";
                getMailContent(m,model,out subject,out body);
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["emailSettings:fromAddress"]),
                    Subject = subject,
                    Body = body.ToString(),
                    IsBodyHtml = true
                };
                if(m.Equals(MAILTYPE.StaffCreated) || m.Equals(MAILTYPE.StaffEdited)){
                    var y = (Models.ViewModels.StaffForm)model;
                    mail = y.Email;
                    mailMessage.To.Add(y.Email);
                }
                else{
                    var x = (Models.ViewModels.StudentForm)model;
                    mail = x.Email;
                    mailMessage.To.Add(x.Email);
                }

                var smtpClient = new SmtpClient
                {
                    Credentials = new NetworkCredential(_config["emailSettings:fromAddress"],_config["emailSettings:fromPassword"]),
                    Host = _config["emailSettings:host"],
                    Port = 25
                };
                smtpClient.EnableSsl=Convert.ToBoolean(_config["emailSettings:ssl"]);
                _logger.LogInformation($"sending mail to {mail}, host-{smtpClient.Host}, port-{smtpClient.Port}");
                smtpClient.Send(mailMessage);
                res=true;
                return res;
            }
            catch(Exception ex)
            {
                res=false;
                _logger.LogCritical($"Error sending mail: {ex.Message}");
                return res;
            }
        }

        private void getMailContent(MAILTYPE m,object model, out string subject, out string body)
        {
            subject="";
            body="";
            StringBuilder sb = new StringBuilder();
            // string fname="",mname="",lname="";
            // if(u!=null){
            //     fname = u.FirstName;
            //     mname = u.MiddleName;
            //     lname = u.LastName;
            // }
            
            switch(m)
            {
                case MAILTYPE.StudentCreated:
                    var x = (Models.ViewModels.StudentForm)model;
                    subject="You have been registered with our school";
                    sb.Clear();
                    if(x!=null)
                    {
                        sb.Append("Dear ");
                        sb.Append(x.FirstName);
                        sb.Append(" ");
                        sb.Append(String.IsNullOrEmpty(x.MiddleName)?"":x.MiddleName+" ");
                        sb.Append(x.LastName);
                        sb.Append(",<br>");
                    }
                    sb.Append("    ");
                    sb.Append("You have been added to our school's database.<br>");
                    if(x!=null)
                    {
                        sb.Append("The following are the details:");
                        sb.Append("<table center width=50% border=\"1\"><tr><td style=\"padding-left:23px\">");
                        sb.Append("First Name: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.FirstName);
                        sb.Append("</td></tr>");
                        if(x.MiddleName!=null)
                        {
                            sb.Append("<tr><td style=\"padding-left:23px\">Middle Name: </td><td style=\"padding-left:23px\">");
                            sb.Append(x.MiddleName);
                            sb.Append("</td></tr>");
                        }
                        sb.Append("<tr><td style=\"padding-left:23px\">Last Name: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.LastName);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Date of birth: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.DOB.ToString("dd-MM-yyyy"));
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Gender: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.Gender);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Slab: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.SlabName);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Class: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.Grade);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Phone Number: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.PhoneNumber);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Address: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.Address1);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Mps Portal username: </td><td style=\"padding-left:23px\">");
                        sb.Append(x.UserName);
                        sb.Append("</td></tr>");
                        sb.Append("</table><br><br>");
                        sb.Append("Best regards,<br>");
                        sb.Append("Mysore Public School");
                    }
                    body = sb.ToString();

                    break;
                case MAILTYPE.StudentEdited:
                    var y = (Models.ViewModels.StudentForm)model;
                    subject="Your details have been updated";
                    sb.Clear();
                    if(y!=null)
                    {
                        sb.Append("Dear ");
                        sb.Append(y.FirstName);
                        sb.Append(" ");
                        sb.Append(String.IsNullOrEmpty(y.MiddleName)?"":y.MiddleName+" ");
                        sb.Append(y.LastName);
                        sb.Append(",<br>");
                    }
                    sb.Append("    ");
                    sb.Append("Your details in our school's database have been updated<br>");
                    if(y!=null)
                    {
                        sb.Append("The following are the details:");
                        sb.Append("<table center width=50% border=\"1\"><tr><td style=\"padding-left:23px\">");
                        sb.Append("First Name: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.FirstName);
                        sb.Append("</td></tr>");
                        if(y.MiddleName!=null)
                        {
                            sb.Append("<tr><td style=\"padding-left:23px\">Middle Name: </td><td style=\"padding-left:23px\">");
                            sb.Append(y.MiddleName);
                            sb.Append("</td></tr>");
                        }
                        sb.Append("<tr><td style=\"padding-left:23px\">Last Name: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.LastName);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Date of birth: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.DOB.ToString("dd-MM-yyyy"));
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Gender: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.Gender);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Slab: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.SlabName);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Class: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.Grade);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Phone Number: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.PhoneNumber);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Address: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.Address1);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Mps Portal username: </td><td style=\"padding-left:23px\">");
                        sb.Append(y.UserName);
                        sb.Append("</td></tr>");
                        sb.Append("</table><br><br>");
                        sb.Append("Best regards,<br>");
                        sb.Append("Mysore Public School");
                    }
                    body = sb.ToString();
                    break;
                case MAILTYPE.StaffCreated:
                    var z = (Models.ViewModels.StaffForm)model;
                    subject="You have been registered with our school";
                    sb.Clear();
                    if(z!=null)
                    {
                        sb.Append("Dear ");
                        sb.Append(z.FirstName);
                        sb.Append(" ");
                        sb.Append(String.IsNullOrEmpty(z.MiddleName)?"":z.MiddleName+" ");
                        sb.Append(z.LastName);
                        sb.Append(",<br>");
                    }
                    sb.Append("    ");
                    sb.Append("You have been added to our school's database.<br>");
                    if(z!=null)
                    {
                        sb.Append("The following are the details:");
                        sb.Append("<table center width=50% border=\"1\"><tr><td style=\"padding-left:23px\">");
                        sb.Append("First Name: </td><td style=\"padding-left:23px\">");
                        sb.Append(z.FirstName);
                        sb.Append("</td></tr>");
                        if(z.MiddleName!=null)
                        {
                            sb.Append("<tr><td style=\"padding-left:23px\">Middle Name: </td><td style=\"padding-left:23px\">");
                            sb.Append(z.FirstName);
                            sb.Append("</td></tr>");
                        }
                        sb.Append("<tr><td style=\"padding-left:23px\">Last Name: </td><td style=\"padding-left:23px\">");
                        sb.Append(z.LastName);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Date of birth: </td><td style=\"padding-left:23px\">");
                        sb.Append(z.DOB.ToString("dd-MM-yyyy"));
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Gender: </td><td style=\"padding-left:23px\">");
                        sb.Append(z.Gender);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Role: </td><td style=\"padding-left:23px\">");
                        sb.Append(z.staffrole);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Phone Number: </td><td style=\"padding-left:23px\">");
                        sb.Append(z.PhoneNumber);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Address: </td><td style=\"padding-left:23px\">");
                        sb.Append(z.Address1);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Mps Portal username: </td><td style=\"padding-left:23px\">");
                        sb.Append(z.UserName);
                        sb.Append("</td></tr>");
                        sb.Append("</table><br><br>");
                        sb.Append("Best regards,<br>");
                        sb.Append("Mysore Public School");
                    }
                    body = sb.ToString();
                    break;
                case MAILTYPE.StaffEdited:
                    var a = (Models.ViewModels.StaffForm)model;
                    subject="You details have been updated";
                    sb.Clear();
                    if(a!=null)
                    {
                        sb.Append("Dear ");
                        sb.Append(a.FirstName);
                        sb.Append(" ");
                        sb.Append(String.IsNullOrEmpty(a.MiddleName)?"":a.MiddleName+" ");
                        sb.Append(a.LastName);
                        sb.Append(",<br>");
                    }
                    sb.Append("    ");
                    sb.Append("Your details in our school's database have been updated<br>");
                   if(a!=null)
                    {
                        sb.Append("The following are the details:");
                        sb.Append("<table center width=50% border=\"1\"><tr><td style=\"padding-left:23px\">");
                        sb.Append("First Name: </td><td style=\"padding-left:23px\">");
                        sb.Append(a.FirstName);
                        sb.Append("</td></tr>");
                        if(a.MiddleName!=null)
                        {
                            sb.Append("<tr><td style=\"padding-left:23px\">Middle Name: </td><td style=\"padding-left:23px\">");
                            sb.Append(a.FirstName);
                            sb.Append("</td></tr>");
                        }
                        sb.Append("<tr><td style=\"padding-left:23px\">Last Name: </td><td style=\"padding-left:23px\">");
                        sb.Append(a.LastName);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Date of birth: </td><td style=\"padding-left:23px\">");
                        sb.Append(a.DOB.ToString("dd-MM-yyyy"));
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Gender: </td><td style=\"padding-left:23px\">");
                        sb.Append(a.Gender);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Role: </td><td style=\"padding-left:23px\">");
                        sb.Append(a.staffrole);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Phone Number: </td><td style=\"padding-left:23px\">");
                        sb.Append(a.PhoneNumber);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Address: </td><td style=\"padding-left:23px\">");
                        sb.Append(a.Address1);
                        sb.Append("</td></tr>");
                        sb.Append("<tr><td style=\"padding-left:23px\">Mps Portal username: </td><td style=\"padding-left:23px\">");
                        sb.Append(a.UserName);
                        sb.Append("</td></tr>");
                        sb.Append("</table><br><br>");
                        sb.Append("Best regards,<br>");
                        sb.Append("Mysore Public School");
                    }
                    body = sb.ToString();
                    break;
            
            
            }
        }
    }
    public enum MAILTYPE{
        None,
        StudentCreated = 1,
        StudentEdited = 2,
        StaffCreated = 3,
        StaffEdited = 4,
        FeesPaid = 5,
        SalaryCredited = 6,
        PasswordChanged = 7
    }
}
