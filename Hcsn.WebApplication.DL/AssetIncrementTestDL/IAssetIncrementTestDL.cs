using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.DL.AssetIncrementTest1DL
{
	public interface IAssetIncrementTestDL : IBaseDL<FixedAssetIncrement>
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
		/// <param name="idAssets">Danh sách tài sản được ghi tăng trong chứng từ</param>
		/// <returns>Số bản ghi được thêm mới</returns>
		public int InsertAssetIncrement(FixedAssetIncrement assetIncrement, List<FixedAssetDTO> idAssets);

		/// <summary>
		/// Hàm truy cập database lấy ra số bản ghi có cùng code nhưng khác id được truyền vào
		/// </summary>
		///	<param name="asset"> Đối tượng tài sản chứa thuôc tính cần kiểm tra trùng </param>
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

		public FixedAssetIncrementDTO GetAssetIncrementDTOById(Guid voucerId);
	}
}
