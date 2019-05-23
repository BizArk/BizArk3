using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.ConsoleApp
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigAttribute :Attribute
    {
    }
}
