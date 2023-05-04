using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using System.Reflection;
using System.Text.RegularExpressions;
using Hcsn.WebApplication.DL.AssetIncrementDL;

namespace Hcsn.WebApplication.BL.AssetIncrementBL
{
	public class AssetIncrementBL : IAssetIncrementBL
	{
		#region Field
		private IAssetIncrementDL _assetIncrementDL;
		protected List<ValidateResult> inValidList = new();
		#endregion
		#region Constructor
		public AssetIncrementBL(IAssetIncrementDL assetIncrementDL)
		{
			_assetIncrementDL = assetIncrementDL;
		}
		#endregion



		#region Method

		/// <summary>
		/// Hàm lấy danh sách chứng từ theo bộ lọc và phân trang
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã chứng từ, ghi chú)</param> 
		/// <param name="pageSize">Số bản ghi trong 1 trang</param> 
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns> 
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện thêm mới:
		/// IsSuccess == true: thêm mới thành công
		/// IsSuccess == false: thêm mới thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public ServiceResult GetPaging(string? keyword, int pageSize, int pageNumber)
		{
			var result = _assetIncrementDL.GetPaging(keyword, pageSize, pageNumber);
			return new ServiceResult
			{
				IsSuccess = result.Data != null,
				ErrorCode = ErrorCode.NotFound,
				Message = ServiceResource.NotFound,
				Data = result
			};
		}

		/// <summary>
		/// Hàm xử lý logic khi lấy thông tin chi tiết 1 chứng từ theo id từ tầng DL 
		/// </summary>
		/// <param name="assetIncrementId">Id chứng từ muốn lấy</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public ServiceResult GetById(Guid assetIncrementId)
		{
			var assetIncrement = _assetIncrementDL.GetById(assetIncrementId);
			return new ServiceResult
			{
				IsSuccess = assetIncrement != null,
				ErrorCode = ErrorCode.NotFound,
				Message = ServiceResource.NotFound,
				Data = assetIncrement
			};
		}

