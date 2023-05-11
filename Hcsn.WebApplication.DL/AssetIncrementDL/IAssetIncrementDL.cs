using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.DL.AssetIncrementDL
{
	public interface IAssetIncrementDL
	{
		/// <summary>
		/// Hàm lấy danh sách chứng từ theo bộ lọc và phân trang
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã chứng từ, ghi chú)</param> 
		/// <param name="pageSize">Số bản ghi trong 1 trang</param> 
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns> 
		/// Đối tượng PagingResultAssetIncrement bao gồm:
		/// - Danh sách chứng từ trong 1 trang
		/// - Tổng số bản ghi thỏa mãn điều kiện
		/// - Tổng nguyên giá
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		public PagingResultAssetIncrement GetPaging
			(string? keyword, int pageSize, int pageNumber);

		/// <summary>
		/// Hàm thêm mới 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrement">Đối tượng thông tin chứng từ</param>
		/// <param name="assets">Danh sách tài sản được ghi tăng trong chứng từ</param>
		/// <returns>
		/// Kết quả việc thêm mới:
		/// true: thêm mới thành công
		/// false: thêm mới thất bại
		/// </returns>
		public bool InsertAssetIncrement(FixedAssetIncrement assetIncrement, List<FixedAssetDTO> assets);

		/// <summary>
		/// Hàm truy cập database lấy ra số bản ghi có cùng code nhưng khác id được truyền vào
		/// </summary>
		///	<param name="assetIncrement"> Đối tượng tài sản chứa thuôc tính cần kiểm tra trùng </param>
		/// <param name="propertName">Tên thuộc tính cần kiểm tra</param>
		/// <returns>Số bản ghi cần tìm</returns>
		/// Created by: LTViet (20/03/2023)
		int GetNumberRecordOfPropertyDuplicate(FixedAssetIncrement assetIncrement, string propertName);

		/// <summary>
		/// Hàm truy cập database lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Mã code của đối tượng</returns>
		/// Created by: LTViet (20/03/2023)
		string? GetNewCode();

		/// <summary>
		/// Hàm truy cập database lấy bản ghi chứng từ theo id
		/// </summary>
		/// <param name="voucherId">id của chứng từ</param>
		/// <returns>Bản ghi chứng từ cần tìm</returns>
		FixedAssetIncrementDTO GetById(Guid voucherId);

		/// <summary>
		/// Hàm sửa đổi 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrementDTO">Bản ghi muốn sửa đổi</param>
		/// <param name="assetsAdd">Danh sách id tài sản được chứng từ thêm</param>
		/// <param name="assetsDelete">Danh sách id tài sản không còn được chứng từ</param>
		/// <returns>
		/// true: update thành công
		/// false: update thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		bool UpdateAssetIncrement(FixedAssetIncrementDTO assetIncrementDTO,List<Guid>? assetsAdd, List<Guid>? assetsDelete);

		/// <summary>
		/// Hàm sửa đổi tổng nguyên giá của bản ghi chứng từ
		/// </summary>
		/// <param name="voucherId">Id của chứng từ cần sửa</param>
		/// <param name="price">Giá trị của tổng nguyên giá</param>
		/// <returns>
		/// 1: update thành công
		/// 0: update thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		public int UpdateAssetIncrementPrice(Guid voucherId, Decimal price);

		/// <summary>
		/// Hàm gọi database thực hiện việc xóa 1 bản ghi
		/// </summary>
		/// <param name="voucherId">Id bản ghi chứng từ muốn xóa</param>
		/// <returns>
		/// Kết quả thực hiện việc xóa
		/// True: thành công
		/// False: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public bool DeleteAssetIncrementById(Guid voucherId);

		/// <summary>
		/// Hàm gọi database để thực hiện việc xóa nhiều bản ghi
		/// </summary>
		/// <param name="ids">Danh sách id bản ghi cần xóa</param>
		/// <returns>
		/// Kết quả việc thực hiện xóa nhiều bản ghi
		/// True: Nếu delete thành công
		/// False: Nếu delete thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		bool DeleteMultipleAssetIncrement(List<Guid> ids);
	}
}
