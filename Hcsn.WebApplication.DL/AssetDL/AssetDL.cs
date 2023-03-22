using Dapper;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.DL.BaseDL;
using Hcsn.WebApplication.DL.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Common;

namespace Hcsn.WebApplication.DL.AssetDL
{
    public class AssetDL : BaseDL<Asset>, IAssetDL
    {
        #region Field
        private IBaseRepository<Asset> _assetRepository;
        #endregion
        public AssetDL(IBaseRepository<Asset> baseRepository) : base(baseRepository)
        {
            _assetRepository = baseRepository;
        }

        public PagingResult GetPaging(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber)
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
            var dbConnection = _assetRepository.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            var result = _assetRepository.QueryMultiple(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            var data = result.Read<Asset>().ToList();
            var totalRecord = result.Read<int>().ToList()[0];
            dbConnection.Close();
            // Xử lý kết quả trả về
            return new PagingResult
            {
                Data = data,
                TotalRecord = totalRecord,
            };

        }
    }
}
