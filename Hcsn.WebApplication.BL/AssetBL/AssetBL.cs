using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.DL.AssetDL;
using System.Text.RegularExpressions;
using Hcsn.WebApplication.Common.Resource;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;
using System.Reflection;

namespace Hcsn.WebApplication.BL.AssetBL
{
    public class AssetBL : BaseBL<FixedAsset>, IAssetBL
    {
        #region Field
        private IAssetDL _assetDL;
		#endregion

		#region Constructor
		public AssetBL(IAssetDL assetDL) : base(assetDL)
        {
            _assetDL = assetDL;
        }
		#endregion

		#region Method
		/// <summary>
		/// Hàm lấy danh sách tài sản theo bộ lọc và phân trang
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm (mã tài sản, tên tài sản)</param> 
		/// <param name="departmentId">Id của phòng ban</param> 
		/// <param name="fixedAssetCatagortId">Id của loại tài sản</param> 
		/// <param name="pageSize">Số bản ghi trong 1 trang</param> 
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns> 
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		public ServiceResult GetPaging(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber)
        {
            var result = _assetDL.GetPaging(keyword, departmentId, fixedAssetCatagortId, pageSize, pageNumber);
            if (result.Data == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = ErrorCode.NotFound,
                    Message = ServiceResource.NotFound
                };
            }
			foreach (var item in result.Data)
			{
				item.residual_value = item.residual_value < 0 ? 0 : item.residual_value;
			}
			return new ServiceResult
            {
                IsSuccess = true,
                Data = result
            };
        }

		/// <summary>
		/// Hàm logic xử lý việc xuất dữ liệu ra file excel
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm</param>
		/// <param name="departmentId">Id phòng ban</param>
		/// <param name="fixedAssetCatagortId">Id loại tài sản</param>
		/// <returns>Đối tượng stream lưu dữ liệu</returns>
		/// Created by: LTVIET (09/03/2023)
		public Stream ExportExcel(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId)
		{
			var result = _assetDL.GetPaging(keyword, departmentId, fixedAssetCatagortId, 0, 0);
			var assets = result.Data;
			Stream data = GenerateExcelFile(assets, result);
			return data;
		}

		/// <summary>
		/// Hàm sinh ra file excel từ dữ liệu cho trước
		/// </summary>
		/// <param name="assets">Danh sách tài sản truyền vào</param>
		/// <param name="pagingResultAsset">Dữ liệu thu được sau khi phân trang,lọc danh sách tài sản</param>
		/// <returns>Đối tượng stream lưu file excel</returns>
		/// Created by: LTVIET (09/03/2023)
		public static Stream GenerateExcelFile(List<FixedAssetDTO> assets, PagingResultAsset pagingResultAsset)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var stream = new MemoryStream();
			using (var package = new ExcelPackage(stream))
			{
				var workSheet = package.Workbook.Worksheets.Add(ExcelResult.SheetName);

				// tạo tiêu đề của bảng trong file excel
				CreateTitleTableExcel(workSheet);
				// tạo header table
				CreateHeaderTableExcel(workSheet);
				// tạo dữ liệu
				CreateDataTableExcel(assets, workSheet);
				// Tạo footer table
				CreateFooterTable(assets.Count, pagingResultAsset, workSheet);
				// format table
				FormatTableExcel(assets.Count, workSheet);

				package.Save();
			}
			stream.Position = 0;
			
