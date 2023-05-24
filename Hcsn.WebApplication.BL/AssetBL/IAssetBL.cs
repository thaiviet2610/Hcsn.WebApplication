using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO.result.service;
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
		/// <param name="pageSize">Số bản ghi trong 1 trang</param> 
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns> 
		/// Đối tượng PagingResult bao gồm:
		/// - Danh sách tài sản trong 1 trang
		/// - Tổng số bản ghi thỏa mãn điều kiện
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		ServiceResult GetPaging(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber);

		/// <summary>
		/// Hàm xử lý logic khi lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Kết quả sinh mã code mới</returns>
		/// Created by: LTViet (20/03/2023)
		ServiceResult GetNewCode();

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
		/// Created by: LTViet (20/03/2023)
		Stream ExportExcel(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId);

		/// <summary>
		/// Hàm lấy danh sách tài sản chưa chứng từ theo bộ lọc và phân trang
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã tài sản, tên tài sản)</param> 
		/// <param name="pageSize">Số bản ghi trong 1 trang</param> 
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <param name="idAssetsNotIn">Danh sách các id của các tài sản chưa active không cần lấy ra</param>
		/// <param name="idAssetsActive">Danh sách các id của các tài sản đã active cần lấy ra</param>
		/// <returns> 
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTVIET (19/04/2023)
		public ServiceResult GetAllAssetNoActive(string? keyword, int pageSize, int pageNumber, List<Guid>? idAssetsNotIn, List<Guid>? idAssetsActive);


	} 
    #endregion
}
