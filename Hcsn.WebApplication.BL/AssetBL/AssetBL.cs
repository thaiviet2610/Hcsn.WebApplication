using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.DL.AssetDL;
using System.Text.RegularExpressions;
using Hcsn.WebApplication.Common.Resource;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Hcsn.WebApplication.Common.Entities.DTO.result.paging;
using Hcsn.WebApplication.Common.Entities.DTO.result.export;
using Hcsn.WebApplication.Common.Entities.DTO.entityDTO;
using Hcsn.WebApplication.Common.Entities.DTO.result.validate;
using Hcsn.WebApplication.Common.Entities.DTO.result.service;

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
				var workSheet = package.Workbook.Worksheets.Add(ExportAssetResult.SheetName);

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
				for (int j = 1; j < 11; j++)
				{
					workSheet.Cells[i + 2, j].Style.Font.Size = 10;
					workSheet.Cells[i + 2, j].Style.Border.Top.Style = ExcelBorderStyle.Medium;
					workSheet.Cells[i + 2, j].Style.Border.Right.Style = ExcelBorderStyle.Medium;
					workSheet.Cells[i + 2, j].Style.Border.Left.Style = ExcelBorderStyle.Medium;
					workSheet.Cells[i + 2, j].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

				}
			}
			// định dạng độ rộng của cột
			for (int i = 1; i < 11; i++)
			{
				workSheet.Column(i).AutoFit();
			}

			// định dạng độ cao của dòng
			for (int i = 2; i < count+3; i++)
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
            count += 3;
			workSheet.Cells[$"A{count}:E{count}"].Merge = true;
			workSheet.Cells[$"A{count}:E{count}"].Value = ExportAssetResult.FooterTotal;
			workSheet.Cells[$"A{count}:E{count}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			workSheet.Cells[$"A{count}:E{count}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			workSheet.Cells[$"A{count}:E{count}"].Style.Font.Bold = true;
			workSheet.Cells[$"A{count}:E{count}"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"A{count}:E{count}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"A{count}:E{count}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"A{count}:E{count}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

			List<string> listFooter = new()
				{
					$"F{count}",$"G{count}",$"H{count}",$"I{count}"
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

			workSheet.Cells[$"J{count}"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"J{count}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"J{count}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
			workSheet.Cells[$"J{count}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
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
				workSheet.Cells["A3:J3"].Merge = true;
				workSheet.Cells["A3:J3"].Value = ExportAssetResult.NoData;
				workSheet.Cells["A3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				workSheet.Cells["A3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				workSheet.Cells["A3:J3"].Style.Font.Bold = true;
				workSheet.Cells["A3:J3"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["A3:J3"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["A3:J3"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["A3:J3"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
			}
			else
			{
				for (int i = 0; i < assets.Count; i++)
				{
					// Chuẩn bị giá trị đầu vào

					var residualValue = Math.Round(assets[i].cost - assets[i].depreciation_value);
					residualValue = residualValue > 0 ? residualValue : 0;
					// Gán giá trị vào từng ô trên từng dòng trong file excel
					workSheet.Cells[i + 3, 1].Value = (i + 1).ToString();
					workSheet.Cells[i + 3, 2].Value = assets[i].fixed_asset_code;
					workSheet.Cells[i + 3, 3].Value = assets[i].fixed_asset_name;
					workSheet.Cells[i + 3, 4].Value = assets[i].fixed_asset_category_name;
					workSheet.Cells[i + 3, 5].Value = assets[i].department_name;
					workSheet.Cells[i + 3, 6].Value = assets[i].quantity == 0 ? 0 : assets[i].quantity.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 3, 7].Value = assets[i].cost == 0 ? 0 : assets[i].cost.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 3, 8].Value = assets[i].depreciation_value == 0 ? 0 : assets[i].depreciation_value.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 3, 9].Value = residualValue <= 0 ? 0 : residualValue.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 3, 10].Value = assets[i].active ? Resources.ActiveTrue:Resources.ActiveFalse;

					workSheet.Cells[i + 3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					workSheet.Cells[i + 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 3, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
					workSheet.Cells[i + 3, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 3, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 3, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					workSheet.Cells[i + 3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
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
			var headers = ExportAssetResult.Headers;

			for (int i = 0; i < headers.Count; i++)
			{
				workSheet.Cells[2, i + 1].Value = headers[i];
				workSheet.Cells[2, i + 1].Style.Font.Bold = true;
				workSheet.Cells[2, i + 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[2, i + 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[2, i + 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[2, i + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[2, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				if (i == 0)
				{
					workSheet.Cells[2, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				}
				else if (i == 1 || i == 2 || i == 3 || i == 4 || i == 10)
				{
					workSheet.Cells[2, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
				}
				else
				{
					workSheet.Cells[2, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
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
			workSheet.Cells["A1:J1"].Merge = true;
			workSheet.Cells["A1:J1"].Value = ExportAssetResult.Title;
			workSheet.Cells["A1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			workSheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			workSheet.Cells["A1:J1"].Style.Font.Bold = true;
			workSheet.Cells["A1:J1"].Style.Font.Size = 20;
			workSheet.Cells["A1:J1"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
			workSheet.Cells["A1:J1"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
			workSheet.Cells["A1:J1"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
			workSheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
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
				var propValue = property.GetValue(asset);
				bool isOutMaxLength = IsOutMaxLength(property, propValue);
				bool isOutRangeOfRate = IsOutRangeOfRate(property, propValue);
				bool isDuplicate = false;
				if (!isOutMaxLength)
				{
					isDuplicate = IsPropertyDuplicate(asset, property);
				}
				if ( isOutMaxLength || isOutRangeOfRate || isDuplicate)
				{
					check = false;
				}
			}
			
            bool isValidCostSource = ValidateCostSource(asset.cost_source);
			bool isPropertyNumberLessThanOrEqualZero = IsCheckPropertyNumberLessThanOrEqualZero(asset.quantity, asset.cost,
																			asset.life_time, asset.depreciation_rate);
			bool isDepreciationYearGreaterThanCost = IsDepreciationYearGreaterThanCost(asset);
			bool isDepreciationRateDifferentOnePerLifeTime = IsDepreciationRateDifferentOnePerLifeTime(asset);
			bool isProductionYearGreaterThanPurchaseDate = IsProductionYearGreaterThanPurchaseDate(asset);

			if(!isValidCostSource || isPropertyNumberLessThanOrEqualZero || isDepreciationYearGreaterThanCost 
				|| isDepreciationRateDifferentOnePerLifeTime || !isProductionYearGreaterThanPurchaseDate)
			{
				check = false;
			}
			return check;
		}

		/// <summary>
		/// Hàm validate riêng dữ liệu cho việc xóa tài sản
		/// </summary>
		/// <param name="assetsId">Danh sách Id các tài sản cần validate</param>
		/// <returns>
		/// Kết quả validate dữ liệu:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		protected override ValidateResult ValidateCustomDelete(List<Guid> assetsId)
		{
			int quantityAssetActive = _assetDL.GetQuantityAssetActive(assetsId);
			
			return new ValidateResult
			{
				IsSuccess = quantityAssetActive == 0,
				ValidateCode = ValidateCode.DeleteAssetActive,
				Message = ValidateResource.DeleteAssetActive,
				Data = assetsId
			};
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
			bool checkProductionYearGreaterThanPurchaseDate = true;
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
				checkProductionYearGreaterThanPurchaseDate = false;
			}

			return checkProductionYearGreaterThanPurchaseDate;
		}

		/// <summary>
		/// Hàm validate tỷ lệ hao mòn phải bằng 1/số năm sử dụng
		/// </summary>
		/// <param name="asset">Đối tượng tài sản cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: có lỗi
		/// flase: Không Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsDepreciationRateDifferentOnePerLifeTime(FixedAsset asset)
		{
			bool checkDepreciationRateDifferentOnePerLifeTime = false;
			if (asset.life_time != 0 && asset.depreciation_rate != (float)Math.Round((decimal)1 / asset.life_time, 3))
			{

				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.DepreciationRateDifferentLifeTime,
					Message = ValidateResource.DepreciationRateDifferentLifeTime,
					Data = Resources.LifeTimePropName
				});
				checkDepreciationRateDifferentOnePerLifeTime = true;
			}

			return checkDepreciationRateDifferentOnePerLifeTime;
		}

		/// <summary>
		/// Hàm validate giá trị hao mòn năm phải nhỏ hơn nguyên giá
		/// </summary>
		/// <param name="asset">Đối tượng tài sản cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: có lỗi
		/// flase: Không Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsDepreciationYearGreaterThanCost(FixedAsset asset)
		{
			bool checkDepreciationYearGreaterThanCost = false;
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
				checkDepreciationYearGreaterThanCost = true;
			}

			return checkDepreciationYearGreaterThanCost;
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
		/// true:  có lỗi
		/// flase: Không Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsCheckPropertyNumberLessThanOrEqualZero(int quantity, decimal cost, int lifeTime, float depreciationRate)
		{
			bool checkPropertyNumberLessThanOrEqualZero = false;
			var propertyNumberLessThanOrEqualZero = new List<string>();
			if (quantity <= 0)
			{
				propertyNumberLessThanOrEqualZero.Add(Resources.QuantityPropName);
				checkPropertyNumberLessThanOrEqualZero = true;
			}
			if(cost <= 0)
			{
				propertyNumberLessThanOrEqualZero.Add(Resources.CostPropName);
				checkPropertyNumberLessThanOrEqualZero = true;
			}
			if (lifeTime <= 0)
			{
				propertyNumberLessThanOrEqualZero.Add(Resources.LifeTimePropName);
				checkPropertyNumberLessThanOrEqualZero = true;
			}
			if (depreciationRate <= 0)
			{
				propertyNumberLessThanOrEqualZero.Add(Resources.DepreciationRatePropName);
				checkPropertyNumberLessThanOrEqualZero = true;
			}
			if (checkPropertyNumberLessThanOrEqualZero)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.NumberLessThanOrEqualZero,
					Message = ValidateResource.NumberLessThanOrEqualZero,
					Data = propertyNumberLessThanOrEqualZero
				});
			}

			return checkPropertyNumberLessThanOrEqualZero;
		}

		/// <summary>
		/// Hàm validate kiểu giá trị tỷ lệ phần trăm phải nằm trong khoảng 0 - 100% 
		/// </summary>
		/// <param name="property">Thuộc tính cần validate</param>
		/// <param name="propValue">Giá trị của thuộc tính cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: có lỗi
		/// flase: không Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsOutRangeOfRate(PropertyInfo property, object? propValue)
		{
			bool checkOutRangeOfRate = false;
			
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
					checkOutRangeOfRate = true;
				}
			}

			
			return checkOutRangeOfRate;
		}

		/// <summary>
		/// Hàm validate giá trị không được vượt quá số ký tự cho trước
		/// </summary>
		/// <param name="property">Thuộc tính cần validate</param>
		/// <param name="propValue">Giá trị của thuộc tính cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: có lỗi
		/// flase: Không Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsOutMaxLength(PropertyInfo property, object? propValue)
		{
			bool checkOutMaxLength = false;
			string propName = property.Name;
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
		/// Hàm validate giá trị các thuộc tính của đối tượng CostSourceDTO không được để trống
		/// </summary>
		/// <param name="costSourceToString">danh sách nguồn chi phí dạng string</param>
		/// <returns>
		/// Kết quả validate
		/// true: Không có lỗi
		/// flase: Có lỗi
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool ValidateCostSource(String costSourceToString)
		{
			bool checkValidate = true;
			var inValidCostSourceList = new List<ValidateResult>();
			var costSources = JsonConvert.DeserializeObject<List<CostSourceDTO>>(costSourceToString);
			
			for (int i = 0; i < costSources.Count;i++)
			{
				var costSource = costSources[i];
				bool checkCostSourceEmpty = IsCheckCostSourceEmpty(inValidCostSourceList, i, costSource);
				bool checkCostSourceDuplicate = IsCheckCostSourceDuplicate(inValidCostSourceList, costSources, i);
				bool checkMountGreaterThanZero = IsCheckCostSourceMountGreaterThanZero(inValidCostSourceList, i, costSource);
				if (checkCostSourceEmpty | checkCostSourceDuplicate | !checkMountGreaterThanZero)
				{
					checkValidate = false;
				}
			}
			if (!checkValidate)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.CostSourceInValid,
					Data = inValidCostSourceList
				});
			}

			return checkValidate;
		}

		/// <summary>
		/// Hàm kiểm tra xem giá trị nguồn hình thành có lớn hơn 0 hay không
		/// </summary>
		/// <param name="inValidCostSourceList">Danh sách chứa thông tin lỗi</param>
		/// <param name="index">vị trí của nguồn cần kiểm tra trong danh sách</param>
		/// <param name="costSource">Đối tượng nguồn chi phí cần kiểm tra</param>
		/// <returns>
		/// true: giá trị nguồn hình thành có lớn hơn 0
		/// false: giá trị nguồn hình thành có nhỏ hơn hoặc bằng 0 
		/// </returns>
		private static bool IsCheckCostSourceMountGreaterThanZero(List<ValidateResult> inValidCostSourceList, int index, CostSourceDTO costSource)
		{
			bool checkMountGreaterThanZero = true;
			if (costSource.mount == 0)
			{
				inValidCostSourceList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.CostSourceMountLessOrEqualThanZero,
					Message = ValidateResource.CostSourceMountLessOrEqualThanZero,
					Data = index
				});
				checkMountGreaterThanZero = false;
			}

			return checkMountGreaterThanZero;
		}

		/// <summary>
		/// Hàm kiểm tra xem có 2 nguồn trùng nhau trong danh sách không
		/// </summary>
		/// <param name="inValidCostSourceList">Danh sách chứa thông tin lỗi</param>
		/// <param name="index">vị trí của nguồn cần kiểm tra trong danh sách</param>
		/// <param name="costSources">Danh sách nguồn chi phí cần kiểm tra</param>
		/// <returns>
		/// true: có trùng nhau
		/// false: không trùng nhau 
		/// </returns>
		private static bool IsCheckCostSourceDuplicate(List<ValidateResult> inValidCostSourceList, List<CostSourceDTO>? costSources, int index)
		{
			bool checkDuplicate = false;
			for (int j = 0; j < index; j++)
			{
				if (costSources[index].budget_id == costSources[j].budget_id)
				{
					inValidCostSourceList.Add(new ValidateResult
					{
						IsSuccess = false,
						ValidateCode = ValidateCode.CostSourceDuplicate,
						Message = String.Format(ValidateResource.CostSourceDuplicate, costSources[index].budget_name),
						Data = index
					});
					checkDuplicate = true;
					break;
				}
			}

			return checkDuplicate;
		}

		/// <summary>
		/// Hàm kiểm tra xem có  nguồn nào có giá trị rỗng hay không
		/// </summary>
		/// <param name="inValidCostSourceList">Danh sách chứa thông tin lỗi</param>
		/// <param name="index">vị trí của nguồn cần kiểm tra trong danh sách</param>
		/// <param name="costSource">Đối tượng nguồn chi phí cần kiểm tra</param>
		/// <returns>
		/// true: có giá trị rỗng
		/// false: không có giá trị rỗng 
		/// </returns>
		private static bool IsCheckCostSourceEmpty(List<ValidateResult> inValidCostSourceList, int index, CostSourceDTO costSource)
		{
			bool checkEmpty = false;
			var propertiesCostSource = typeof(CostSourceDTO).GetProperties();
			List<String> validateEmpty = new();
			foreach (var property in propertiesCostSource)
			{
				var propValue = property.GetValue(costSource);
				var propName = property.Name;

				var requiredAttribute = (HcsnRequiredAttribute?)property.GetCustomAttributes(typeof(HcsnRequiredAttribute)).FirstOrDefault();
				if (requiredAttribute != null && (propValue == null || String.IsNullOrEmpty(propValue.ToString().Trim())))
				{
					validateEmpty.Add(propName);
					checkEmpty = true;
				}
			}
			if (checkEmpty)
			{
				inValidCostSourceList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.CostSourceEmpty,
					Message = ValidateResource.CostSourceEmpty,
					Data = new
					{
						Index = index,
						Data = validateEmpty
					}
				});
			}

			return checkEmpty;
		}


		/// <summary>
		/// Hàm validate giá trị không được trùng nhau
		/// </summary>
		/// <param name="asset">Đối tượng tài sản cần validate</param>
		/// <param name="property">Thuộc tính cần validate</param>
		/// <returns>
		/// Kết quả validate
		/// true: có trùng 
		/// flase: không có trùng
		/// </returns>
		/// Created by: LTVIET (09/03/2023)
		private bool IsPropertyDuplicate(FixedAsset asset, PropertyInfo property)
		{
			bool checkDuplicate = false;
			string propName = property.Name;
			var propValue = property.GetValue(asset);
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
							Message = String.Format(ValidateResource.Duplicate, attributeName, propValue),
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
