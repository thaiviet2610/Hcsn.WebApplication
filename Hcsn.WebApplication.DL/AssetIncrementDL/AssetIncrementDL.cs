using Dapper;
using Hcsn.WebApplication.Common.Constants.ProcedureName;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.DL.DBConfig;
using System.Data;
using Org.BouncyCastle.Crypto;
using Hcsn.WebApplication.Common.Enums;
using System.Transactions;
using Hcsn.WebApplication.Common.Entities.DTO.result.paging;
using Hcsn.WebApplication.Common.Entities.DTO.entityDTO;

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
		/// Created by: LTVIET (20/04/2023)
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
				voucher_code = assetIncrement.voucher_code.Trim(),
				voucher_date = assetIncrement.voucher_date,
				increment_date = assetIncrement.increment_date,
				price = assetIncrement.price,
				description = assetIncrement.description?.Trim(),
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
		/// Created by: LTVIET (20/04/2023)
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
		/// <returns>
		/// Kết quả việc thêm mới:
		/// true: thêm mới thành công
		/// false: thêm mới thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public bool InsertAssetIncrement(FixedAssetIncrement assetIncrement, List<FixedAssetDTO> assets)
		{
			bool check = true;
			assetIncrement.voucher_id = Guid.NewGuid();
			PrepareInsertDataInsertAssetIncrement(assetIncrement, out string storedProcedureNameInsertAssetIncrement, out DynamicParameters parametersAssetIncrement);
			PrepareInsertDataInsertAssetIncrementDetail(assetIncrement, assets, out string storedProcedureNameInsertAssetIncrementDetail, out DynamicParameters parametersAssetIncrementDetail);

			var idAssets = new List<Guid>();
			foreach (var asset in assets)
			{
				idAssets.Add(asset.fixed_asset_id);
			}
			PrepareUpdateAssetActive(idAssets, out string storedProcedureNameUpdateAssetActive, out DynamicParameters parametersUpdateAssetActive, (int)Active.True);
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					int numberOfAffectedRowsInsertAssetIncrement = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameInsertAssetIncrement, parametersAssetIncrement, transaction: transaction, commandType: CommandType.StoredProcedure);
					int numberOfAffectedRowsInsertAssetIncrementDetail = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameInsertAssetIncrementDetail, parametersAssetIncrementDetail, transaction: transaction, commandType: CommandType.StoredProcedure);
					int numberOfAffectedRowsUpdateAssetActive = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameUpdateAssetActive, parametersUpdateAssetActive, transaction: transaction, commandType: CommandType.StoredProcedure);
					if (numberOfAffectedRowsInsertAssetIncrement == 0 || numberOfAffectedRowsInsertAssetIncrementDetail != assets.Count || numberOfAffectedRowsUpdateAssetActive != assets.Count)
					{
						transaction.Rollback();
						check = false;
					}
					else
					{
						transaction.Commit();
					}
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
		/// Hàm chuẩn bị dữ liệu đầu vào để truy cập vào database thêm mới chứng từ để thêm mới thông tin cho đối tượng chứng từ
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dũ liệu cần thêm mới</param>
		/// <param name="storedProcedureNameInsertAssetIncrement">Tên stored procedure</param>
		/// <param name="parametersAssetIncrement">Tham số đầu vào cho stored</param>
		/// Created by: LTVIET (20/04/2023)
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
		/// Created by: LTVIET (20/04/2023)
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
		}

		/// <summary>
		/// Hàm thêm các giá trị vào parameters để truyền vào storeProcedure
		/// </summary>
		/// <param name="parameters">Các tham số truyền vào</param>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dữ liệu</param>
		/// Create by: LTVIET (20/04/2023)
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
		/// Created by: LTViet (20/04/2023)
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
		/// <returns>Mã chứng từ của đối tượng</returns>
		/// Created by: LTViet (20/04/2023)
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

			return assetIncrement?.voucher_code.Trim();
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
		/// Created by: LTViet (20/04/2023)
		public bool UpdateAssetIncrement(FixedAssetIncrementDTO assetIncrement, List<Guid>? idAssetsAdd, List<Guid>? idAssetsDelete)
		{
			// Chuẩn bị tên stored procedure
			PrepareUpdateDataUpdateAssetIncrement(assetIncrement, idAssetsDelete, out string storedProcedureNameUpdateAssetIncrement, out DynamicParameters parametersUpdateAssetIncrement);
			PrepareUpdataDataInsertAssetIncrementDetail(assetIncrement, idAssetsAdd, out string storedProcedureNameInsertAssetIncrementDetail, out DynamicParameters parametersInsertAssetIncrementDetail);
			PrepareUpdateAssetActive(idAssetsAdd, out string storedProcedureNameUpdateAssetTrue, out DynamicParameters parametersUpdateAssetActiveTrue, (int)Active.True);
			PrepareDataDeleteAssetIncrementDetailByIdAssets(idAssetsDelete, out string storedProcedureNameDeleteAssetIncrementDetail, out DynamicParameters parametersDeleteAssetIncrementDetail);
			PrepareUpdateAssetActive(idAssetsDelete, out string storedProcedureNameUpdateAssetFalse, out DynamicParameters parametersUpdateAssetActiveFalse, (int)Active.False);
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
					if (numberOfAffectedRowsUpdateAssetIncrement == 0)
					{
						transaction.Rollback();
						check = false;
					}
					if (idAssetsAdd != null && check)
					{
						int numberOfAffectedRowsInsertAssetIncrementDetail = _assetIncrementRepository.Execute(dbConnection,
															storedProcedureNameInsertAssetIncrementDetail, parametersInsertAssetIncrementDetail,
															transaction: transaction, commandType: CommandType.StoredProcedure);
						int numberOfAffectedRowsUpdateAssetActiveTrue = _assetIncrementRepository.Execute(dbConnection,
															storedProcedureNameUpdateAssetTrue, parametersUpdateAssetActiveTrue,
															transaction: transaction, commandType: CommandType.StoredProcedure);
						if (numberOfAffectedRowsInsertAssetIncrementDetail != idAssetsAdd.Count || numberOfAffectedRowsUpdateAssetActiveTrue != idAssetsAdd.Count)
						{
							transaction.Rollback();
							check = false;
						}

					}
					if (idAssetsDelete != null && check)
					{
						int numberOfAffectedRowsDeleteAssetIncrementDetail = _assetIncrementRepository.Execute(dbConnection,
															storedProcedureNameDeleteAssetIncrementDetail, parametersDeleteAssetIncrementDetail,
															transaction: transaction, commandType: CommandType.Text);
						int numberOfAffectedRowsUpdateAssetActiveFalse = _assetIncrementRepository.Execute(dbConnection,
															storedProcedureNameUpdateAssetFalse, parametersUpdateAssetActiveFalse,
															transaction: transaction, commandType: CommandType.StoredProcedure);
						if (numberOfAffectedRowsDeleteAssetIncrementDetail != idAssetsDelete.Count || numberOfAffectedRowsUpdateAssetActiveFalse != idAssetsDelete.Count)
						{
							transaction.Rollback();
							check = false;
						}
					}
					if (check)
					{
						transaction.Commit();
					}
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
		/// Hàm sửa đổi tổng nguyên giá của bản ghi chứng từ
		/// </summary>
		/// <param name="voucherId">Id của chứng từ cần sửa</param>
		/// <param name="price">Giá trị của tổng nguyên giá</param>
		/// <returns>
		/// true: update thành công
		/// false: update thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		public bool UpdateAssetIncrementPrice(Guid voucherId, Decimal price)
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
			return numberOfAffectedRows == 1;
		}

		/// <summary>
		/// Hàm gọi database thực hiện việc xóa 1 bản ghi
		/// </summary>
		/// <param name="voucherId">Id bản ghi chứng từ muốn xóa</param>
		/// <returns>
		/// Kết quả thực hiện việc xóa
		/// True: thành công
		/// False: thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		public bool DeleteAssetIncrementById(Guid voucherId)
		{
			// Chuẩn bị tên stored procedure
			var idVouchers = new List<Guid>
			{
				voucherId
			};
			PrepareDataGetAssetsByVoucherId(idVouchers, out string storedProcedureNameGetAssetsByIdVoucher, out DynamicParameters parametersGetAssetsByIdVoucher);
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			var result = _assetIncrementRepository.QueryMultiple(dbConnection, storedProcedureNameGetAssetsByIdVoucher, parametersGetAssetsByIdVoucher, commandType: CommandType.Text);
			var idAssets = result.Read<Guid>().ToList();
			dbConnection.Close();

			PrepareDataDeleteAssetIncrementById(voucherId, out string storedProcedureNameDeleteAssetIncrement,
												out DynamicParameters parametersDeleteAssetIncrement);

			PrepareUpdateAssetActive(idAssets, out string storedProcedureNameUpdateAssetActive, out DynamicParameters parametersUpdateAssetActive, (int)Active.False);
			// Khởi tạo kết nối tới Database
			bool checkDelete = true;
			dbConnection.Open();
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					// Thực hiện gọi vào Database để chạy stored procedure
					int numberOfAffectedRowsDeleteAssetIncrement = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameDeleteAssetIncrement, parametersDeleteAssetIncrement, transaction: transaction, commandType: CommandType.StoredProcedure);
					int numberOfAffectedRowsUpdateAssetActive = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameUpdateAssetActive, parametersUpdateAssetActive, transaction: transaction, commandType: CommandType.StoredProcedure);

					if (numberOfAffectedRowsUpdateAssetActive != idAssets.Count || numberOfAffectedRowsDeleteAssetIncrement == 0)
					{
						transaction.Rollback();
						checkDelete = false;
					}
					else
					{
						transaction.Commit();
					}

				}
				catch (Exception)
				{
					transaction.Rollback();
					checkDelete = false;
				}
			}
			dbConnection.Close();
			// Xử lý kết quả trả về
			return checkDelete;
		}



		/// <summary>
		/// Hàm gọi database để thực hiện việc xóa nhiều bản ghi
		/// </summary>
		/// <param name="ids">Danh sách id bản ghi cần xóa</param>
		/// <returns>
		/// Kết quả việc thực hiện xóa nhiều bản ghi
		/// True: Nếu delete thành công
		/// False: Nếu delete thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public bool DeleteMultipleAssetIncrement(List<Guid> ids)
		{
			PrepareDataGetAssetsByVoucherId(ids, out string storedProcedureNameGetAssetsByIdVoucher, out DynamicParameters parametersGetAssetsByIdVoucher);
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			var result = _assetIncrementRepository.QueryMultiple(dbConnection, storedProcedureNameGetAssetsByIdVoucher, parametersGetAssetsByIdVoucher, commandType: CommandType.Text);
			var idAssets = result.Read<Guid>().ToList();
			dbConnection.Close();
			PrepareDataDeleteMultipleAssetIncrement(ids, out string storedProcedureNameDeleteMultipleAssetIncrement, out DynamicParameters parametersDeleteMultipleAssetIncrement);
			PrepareUpdateAssetActive(idAssets, out string storedProcedureNameUpdateAssetActive, out DynamicParameters parametersUpdateAssetActive, (int)Active.False);
			bool checkDeleteMultiple = true;
			dbConnection.Open();
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					// Thực hiện gọi vào Database để chạy stored procedure
					int numberOfAffectedRowsDeleteMultipleAssetIncrement = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameDeleteMultipleAssetIncrement, parametersDeleteMultipleAssetIncrement, transaction: transaction, commandType: CommandType.StoredProcedure);
					int numberOfAffectedRowsUpdateAssetActive = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameUpdateAssetActive, parametersUpdateAssetActive, transaction: transaction, commandType: CommandType.StoredProcedure);

					if (numberOfAffectedRowsUpdateAssetActive != idAssets.Count || numberOfAffectedRowsDeleteMultipleAssetIncrement == 0)
					{
						transaction.Rollback();
						checkDeleteMultiple = false;
					}
					else
					{
						transaction.Commit();
					}

				}
				catch (Exception)
				{
					transaction.Rollback();
					checkDeleteMultiple = false;
				}
			}
			dbConnection.Close();
			// Xử lý kết quả trả về
			return checkDeleteMultiple;
		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu để gọi vào database sửa trạng thái tài sản
		/// </summary>
		/// <param name="idAssets">danh sách id tài sản</param>
		/// <param name="storedProcedureNameUpdateAssetActive">Tên stored procedure</param>
		/// <param name="parametersUpdateAssetActive">Tham số đầu vào cho stored</param>
		/// <param name="valueActive">giá trị trạng thái</param>
		/// Creted by: LTVIET (12/05/2023)
		private static void PrepareUpdateAssetActive(List<Guid>? idAssets, out string storedProcedureNameUpdateAssetActive, out DynamicParameters parametersUpdateAssetActive, int valueActive)
		{
			storedProcedureNameUpdateAssetActive = ProcedureNameAsset.UpdateActive;
			parametersUpdateAssetActive = new DynamicParameters();
			string idAssetsToString = "('')";
			if (idAssets != null)
			{
				idAssetsToString = $"('{string.Join("','", idAssets)}')";
			}
			parametersUpdateAssetActive.Add("p_ids", idAssetsToString);
			parametersUpdateAssetActive.Add("p_value", valueActive);

		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu để gọi vào database để xóa các chứng từ chi tiết theo id tài sản
		/// </summary>
		/// <param name="idAssetsDelete">danh sách id tài sản</param>
		/// <param name="storedProcedureNameDeleteAssetIncrementDetail">Tên stored procedure</param>
		/// <param name="parametersDeleteAssetIncrementDetail">Tham số đầu vào cho stored</param>
		/// Created by: LTVIET (12/05/2023)
		private static void PrepareDataDeleteAssetIncrementDetailByIdAssets(List<Guid>? idAssetsDelete, out string storedProcedureNameDeleteAssetIncrementDetail, out DynamicParameters parametersDeleteAssetIncrementDetail)
		{

			storedProcedureNameDeleteAssetIncrementDetail = ProcedureNameAssetIncrementDetail.DeleteMultipleByIdAssets;
			parametersDeleteAssetIncrementDetail = new DynamicParameters();
			//var listIdToString = "('')";
			//if (idAssetsDelete != null)
			//{
			//	listIdToString = $"('{string.Join("','", idAssetsDelete)}')";
			//}
			parametersDeleteAssetIncrementDetail.Add("@p_ids", idAssetsDelete);

		}


		/// <summary>
		/// Hàm chuẩn bị dữ liệu đầu vào để truy cập vào database sửa chứng từ để thêm mới danh sách tài sản
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dũ liệu cần sửa</param>
		/// <param name="idAssetsAdd">Danh sách id các tài sản cần thêm mới</param>
		/// <param name="storedProcedureNameInsertAssetIncrementDetail">Tên stored procedure</param>
		/// <param name="parametersInsertAssetIncrementDetail">Tham số đầu vào cho stored</param>
		/// Created by: LTVIET (12/05/2023)
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
			}
		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu đầu vào để truy cập vào database sửa chứng từ để xóa danh sách tài sản cần xóa
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ chứa dũ liệu cần sửa</param>
		/// <param name="idAssetsDelete">Danh sách id các tài sản cần xóa</param>
		/// <param name="storedProcedureNameUpdateAssetIncrement">Tên stored procedure</param>
		/// <param name="parametersUpdateAssetIncrement">Tham số đầu vào cho stored</param>
		/// Created by: LTVIET (12/05/2023)
		private void PrepareUpdateDataUpdateAssetIncrement(FixedAssetIncrementDTO assetIncrement, List<Guid>? idAssetsDelete, out string storedProcedureNameUpdateAssetIncrement, out DynamicParameters parametersUpdateAssetIncrement)
		{
			storedProcedureNameUpdateAssetIncrement = String.Format(ProcedureName.Update, typeof(FixedAssetIncrement).Name);
			parametersUpdateAssetIncrement = new DynamicParameters();
			AddParametersValue(parametersUpdateAssetIncrement, assetIncrement);
		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu để gọi vào database để lấy danh sách tài sản theo id chứng từ
		/// </summary>
		/// <param name="idVouchers">Danh sách id chứng từ</param>
		/// <param name="storedProcedureNameGetAssetsByIdVoucher">Tên stored procedure</param>
		/// <param name="parametersGetAssetsByIdVoucher">Tham số đầu vào cho stored</param>
		/// Created by: LTVIET (12/05/2023)
		private static void PrepareDataGetAssetsByVoucherId(List<Guid> idVouchers, out string storedProcedureNameGetAssetsByIdVoucher, out DynamicParameters parametersGetAssetsByIdVoucher)
		{
			storedProcedureNameGetAssetsByIdVoucher = ProcedureNameAsset.GetByVoucherId;
			// Chuẩn bị tham số đầu vào cho stored
			parametersGetAssetsByIdVoucher = new DynamicParameters();
			parametersGetAssetsByIdVoucher.Add("@p_ids", idVouchers);
		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu để gọi vào database để xóa chứng từ theo id
		/// </summary>
		/// <param name="voucherId">id chứng từ</param>
		/// <param name="storedProcedureNameDeleteAssetIncrement">Tên stored procedure</param>
		/// <param name="parametersDeleteAssetIncrement">Tham số đầu vào cho stored</param>
		/// Created by: LTVIET (12/05/2023)
		private static void PrepareDataDeleteAssetIncrementById(Guid voucherId, out string storedProcedureNameDeleteAssetIncrement, out DynamicParameters parametersDeleteAssetIncrement)
		{
			storedProcedureNameDeleteAssetIncrement = String.Format(ProcedureName.DeleteById, typeof(FixedAssetIncrement).Name);
			// Chuẩn bị tham số đầu vào cho stored
			parametersDeleteAssetIncrement = new DynamicParameters();
			parametersDeleteAssetIncrement.Add("p_voucher_id", voucherId);

		}

		/// <summary>
		/// Hàm chuẩn bị dữ liệu để gọi vào database để xóa nhiều chứng từ
		/// </summary>
		/// <param name="ids">danh sách id chứng từ</param>
		/// <param name="storedProcedureNameAssetIncrementDeleteMultiple">Tên stored procedure</param>
		/// <param name="parametersAssetIncrementDeleteMultiple">Tham số đầu vào cho stored</param>
		/// Created by: LTVIET (12/05/2023)
		private static void PrepareDataDeleteMultipleAssetIncrement(List<Guid> ids, out string storedProcedureNameAssetIncrementDeleteMultiple, out DynamicParameters parametersAssetIncrementDeleteMultiple)
		{
			// Chuẩn bị tham số đầu vào
			storedProcedureNameAssetIncrementDeleteMultiple = string.Format(ProcedureName.DeleteMultiple, typeof(FixedAssetIncrement).Name);
			// Khởi tạo kết nối tới Database
			parametersAssetIncrementDeleteMultiple = new DynamicParameters();
			var listIdToString = $"('{string.Join("','", ids)}')";

			parametersAssetIncrementDeleteMultiple.Add("p_ids", listIdToString);
		}


		#endregion
	}

}