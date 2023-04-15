using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Constants
{
	/// <summary>
	/// Attribute thể hiện các trường bắt buộc nhập
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnRequiredAttribute : Attribute
    {


    }

	/// <summary>
	/// Attribute thể hiện các trường không được phép trùng
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
	public class HcsnDuplicateAttribute : Attribute
	{


	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnNumberAttribute : Attribute
    {
		/// <summary>
		/// Attribute thể hiện các trường là kiểu number
		/// </summary>
		/// <param name="name"> Tên loại dữ liệu trong number(int,float,decimal...)</param>
		public HcsnNumberAttribute(object type)
        {
            PropType = type;
        }
        public object PropType { get; set; }
    }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
	public class HcsnValueTypeAttribute : Attribute
	{
		/// <summary>
		/// Attribute thể hiện kiểu dữ liệu của các trường
		/// </summary>
		/// <param name="name"> Tên kiểu dữ liệu trong number(text,date,rate...)</param>
		public HcsnValueTypeAttribute(object type)
		{
			PropType = type;
		}
		public object PropType { get; set; }
	}

	/// <summary>
	/// Attribute thể hiện các trường là khóa chính
	/// </summary>

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnPrimaryKeyAttribute : Attribute
    {


    }

	/// <summary>
	/// Attribute thể hiện các trường có giá trị phải lớn hơn 0
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnGreateThanZeroAttribute : Attribute
    {


    }

	/// <summary>
	/// Attribute thể hiện các trường là khóa ngoại
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnForeignKeyAttribute : Attribute
    {


    }

	/// <summary>
	/// Attribute thể hiện các trường là code 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnCodeAttribute : Attribute
    {


    }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
	public class HcsnMaxLengthAttribute : Attribute
	{
		/// <summary>
		/// Attribute thể hiện các trường có giới hạn về độ dài ký tự
		/// </summary>
		/// <param name="length"> Độ dài giới hạn ký tự</param>
		public HcsnMaxLengthAttribute(int length)
		{
			Length = length;
		}
		public int Length { get; set; }

	}

	
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HcsnNameAttribute : Attribute
    {
		/// <summary>
		/// Attribute thể hiện tên của các trường 
		/// </summary>
		/// <param name="name">Tên muốn đặt cho các trường</param>
		public HcsnNameAttribute(string name)
        {
            PropName = name;
        }
        public string PropName { get; set; }

    }

    
}