			return stream;

		}

		/// <summary>
		/// Hàm format cột và ô của table trong excel
		/// </summary>
		/// <param name="count">Số lượng bản ghi trong danh sách dữ liệu</param>
		/// /// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/03/2023)
		private static void FormatTableExcel(int count, ExcelWorksheet workSheet)
		{
			// Định dạng cell border
			for (int i = 0; i <= count; i++)
			{
				for (int j = 2; j < 11; j++)
				{
					workSheet.Cells[i + 3, j].Style.Font.Size = 10;
					workSheet.Cells[i + 3, j].Style.Border.Top.Style = ExcelBorderStyle.Medium;
					workSheet.Cells[i + 3, j].Style.Border.Right.Style = ExcelBorderStyle.Medium;
					workSheet.Cells[i + 3, j].Style.Border.Left.Style = ExcelBorderStyle.Medium;
					workSheet.Cells[i + 3, j].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

				}
			}
			// định dạng độ rộng của cột
			for (int i = 2; i < 11; i++)
			{
				workSheet.Column(i).AutoFit();
			}

			// định dạng độ cao của dòng
			for (int i = 3; i < count+4; i++)
			{
				workSheet.Row(i).Height = 20;
			}

		}

		/// <summary>
		/// Hàm tạo footer cho table trong excel
		/// </summary>
		/// <param name="count">Số bản ghi trong danh sách dữ liệu</param>
		/// <param name="pagingResultAsset">Dữ liệu thu được sau khi phân trang,lọc danh sách tài sản</param>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/03/2023)
		private static void CreateFooterTable(int count, PagingResultAsset pagingResultAsset, ExcelWorksheet workSheet)
		{
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
			string quantityTotal = pagingResultAsset.QuantityTotal.ToString("#,###", cul.NumberFormat);
			string costTotal = pagingResultAsset.CostTotal.ToString("#,###", cul.NumberFormat);
			string depreciationValueTotal = pagingResultAsset.DepreciationValueTotal.ToString("#,###", cul.NumberFormat);
			string residualValueTotal = pagingResultAsset.ResidualValueTotal.ToString("#,###", cul.NumberFormat);
			
            if (count == 0)
            {
				count = 1;
				quantityTotal = "0";
				costTotal = "0";	
				depreciationValueTotal = "0";
				residualValueTotal = "0";
            }
            count += 4;
			workSheet.Cells[$"B{count}:F{count}"].Merge = true;
			workSheet.Cells[$"B{count}:F{count}"].Value = ExcelResult.FooterTotal;
			workSheet.Cells[$"B{count}:F{count}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			workSheet.Cells[$"B{count}:F{count}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			workSheet.Cells[$"B{count}:F{count}"].Style.Font.Bold = true;
			workSheet.Cells[$"B{count}:F{count}"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"B{count}:F{count}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"B{count}:F{count}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"B{count}:F{count}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

			List<string> listFooter = new()
				{
					$"G{count}",$"H{count}",$"I{count}",$"J{count}"
				};

			for (int i = 0; i < listFooter.Count; i++)
			{

				workSheet.Cells[listFooter[i]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				workSheet.Cells[listFooter[i]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
				workSheet.Cells[listFooter[i]].Style.Font.Bold = true;
				workSheet.Cells[listFooter[i]].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[listFooter[i]].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[listFooter[i]].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[listFooter[i]].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
				switch (i)
				{
					case 0:
						workSheet.Cells[listFooter[i]].Value = quantityTotal;
						break;
					case 1:
						workSheet.Cells[listFooter[i]].Value = costTotal;
						break;
					case 2:
						workSheet.Cells[listFooter[i]].Value = depreciationValueTotal;
						break;
					case 3:
						workSheet.Cells[listFooter[i]].Value = residualValueTotal;
						break;
				}
			}
		}

		/// <summary>
		/// Hàm tạo dữ liệu trong bảng excel
		/// </summary>
		/// <param name="assets">Đối tượng tài sản truyền dữ liệu vào table</param>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/03/2023)
		private static void CreateDataTableExcel(List<FixedAssetDTO> assets, ExcelWorksheet workSheet)
		{
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
			if(assets.Count == 0)
			{
				workSheet.Cells["B4:J4"].Merge = true;
				workSheet.Cells["B4:J4"].Value = ExcelResult.NoData;
				workSheet.Cells["B4:J4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				workSheet.Cells["B4:J4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				workSheet.Cells["B4:J4"].Style.Font.Bold = true;
				workSheet.Cells["B4:J4"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["B4:J4"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["B4:J4"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["B4:J4"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
			}
			else
			{
				for (int i = 0; i < assets.Count; i++)
				{
					// Chuẩn bị giá trị đầu vào

					var residualValue = Math.Round(assets[i].cost - assets[i].depreciation_value);
					residualValue = residualValue > 0 ? residualValue : 0;
					// Gán giá trị vào từng ô trên từng dòng trong file excel
					workSheet.Cells[i + 4, 2].Value = (i + 1).ToString();
					workSheet.Cells[i + 4, 3].Value = assets[i].fixed_asset_code;
					workSheet.Cells[i + 4, 4].Value = assets[i].fixed_asset_name;
					workSheet.Cells[i + 4, 5].Value = assets[i].fixed_asset_category_name;
					workSheet.Cells[i + 4, 6].Value = assets[i].department_name;
					workSheet.Cells[i + 4, 7].Value = assets[i].quantity == 0 ? 0 : assets[i].quantity.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 4, 8].Value = assets[i].cost == 0 ? 0 : assets[i].cost.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 4, 9].Value = assets[i].depreciation_value == 0 ? 0 : assets[i].depreciation_value.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 4, 10].Value = residualValue <= 0 ? 0 : residualValue.ToString("#,###", cul.NumberFormat);

					workSheet.Cells[i + 4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					workSheet.Cells[i + 4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 4, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
				}
			}
			
		}

		/// <summary>
		/// Hàm tạo header cho table trong excel
		/// </summary>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/03/2023)
		private static void CreateHeaderTableExcel(ExcelWorksheet workSheet)
		{
			List<string> headers = ExcelResult.Headers;

			for (int i = 0; i < headers.Count; i++)
			{
				workSheet.Cells[3, i + 2].Value = headers[i];
				workSheet.Cells[3, i + 2].Style.Font.Bold = true;
				workSheet.Cells[3, i + 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[3, i + 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[3, i + 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[3, i + 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
				if (i == 0)
				{
					workSheet.Cells[3, i + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					workSheet.Cells[3, i + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				}
				else if (i == 1 || i == 2 || i == 3 || i == 4)
				{
					workSheet.Cells[3, i + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					workSheet.Cells[3, i + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
				}
				else
				{
					workSheet.Cells[3, i + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					workSheet.Cells[3, i + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
				}
			}
		}

		/// <summary>
		/// Hàm tạo tiêu đề cho table trong excel
		/// </summary>
		/// <param name="workSheet">Đối tượng worksheet cần tạo table</param>
		/// Created by: LTVIET (29/03/2023)
		private static void CreateTitleTableExcel(ExcelWorksheet workSheet)
		{
			workSheet.Cells["B2:J2"].Merge = true;
			workSheet.Cells["B2:J2"].Value = ExcelResult.Title;
			workSheet.Cells["B2:J2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			workSheet.Cells["B2:J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			workSheet.Cells["B2:J2"].Style.Font.Bold = true;
			workSheet.Cells["B2:J2"].Style.Font.Size = 20;
			workSheet.Cells["B2:J2"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
			workSheet.Cells["B2:J2"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
			workSheet.Cells["B2:J2"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
			workSheet.Cells["B2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
		}

		/// <summary>
		/// Validate dữ liệu cho đối tượng asset
		/// </summary>
		/// <param name="asset">Đối tượng cần validate</param>
		/// <returns>
		/// Trả về kết quả validate, bao gồm:
		/// True: Nếu validate dữ liệu thành công
		/// False: Nếu validate dữ liệu thất bại
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		protected override bool ValidateCustom(FixedAsset asset)
		{
			bool check = true;
			var properties = typeof(FixedAsset).GetProperties();


			foreach (var property in properties)
			{
				string propName = property.Name;
				var propValue = property.GetValue(asset);
				bool isDuplicate = IsPropertyDuplicate(asset, property, propName, propValue);
				bool isOutMaxLength = IsOutMaxLength(property, propName, propValue);
				bool isOutRangeOfRate = IsOutRangeOfRate(property, propValue);

				if (!isDuplicate | !isOutMaxLength | !isOutRangeOfRate)
				{
					check = false;
				}
			}
			bool isPropertyNumberGreaterThanZero = IsCheckPropertyNumberLessThanOrEqualZero(asset.quantity, asset.cost,
																			asset.life_time, asset.depreciation_rate);
			bool isDepreciationYearGreaterThanCost = IsDepreciationYearGreaterThanCost(asset);
			bool isDepreciationRateDifferentOnePerLifeTime = IsDepreciationRateDifferentOnePerLifeTime(asset);
			bool isProductionYearGreaterThanPurchaseDate = IsProductionYearGreaterThanPurchaseDate(asset);

			if(!isPropertyNumberGreaterThanZero | !isDepreciationYearGreaterThanCost 
				| !isDepreciationRateDifferentOnePerLifeTime | !isProductionYearGreaterThanPurchaseDate)
			{
				check = false;
			}
			return check;
		}

		/// <summary>
		/// Hàm validate giá trị ngày mua phải là ngày diễn ra trước ngày sử dụng
		/// </summary>
		/// <param name="asset">Đối tượng tài sản cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsProductionYearGreaterThanPurchaseDate(FixedAsset asset)
		{
			bool check = true;
			if ((asset.purchase_date != null && asset.production_year != null) &&
							((TimeSpan)(asset.purchase_date - asset.production_year)).TotalDays > 0)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.PurchaseDateGreaterThanProductionYear,
					Message = ValidateResource.PurchaseDateGreaterThanProductionYear,
					Data = Resources.ProductionYear
				});
				check = false;
			}

			return check;
		}

		/// <summary>
		/// Hàm validate tỷ lệ hao mòn phải bằng 1/số năm sử dụng
		/// </summary>
		/// <param name="asset">Đối tượng tài sản cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsDepreciationRateDifferentOnePerLifeTime(FixedAsset asset)
		{
			bool check = true;
			if (asset.life_time != 0 && asset.depreciation_rate != (float)Math.Round((decimal)1 / asset.life_time, 3))
			{

				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.DepreciationRateDifferentLifeTime,
					Message = ValidateResource.DepreciationRateDifferentLifeTime,
					Data = Resources.LifeTimePropName
				});
				check = false;
			}

			return check;
		}

		/// <summary>
		/// Hàm validate giá trị hao mòn năm phải nhỏ hơn nguyên giá
		/// </summary>
		/// <param name="asset">Đối tượng tài sản cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsDepreciationYearGreaterThanCost(FixedAsset asset)
		{
			bool check = true;
			float depreciationValueYear = (float)asset.cost * (asset.depreciation_rate / 100);
			if (depreciationValueYear > (float)asset.cost)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.DepreciationYearGreaterThanCost,
					Message = ValidateResource.DepreciationYearGreateThanCost,
					Data = Resources.Cost
				});
				check = false;
			}

			return check;
		}

		/// <summary>
		/// Hàm validate giá trị kiểu number có điều kiện phải lớn hơn 0
		/// </summary>
		/// <param name="quantity">Số lượng</param>
		/// <param name="cost">Nguyên giá</param>
		/// <param name="lifeTime">Số năm sử dụng</param>
		/// <param name="depreciationRate">Tỷ lệ hao mòn</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsCheckPropertyNumberLessThanOrEqualZero(int quantity, decimal cost, int lifeTime, float depreciationRate)
		{
			bool check = true;
			var propertyNumberLessThanOrEqualZero = new List<string>();
			if (quantity <= 0)
			{
				propertyNumberLessThanOrEqualZero.Add(Resources.QuantityPropName);
				check = false;
			}
			if(cost <= 0)
			{
				propertyNumberLessThanOrEqualZero.Add(Resources.CostPropName);
				check = false;
			}
			if (lifeTime <= 0)
			{
				propertyNumberLessThanOrEqualZero.Add(Resources.LifeTimePropName);
				check = false;
			}
			if (depreciationRate <= 0)
			{
				propertyNumberLessThanOrEqualZero.Add(Resources.DepreciationRatePropName);
				check = false;
			}
			if (!check)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.NumberLessThanOrEqualZero,
					Message = ValidateResource.NumberLessThanOrEqualZero,
					Data = propertyNumberLessThanOrEqualZero
				});
				check = false;
			}

			return check;
		}

		/// <summary>
		/// Hàm validate kiểu giá trị tỷ lệ phần trăm phải nằm trong khoảng 0 - 100% 
		/// </summary>
		/// <param name="property">Thuộc tính cần validate</param>
		/// <param name="propValue">Giá trị của thuộc tính cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsOutRangeOfRate(PropertyInfo property, object? propValue)
		{
			bool check = true;
			
			if (property.IsDefined(typeof(HcsnNumberAttribute), false) && property.IsDefined(typeof(HcsnValueTypeAttribute),false))
			{
				var attHcsnNumber = property.GetCustomAttributes(typeof(HcsnValueTypeAttribute), false).FirstOrDefault();
				var propNumberType = (attHcsnNumber as HcsnValueTypeAttribute).PropType;
				if (propNumberType == (object)TypeValue.Rate && ((float)propValue < 0 || (float)propValue > 1))
				{
					var attributeName = "";
					if (property.IsDefined(typeof(HcsnNameAttribute), false))
					{
						var attName = property.GetCustomAttributes(typeof(HcsnNameAttribute), false).FirstOrDefault();
						attributeName = (attName as HcsnNameAttribute).PropName;
					}
					inValidList.Add(new ValidateResult
					{
						IsSuccess = false,
						ValidateCode = ValidateCode.OutOfRate,
						Message = String.Format(ValidateResource.OutOfRate, attributeName)
					});
					check = false;
				}
			}

			
			return check;
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
		/// Hàm validate giá trị không được trùng nhau
		/// </summary>
		/// <param name="asset">Đối tượng tài sản cần validate</param>
		/// <param name="property">Thuộc tính cần validate</param>
		/// <param name="propName">Tên của thuộc tính cần validate</param>
		/// <param name="propValue">Giá trị của thuộc tính cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsPropertyDuplicate(FixedAsset asset, PropertyInfo property, string propName, object? propValue)
		{
			bool check = true;
			if (property.IsDefined(typeof(HcsnDuplicateAttribute), false))
			{
				var attDuplicate = (HcsnDuplicateAttribute?)property.GetCustomAttributes(typeof(HcsnDuplicateAttribute), false).FirstOrDefault();

				if (attDuplicate != null && !String.IsNullOrEmpty(propValue.ToString().Trim()))
				{
					var isSameCode = IsDuplicate(asset,propName);
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
		/// <param name="asset"> Đối tượng tài sản chứa thuôc tính cần kiểm tra trùng </param>
		/// <param name="propertyName">tên thuộc tính cần kiểm tra</param>
		/// <returns>
		/// Kết quả kiểm tra:
		/// IsSuccess == true: không bị trùng
		/// IsSuccess == false:  bị trùng
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public ValidateResult IsDuplicate(FixedAsset asset, string propertyName)
		{
			var numberOfAffectedRows = _assetDL.GetNumberRecordOfPropertyDuplicate(asset, propertyName);
			return new ValidateResult
			{
				IsSuccess = numberOfAffectedRows == 0,
				ValidateCode = ValidateCode.Duplicate,
			};
		}

		/// <summary>
		/// Hàm xử lý logic khi lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public ServiceResult GetNewCode()
		{
			var oldCode = _assetDL.GetNewCode();
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
            if (oldCode == null)
            {
				return Resources.NewAssetCodeDefault;
            }
            // lấy ra code của đối tượng asset 
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
				var asset = new FixedAsset
				{
					fixed_asset_id = Guid.NewGuid(),
					fixed_asset_code = newCode,
				};
				var result = IsDuplicate(asset,Resources.FixedAssetCodePropName);
				check = result.IsSuccess;
				if (!check)
				{
					oldCode = newCode;
				}
			}

			return newCode;
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
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		public ServiceResult GetAllAssetNotIn(string? keyword, int pageSize, int pageNumber, List<Guid>? notInIdAssets, List<Guid>? activeIdAssets)
		{
			var result = _assetDL.GetAllAssetNotIn(keyword, pageSize, pageNumber, notInIdAssets, activeIdAssets);
			if (result.Data == null)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.NotFound,
					Message = ServiceResource.NotFound
				};
			}
			foreach (var item in result.Data)
			{
				item.residual_value = item.residual_value < 0 ? 0 : item.residual_value;
			}
			return new ServiceResult
			{
				IsSuccess = true,
				Data = result
			};
		}


		#endregion




	}
}
