using Hcsn.WebApplication.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class CostSourceDTO : Budget
	{
		/// <summary>
		/// Giá trị của nguồn tiền
		/// </summary>
		[HcsnRequired]
        public decimal? mount { get; set; }

	}
}
