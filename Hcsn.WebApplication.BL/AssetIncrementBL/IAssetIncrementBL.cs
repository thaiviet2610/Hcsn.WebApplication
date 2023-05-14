using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.BL.AssetIncrementBL
{
	public interface IAssetIncrementBL
	{
		/// <summary>
		/// Hàm lấy danh sách chứng từ theo bộ lọc và phân trang
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã chứng từ, ghi chú)</param> 
		/// <param name="pageSize">Số bản ghi trong 1 trang</param> 
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns> 
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện thêm mới:
		/// IsSuccess == true:  thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		ServiceResult GetPaging(string? keyword, int pageSize, int pageNumber);

		/// <summary>
		/// Hàm thêm mới 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrementDTO">Đối tượng chứa thông tin chứng từ cần thêm mới(thông tin chứng từ, danh sách tài sản ghi tăng)</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện thêm mới:
		/// IsSuccess == true: thêm mới thành công
		/// IsSuccess == false: thêm mới thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		ServiceResult InsertAssetIncrement(FixedAssetIncrementDTO assetIncrementDTO);

		/// <summary>
		/// Hàm xử lý logic khi lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Kết quả sinh mã code mới</returns>
		/// Created by: LTVIET (20/04/2023)
		ServiceResult GetNewCode();

		/// <summary>
		/// Hàm xử lý logic khi lấy thông tin chi tiết 1 chứng từ theo id từ tầng DL 
		/// </summary>
		/// <param name="assetIncrementId">Id chứng từ muốn lấy</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		ServiceResult GetById(Guid assetIncrementId);

		/// <summary>
		/// Hàm sửa đổi 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrementDTO">Thông tin chứng từ cần sửa sửa đổi</param>
		/// <param name="assetsAdd">Danh sách id tài sản được chứng từ thêm</param>
		/// <param name="assetsDelete">Danh sách id tài sản không còn được chứng từ</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện sửa:
		/// IsSuccess == true: sửa thành công
		/// IsSuccess == false: sửa thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		ServiceResult UpdateAssetIncrement(FixedAssetIncrementDTO assetIncrementDTO, List<Guid>? assetsAdd, List<Guid>? assetsDelete);

		/// <summary>
		/// Hàm sửa đổi tổng nguyên giá của bản ghi chứng từ
		/// </summary>
		/// <param name="voucherId">Id của chứng từ cần sửa</param>
		/// <param name="price">Giá trị của tổng nguyên giá</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện sửa:
		/// IsSuccess == true: sửa thành công
		/// IsSuccess == false: sửa thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		ServiceResult UpdateAssetIncrementPrice(Guid voucherId, Decimal price);

		/// <summary>
		/// Hàm gọi database thực hiện việc xóa 1 bản ghi
		/// </summary>
		/// <param name="voucherId">Id bản ghi chứng từ muốn xóa</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện xóa:
		/// IsSuccess == true: xóa thành công
		/// IsSuccess == false: xóa thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		ServiceResult DeleteAssetIncrementById(Guid voucherId);

		/// <summary>
		/// Hàm gọi database để thực hiện việc xóa nhiều bản ghi
		/// </summary>
		/// <param name="ids">Danh sách id bản ghi cần xóa</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện xóa:
		/// IsSuccess == true: xóa thành công
		/// IsSuccess == false: xóa thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		ServiceResult DeleteMultipleAssetIncrement(List<Guid> ids);

		/// <summary>
		/// Hàm logic xử lý việc xuất toàn bộ danh sách chứng từ ra file excel
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã tài sản, tên tài sản)</param> 
		/// <returns>
		/// Kết quả việc thực hiện xuất file excel:
		/// Thành công: Trả về đường đẫn lưu file excel
		/// Thất bại: Thông báo lỗi
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		Stream ExportAllExcel(string? keyword);

		/// <summary>
		/// Hàm logic xử lý việc xuất dữ liệu của 1 chứng từ ra file excel
		/// </summary>
		/// <param name="voucherId">Id chứng từ</param>
		/// <returns>Đối tượng stream lưu dữ liệu</returns>
		/// Created by: LTVIET (29/04/2023)
		Stream ExportDetailExcel(Guid voucherId);

	}
}
