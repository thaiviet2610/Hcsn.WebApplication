using Dapper;
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
using Hcsn.WebApplication.DL.DBConfig;
using Hcsn.WebApplication.Common.Constants.ProcedureName;
using Newtonsoft.Json;
using Hcsn.WebApplication.Common.Constants;

namespace Hcsn.WebApplication.DL.AssetDL
{
    public class AssetDL : BaseDL<FixedAsset>, IAssetDL
    {
		#region Field
		private IRepositoryDB _assetRepository;
		#endregion
		public AssetDL(IRepositoryDB repositoryDB) : base(repositoryDB)
		{
			_assetRepository = repositoryDB;
		}
		#region Mehthod
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
		/// - Tổng số lượng
		/// - Tổng nguyên giá
		/// - Tổng hao mòn lũy kế
		/// - Tổng giá trị còn lại
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		public PagingResultAsset GetPaging
            (string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.Filter, typeof(FixedAsset).Name);
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
			var assetDTOs = result.Read<FixedAssetDTO>().ToList();
			int totalRecord = result.Read<int>().Single();
			int quantityTotal = result.Read<int>().Single();
			decimal costTotal = result.Read<decimal>().Single();
			decimal depreciationValueTotal = result.Read<decimal>().Single();
			decimal residualValueTotal = result.Read<decimal>().Single();
			
			dbConnection.Close();
            // Xử lý kết quả trả về
            return new PagingResultAsset
            {
                Data = assetDTOs,
                TotalRecord = totalRecord,
                QuantityTotal = quantityTotal,
                CostTotal = costTotal,
                DepreciationValueTotal = depreciationValueTotal,
                ResidualValueTotal = residualValueTotal,
            };
        }

		/// <summary>
		/// Hàm lấy ra số bản ghi có cùng code nhưng khác id được truyền vào
		/// </summary>
		/// <param name="asset"> Đối tượng tài sản chứa thuôc tính cần kiểm tra trùng </param>
		/// <param name="propertyName">Tên thuộc tính cần kiểm tra</param>
		/// <returns>Số bản ghi cần tìm</returns>
		/// Created by: LTViet (20/03/2023)
		public int GetNumberRecordOfPropertyDuplicate(FixedAsset asset, string propertyName)
		{
			var duplicateProperty = typeof(FixedAsset).GetProperties().FirstOrDefault(prop => prop.Name == propertyName);
			var propertyValue = duplicateProperty.GetValue(asset);
			var assetId = asset.fixed_asset_id;
			var attHcsnDuplicate = duplicateProperty.GetCustomAttributes(typeof(HcsnDuplicateAttribute), false).FirstOrDefault();
			var propDuplicateName = (attHcsnDuplicate as HcsnDuplicateAttribute).Name;
			string storedProcedureNameGetNumberRecordOfPropertyDuplicate = String.Format(ProcedureName.CheckDuplicate, typeof(FixedAsset).Name, propDuplicateName);
			var parametersCheckSameCode = new DynamicParameters();
			parametersCheckSameCode.Add($"p_{propertyName}", propertyValue);
			parametersCheckSameCode.Add("p_id", assetId);
			var dbConnection = _assetRepository.GetOpenConnection();
			// Thực hiện gọi vào Database để chạy stored procedure
			int numberOfAffectedRowsDuplicate =
				_assetRepository.QueryFirstOrDefault<int>(dbConnection, storedProcedureNameGetNumberRecordOfPropertyDuplicate,
										 parametersCheckSameCode, commandType: CommandType.StoredProcedure);
			dbConnection.Close();
			return numberOfAffectedRowsDuplicate;
		}

		/// <summary>
		/// Hàm lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Mã code của đối tượng</returns>
		/// Created by: LTViet (20/03/2023)
		public string? GetNewCode()
		{
			// Chuẩn bị tên stored procedure
			string storedProcedureName = String.Format(ProcedureName.GetNewCode, typeof(FixedAsset).Name);
			// Chuẩn bị tham số đầu vào cho stored
			// Khởi tạo kết nối tới Database
			var dbConnection = _assetRepository.GetOpenConnection();
			// Thực hiện gọi vào Database để chạy stored procedure
			var asset = _assetRepository.QueryFirstOrDefault<FixedAsset>(dbConnection, storedProcedureName, commandType: CommandType.StoredProcedure);
			dbConnection.Close();

			// Xử lý kết quả trả về 
			if (asset == null)
			{
				// Nếu không có đối tượng nào trong database thì trả về kết quả null
				return null;
			}
			else
			{
				return asset.fixed_asset_code;
			}
		}

