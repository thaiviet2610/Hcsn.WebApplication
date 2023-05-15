using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using System.Reflection;
using System.Text.RegularExpressions;
using Hcsn.WebApplication.DL.AssetIncrementDL;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;

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
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
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
			bool check = _assetIncrementDL.InsertAssetIncrement(assetIncrementDTO, assets);
			return new ServiceResult
			{
				IsSuccess = check,
				ErrorCode = ErrorCode.InsertFailed,
				Message = ServiceResource.InsertFailed,
			};
		}

		/// <summary>
		/// Hàm validate chung dữ liệu 
		/// </summary>
		/// <param name="assetIncrementDTO">bản ghi cần validate</param>
		/// <returns>
		/// Kết quả validate dữ liệu:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		private ValidateResult ValidateRequesData(FixedAssetIncrementDTO assetIncrementDTO)
		{
			var validateEmptyResult = ValidateEmpty(assetIncrementDTO);
			var validateCustomResult = ValidateCustom(assetIncrementDTO);
			bool checkAssetsNull = false;
			if (assetIncrementDTO.assets.Count == 0 || assetIncrementDTO.assets == null)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.NoAssetIncrements,
					Message = ValidateResource.NoAssetIncrements,
				});
				checkAssetsNull = true;
			}

			return new ValidateResult
			{
				IsSuccess = validateEmptyResult && validateCustomResult && !checkAssetsNull,
				Data = inValidList
			};
			
		}

		/// <summary>
		/// Hàm validate giá trị các thuộc tính không được để trống
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứa các thuộc tính</param>
		/// <returns>
		/// Kết quả validate
		/// true: thành công
		/// fasle: thất bại
		/// </returns>
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
				bool isDuplicate = false;
				if (!isOutMaxLength)
				{
					isDuplicate = IsPropertyDuplicate(assetIncrement, property, propName, propValue);
				}

				if (isOutMaxLength || isDuplicate)
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
		/// true: có lỗi
		/// flase: Không Có lỗi
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		private bool IsPropertyDuplicate(FixedAssetIncrement assetIncrement, PropertyInfo property, string propName, object? propValue)
		{
			bool checkDuplicate = false;
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
						checkDuplicate = true;
					}
				}
			}

			return checkDuplicate;
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
		/// true:  có lỗi
		/// flase: Không Có lỗi
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		private bool IsOutMaxLength(PropertyInfo property, string propName, object? propValue)
		{
			bool checkOutMaxLength = false;
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
					checkOutMaxLength = true;
				}
			}

			return checkOutMaxLength;
		}

		/// <summary>
		/// Hàm xử lý logic khi lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Đối tượng mã mới</returns>
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
		/// <returns>Mã mới</returns>
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
		/// Hàm sửa đổi tổng nguyên giá của bản ghi chứng từ
		/// </summary>
		/// <param name="voucherId">Id của chứng từ cần sửa</param>
		/// <param name="price">Giá trị của tổng nguyên giá</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện sửa:
		/// IsSuccess == true: sửa thành công
		/// IsSuccess == false: sửa thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		public ServiceResult UpdateAssetIncrementPrice(Guid voucherId, Decimal price)
		{
			if (price <= 0)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.InvalidateData,
					Message = ServiceResource.InvalidData,
				};
			}

			// Thành công

			var isUpdateSuccess = _assetIncrementDL.UpdateAssetIncrementPrice(voucherId, price);
			return new ServiceResult
			{
				IsSuccess = isUpdateSuccess == 1,
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
			bool checkDelete = _assetIncrementDL.DeleteAssetIncrementById(voucherId);
			return new ServiceResult
			{
				IsSuccess = checkDelete,
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
			var checkDeleteMultiple = _assetIncrementDL.DeleteMultipleAssetIncrement(ids);
			return new ServiceResult
			{
				IsSuccess = checkDeleteMultiple,
				ErrorCode = ErrorCode.DeleteMultipleFailed,
				Message = ServiceResource.DeleteMultipleFailed,
			};
		}

		/// <summary>
		/// Hàm logic xử lý việc xuất toàn bộ danh sách chứng từ ra file excel
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm</param>
		/// <returns>Đối tượng stream lưu dữ liệu</returns>
		/// Created by: LTVIET (29/04/2023)
		public Stream ExportAllExcel(string? keyword)
		{
			var result = _assetIncrementDL.GetPaging(keyword, 0, 0);
			var assetIncrements = result.Data;
			Stream data = GenerateExcelAssetIncrementsFile(assetIncrements, result);
			return data;
		}

		/// <summary>
		/// Hàm sinh ra file excel từ dữ liệu cho trước
		/// </summary>
		/// <param name="assetIncrements">Danh sách chứng từ truyền vào</param>
		/// <param name="pagingResultAssetIncrement">Dữ liệu thu được sau khi lọc danh sách chứng từ</param>
		/// <returns>Đối tượng stream lưu file excel</returns>
		/// Created by: LTVIET (29/04/2023)
		public static Stream GenerateExcelAssetIncrementsFile(List<FixedAssetIncrementDTO> assetIncrements, PagingResultAssetIncrement pagingResultAssetIncrement)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var stream = new MemoryStream();
			using (var package = new ExcelPackage(stream))
			{
				var workSheet = package.Workbook.Worksheets.Add(ExportAssetIncrementResult.SheetName);

				// tạo tiêu đề của bảng trong file excel
				CreateTitleTableExcel(workSheet, "A1:F1", ExportAssetIncrementResult.Title, 20);
				// tạo header table
				CreateHeaderTableAssetIncrementExcel(workSheet);
				// tạo dữ liệu
				CreateDataTableAssetIncrementExcel(assetIncrements, workSheet);
				// Tạo footer table
				CreateFooterTable(assetIncrements.Count, pagingResultAssetIncrement, workSheet);
				// format table
				FormatTableAssetIncrementExcel(assetIncrements.Count, workSheet, ExportAssetIncrementResult.Headers.Count);

				package.Save();
			}
			stream.Position = 0;

			return stream;

		}

		/// <summary>
		/// Hàm format cột và ô của table trong excel
		/// </summary>
		/// <param name="count">Số lượng bản ghi trong danh sách dữ liệu</param>
		/// <param name="countColumn">Số lượng cột trong table</param>
		/// /// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/04/2023)
		private static void FormatTableAssetIncrementExcel(int count, ExcelWorksheet workSheet,int countColumn)
		{
			// Định dạng cell border
			for (int i = 0; i <= count; i++)
			{
				for (int j = 1; j < (countColumn + 1); j++)
				{
					workSheet.Cells[i + 2, j].Style.Font.Size = 10;
					FormatBorderCell(workSheet, "", i + 2, j);

				}
			}
			// định dạng độ rộng của cột
			for (int i = 1; i < (countColumn + 1); i++)
			{
				workSheet.Column(i).AutoFit();
			}

			// định dạng độ cao của dòng
			for (int i = 2; i < count + 3; i++)
			{
				workSheet.Row(i).Height = 20;
			}

		}

		/// <summary>
		/// Hàm tạo footer cho table trong excel
		/// </summary>
		/// <param name="count">Số bản ghi trong danh sách dữ liệu</param>
		/// <param name="pagingResultAssetIncrement">Dữ liệu thu được sau khi lọc danh sách chứng từ</param>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/04/2023)
		private static void CreateFooterTable(int count, PagingResultAssetIncrement pagingResultAssetIncrement, ExcelWorksheet workSheet)
		{
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
			string priceTotal = pagingResultAssetIncrement.PriceTotal.ToString("#,###", cul.NumberFormat);

			if (count == 0)
			{
				count = 1;
				priceTotal = "0";
			}
			count += 3;
			workSheet.Cells[$"A{count}:D{count}"].Merge = true;
			workSheet.Cells[$"A{count}:D{count}"].Value = ExportAssetIncrementResult.FooterTotal;
			workSheet.Cells[$"A{count}:D{count}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			workSheet.Cells[$"A{count}:D{count}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			workSheet.Cells[$"A{count}:D{count}"].Style.Font.Bold = true;
			FormatBorderCell(workSheet, $"A{count}:D{count}" , 0, 0);

			workSheet.Cells[$"E{count}"].Value = priceTotal;
			workSheet.Cells[$"E{count}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			workSheet.Cells[$"E{count}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			workSheet.Cells[$"E{count}"].Style.Font.Bold = true;
			FormatBorderCell(workSheet, $"E{count}", 0,0);

			workSheet.Cells[$"F{count}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			workSheet.Cells[$"F{count}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			workSheet.Cells[$"F{count}"].Style.Font.Bold = true;
			FormatBorderCell(workSheet, $"F{count}", 0, 0);
		}

		/// <summary>
		/// Hàm format phần border của 1 ô hoặc 1 hàng các ô trong excel
		/// </summary>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// <param name="cellAddress">Địa chỉ dạng String của ô cần format</param>
		/// <param name="indexRowCell">Địa chỉ của ô theo hàng ngang </param>
		/// <param name="indexColumnCell">Địa chỉ của ô theo hàng dọc</param>
		/// Created by: LTVIET (08/05/2023)
		private static void FormatBorderCell(ExcelWorksheet workSheet, String cellAddress,int indexRowCell, int indexColumnCell)
		{
			if(cellAddress != "")
			{
				workSheet.Cells[cellAddress].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[cellAddress].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[cellAddress].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[cellAddress].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
			}
			else
			{
				workSheet.Cells[indexRowCell, indexColumnCell].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[indexRowCell, indexColumnCell].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[indexRowCell, indexColumnCell].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[indexRowCell, indexColumnCell].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
			}
		}

		/// <summary>
		/// Hàm tạo dữ liệu trong bảng excel
		/// </summary>
		/// <param name="assetIncrements">Đối tượng chứng từ truyền dữ liệu vào table</param>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/04/2023)
		private static void CreateDataTableAssetIncrementExcel(List<FixedAssetIncrementDTO> assetIncrements, ExcelWorksheet workSheet)
		{
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
			if (assetIncrements.Count == 0)
			{
				workSheet.Cells["A3:F3"].Merge = true;
				workSheet.Cells["A3:F3"].Value = ExportAssetResult.NoData;
				workSheet.Cells["A3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				workSheet.Cells["A3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				workSheet.Cells["A3:F3"].Style.Font.Bold = true;
				FormatBorderCell(workSheet, "A3:F3", 0, 0);
			}
			else
			{
				for (int i = 0; i < assetIncrements.Count; i++)
				{
					// Gán giá trị vào từng ô trên từng dòng trong file excel
					workSheet.Cells[i + 3, 1].Value = i+1;
					workSheet.Cells[i + 3, 2].Value = assetIncrements[i].voucher_code;
					workSheet.Cells[i + 3, 3].Value = ((DateTime)assetIncrements[i].voucher_date).ToString("dd/MM/yyyy");
					workSheet.Cells[i + 3, 4].Value = ((DateTime)assetIncrements[i].increment_date).ToString("dd/MM/yyyy");
					workSheet.Cells[i + 3, 5].Value = assetIncrements[i].price.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 3, 6].Value = assetIncrements[i].description; 
										
					workSheet.Cells[i + 3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					workSheet.Cells[i + 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					workSheet.Cells[i + 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					workSheet.Cells[i + 3, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 3, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
				}
			}

		}

		/// <summary>
		/// Hàm tạo header cho table chứng từ trong excel
		/// </summary>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/04/2023)
		private static void CreateHeaderTableAssetIncrementExcel(ExcelWorksheet workSheet)
		{
			var headers = ExportAssetIncrementResult.Headers;

			for (int i = 0; i < headers.Count; i++)
			{
				workSheet.Cells[2, i + 1].Value = headers[i];
				workSheet.Cells[2, i + 1].Style.Font.Bold = true;
				FormatBorderCell(workSheet, "", 2, i+1);
				if (i == 0 || i == 2 || i== 3)
				{
					workSheet.Cells[2, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					workSheet.Cells[2, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				}
				else if (i == 1 || i == 5)
				{
					workSheet.Cells[2, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					workSheet.Cells[2, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
				}
				else
				{
					workSheet.Cells[2, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					workSheet.Cells[2, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
				}
			}
		}

		/// <summary>
		/// Hàm tạo tiêu đề cho table trong excel
		/// </summary>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// <param name="cellAddress">Địa chỉ của tiêu đề</param>
		/// <param name="title">Tên tiêu đề</param>
		/// <param name="size">Kích cỡ của tiêu đề</param>
		/// Created by: LTVIET (29/04/2023)
		private static void CreateTitleTableExcel(ExcelWorksheet workSheet,String cellAddress,String title,int size)
		{
			workSheet.Cells[cellAddress].Merge = true;
			workSheet.Cells[cellAddress].Value = title;
			workSheet.Cells[cellAddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			workSheet.Cells[cellAddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			workSheet.Cells[cellAddress].Style.Font.Bold = true;
			workSheet.Cells[cellAddress].Style.Font.Size = size;
			FormatBorderCell(workSheet, cellAddress, 0, 0);
		}

		/// <summary>
		/// Hàm logic xử lý việc xuất dữ liệu của 1 chứng từ ra file excel
		/// </summary>
		/// <param name="voucherId">Id chứng từ</param>
		/// <returns>Đối tượng stream lưu dữ liệu</returns>
		/// Created by: LTVIET (29/04/2023)
		public Stream ExportDetailExcel(Guid voucherId)
		{
			var assetIncrement = _assetIncrementDL.GetById(voucherId);
			Stream data = GenerateExcelFileAssetIncrementDetail(assetIncrement);
			return data;
		}



		/// <summary>
		/// Hàm sinh ra file excel chứng từ chi tiết từ dữ liệu cho trước
		/// </summary>
		/// <param name="assetIncrement">Đối tượng chứng từ truyền vào</param>
		/// <returns>Đối tượng stream lưu file excel</returns>
		/// Created by: LTVIET (29/04/2023)
		public static Stream GenerateExcelFileAssetIncrementDetail(FixedAssetIncrementDTO assetIncrement)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var stream = new MemoryStream();
			using (var package = new ExcelPackage(stream))
			{
				var workSheet = package.Workbook.Worksheets.Add(ExportAssetIncrementDetailResult.SheetName);

				// tạo tiêu đề của bảng trong file excel
				var assetIncrements = new List<FixedAssetIncrementDTO>
				{
					assetIncrement
				};
				CreateTitleTableExcel(workSheet, "A1:F1", ExportAssetIncrementDetailResult.TitleAssetIncrementTable,10);
				// tạo header table
				CreateHeaderTableAssetIncrementExcel(workSheet);
				// tạo dữ liệu
				CreateDataTableAssetIncrementExcel(assetIncrements, workSheet);

				FormatTableAssetIncrementExcel(assetIncrements.Count, workSheet, ExportAssetIncrementDetailResult.HeaderAssetIncrements.Count);
				var assets = assetIncrement.assets;
				// tạo tiêu đề của bảng trong file excel
				CreateTitleTableExcel(workSheet, "A5:G5", ExportAssetIncrementDetailResult.TitleAssetTable, 10);
				// tạo header table
				CreateHeaderTableAssetsExcel(workSheet);
				// tạo dữ liệu
				CreateDataTableAssetsExcel(assets, workSheet);
				// format table
				FormatTableAssetsExcel(assets.Count, workSheet, ExportAssetIncrementDetailResult.HeaderAssets.Count);

				package.Save();
			}
			stream.Position = 0;

			return stream;

		}

		/// <summary>
		/// Hàm tạo header cho table tài sản chứng từ trong file excel chứng từ chi tiết
		/// </summary>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/04/2023)
		private static void CreateHeaderTableAssetsExcel(ExcelWorksheet workSheet)
		{
			var headers = new List<String>() 
				{
					Resources.TableExcelHeaderColumnIndex,Resources.TableExcelHeaderColumnAssetCode,
					Resources.TableExcelHeaderColumnAssetName,Resources.TableExcelHeaderColumnDepartment,Resources.TableExcelHeaderColumnCost,
					Resources.TableExcelHeaderColumnDepreciationValue,Resources.TableExcelHeaderColumnResidualValue
				}; ;

			for (int i = 0; i < headers.Count; i++)
			{
				workSheet.Cells[6, i + 1].Value = headers[i];
				workSheet.Cells[6, i + 1].Style.Font.Bold = true;
				FormatBorderCell(workSheet, "", 6, i + 1);
				workSheet.Cells[6, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				if (i == 0)
				{
					workSheet.Cells[6, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				}
				else if (i == 1 || i == 2 || i == 3)
				{
					workSheet.Cells[6, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
				}
				else
				{
					workSheet.Cells[6, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
				}
			}
		}

		/// <summary>
		/// Hàm tạo dữ liệu trong bảng tài sản chứng từ trong file excel chứng từ chi tiết
		/// </summary>
		/// <param name="assets">Đối tượng tài sản truyền dữ liệu vào table</param>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/04/2023)
		private static void CreateDataTableAssetsExcel(List<FixedAssetDTO> assets, ExcelWorksheet workSheet)
		{
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
			if (assets.Count == 0)
			{
				workSheet.Cells["A7:G7"].Merge = true;
				workSheet.Cells["A7:G7"].Value = ExportAssetResult.NoData;
				workSheet.Cells["A7:G7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				workSheet.Cells["A7:G7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				workSheet.Cells["A7:G7"].Style.Font.Bold = true;
				workSheet.Cells["A7:G7"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["A7:G7"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["A7:G7"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["A7:G7"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
			}
			else
			{
				for (int i = 0; i < assets.Count; i++)
				{
					// Gán giá trị vào từng ô trên từng dòng trong file excel
					workSheet.Cells[i + 7, 1].Value = i+1;
					workSheet.Cells[i + 7, 2].Value = assets[i].fixed_asset_code;
					workSheet.Cells[i + 7, 3].Value = assets[i].fixed_asset_name;
					workSheet.Cells[i + 7, 4].Value = assets[i].department_name;
					workSheet.Cells[i + 7, 5].Value = assets[i].cost == 0 ? 0 : assets[i].cost.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 7, 6].Value = assets[i].depreciation_value == 0 ? 0 : assets[i].depreciation_value.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 7, 7].Value = assets[i].residual_value <= 0 ? 0 : assets[i].residual_value.ToString("#,###", cul.NumberFormat);
										
					workSheet.Cells[i + 7, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					workSheet.Cells[i + 7, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 7, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 7, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 7, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 7, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 7, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
				}
			}

		}

		/// <summary>
		/// Hàm format cột và ô của table trong excel
		/// </summary>
		/// <param name="count">Số lượng bản ghi trong danh sách dữ liệu</param>
		/// <param name="countColumn">Số lượng cột trong table</param>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/04/2023)
		private static void FormatTableAssetsExcel(int count, ExcelWorksheet workSheet, int countColumn)
		{
			// Định dạng cell border
			for (int i = 0; i < count; i++)
			{
				for (int j = 1; j < (countColumn + 1); j++)
				{
					workSheet.Cells[i + 7, j].Style.Font.Size = 10;
					FormatBorderCell(workSheet, "", i + 7, j);

				}
			}
			// định dạng độ rộng của cột
			for (int i = 1; i < (countColumn + 1); i++)
			{
				workSheet.Column(i).AutoFit();
			}

			// định dạng độ cao của dòng
			for (int i = 7; i < count + 7; i++)
			{
				workSheet.Row(i).Height = 20;
			}

		}
		#endregion
	}
}
