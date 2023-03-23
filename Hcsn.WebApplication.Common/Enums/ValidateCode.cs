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
        DuplicateCode = 5,

        /// <summary>
        /// Lỗi dữ liệu kiểu number thuộc trường bắt buộc lớn hơn 0 nhưng có giá trị <= 0
        /// </summary>
        NumberEqual0 = 6,

        /// <summary>
        /// Lỗi validate tỷ lệ hao mòn != (1/số năm sử dụng)*100
        /// </summary>
        DepreciationRateDifferentLifeTime = 7,
    }
}
