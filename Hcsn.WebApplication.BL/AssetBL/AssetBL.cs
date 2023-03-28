using Dapper;
using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.DL.AssetDL;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hcsn.WebApplication.Common.Resource;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;

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
        /// <param name="keyword"></param> Từ khóa tìm kiếm (mã tài sản, tên tài sản)
        /// <param name="departmentId"></param> Id của phòng ban
        /// <param name="fixedAssetCatagortId"></param> Id của loại tài sản
        /// <returns> Đối tượng PagingResult bao gồm:
        /// - Danh sách tài sản trong 1 trang
        /// - Tổng số bản ghi thỏa mãn điều kiện
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
            return new ServiceResult
            {
                IsSuccess = true,
                Data = result
            };
        }

		public ServiceResult ExportExcel(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId)
		{
			var result = _assetDL.GetPaging(keyword, departmentId, fixedAssetCatagortId, 0, 0);
			if (result.Data != null)
			{
				var assets = result.Data;
				int quantityTotal = result.QuantityTotal;
				decimal costTotal = result.CostTotal;
				double depreciationValueTotal = result.DepreciationValueTotal;
				byte[] data = GenerateExcelFile(assets, quantityTotal, costTotal, depreciationValueTotal);
				string currentDatetime = DateTime.Now.ToString("dd-MM-yyyyTHH.mm.ss");
				string exelFileName = $"assets({currentDatetime}).xlsx";
				string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{typeof(FixedAsset).Name}s", exelFileName);
				File.WriteAllBytes(filePath, data);
				return new ServiceResult
				{
					IsSuccess = true,
					Data = filePath
				};
			}
			else
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.NotFound,
					Message = ServiceResource.NotFound
				};
			}
		}

		public byte[] GenerateExcelFile(List<FixedAsset> assets, int quantityTotal, decimal costTotal, double depreciationValueTotal)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			using (var package = new ExcelPackage())
			{
				var workSheet = package.Workbook.Worksheets.Add("Assets");

				// tạo tiêu đề của bảng trong file excel
				workSheet.Cells["B2:J2"].Merge = true;
				workSheet.Cells["B2:J2"].Value = "Danh sách tài sản";
				workSheet.Cells["B2:J2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				workSheet.Cells["B2:J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				workSheet.Cells["B2:J2"].Style.Font.Bold = true;
				workSheet.Cells["B2:J2"].Style.Font.Size = 20;
				workSheet.Cells["B2:J2"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["B2:J2"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["B2:J2"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells["B2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
				List<string> listHeader = new List<string>()
				{
					"STT","Mã tài sản","Tên tài sản","Loại tài sản","Bộ phận sử dụng","Số lượng","Nguyên giá","HM/KH lũy kế","Giá trị còn lại"
				};

				for (int i = 0; i < listHeader.Count; i++)
				{
					workSheet.Cells[3, i + 2].Value = listHeader[i];
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
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
				// tạo dữ liệu
				for (int i = 0; i < assets.Count; i++)
				{
					workSheet.Cells[i + 4, 2].Value = (i + 1).ToString();
					workSheet.Cells[i + 4, 3].Value = assets[i].fixed_asset_code;
					workSheet.Cells[i + 4, 4].Value = assets[i].fixed_asset_name;
					workSheet.Cells[i + 4, 5].Value = assets[i].fixed_asset_category_name;
					workSheet.Cells[i + 4, 6].Value = assets[i].department_name;
					workSheet.Cells[i + 4, 7].Value = assets[i].quantity.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 4, 8].Value = assets[i].cost.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 4, 9].Value = assets[i].depreciation_value.ToString("#,###", cul.NumberFormat);
					workSheet.Cells[i + 4, 10].Value = Math.Round(assets[i].cost - assets[i].depreciation_value).ToString("#,###", cul.NumberFormat);

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



				// Định dạng cell border
				for (int i = 0; i <= assets.Count; i++)
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

				// Tạo footer table
				int count = assets.Count + 4;
				workSheet.Cells[$"B{count}:F{count}"].Merge = true;
				workSheet.Cells[$"B{count}:F{count}"].Value = "Tổng cộng";
				workSheet.Cells[$"B{count}:F{count}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				workSheet.Cells[$"B{count}:F{count}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				workSheet.Cells[$"B{count}:F{count}"].Style.Font.Bold = true;
				workSheet.Cells[$"B{count}:F{count}"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[$"B{count}:F{count}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[$"B{count}:F{count}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
				workSheet.Cells[$"B{count}:F{count}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

				List<string> listFooter = new List<string>()
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
							workSheet.Cells[listFooter[i]].Value = quantityTotal.ToString("#,###", cul.NumberFormat);
							break;
						case 1:
							workSheet.Cells[listFooter[i]].Value = costTotal.ToString("#,###", cul.NumberFormat);
							break;
						case 2:
							workSheet.Cells[listFooter[i]].Value = depreciationValueTotal.ToString("#,###", cul.NumberFormat);
							break;
						case 3:
							workSheet.Cells[listFooter[i]].Value = (costTotal - (decimal)depreciationValueTotal).ToString("#,###", cul.NumberFormat);
							break;
					}
				}

				// định dạng độ rộng của cột
				for (int i = 2; i < 11; i++)
				{
					workSheet.Column(i).AutoFit();
				}


				return package.GetAsByteArray();
			}




		}

		/// <summary>
		/// Validate dữ liệu cho đối tượng asset
		/// </summary>
		/// <param name="asset">Đối tượng cần validate</param>
		/// <returns>
		/// Trả về 1 đối tượng ValidateResult mô tả kết quả validate, bao gồm:
		/// IsSuccess = True: Nếu validate dữ liệu thành công
		/// IsSuccess = False: Nếu validate dữ liệu thất bại
		/// </returns>
		protected override ValidateResult ValidateCustom(FixedAsset asset)
        {
            float depreciationValueYear = (float)asset.cost * (asset.depreciation_rate / 100);
            if (depreciationValueYear > (float)asset.cost)
            {
                return new ValidateResult
                {
                    IsSuccess = false,
                    ValidateCode = ValidateCode.DepreciationYearGreaterThanCost,
                    Message = ValidateResource.DepreciationYearGreateThanCost
                };
            }
            if (asset.depreciation_rate != (float)Math.Round((decimal)1 / asset.life_time, 3))
            {
                return new ValidateResult
                {
                    IsSuccess = false,
                    ValidateCode = ValidateCode.DepreciationRateDifferentLifeTime,
                    Message = ValidateResource.DepreciationRateDifferentLifeTime
                };
            }
            if((asset.purchase_date - asset.production_year).TotalDays > 0)
            {
                return new ValidateResult
                {
                    IsSuccess = false,
                    ValidateCode = ValidateCode.PurchaseDateGreaterThanProductionYear,
                    Message = ValidateResource.PurchaseDateGreaterThanProductionYear
                };
            }

            var properties = typeof(FixedAsset).GetProperties();
            foreach (var property in properties)
            {
                string propName = property.Name;
                var propValue = property.GetValue(asset);
				if (property.IsDefined(typeof(HcsnMaxLengthAttribute), false))
				{
					var attHcsnLength = property.GetCustomAttributes(typeof(HcsnMaxLengthAttribute), false).FirstOrDefault();
					int propLength = (attHcsnLength as HcsnMaxLengthAttribute).Length;
					if (propValue.ToString().Length > propLength)
					{
						return new ValidateResult
						{
							IsSuccess = false,
							ValidateCode = ValidateCode.MaxLength,
							Message = propName + ValidateResource.MaxLength
						};
					}
				}
			}
            
			return new ValidateResult
            {
                IsSuccess = true,
            };
        } 
        #endregion




    }
}
