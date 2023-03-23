﻿using Hcsn.WebApplication.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.BL.BaseBL
{
    public interface IBaseBL<T>
    {
        #region Method
        /// <summary>
        /// Hàm xử lý logic khi lấy ra danh sách tất cả các bản ghi
        /// </summary>
        /// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
        /// Created by: LTVIET (20/03/2023)
        ServiceResult GetAllRecord();

        /// <summary>
        /// Hàm xử lý logic khi lấy thông tin chi tiết 1 bản ghi theo id từ tầng DL 
        /// </summary>
        /// <param name="recordId">ID bản ghi muốn lấy</param>
        /// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
        /// Created by: LTVIET (20/03/2023)
        ServiceResult GetRecordById(Guid recordId);

        /// <summary>
        /// Hàm xử lý logic khi thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn thêm</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        ServiceResult InsertRecord(T record);

        /// <summary>
        /// Hàm xử lý logic khi sửa đổi 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn sửa đổi</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        ServiceResult UpdateRecord(Guid recordId, T record);

        /// <summary>
        /// Hàm xử lý logic khi xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">Id bản ghi muốn xóa</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        ServiceResult DeleteRecord(Guid recordId);

        /// <summary>
        /// Hàm xử lý logic khi lấy ra mã code ở lần nhập gần nhất
        /// </summary>
        /// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
        /// Created by: LTViet (20/03/2023)
        ServiceResult GetNewCode();

        /// <summary>
        /// Hàm xử lý logic khi kiểm tra xem code có bị trùng không ?
        /// </summary>
        /// <param name="recordCode">Code cần kiểm tra</param>
        /// <param name="recordId">Id </param>
        /// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
        /// Created by: LTViet (20/03/2023)
        ServiceResult IsSameCode(string recordCode, Guid recordId);
        #endregion
    }
}
