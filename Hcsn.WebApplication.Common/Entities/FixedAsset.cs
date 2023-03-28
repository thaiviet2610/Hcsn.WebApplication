using Hcsn.WebApplication.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Hcsn.WebApplication.Common.Entities
{
    /// <summary>
    /// Thông tin tài sản
    /// </summary>
    public class FixedAsset : BaseEntity
    {
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
        [HcsnMaxLength(100)]
        public string fixed_asset_code { get; set; }

        /// <summary>
        /// Tên tài sản
        /// </summary>
        [HcsnRequired]
        [HcsnName("Tên tài sản")]
        [HcsnMaxLength(255)]
        public string fixed_asset_name { get; set; }

        /// <summary>
        /// Id của đơn vị
        /// </summary>
        [AllowNull]
        public Guid? organization_id { get; set; }

        /// <summary>
        /// Mã đơn vị
        /// </summary>
        [AllowNull]
        public string? organization_code { get; set; }

        /// <summary>
        /// Tên của đơn vị
        /// </summary>
        [AllowNull]
        public string? organization_name { get; set; }

        /// <summary>
        /// Id bộ phận sử dụng
        /// </summary>
        [HcsnRequired]
        [HcsnName("Id bộ phận sử dụng")]
        [HcsnForeignKey]
        public Guid department_id { get; set; }

        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        [HcsnRequired]
        [HcsnName("Mã bộ phận sử dụng")]
        public string department_code { get; set; }

        /// <summary>
        /// Tên bộ phận sử dụng
        /// </summary>
        [HcsnRequired]
        [HcsnName("Tên bộ phận sử dụng")]
        public string department_name { get; set; }

        /// <summary>
        /// Id loại tài sản
        /// </summary>
        [HcsnRequired]
        [HcsnName("Id loại tài sản")]
        [HcsnForeignKey]
        public Guid fixed_asset_category_id { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        [HcsnRequired]
        [HcsnName("Mã loại tài sản")]
        public string fixed_asset_category_code { get; set; }

        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        [HcsnRequired]
        [HcsnName("Tên loại tài sản")]
        public string fixed_asset_category_name { get; set; }


        /// <summary>
        /// Ngày mua
        /// </summary>
        [HcsnRequired]
        [HcsnName("Ngày mua")]
        public DateTime purchase_date { get; set; }

        /// <summary>
        /// Nguyên giá
        /// </summary>
        [HcsnRequired]
        [HcsnNumber("decimal")]
        [HcsnGreateThanZero]
        [HcsnName("Nguyên giá")]
        public decimal cost { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        [HcsnRequired]
        [HcsnNumber("int")]
        [HcsnGreateThanZero]
        [HcsnName("Số lượng")]
        public int quantity { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        [HcsnRequired]
        [HcsnNumber("rate")]
        [HcsnName("Tỷ lệ hao mòn")]
        public float depreciation_rate { get; set; }

		/// <summary>
		/// Hao mòn lũy kế
		/// </summary>
		[HcsnNumber("decimal")]
		[HcsnName("Hao mòn lũy kế")]
		public decimal depreciation_value { get; set; }

		/// <summary>
		/// Năm bắt đầu theo dõi tài sản trên phần mềm
		/// </summary>
		[HcsnRequired]
        [HcsnNumber("int")]
        [HcsnGreateThanZero]
        [HcsnName("Năm theo dõi")]

        public int tracked_year { get; set; }

        /// <summary>
        /// Số năm sử dụng
        /// </summary>
        [HcsnRequired]
        [HcsnNumber("int")]
        [HcsnName("Số năm sử dụng")]
        public int life_time { get; set; }

        /// <summary>
        /// Năm sử dụng
        /// </summary>
        [HcsnRequired]
        [HcsnName("Năm sử dụng")]
        public DateTime production_year { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        [AllowNull]
        public bool? active { get; set; }

        public int quantity_total { get; set; }

        public decimal cost_total { get; set; }

        public double depreciation_value_total { get; set; }
    }
}
