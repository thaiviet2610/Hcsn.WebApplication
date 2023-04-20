using Hcsn.WebApplication.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities
{
	public class Budget : BaseEntity
	{
		/// <summary>
		/// Id nguồn hình thành
		/// </summary>
		[HcsnPrimaryKey]
		public Guid budget_id { get; set; }

		/// <summary>
		/// Mã nguồn hình thành
		/// </summary>
        public string budget_code { get; set; }

		/// <summary>
		/// Tên nguồn hình thành
		/// </summary>
        public string budget_name { get; set; }
	}
}
