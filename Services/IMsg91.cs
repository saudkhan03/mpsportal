using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using portal.mps.Data;
using portal.mps.Models.ViewModels;

namespace portal.mps.Services
{
    public interface IMsg91
    {
        bool SendText(string[] to, string message, out string exep);
        string getRemaining(out string exep);
        bool SendUserText(TEXTTYPE studentCreated, object model);
    }
}