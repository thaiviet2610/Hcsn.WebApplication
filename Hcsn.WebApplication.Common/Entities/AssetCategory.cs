using Hcsn.WebApplication.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Hcsn.WebApplication.Common.Entities
{
    /// <summary>
    /// Thông tin loại tài sản
    /// </summary>
    public class AssetCategory : BaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [HcsnPrimaryKey]
        [HcsnRequired]
        [HcsnName("Id loại tài sản")]
        public Guid fixed_asset_category_id { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        [HcsnRequired]
        [HcsnName("Mã loại tài sản")]
        [HcsnCode]
        [HcsnMaxLength(50)]
        public string fixed_asset_category_code { get; set; }

        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        [HcsnRequired]
        [HcsnName("Tên loại tài sản")]
		[HcsnMaxLength(255)]
		public string fixed_asset_category_name { get; set; }

        /// <summary>
        /// Id của đơn vị
        /// </summary>
        [AllowNull]
        public Guid? organization_id { get; set; }

        /// <summary>
        /// Tỷ lệ hao mòn (%)
        /// </summary>
        [HcsnRequired]
        [HcsnNumber("rate")]
        [HcsnName("Tỷ lệ hao mòn")]
        public float depreciation_rate { get; set; }


        /// <summary>
        /// Số năm sử dụng
        /// </summary>
        [HcsnRequired]
        [HcsnNumber("int")]
        [HcsnName("Số năm sử dụng")]
        public int life_time { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [AllowNull]
        public string? description { get; set; }
    }
}
