using Hcsn.WebApplication.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Hcsn.WebApplication.Common.Entities
{
    /// <summary>
    /// Thông tin phòng ban
    /// </summary>
    public class Department
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [HcsnPrimaryKey]
        [HcsnRequired]
        [HcsnName("Id phòng ban")]
        public Guid department_id { get; set; }


        /// <summary>
        /// Mã phòng ban
        /// </summary>
        [HcsnRequired]
        [HcsnName("Mã phòng ban")]
        [HcsnCode]
        public string department_code { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [HcsnRequired]
        [HcsnName("Tên phòng ban")]
        public string department_name { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// Có phải là cha không
        /// </summary>
        [AllowNull]
        public bool? is_parent { get; set; }

        /// <summary>
        /// Id phòng ban cha
        /// </summary>
        [AllowNull]
        public Guid? parent_id { get; set; }

        /// <summary>
        /// Id của đơn vị
        /// </summary>
        [AllowNull]
        public Guid? organization_id { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [AllowNull]
        public string? created_by { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        public string? modified_by { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? modified_date { get; set; }
    }
}
