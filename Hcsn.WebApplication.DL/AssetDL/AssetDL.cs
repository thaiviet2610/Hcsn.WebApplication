using Dapper;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Common;
using System.Collections;

namespace Hcsn.WebApplication.DL.AssetDL
{
    public class AssetDL : BaseDL<Asset>, IAssetDL
    {
        #region Mehthod
        /// <summary>
        /// Hàm lấy danh sách tài sản theo bộ lọc và phân trang
        /// </summary>
        /// <param name="keyword"></param> Từ khóa tìm kiếm (mã tài sản, tên tài sản)
        /// <param name="departmentId"></param> Id của phòng ban
        /// <param name="fixedAssetCatagortId"></param> Id của loại tài sản
        /// <returns> 
        /// Đối tượng PagingResult bao gồm:
        /// - Danh sách tài sản trong 1 trang
        /// - Tổng số bản ghi thỏa mãn điều kiện
        /// </returns>
        /// Created by: LTVIET (09/03/2023)
        public PagingResultAsset GetPaging(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.Filter, typeof(Asset).Name);
            // Chuẩn bị tham số đầu vào cho stored
            int limit = pageSize;
            int offset = (pageNumber - 1) * limit;
            var parameters = new DynamicParameters();
            parameters.Add("p_department_id", departmentId);
            parameters.Add("p_asset_category_id", fixedAssetCatagortId);
            parameters.Add("p_keyword", keyword);
            parameters.Add("p_limit", limit);
            parameters.Add("p_offset", offset);
            // Khởi tạo kết nối tới Database
            var dbConnection = GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            var result = QueryMultiple(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            
            var data = result.Read<Asset>().ToList();

            var totalRecord = result.Read<int>().Single();

            var totalResult = result.Read<Asset>().Single();

            dbConnection.Close();
            // Xử lý kết quả trả về
            return new PagingResultAsset
            {
                Data = data,
                TotalRecord = totalRecord,
                QuantityTotal = totalResult.quantity_total,
                CostTotal = totalResult.cost_total,
                DepreciationValueTotal = totalResult.depreciation_value_total
            };

        } 
        #endregion
    }
}
