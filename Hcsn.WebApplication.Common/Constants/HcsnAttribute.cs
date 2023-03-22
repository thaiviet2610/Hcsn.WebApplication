using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Constants
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnRequiredAttribute : Attribute
    {


    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnNumberAttribute : Attribute
    {
        public HcsnNumberAttribute(string name)
        {
            PropType = name;
        }
        public string PropType { get; set; }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnPrimaryKeyAttribute : Attribute
    {


    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnGreateThanZeroAttribute : Attribute
    {


    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnForeignKeyAttribute : Attribute
    {


    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnCodeAttribute : Attribute
    {


    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnNameAttribute : Attribute
    {
        public HcsnNameAttribute(string name)
        {
            PropName = name;
        }
        public string PropName { get; set; }

    }

    
}
