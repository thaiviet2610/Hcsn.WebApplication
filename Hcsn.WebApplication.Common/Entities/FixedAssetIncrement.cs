using Hcsn.WebApplication.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities
{
	public class FixedAssetIncrement : BaseEntity
	{
		/// <summary>
		/// Id chứng từ
		/// </summary>
		[HcsnPrimaryKey]
        public Guid voucher_id { get; set; }

		/// <summary>
		/// Mã chứng từ
		/// </summary>
		[HcsnDuplicate("code")]
		[HcsnCode]
		[HcsnRequired]
		[HcsnMaxLength(50)]
		[HcsnName("Mã chứng từ")]
        public string voucher_code { get; set; }

		/// <summary>
		/// Ngày chứng từ
		/// </summary>
		[HcsnRequired]
		[HcsnName("Ngày chứng từ")]
		public DateTime? voucher_date { get; set; }

		/// <summary>
		/// Ngày ghi tăng
		/// </summary>
		[HcsnRequired]
		[HcsnName("Ngày ghi tăng")]
		public DateTime? increment_date { get; set; }

		/// <summary>
		/// Tổng nguyên giá
		/// </summary>
		[HcsnName("Tổng nguyên giá")]
		[HcsnMaxLength(19)]
		public decimal price { get; set; }

		/// <summary>
		/// Ghi chú
		/// </summary>
		[HcsnMaxLength(255)]
		[HcsnName("Ghi chú")]
		public string? description { get; set; }
	}
}
