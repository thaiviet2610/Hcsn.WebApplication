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
    public interface IAssetDL : IBaseDL<Asset>
    {
        /// <summary>
        /// Hàm lấy danh sách tài sản theo bộ lọc và phân trang
        /// </summary>
        /// <param name="keyword"></param> Từ khóa tìm kiếm (mã tài sản, tên tài sản)
        /// <param name="departmentId"></param> Id của phòng ban
        /// <param name="fixedAssetCatagortId"></param> Id của loại tài sản
        /// <returns> Đối tượng PagingResult bao gồm:
        /// - Danh sách tài sản trong 1 trang
        /// - Tổng số bản ghi thỏa mãn điều kiện
        /// </returns>
        /// Created by: LTVIET (09/03/2023)
        PagingResult GetPaging(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber);
    }
}
