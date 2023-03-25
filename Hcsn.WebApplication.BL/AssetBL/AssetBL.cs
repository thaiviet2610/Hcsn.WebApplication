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

namespace Hcsn.WebApplication.BL.AssetBL
{
    public class AssetBL : BaseBL<Asset>, IAssetBL
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

        /// <summary>
        /// Validate dữ liệu cho đối tượng asset
        /// </summary>
        /// <param name="asset">Đối tượng cần validate</param>
        /// <returns>
        /// Trả về 1 đối tượng ValidateResult mô tả kết quả validate, bao gồm:
        /// IsSuccess = True: Nếu validate dữ liệu thành công
        /// IsSuccess = False: Nếu validate dữ liệu thất bại
        /// </returns>
        protected override ValidateResult ValidateCustom(Asset asset)
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

            var properties = typeof(Asset).GetProperties();
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
