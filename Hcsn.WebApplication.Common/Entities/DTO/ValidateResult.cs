using Hcsn.WebApplication.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
    /// <summary>
    /// Đối tượng chứa thông tin lỗi nhận được khi validate dữ liệu
    /// Created by: LTVIET (10/03/2023)
    /// </summary>
    public class ValidateResult
    {
        /// <summary>
        /// Thành công hay không
        /// true: thành công
        /// false: thất bại
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Mã code của lỗi
        /// </summary>
        public ValidateCode? ValidateCode { get; set; }

        /// <summary>
        /// Thông báo lỗi chi tiết
        /// </summary>
        public string? Message { get; set; }

        public Object? Data { get; set; }
    }
}
