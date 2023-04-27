using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using Hcsn.WebApplication.DL.AssetIncrementTest1DL;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.BL.AssetIncrementTest1BL
{
	public class AssetIncrementTestBL : BaseBL<FixedAssetIncrement>, IAssetIncrementTestBL
	{
		#region Field
        private IAssetIncrementTestDL _assetIncrementDL;
		#endregion
		#region Constructor
		public AssetIncrementTestBL(IAssetIncrementTestDL assetIncrementDL) : base(assetIncrementDL)
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
		/// Created by: LTVIET (09/03/2023)
		public ServiceResult GetPaging(string? keyword, int pageSize, int pageNumber)
		{
			var result = _assetIncrementDL.GetPaging(keyword, pageSize, pageNumber);
			if (result.Data == null)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.NotFound,
					Message = ServiceResource.NotFound
				};
			}
			return new ServiceResult
			{
				IsSuccess = true,
				Data = result
			};
		}

		public ServiceResult GetAssetIncrementDTOById(Guid assetIncrementId)
		{
			var record = _assetIncrementDL.GetAssetIncrementDTOById(assetIncrementId);
			if (record == null)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.NotFound,
					Message = ServiceResource.NotFound
				};
			}
			return new ServiceResult
			{
				IsSuccess = true,
				Data = record
			};
		}

		/// <summary>
		/// Hàm thêm mới 1 bản ghi chứng từ
		/// </summary>
		/// <param name="entity">Đối tượng chứa thông tin chứng từ cần thêm mới(thông tin chứng từ, danh sách tài sản ghi tăng)</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện thêm mới:
		/// IsSuccess == true: thêm mới thành công
		/// IsSuccess == false: thêm mới thất bại
		/// </returns>
		public ServiceResult InsertAssetIncrement(FixedAssetIncrementDTO entity)
		{
			var assetIncrement = new FixedAssetIncrement()
			{
				voucher_id = entity.voucher_id,
				voucher_code = entity.voucher_code,
				voucher_date = entity.voucher_date,
				increment_date = entity.increment_date,
				price = entity.price,
				description = entity.description,
			};
			var assets = entity.assets;
			var validateResult = ValidateRequesData(assetIncrement);
			if (assets.Count == 0 || assets == null)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.NoAssetIncrements,
					Message = ValidateResource.NoAssetIncrements,
				});
				validateResult.IsSuccess = false;
				validateResult.Data = inValidList;
			}

			if (!validateResult.IsSuccess)
			{
				return new ServiceResult
				{
					// Thất bại
					IsSuccess = false,
					ErrorCode = ErrorCode.InvalidateData,
					Message = ServiceResource.InvalidData,
					Data = validateResult.Data
				};
			}

			int numberOfAffectedRows = _assetIncrementDL.InsertAssetIncrement(assetIncrement, assets);
			if (numberOfAffectedRows > 0)
			{
				return new ServiceResult
				{
					IsSuccess = true,
				};
			}
			else
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.InsertFailed,
					Message = ServiceResource.InsertFailed,
				};
			}


		}

		protected override ValidateResult ValidateRequesData(FixedAssetIncrement record)
		{
			return base.ValidateRequesData(record);
		}

		protected override bool ValidateCustom(FixedAssetIncrement assetIncrement)
		{
			bool check = true;
			var properties = typeof(FixedAssetIncrement).GetProperties();


			foreach (var property in properties)
			{
				string propName = property.Name;
				var propValue = property.GetValue(assetIncrement);
				bool isDuplicate = IsPropertyDuplicate(assetIncrement, property, propName, propValue);
				bool isOutMaxLength = IsOutMaxLength(property, propName, propValue);

				if (!isDuplicate | !isOutMaxLength)
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
		/// Created by: LTVIET (09/03/2023)
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
		/// Created by: LTViet (20/03/2023)
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
		/// Created by: LTVIET (09/03/2023)
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
		/// Created by: LTViet (20/03/2023)
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
		/// Created by: LTViet (20/03/2023)
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

		
		#endregion
	}
}