		/// <summary>
		/// Hàm lấy danh sách tài sản theo bộ lọc và phân trang
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã tài sản, tên tài sản)</param> 
		/// <param name="pageSize">Số bản ghi trong 1 trang</param> 
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <param name="notInIdAssets">Danh sách các id của các tài sản chưa active không cần lấy ra</param>
		/// <param name="activeIdAssets">Danh sách các id của các tài sản đã active cần lấy ra</param>
		/// <returns> 
		/// Đối tượng PagingResult bao gồm:
		/// - Danh sách tài sản trong 1 trang không nằm trong danh sách cho trước
		/// - Tổng số bản ghi thỏa mãn điều kiện
		/// - Tổng số lượng
		/// - Tổng nguyên giá
		/// - Tổng hao mòn lũy kế
		/// - Tổng giá trị còn lại
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		public PagingResultAsset GetAllAssetNotIn(string? keyword, int pageSize, int pageNumber, List<Guid>? notInIdAssets, List<Guid>? activeIdAssets)
		{
			// Chuẩn bị tên stored procedure
			string storedProcedureName = String.Format(ProcedureName.FilterRecordNotIn, typeof(FixedAsset).Name);
			// Chuẩn bị tham số đầu vào cho stored
			int limit = pageSize;
			int offset = (pageNumber - 1) * limit;
			var parameters = new DynamicParameters();
			parameters.Add("p_keyword", keyword);
			parameters.Add("p_limit", limit);
			parameters.Add("p_offset", offset);
			parameters.Add("p_not_in_ids", notInIdAssets==null ? null: $"('{string.Join("','", notInIdAssets)}')");
			parameters.Add("p_active_ids", activeIdAssets == null ? null: $"('{string.Join("','", activeIdAssets)}')");

			// Khởi tạo kết nối tới Database
			var dbConnection = _assetRepository.GetOpenConnection();
			// Thực hiện gọi vào Database để chạy stored procedure
			var result = _assetRepository.QueryMultiple(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
			var assetDTOs = result.Read<FixedAssetDTO>().ToList();
			int totalRecord = result.Read<int>().Single();
			int quantityTotal = result.Read<int>().Single();
			decimal costTotal = result.Read<decimal>().Single();
			decimal depreciationValueTotal = result.Read<decimal>().Single();
			decimal residualValueTotal = result.Read<decimal>().Single();
			dbConnection.Close();
			// Xử lý kết quả trả về
			return new PagingResultAsset
			{
				Data = assetDTOs,
				TotalRecord = totalRecord,
				QuantityTotal = quantityTotal,
				CostTotal = costTotal,
				DepreciationValueTotal = depreciationValueTotal,
				ResidualValueTotal = residualValueTotal,
			};
		}

		/// <summary>
		/// Hàm gọi api lấy ra ố lượng tài sản đã chứng từ
		/// </summary>
		/// <param name="ids">danh sách id tài sản cần kiểm tra</param>
		/// <returns>
		/// Số lượng tài sản đã chứng từ
		/// </returns>
		public int GetQuantityAssetActive(List<Guid> ids)
		{
			string idsToString = $"('{string.Join("','", ids)}')";
			string storedProcedureName = ProcedureNameAsset.CheckIncrement;
			var parameters = new DynamicParameters();
				parameters.Add("@ids", ids);
            var dbConnection = _assetRepository.GetOpenConnection();
			// Thực hiện gọi vào Database để chạy stored procedure
			var quantityAssetActive = _assetRepository.QueryFirstOrDefault<int>(dbConnection, storedProcedureName, parameters, commandType: CommandType.Text);
			dbConnection.Close();
			return quantityAssetActive;
		}
		#endregion
	}
}