		/// <summary>
		/// Hàm thêm mới 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrementDTO">Đối tượng chứa thông tin chứng từ cần thêm mới(thông tin chứng từ, danh sách tài sản ghi tăng)</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện thêm mới:
		/// IsSuccess == true: thêm mới thành công
		/// IsSuccess == false: thêm mới thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public ServiceResult InsertAssetIncrement(FixedAssetIncrementDTO assetIncrementDTO)
		{
			var assets = assetIncrementDTO.assets;
			var validateResult = ValidateRequesData(assetIncrementDTO);
			// Thất bại
			if (!validateResult.IsSuccess)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.InvalidateData,
					Message = ServiceResource.InvalidData,
					Data = validateResult.Data
				};
			}
			// Thành công
			int numberOfAffectedRows = _assetIncrementDL.InsertAssetIncrement(assetIncrementDTO, assets);
			return new ServiceResult
			{
				IsSuccess = numberOfAffectedRows > 0,
				ErrorCode = ErrorCode.InsertFailed,
				Message = ServiceResource.InsertFailed,
			};
		}

		/// <summary>
		/// Hàm validate chung dữ liệu 
		/// </summary>
		/// <param name="assetIncrementDTO">bản ghi cần validate</param>
		/// <returns>Kết quả validate dữ liệu</returns>
		/// Created by: LTVIET (20/04/2023)
		public ValidateResult ValidateRequesData(FixedAssetIncrementDTO assetIncrementDTO)
		{
			var validateEmptyResult = ValidateEmpty(assetIncrementDTO);
			var validateCustomResult = ValidateCustom(assetIncrementDTO);
			bool checkAssetsNull = true;
			if (assetIncrementDTO.assets.Count == 0 || assetIncrementDTO.assets == null)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.NoAssetIncrements,
					Message = ValidateResource.NoAssetIncrements,
				});
				checkAssetsNull = false;
			}

			return new ValidateResult
			{
				IsSuccess = validateEmptyResult && validateCustomResult && checkAssetsNull,
				Data = inValidList
			};
			
		}

		/// <summary>
		/// Hàm validate giá trị các thuộc tính không được để trống
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứa các thuộc tính</param>
		/// <returns>Kết quả validate</returns>
		/// Created by: LTVIET (20/04/2023)
		private bool ValidateEmpty(FixedAssetIncrement assetIncrement)
		{
			bool check = true;
			var validateEmpty = new List<String>();
			var properties = typeof(FixedAssetIncrement).GetProperties();

			foreach (var property in properties)
			{
				var propValue = property.GetValue(assetIncrement);
				var propName = property.Name;

				var requiredAttribute = (HcsnRequiredAttribute?)property.GetCustomAttributes(typeof(HcsnRequiredAttribute)).FirstOrDefault();
				if (requiredAttribute != null && (propValue == null || String.IsNullOrEmpty(propValue.ToString().Trim())))
				{
					validateEmpty.Add(propName);
					check = false;
				}
			}
			if (!check)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.Empty,
					Message = ValidateResource.Empty,
					Data = validateEmpty
				});
			}
			return check;
		}

		/// <summary>
		/// Hàm validate riêng dữ liệu 
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ cần validate</param>
		/// <returns>
		/// Kết quả validate dữ liệu:
		/// true: thành công
		/// false: thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		public bool ValidateCustom(FixedAssetIncrement assetIncrement)
		{
			bool check = true;
			var properties = typeof(FixedAssetIncrement).GetProperties();

			foreach (var property in properties)
			{
				string propName = property.Name;
				var propValue = property.GetValue(assetIncrement);
				bool isOutMaxLength = IsOutMaxLength(property, propName, propValue);
				bool isDuplicate = true;
				if (isOutMaxLength)
				{
					isDuplicate = IsPropertyDuplicate(assetIncrement, property, propName, propValue);
				}
				if (!isOutMaxLength || !isDuplicate)
				{
					check = false;
				}
			}
			return check;
		}

		/// <summary>
		/// Hàm validate giá trị không được trùng nhau
		/// </summary>
		/// <param name="assetIncrement">Đối tượng tài sản ghi tăng cần validate</param>
		/// <param name="property">Thuộc tính cần validate</param>
		/// <param name="propName">Tên của thuộc tính cần validate</param>
		/// <param name="propValue">Giá trị của thuộc tính cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		private bool IsPropertyDuplicate(FixedAssetIncrement assetIncrement, PropertyInfo property, string propName, object? propValue)
		{
			bool check = true;
			if (property.IsDefined(typeof(HcsnDuplicateAttribute), false))
			{
				var attDuplicate = (HcsnDuplicateAttribute?)property.GetCustomAttributes(typeof(HcsnDuplicateAttribute), false).FirstOrDefault();

				if (attDuplicate != null && !String.IsNullOrEmpty(propValue.ToString().Trim()))
				{
					var isSameCode = IsDuplicate(assetIncrement, propName);
					if (!isSameCode.IsSuccess)
					{
						var attName = (HcsnNameAttribute?)property.GetCustomAttributes(typeof(HcsnNameAttribute), false).FirstOrDefault();
						var attributeName = (attName as HcsnNameAttribute).PropName;
						inValidList.Add(new ValidateResult
						{
							IsSuccess = false,
							ValidateCode = ValidateCode.Duplicate,
							Message = String.Format(ValidateResource.Duplicate, attributeName, propValue)
						});
						check = false;
					}
				}
			}

			return check;
		}

		/// <summary>
		/// Hàm xử lý logic khi kiểm tra xem thuộc tính có bị trùng không ?
		/// </summary>
		/// <param name="assetIncrement"> Đối tượng tài sản chứa thuôc tính cần kiểm tra trùng </param>
		/// <param name="propertyName">tên thuộc tính cần kiểm tra</param>
		/// <returns>
		/// Kết quả kiểm tra:
		/// IsSuccess == true: không bị trùng
		/// IsSuccess == false:  bị trùng
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public ValidateResult IsDuplicate(FixedAssetIncrement assetIncrement, string propertyName)
		{
			var numberOfAffectedRows = _assetIncrementDL.GetNumberRecordOfPropertyDuplicate(assetIncrement, propertyName);
			return new ValidateResult
			{
				IsSuccess = numberOfAffectedRows == 0,
				ValidateCode = ValidateCode.Duplicate,
			};
		}

		/// <summary>
		/// Hàm validate giá trị không được vượt quá số ký tự cho trước
		/// </summary>
		/// <param name="property">Thuộc tính cần validate</param>
		/// <param name="propName">Tên của thuộc tính cần validate</param>
		/// <param name="propValue">Giá trị của thuộc tính cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		private bool IsOutMaxLength(PropertyInfo property, string propName, object? propValue)
		{
			bool check = true;
			if (property.IsDefined(typeof(HcsnMaxLengthAttribute), false))
			{
				var attHcsnLength = property.GetCustomAttributes(typeof(HcsnMaxLengthAttribute), false).FirstOrDefault();
				int propLength = (attHcsnLength as HcsnMaxLengthAttribute).Length;
				if (propValue.ToString().Length > propLength)
				{
					var attributeName = "";
					if (property.IsDefined(typeof(HcsnNameAttribute), false))
					{
						var attName = (HcsnNameAttribute?)property.GetCustomAttributes(typeof(HcsnNameAttribute), false).FirstOrDefault();
						attributeName = (attName as HcsnNameAttribute).PropName;
					}
					inValidList.Add(new ValidateResult
					{
						IsSuccess = false,
						ValidateCode = ValidateCode.MaxLength,
						Message = String.Format(ValidateResource.OutMaxLength, attributeName, propLength),
						Data = propName
					});
					check = false;
				}
			}

			return check;
		}

		/// <summary>
		/// Hàm xử lý logic khi lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Đối tượng mã code mới</returns>
		/// Created by: LTVIET (20/04/2023)
		public ServiceResult GetNewCode()
		{
			var oldCode = _assetIncrementDL.GetNewCode();
			return new ServiceResult
			{
				IsSuccess = true,
				Data = GenerateNewCode(oldCode)
			};
		}

		/// <summary>
		/// Hàm sinh ra code mới từ việc tách code ra phần chữ và số rồi tăng phần số lên 1 đơn vị
		/// </summary>
		/// <param name="oldCode">Code cần tách ra</param>
		/// <returns>Code mới</returns>
		/// Created by: LTVIET (20/04/2023)
		private string GenerateNewCode(string? oldCode)
		{
			// Thành công
			// lấy ra code của đối tượng asset 
			if (oldCode == null)
			{
				return Resources.NewVoucherCodeDefault;
			}
			string newCode = "";
			bool check = false;
			var regex = new Regex(@"(\D)");
			// vòng lặp lấy 
			while (!check)
			{
				// Tách phần số và phần chữ của code ra
				int lengthOldCode = oldCode.Length;
				string numberPart = "";
				string alphaPart = "";
				for (int i = lengthOldCode - 1; i >= 0; i--)
				{
					if (regex.IsMatch(oldCode[i].ToString()))
					{
						alphaPart = oldCode.Substring(0, i + 1);
						break;
					}
					numberPart = oldCode[i].ToString() + numberPart;
				}
				int lengthNumberPart = numberPart.Length;
				// Tăng phần số lên một đơn vị
				int numberCode = int.Parse(numberPart) + 1;
				// Tạo ra code mới bằng cách nối phần chữ và phần số mới 
				string strNumberCode = "" + numberCode;
				while (strNumberCode.Length < lengthNumberPart)
				{
					strNumberCode = '0' + strNumberCode;
				}
				newCode = alphaPart + strNumberCode;
				// Kiểm tra xem code mới có bị trùng không
				// Lấy ra số bản ghi tài sản có code bằng code mới
				var assetIncrement = new FixedAssetIncrement
				{
					voucher_id = Guid.NewGuid(),
					voucher_code = newCode,
				};
				var result = IsDuplicate(assetIncrement, Resources.FixedAssetIncrementCodePropName);
				check = result.IsSuccess;
				if (!check)
				{
					oldCode = newCode;
				}
			}

			return newCode;
		}

		/// <summary>
		/// Hàm sửa đổi 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrementDTO">Thông tin chứng từ cần sửa sửa đổi</param>
		/// <param name="assetsAdd">Danh sách id tài sản được chứng từ thêm</param>
		/// <param name="assetsDelete">Danh sách id tài sản không còn được chứng từ</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện sửa:
		/// IsSuccess == true: sửa thành công
		/// IsSuccess == false: sửa thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public ServiceResult UpdateAssetIncrement(FixedAssetIncrementDTO assetIncrementDTO, List<Guid>? assetsAdd, List<Guid>? assetsDelete)
		{
			// Validate dữ liệu đầu vào
			var validateResult = ValidateRequesData(assetIncrementDTO);
			// Thất bại
			if (!validateResult.IsSuccess)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.InvalidateData,
					Message = ServiceResource.InvalidData,
					Data = validateResult.Data
				};
			}

			// Thành công

			var isUpdateSuccess = _assetIncrementDL.UpdateAssetIncrement(assetIncrementDTO, assetsAdd, assetsDelete);
			return new ServiceResult
			{
				IsSuccess = isUpdateSuccess,
				ErrorCode = ErrorCode.UpdateFailed,
				Message = ServiceResource.UpdateFailed,
			};
		}

		/// <summary>
		/// Hàm gọi database thực hiện việc xóa 1 bản ghi
		/// </summary>
		/// <param name="voucherId">Id bản ghi chứng từ muốn xóa</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện xóa:
		/// IsSuccess == true: xóa thành công
		/// IsSuccess == false: xóa thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public ServiceResult DeleteAssetIncrementById(Guid voucherId)
		{
			var numberOfAffectedRows = _assetIncrementDL.DeleteAssetIncrementById(voucherId);
			return new ServiceResult
			{
				IsSuccess = numberOfAffectedRows != 0,
				ErrorCode = ErrorCode.DeleteFailed,
				Message = ServiceResource.DeleteFailed
			};
		}

		/// <summary>
		/// Hàm gọi database để thực hiện việc xóa nhiều bản ghi
		/// </summary>
		/// <param name="ids">Danh sách id bản ghi cần xóa</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện xóa:
		/// IsSuccess == true: xóa thành công
		/// IsSuccess == false: xóa thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		public ServiceResult DeleteMultipleAssetIncrement(List<Guid> ids)
		{
			var numberOfAffectedRows = _assetIncrementDL.DeleteMultipleAssetIncrement(ids);
			return new ServiceResult
			{
				IsSuccess = numberOfAffectedRows != 0,
				ErrorCode = ErrorCode.DeleteMultipleFailed,
				Message = ServiceResource.DeleteMultipleFailed,
				Data = numberOfAffectedRows
			};
		}


		#endregion
	}
}
