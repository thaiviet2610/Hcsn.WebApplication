using Hcsn.WebApplication.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities
{
	public class FixedAssetIncrementDetail : BaseEntity
	{
		/// <summary>
		/// Id chứng từ chi tiết
		/// </summary>
		[HcsnPrimaryKey]		
        public Guid voucher_detail_id { get; set; }

		/// <summary>
		/// Id chứng từ
		/// </summary>
		[HcsnForeignKey]
        public Guid voucher_id { get; set; }

		/// <summary>
		/// Id tài sản
		/// </summary>
		[HcsnForeignKey]
		public Guid fixed_asset_id { get; set; }

	}
}
