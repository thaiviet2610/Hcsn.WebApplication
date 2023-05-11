using Hcsn.WebApplication.Common.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class FixedAssetDTO
	{
		/// <summary>
		/// Số thứ tự
		/// </summary>
		public int index { get; set; }

		/// <summary>
		/// Khóa chính
		/// </summary>
		[HcsnPrimaryKey]
		[HcsnRequired]
		[HcsnName("Id tài sản")]
		public Guid fixed_asset_id { get; set; }

		/// <summary>
		/// Mã tài sản
		/// </summary>
		[HcsnRequired]
		[HcsnName("Mã tài sản")]
		[HcsnCode]
		[HcsnMaxLength(10)]
		public string fixed_asset_code { get; set; }

		/// <summary>
		/// Tên tài sản
		/// </summary>
		[HcsnRequired]
		[HcsnName("Tên tài sản")]
		[HcsnMaxLength(100)]
		public string fixed_asset_name { get; set; }

		/// <summary>
		/// Tên bộ phận sử dụng
		/// </summary>
		[HcsnRequired]
		[HcsnName("Tên bộ phận sử dụng")]
		public string department_name { get; set; }

		/// <summary>
		/// Tên loại tài sản
		/// </summary>
		[HcsnRequired]
		[HcsnName("Tên loại tài sản")]
		public string fixed_asset_category_name { get; set; }

		/// <summary>
		/// Nguyên giá
		/// </summary>
		[HcsnRequired]
		[HcsnNumber("decimal")]
		[HcsnGreateThanZero]
		[HcsnName("Nguyên giá")]
		public decimal cost { get; set; }

		/// <summary>
		/// Nguyên giá
		/// </summary>
		public string cost_source { get; set; }

		/// <summary>
		/// Số lượng
		/// </summary>
		[HcsnRequired]
		[HcsnNumber("int")]
		[HcsnGreateThanZero]
		[HcsnName("Số lượng")]
		public int quantity { get; set; }

		/// <summary>
		/// Hao mòn lũy kế
		/// </summary>
		[HcsnNumber("decimal")]
		[HcsnName("Hao mòn lũy kế")]
		public decimal depreciation_value { get; set; }

		/// <summary>
		/// Giá trị còn lại
		/// </summary>
		public decimal residual_value { get; set; }

		/// <summary>
		/// Trạng thái sử dụng
		/// </summary>
		[AllowNull]
		public bool active { get; set; }

		/// <summary>
		/// Mã chứng từ
		/// </summary>
		public string? voucher_code { get; set; }


		
	}
}
