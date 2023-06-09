﻿using Hcsn.WebApplication.Common.Enums;

namespace Hcsn.WebApplication.Common.Entities.DTO.result.error
{
    /// <summary>
    /// Đối tượng chứa thông tin lỗi được trả về cho frontend
    /// Created by: LTVIET (10/03/2023)
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public ErrorCode? ErrorCode { get; set; }

        /// <summary>
        /// Thông báo lỗi cho dev
        /// </summary>
        public string? DevMsg { get; set; }

        /// <summary>
        /// Thông báo lỗi cho user
        /// </summary>
        public string? UserMsg { get; set; }

        /// <summary>
        /// Thông tin thêm
        /// </summary>
        public object? MoreInfo { get; set; }

        /// <summary>
        /// Id truy vết lỗi
        /// </summary>
        public string? TraceId { get; set; }


    }
}
