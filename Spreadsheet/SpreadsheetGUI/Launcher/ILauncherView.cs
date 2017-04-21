using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    interface ILauncherView
    {
        event Action<string, string> OpenSpreadsheet;

    }
}
