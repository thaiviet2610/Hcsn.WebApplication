using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.DL.AssetDL
{
    public interface IAssetDL : IBaseDL<FixedAsset>
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
		/// Created by: LTVIET (09/03/2023)
		PagingResultAsset GetPaging
            (string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber);

		/// <summary>
		/// Hàm truy cập database lấy ra số bản ghi có cùng code nhưng khác id được truyền vào
		/// </summary>
		///	<param name="asset"> Đối tượng tài sản chứa thuôc tính cần kiểm tra trùng </param>
		/// <param name="propertName">Tên thuộc tính cần kiểm tra</param>
		/// <returns>Số bản ghi cần tìm</returns>
		/// Created by: LTViet (20/03/2023)
		int GetNumberRecordOfPropertyDuplicate(FixedAsset asset, string propertName);

		/// <summary>
		/// Hàm truy cập database lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Mã code của đối tượng</returns>
		/// Created by: LTViet (20/03/2023)
		string? GetNewCode();
	}
}
