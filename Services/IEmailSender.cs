using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using portal.mps.Data;

namespace portal.mps.Services
{
    public interface IEmailSender
    {
        bool SendEmail(string to, string cc, string subject, string message);
        bool SendUserEmail(MAILTYPE m, object model);
    }
}
