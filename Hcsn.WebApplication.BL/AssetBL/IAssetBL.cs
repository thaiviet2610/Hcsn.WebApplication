﻿using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.BL.AssetBL
{
    #region Method
    public interface IAssetBL : IBaseBL<FixedAsset>
    {
		/// <summary>
		/// Hàm lấy danh sách tài sản theo bộ lọc và phân trang
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã tài sản, tên tài sản)</param> 
		/// <param name="departmentId">Id của phòng ban</param> 
		/// <param name="fixedAssetCatagortId">Id của loại tài sản</param> 
		/// <returns> Đối tượng PagingResult bao gồm:
		/// - Danh sách tài sản trong 1 trang
		/// - Tổng số bản ghi thỏa mãn điều kiện
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		ServiceResult GetPaging(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber);

		/// <summary>
		/// Hàm xuất dữ liệu ra file excel
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã tài sản, tên tài sản)</param> 
		/// <param name="departmentId">Id của phòng ban</param> 
		/// <param name="fixedAssetCatagortId">Id của loại tài sản</param> 
		/// <returns>
		/// Kết quả việc thực hiện xuất file excel:
		/// Thành công: Trả về đường đẫn lưu file excel
		/// Thất bại: Thông báo lỗi
		/// </returns>
		Stream ExportExcel(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId);

		
		ServiceResult ImportExcel();
	} 
    #endregion
}
