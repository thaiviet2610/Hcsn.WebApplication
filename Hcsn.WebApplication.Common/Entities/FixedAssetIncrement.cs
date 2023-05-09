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
		[HcsnPrimaryKey]
        public Guid voucher_id { get; set; }

		[HcsnDuplicate("code")]
		[HcsnCode]
		[HcsnRequired]
		[HcsnMaxLength(50)]
		[HcsnName("Mã chứng từ")]
        public string voucher_code { get; set; }

		[HcsnRequired]
		[HcsnName("Ngày chứng từ")]
		public DateTime? voucher_date { get; set; }

		[HcsnRequired]
		[HcsnName("Ngày ghi tăng")]
		public DateTime? increment_date { get; set; }

		[HcsnName("Tổng nguyên giá")]
		[HcsnMaxLength(19)]
		public decimal price { get; set; }

		[HcsnMaxLength(255)]
		[HcsnName("Ghi chú")]
		public string? description { get; set; }
	}
}
