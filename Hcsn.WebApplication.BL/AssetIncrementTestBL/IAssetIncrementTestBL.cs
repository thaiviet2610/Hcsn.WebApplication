using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.BL.AssetIncrementTest1BL
{
    public interface IAssetIncrementTestBL : IBaseBL<FixedAssetIncrement>
    {
		/// <summary>
		/// Hàm lấy danh sách chứng từ theo bộ lọc và phân trang
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã chứng từ, ghi chú)</param> 
		/// <param name="pageSize">Số bản ghi trong 1 trang</param> 
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns> 
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện thêm mới:
		/// IsSuccess == true: thêm mới thành công
		/// IsSuccess == false: thêm mới thất bại
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		public ServiceResult GetPaging(string? keyword, int pageSize, int pageNumber);

		/// <summary>
		/// Hàm thêm mới 1 bản ghi chứng từ
		/// </summary>
		/// <param name="entity">Đối tượng chứa thông tin chứng từ cần thêm mới(thông tin chứng từ, danh sách tài sản ghi tăng)</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện thêm mới:
		/// IsSuccess == true: thêm mới thành công
		/// IsSuccess == false: thêm mới thất bại
		/// </returns>
		public ServiceResult InsertAssetIncrement(FixedAssetIncrementDTO entity);

		/// <summary>
		/// Hàm xử lý logic khi lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Kết quả sinh mã code mới</returns>
		/// Created by: LTViet (20/03/2023)
		ServiceResult GetNewCode();

		ServiceResult GetAssetIncrementDTOById(Guid assetIncrementId);
	}
}
