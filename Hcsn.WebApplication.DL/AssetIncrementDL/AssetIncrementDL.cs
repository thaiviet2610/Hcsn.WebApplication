using Dapper;
using Hcsn.WebApplication.Common.Constants.ProcedureName;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.DL.DBConfig;
using System.Data;

namespace Hcsn.WebApplication.DL.AssetIncrementDL
{
	public class AssetIncrementDL : IAssetIncrementDL
	{
		#region Field
		private IRepositoryDB _assetIncrementRepository;
		#endregion
		#region Constructor
		public AssetIncrementDL(IRepositoryDB repositoryDB) 
		{
			_assetIncrementRepository = repositoryDB;
		}
		#endregion


		#region Method
		/// <summary>
		/// Hàm truy cập database lấy bản ghi chứng từ theo id
		/// </summary>
		/// <param name="voucherId">id của chứng từ</param>
		/// <returns>Bản ghi chứng từ cần tìm</returns>
		public FixedAssetIncrementDTO GetById(Guid voucherId)
		{
			// Chuẩn bị tên stored procedure
			string storedProcedureName = String.Format(ProcedureName.GetById, typeof(FixedAssetIncrement).Name);
			// Chuẩn bị tham số đầu vào cho stored
			var parameters = new DynamicParameters();
			parameters.Add("p_voucher_id", voucherId);

			// Khởi tạo kết nối tới Database
			var dbConnection = _assetIncrementRepository.GetOpenConnection();

			// Thực hiện gọi vào Database để chạy stored procedure
			var result = _assetIncrementRepository.QueryMultiple(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
			var assetIncrement = result.Read<FixedAssetIncrement>().Single();
			var assets = result.Read<FixedAssetDTO>().ToList();
			foreach (var asset in assets)
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
		/// <param name="assets">Danh sách tài sản được ghi tăng trong chứng từ</param>
		/// <returns>Số bản ghi được thêm mới</returns>
		public int InsertAssetIncrement(FixedAssetIncrement assetIncrement, List<FixedAssetDTO> assets)
		{
			assetIncrement.voucher_id = Guid.NewGuid();
			PrepareInsertDataInsertAssetIncrement(assetIncrement, out string storedProcedureNameInsertAssetIncrement, out DynamicParameters parametersAssetIncrement);
			PrepareInsertDataInsertAssetIncrementDetail(assetIncrement, assets, out string storedProcedureNameInsertAssetIncrementDetail, out DynamicParameters parametersAssetIncrementDetail);
			int numberOfAffectedRowsInsertAssetIncrement = 0;
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					numberOfAffectedRowsInsertAssetIncrement = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameInsertAssetIncrement, parametersAssetIncrement, transaction: transaction, commandType: CommandType.StoredProcedure);
					int numberOfAffectedRowsInsertAssetIncrementDetail = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameInsertAssetIncrementDetail, parametersAssetIncrementDetail, transaction: transaction, commandType: CommandType.StoredProcedure);
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
		/// Hàm chuẩn bị dữ liệu đầu vào để truy cập vào database thêm mới chứng từ để thêm mới thông tin cho đối tượng chứng từ
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dũ liệu cần thêm mới</param>
		/// <param name="storedProcedureNameInsertAssetIncrement">Tên stored procedure</param>
		/// <param name="parametersAssetIncrement">Tham số đầu vào cho stored</param>
		private void PrepareInsertDataInsertAssetIncrement(FixedAssetIncrement assetIncrement, out string storedProcedureNameInsertAssetIncrement, out DynamicParameters parametersAssetIncrement)
		{
			storedProcedureNameInsertAssetIncrement = String.Format(ProcedureName.Insert, typeof(FixedAssetIncrement).Name);
			parametersAssetIncrement = new DynamicParameters();
			AddParametersValue(parametersAssetIncrement, assetIncrement);
		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu đầu vào để truy cập vào database thêm mới chứng từ để thêm mới thông tin cho đối tượng chứng từ chi tiết
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dũ liệu cần thêm mới</param>
		/// <param name="assets">Danh sách tài sản chứa dũ liệu cần thêm mới</param>
		/// <param name="storedProcedureNameInsertAssetIncrementDetail">Tên stored procedure</param>
		/// <param name="parametersAssetIncrementDetail">Tham số đầu vào cho stored</param>
		private static void PrepareInsertDataInsertAssetIncrementDetail(FixedAssetIncrement assetIncrement, List<FixedAssetDTO> assets, out string storedProcedureNameInsertAssetIncrementDetail, out DynamicParameters parametersAssetIncrementDetail)
		{
			storedProcedureNameInsertAssetIncrementDetail = String.Format(ProcedureName.Insert, typeof(FixedAssetIncrementDetail).Name);
			parametersAssetIncrementDetail = new DynamicParameters();
			var assetToString = new List<string>();
			foreach (var asset in assets)
			{
				var value = new List<string>
				{
					Guid.NewGuid().ToString(),
					assetIncrement.voucher_id.ToString(),
					asset.fixed_asset_id.ToString(),
					"LTVIET",
					DateTime.Now.ToString("yyyy-MM-dd"),
					"LTVIET",
					DateTime.Now.ToString("yyyy-MM-dd")
				};
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
			parametersAssetIncrementDetail.Add("p_id_assets", idsToString);
		}

		/// <summary>
		/// Hàm thêm các giá trị vào parameters để truyền vào storeProcedure
		/// </summary>
		/// <param name="parameters">Các tham số truyền vào</param>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dữ liệu</param>
		/// Create by: LTVIET (20/03/2023)
		protected virtual void AddParametersValue(DynamicParameters parameters, FixedAssetIncrement assetIncrement)
		{
			var properties = typeof(FixedAssetIncrement).GetProperties();
			foreach (var property in properties)
			{
				parameters.Add($"p_{property.Name}", property.GetValue(assetIncrement));
			}
		}

		/// <summary>
		/// Hàm lấy ra số bản ghi có cùng code nhưng khác id được truyền vào
		/// </summary>
		/// <param name="assetIncrement"> Đối tượng tài sản chứa thuôc tính cần kiểm tra trùng </param>
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

			return assetIncrement?.voucher_code;
		}

		/// <summary>
		/// Hàm sửa đổi 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrement">Bản ghi muốn sửa đổi</param>
		/// <param name="idAssetsAdd">Danh sách id tài sản được chứng từ thêm</param>
		/// <param name="idAssetsDelete">Danh sách id tài sản không còn được chứng từ</param>
		/// <returns>
		/// true: update thành công
		/// false: update thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public bool UpdateAssetIncrement(FixedAssetIncrementDTO assetIncrement,List<Guid>? idAssetsAdd, List<Guid>? idAssetsDelete)
		{
			// Chuẩn bị tên stored procedure
			PrepareUpdateDataUpdateAssetIncrement(assetIncrement, idAssetsDelete, out string storedProcedureNameUpdateAssetIncrement, out DynamicParameters parametersUpdateAssetIncrement);
			PrepareUpdataDataInsertAssetIncrementDetail(assetIncrement, idAssetsAdd, out string storedProcedureNameInsertAssetIncrementDetail, out DynamicParameters parametersInsertAssetIncrementDetail);

			// Khởi tạo kết nối tới Database
			// Thực hiện gọi vào Database để chạy stored procedure
			bool check = true;
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					int numberOfAffectedRowsUpdateAssetIncrement = _assetIncrementRepository.Execute(dbConnection,
															storedProcedureNameUpdateAssetIncrement, parametersUpdateAssetIncrement,
															transaction: transaction, commandType: CommandType.StoredProcedure);
					if (idAssetsAdd != null)
					{
						int numberOfAffectedRowsInsertAssetIncrementDetail = _assetIncrementRepository.Execute(dbConnection,
															storedProcedureNameInsertAssetIncrementDetail, parametersInsertAssetIncrementDetail,
															transaction: transaction, commandType: CommandType.StoredProcedure);
					}
					transaction.Commit();
				}
				catch (Exception)
				{
					transaction.Rollback();
					check = false;
				}
			}
			dbConnection.Close();
			return check;
		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu đầu vào để truy cập vào database sửa chứng từ để thêm mới danh sách tài sản
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dũ liệu cần sửa</param>
		/// <param name="idAssetsAdd">Danh sách id các tài sản cần thêm mới</param>
		/// <param name="storedProcedureNameInsertAssetIncrementDetail">Tên stored procedure</param>
		/// <param name="parametersInsertAssetIncrementDetail">Tham số đầu vào cho stored</param>
		private static void PrepareUpdataDataInsertAssetIncrementDetail(FixedAssetIncrementDTO assetIncrement, List<Guid>? idAssetsAdd, out string storedProcedureNameInsertAssetIncrementDetail, out DynamicParameters parametersInsertAssetIncrementDetail)
		{
			storedProcedureNameInsertAssetIncrementDetail = String.Format(ProcedureName.Insert, typeof(FixedAssetIncrementDetail).Name);

			// Chuẩn bị tham số đầu vào cho stored

			parametersInsertAssetIncrementDetail = new DynamicParameters();
			if (idAssetsAdd != null)
			{
				var assetToString = new List<string>();
				foreach (var idAsset in idAssetsAdd)
				{
					var value = new List<string>
					{
						Guid.NewGuid().ToString(),
						assetIncrement.voucher_id.ToString(),
						idAsset.ToString(),
						"LTVIET",
						DateTime.Now.ToString("yyyy-MM-dd"),
						"LTVIET",
						DateTime.Now.ToString("yyyy-MM-dd")
					};
					assetToString.Add($"('{string.Join("','", value)}')");
				}
				var assetsToString = $"{string.Join(",", assetToString)}";
				parametersInsertAssetIncrementDetail.Add("p_values", assetsToString);
				var idAssetAddsToString = $"('{string.Join("','", idAssetsAdd)}')";
				parametersInsertAssetIncrementDetail.Add("p_id_assets", idAssetAddsToString);
			}
		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu đầu vào để truy cập vào database sửa chứng từ để xóa danh sách tài sản cần xóa
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dũ liệu cần sửa</param>
		/// <param name="idAssetsDelete">Danh sách id các tài sản cần xóa</param>
		/// <param name="storedProcedureNameUpdateAssetIncrement">Tên stored procedure</param>
		/// <param name="parametersUpdateAssetIncrement">Tham số đầu vào cho stored</param>
		private void PrepareUpdateDataUpdateAssetIncrement(FixedAssetIncrementDTO assetIncrement, List<Guid>? idAssetsDelete, out string storedProcedureNameUpdateAssetIncrement, out DynamicParameters parametersUpdateAssetIncrement)
		{
			storedProcedureNameUpdateAssetIncrement = String.Format(ProcedureName.Update, typeof(FixedAssetIncrement).Name);
			parametersUpdateAssetIncrement = new DynamicParameters();
			AddParametersValue(parametersUpdateAssetIncrement, assetIncrement);
			var idAssetDelete = idAssetsDelete == null ? null : $"('{string.Join("','", idAssetsDelete)}')";
			parametersUpdateAssetIncrement.Add("p_id_assets_delete", idAssetDelete);
		}

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
		public int UpdateAssetIncrementPrice(Guid voucherId,Decimal price)
		{
			// Chuẩn bị tên stored procedure
			string storedProcedureName = ProcedureNameAssetIncrement.UpdatePrice;
			// Chuẩn bị tham số đầu vào cho stored
			var parameters = new DynamicParameters();
			parameters.Add("p_voucher_id", voucherId);
			parameters.Add("p_price", price);
			// Khởi tạo kết nối tới Database
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			
			// Thực hiện gọi vào Database để chạy stored procedure
			int numberOfAffectedRows = _assetIncrementRepository.Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
			
			dbConnection.Close();
			// Xử lý kết quả trả về
			return numberOfAffectedRows;
		}

		/// <summary>
		/// Hàm gọi database thực hiện việc xóa 1 bản ghi
		/// </summary>
		/// <param name="voucherId">Id bản ghi chứng từ muốn xóa</param>
		/// <returns>
		/// Số bản ghi được xóa
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public int DeleteAssetIncrementById(Guid voucherId)
		{
			// Chuẩn bị tên stored procedure
			string storedProcedureName = String.Format(ProcedureName.DeleteById, typeof(FixedAssetIncrement).Name);
			// Chuẩn bị tham số đầu vào cho stored
			var parameters = new DynamicParameters();
			var properties = typeof(FixedAssetIncrement).GetProperties();
			parameters.Add("p_voucher_id", voucherId);
			// Khởi tạo kết nối tới Database
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			int numberOfAffectedRows = 0;
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					// Thực hiện gọi vào Database để chạy stored procedure
					numberOfAffectedRows = _assetIncrementRepository.Execute(dbConnection, storedProcedureName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
					if (numberOfAffectedRows == 0)
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
					transaction.Rollback();
				}
			}
			dbConnection.Close();
			// Xử lý kết quả trả về
			return numberOfAffectedRows;
		}

		/// <summary>
		/// Hàm gọi database để thực hiện việc xóa nhiều bản ghi
		/// </summary>
		/// <param name="ids">Danh sách id bản ghi cần xóa</param>
		/// <returns>
		/// Kết quả việc thực hiện xóa nhiều bản ghi
		/// 1: Nếu delete thành công
		/// 0: Nếu delete thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public int DeleteMultipleAssetIncrement(List<Guid> ids)
		{
			// Chuẩn bị tham số đầu vào
			string sqlAssetIncrementDeleteMultiple = string.Format(ProcedureName.DeleteMultiple, typeof(FixedAssetIncrement).Name);
			// Khởi tạo kết nối tới Database
			var parametersAssetIncrementDeleteMultiple = new DynamicParameters();
			var listIdToString = $"('{string.Join("','", ids)}')";

			parametersAssetIncrementDeleteMultiple.Add("p_ids", listIdToString);
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			int numberOfAffectedRows = 0;
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					// Thực hiện gọi vào Database để chạy stored procedure
					numberOfAffectedRows = _assetIncrementRepository.Execute(dbConnection, sqlAssetIncrementDeleteMultiple, parametersAssetIncrementDeleteMultiple, transaction: transaction, commandType: CommandType.StoredProcedure);
					transaction.Commit();
					numberOfAffectedRows = 1;

				}
				catch (Exception)
				{
					numberOfAffectedRows = 0;
					transaction.Rollback();
				}
			}
			dbConnection.Close();
			// Xử lý kết quả trả về
			return numberOfAffectedRows;
		}


		#endregion
	}

}
