using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Enums
{
    /// <summary>
    /// Thông tin các mã code lỗi
    /// </summary>
    public enum ValidateCode
    {
        /// <summary>
        /// Lỗi validate các trường để trống
        /// </summary>
        Empty = 1,

        /// <summary>
        /// Lỗi validate nguyên giá tài sản nhỏ hơn hao mòn năm
        /// </summary>
        DepreciationYearGreaterThanCost = 2,

        /// <summary>
        /// Lỗi dữ liệu kiểu number vượt quá độ dài cho phép
        /// </summary>
        OutMaxLength = 3,

        /// <summary>
        /// Lỗi dữ liệu kiểu tỷ lệ phần trăm nằm ngoài khoảng 0-100 
        /// </summary>
        OutOfRate = 4,

        /// <summary>
        /// Lỗi trùng mã
        /// </summary>
        Duplicate = 5,

        /// <summary>
        /// Lỗi dữ liệu kiểu number thuộc trường bắt buộc lớn hơn 0 nhưng có giá trị <= 0
        /// </summary>
        NumberLessThanOrEqualZero = 6,

        /// <summary>
        /// Lỗi validate tỷ lệ hao mòn != (1/số năm sử dụng)*100
        /// </summary>
        DepreciationRateDifferentLifeTime = 7,

        /// <summary>
        /// Lỗi validate ngày mua lớn hơn ngày bắt đầu sử dụng
        /// </summary>
        PurchaseDateGreaterThanProductionYear = 8,

        /// <summary>
        /// Lỗi vượt quá độ dài ký tự cho phép
        /// </summary>
        MaxLength = 9,

		/// <summary>
		/// Lỗi khi insert chứng từ thì danh sách tài sản rỗng
		/// </summary>
		NoAssetIncrements = 10
    }
}
