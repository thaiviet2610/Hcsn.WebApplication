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
		/// Mã chứng từ
		/// </summary>
		[HcsnMaxLength(50)]
		[HcsnRequired]
		[HcsnName("Mã chứng từ")]
		public string voucher_code { get; set; }

		/// <summary>
		/// Id tài sản
		/// </summary>
		[HcsnForeignKey]
		public Guid fixed_asset_id { get; set; }

		/// <summary>
		/// Mã tài sản
		/// </summary>
		[HcsnMaxLength(50)]
		[HcsnRequired]
		[HcsnName("Mã tài sản")]
		public string fixed_asset_code { get; set; }

		/// <summary>
		/// Tên tài sản
		/// </summary>
		[HcsnMaxLength(255)]
		[HcsnName("Tên tài sản")]
		public string fixed_asset_name { get; set; }

		/// <summary>
		/// Tên bộ phận sử dụng
		/// </summary>
		[HcsnMaxLength(255)]
		[HcsnName("Tên bộ phận sử dụng")]
		public string department_name { get; set; }

		/// <summary>
		/// Nguyên giá
		/// </summary>
		[HcsnName("Nguyên giá")]

		public string cost { get; set; }

		/// <summary>
		/// Số lượng
		/// </summary>
		public int quantity { get; set; }

		/// <summary>
		/// Tỷ lệ hao mòn
		/// </summary>
		public float depreciation_rate { get; set; }

		/// <summary>
		/// Năm sử dụng
		/// </summary>
		public DateTime production_year { get; set; }

		/// <summary>
		/// Ngày chứng từ
		/// </summary>
		public DateTime voucher_date { get; set; }

		/// <summary>
		/// Ngày ghi tăng
		/// </summary>
		public DateTime increment_date { get; set; }
	}
}
