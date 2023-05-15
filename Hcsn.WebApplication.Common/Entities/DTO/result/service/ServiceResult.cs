using Hcsn.WebApplication.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO.result.service
{
    /// <summary>
    /// Đối tượng chứa thông tin lỗi nhận đucợ ở tầng BL trả về cho tầng Controller 
    /// Created by: LTVIET (10/03/2023)
    /// </summary>
    public class ServiceResult
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
        public ErrorCode? ErrorCode { get; set; }

        /// <summary>
        /// Thông tin đối tượng trả về
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Thông báo lỗi chi tiết
        /// </summary>
        public string? Message { get; set; }
    }
}
