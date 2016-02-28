using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.ConsoleApp
{

    public class CmdLineException : ApplicationException
    {

        public CmdLineException(string message, Exception innerEx = null)
            : base(message, innerEx)
        {

        }

    }

    public class CmdLineArgException : CmdLineException
    {

        public CmdLineArgException(string argName, string message, Exception innerEx = null)
            : base(message, innerEx)
        {
            ArgName = argName;
        }

        public string ArgName { get; private set; }

    }
}
