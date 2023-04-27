using Dapper;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Constants.ProcedureName;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.DL.AssetIncrementTest1DL;
using Hcsn.WebApplication.DL.BaseDL;
using Hcsn.WebApplication.DL.DBConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.DL.AssetIncrementTest1DL
{
	public class AssetIncrementTestDL : BaseDL<FixedAssetIncrement>, IAssetIncrementTestDL
	{
		#region Field
		private IRepositoryDB _assetIncrementRepository;
		#endregion
		#region Constructor
		public AssetIncrementTestDL(IRepositoryDB repositoryDB) : base(repositoryDB)
		{
			_assetIncrementRepository = repositoryDB;
		}
		#endregion


		#region Method

		public FixedAssetIncrementDTO GetAssetIncrementDTOById(Guid voucerId)
		{
			// Chuẩn bị tên stored procedure
			string storedProcedureName = String.Format(ProcedureName.GetById, typeof(FixedAssetIncrement).Name);
			// Chuẩn bị tham số đầu vào cho stored
			var parameters = new DynamicParameters();
			parameters.Add("p_voucher_id", voucerId);

			// Khởi tạo kết nối tới Database
			var dbConnection = _assetIncrementRepository.GetOpenConnection();

			// Thực hiện gọi vào Database để chạy stored procedure
			var result = _assetIncrementRepository.QueryMultiple(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
			var assetIncrement = result.Read<FixedAssetIncrement>().Single();
			var assets = result.Read<FixedAssetDTO>().ToList();
			foreach( var asset in assets)
			{
				decimal residualValue = asset.residual_value;
				asset.residual_value = residualValue > 0 ? residualValue : 0;
			}
			var assetIncrementDTO = new FixedAssetIncrementDTO()
			{
				voucher_id = assetIncrement.voucher_id,
				voucher_code = assetIncrement.voucher_code,
				voucher_date = assetIncrement.voucher_date,
				price = assetIncrement.price,
				description = assetIncrement.description,
				assets = assets
			};
			dbConnection.Close();
			// Xử lý kết quả trả về 
			return assetIncrementDTO;
		}

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
			(string? keyword, int pageSize, int pageNumber)
		{
			// Chuẩn bị tên stored procedure
			string storedProcedureName = String.Format(ProcedureName.Filter, typeof(FixedAssetIncrement).Name);
			// Chuẩn bị tham số đầu vào cho stored
			int limit = pageSize;
			int offset = (pageNumber - 1) * limit;
			var parameters = new DynamicParameters();
			parameters.Add("p_keyword", keyword);
			parameters.Add("p_limit", limit);
			parameters.Add("p_offset", offset);
			// Khởi tạo kết nối tới Database
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			// Thực hiện gọi vào Database để chạy stored procedure
			var result = _assetIncrementRepository.QueryMultiple(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
			var data = result.Read<FixedAssetIncrementDTO>().ToList();
			int totalRecord = result.Read<int>().Single();
			decimal priceTotal = result.Read<decimal>().Single();
			dbConnection.Close();
			// Xử lý kết quả trả về
			return new PagingResultAssetIncrement
			{
				Data = data,
				TotalRecord = totalRecord,
				PriceTotal = priceTotal,
			};
		}

		/// <summary>
		/// Hàm thêm mới 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrement">Đối tượng thông tin chứng từ</param>
		/// <param name="idAssets">Danh sách tài sản được ghi tăng trong chứng từ</param>
		/// <returns>Số bản ghi được thêm mới</returns>
		public int InsertAssetIncrement(FixedAssetIncrement assetIncrement, List<FixedAssetDTO> assets)
		{
			string storedProcedureNameInsertAssetIncrement = String.Format(ProcedureName.Insert, typeof(FixedAssetIncrement).Name);
			string storedProcedureNameInsertAssetIncrementDetail = String.Format(ProcedureName.Insert, typeof(FixedAssetIncrementDetail).Name);
			string storedProcedureNameUpdateAssetActive = String.Format(ProcedureNameAsset.UpdateActive, typeof(FixedAsset).Name);

			var parametersAssetIncrement = new DynamicParameters();
			var parametersAssetIncrementDetail = new DynamicParameters();
			var parametersAsset = new DynamicParameters();

			assetIncrement.voucher_id = Guid.NewGuid();
			var propertiesAssetIncrement = typeof(FixedAssetIncrement).GetProperties();
			AddParametersValue(propertiesAssetIncrement, parametersAssetIncrement, assetIncrement);
			var assetToString = new List<string>();
			foreach (var asset in assets)
			{
				var value = new List<string>();
				value.Add(Guid.NewGuid().ToString());
				value.Add(assetIncrement.voucher_id.ToString());
				value.Add(asset.fixed_asset_id.ToString());
				value.Add("LTVIET");
				value.Add(DateTime.Now.ToString("yyyy-MM-dd"));
				value.Add("LTVIET");
				value.Add(DateTime.Now.ToString("yyyy-MM-dd"));
				assetToString.Add($"('{string.Join("','", value)}')");
			}
			var assetsToString = $"{string.Join(",", assetToString)}";
			parametersAssetIncrementDetail.Add("p_values", assetsToString);

			var ids = new List<Guid>();
			foreach (var asset in assets)
			{
				ids.Add(asset.fixed_asset_id);
			}
			var idsToString = $"('{string.Join("','", ids)}')";
			parametersAsset.Add("p_ids", idsToString);
			int numberOfAffectedRowsInsertAssetIncrement = 0;
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					numberOfAffectedRowsInsertAssetIncrement = 0;
					numberOfAffectedRowsInsertAssetIncrement = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameInsertAssetIncrement, parametersAssetIncrement, transaction: transaction, commandType: CommandType.StoredProcedure);


					int numberOfAffectedRowsInsertAssetIncrementDetail = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameInsertAssetIncrementDetail, parametersAssetIncrementDetail, transaction: transaction, commandType: CommandType.StoredProcedure);
					int numberOfAffectedRowsUpdateAsset = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameUpdateAssetActive, parametersAsset, transaction: transaction, commandType: CommandType.StoredProcedure);

					if (numberOfAffectedRowsInsertAssetIncrement == 0)
					{
						transaction.Rollback();
					}
					else
					{
						transaction.Commit();
					}




				}
				catch (Exception)
				{
					numberOfAffectedRowsInsertAssetIncrement = 0;
					transaction.Rollback();
				}
			}
			dbConnection.Close();
			return numberOfAffectedRowsInsertAssetIncrement;


		}

		/// <summary>
		/// Hàm lấy ra số bản ghi có cùng code nhưng khác id được truyền vào
		/// </summary>
		/// <param name="asset"> Đối tượng tài sản chứa thuôc tính cần kiểm tra trùng </param>
		/// <param name="propertyName">Tên thuộc tính cần kiểm tra</param>
		/// <returns>Số bản ghi cần tìm</returns>
		/// Created by: LTViet (20/03/2023)
		public int GetNumberRecordOfPropertyDuplicate(FixedAssetIncrement assetIncrement, string propertyName)
		{
			var duplicateProperty = typeof(FixedAssetIncrement).GetProperties().FirstOrDefault(prop => prop.Name == propertyName);
			var propertyValue = duplicateProperty.GetValue(assetIncrement);
			var voucherId = assetIncrement.voucher_id;
			var attHcsnDuplicate = duplicateProperty.GetCustomAttributes(typeof(HcsnDuplicateAttribute), false).FirstOrDefault();
			var propDuplicateName = (attHcsnDuplicate as HcsnDuplicateAttribute).Name;
			string storedProcedureNameGetNumberRecordOfPropertyDuplicate = String.Format(ProcedureName.CheckDuplicate, typeof(FixedAssetIncrement).Name, propDuplicateName);
			var parametersCheckSameCode = new DynamicParameters();
			parametersCheckSameCode.Add($"p_{propertyName}", propertyValue);
			parametersCheckSameCode.Add("p_id", voucherId);
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			// Thực hiện gọi vào Database để chạy stored procedure
			int numberOfAffectedRowsDuplicate =
				_assetIncrementRepository.QueryFirstOrDefault<int>(dbConnection, storedProcedureNameGetNumberRecordOfPropertyDuplicate,
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
			string storedProcedureName = String.Format(ProcedureName.GetNewCode, typeof(FixedAssetIncrement).Name);
			// Chuẩn bị tham số đầu vào cho stored
			// Khởi tạo kết nối tới Database
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			// Thực hiện gọi vào Database để chạy stored procedure
			var assetIncrement = _assetIncrementRepository.QueryFirstOrDefault<FixedAssetIncrement>(dbConnection, storedProcedureName, commandType: CommandType.StoredProcedure);
			dbConnection.Close();

			// Xử lý kết quả trả về 
			if (assetIncrement == null)
			{
				// Nếu không có đối tượng nào trong database thì trả về kết quả null
				return null;
			}
			else
			{
				return assetIncrement.voucher_code;
			}
		}

		#endregion
	}
}
