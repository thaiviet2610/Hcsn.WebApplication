using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hcsn.WebApplication.Common.Resource;

namespace Hcsn.WebApplication.BL.BaseBL
{
    public class BaseBL<T> : IBaseBL<T>
    {
        #region Field
        private IBaseDL<T> _baseDL;
        #endregion

        #region Constructor
        public BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }
        #endregion

        #region Method

        /// <summary>
        /// Hàm xử lý logic khi xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">Id bản ghi muốn xóa</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public ServiceResult DeleteRecord(Guid recordId)
        {
            var numberOfAffectedRows = _baseDL.DeleteRecord(recordId);
            if (numberOfAffectedRows == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = ErrorCode.DeleteFailed,
                    Message = ServiceResource.DeleteFailed
                };
            }
            return new ServiceResult
            {
                IsSuccess = true
            };
        }

		public ServiceResult DeleteMultipleRecord(List<Guid> entitiesId)
		{
			var numberOfAffectedRows = _baseDL.DeleteMultipleRecord(entitiesId);
			if (numberOfAffectedRows == 0)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.DeleteMultipleFailed,
					Message = ServiceResource.DeleteMultipleFailed,
				};
			}
			return new ServiceResult
			{
				IsSuccess = true,
                Data = numberOfAffectedRows
			};
		}

		/// <summary>
		/// Hàm xử lý logic khi lấy ra danh sách tất cả các bản ghi
		/// </summary>
		/// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
		/// Created by: LTVIET (20/03/2023)
		public ServiceResult GetAllRecord()
        {
            var records = _baseDL.GetAllRecord();
            if(records == null)
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
                Data = records
            };
        }

        /// <summary>
        /// Hàm xử lý logic khi lấy thông tin chi tiết 1 bản ghi theo id từ tầng DL 
        /// </summary>
        /// <param name="recordId">ID bản ghi muốn lấy</param>
        /// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
        /// Created by: LTVIET (20/03/2023)
        public ServiceResult GetRecordById(Guid recordId)
        {
            var record = _baseDL.GetRecordById(recordId);
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
        /// Hàm xử lý logic khi thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn thêm</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public ServiceResult InsertRecord(T record)
        {
            var validateResult = ValidateRequesData(record,Guid.Empty);
            if(!validateResult.IsSuccess)
            {
                if(validateResult.ValidateCode == ValidateCode.DuplicateCode)
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        ErrorCode = ErrorCode.DuplicateCode,
                        Message = ServiceResource.DuplicateCode,
                        Data = validateResult
                    };
                }
                return new ServiceResult
                {
                    // Thất bại
                    IsSuccess = false,
                    ErrorCode = ErrorCode.InvalidateData,
                    Message = ServiceResource.InvalidData,
                    Data = validateResult
                };
            }
            // Thành công
            var numberOfAffectedRows = _baseDL.InsertRecord(record);
            if(numberOfAffectedRows > 0)
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


        /// <summary>
        /// Hàm xử lý logic khi sửa đổi 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn sửa đổi</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public ServiceResult UpdateRecord(Guid recordId, T record)
        {
            // Validate dữ liệu đầu vào
            var validateResult = ValidateRequesData(record,recordId);
            if (!validateResult.IsSuccess)
            {
                if (validateResult.ValidateCode == ValidateCode.DuplicateCode)
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        ErrorCode = ErrorCode.DuplicateCode,
                        Message = ServiceResource.DuplicateCode,
                        Data = validateResult
                    };
                }
                return new ServiceResult
                {
                    // Thất bại
                    IsSuccess = false,
                    ErrorCode = ErrorCode.InvalidateData,
                    Message = ServiceResource.InvalidData,
                    Data = validateResult
                };
            }

            // Thành công

            var numberOfAffectedRows = _baseDL.UpdateRecord(recordId, record);
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
                    ErrorCode = ErrorCode.UpdateFailed,
                    Message = ServiceResource.UpdateFailed,
                };
            }
        }

        /// <summary>
        /// Hàm xử lý logic khi kiểm tra xem code có bị trùng không ?
        /// </summary>
        /// <param name="recordCode">Code cần kiểm tra</param>
        /// <param name="recordId">Id </param>
        /// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
        /// Created by: LTViet (20/03/2023)
        public ServiceResult IsSameCode(string recordCode, Guid recordId)
        {
            var numberOfAffectedRows = _baseDL.GetRecordByCode(recordCode, recordId);
            if (numberOfAffectedRows == 0)
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
                    ErrorCode = ErrorCode.DuplicateCode,
                    Message = ServiceResource.DuplicateCode,
                };
            }
        }

        /// <summary>
        /// Hàm xử lý logic khi lấy ra mã code ở lần nhập gần nhất
        /// </summary>
        /// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
        /// Created by: LTViet (20/03/2023)
        public ServiceResult GetNewCode()
        {
            var oldCode = _baseDL.GetNewCode();
            if (oldCode == null)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Data = "TS00001"
                };
            }
            else
            {
                string newCode = GenerateNewCode(oldCode);
                return new ServiceResult
                {
                    IsSuccess = true,
                    Data = newCode
                };
            }

        }

        /// <summary>
        /// Hàm sinh ra code mới từ việc tách code ra phần chữ và số rồi tăng phần số lên 1 đơn vị
        /// </summary>
        /// <param name="oldCode">Code cần tách ra</param>
        /// <returns>Code mới</returns>
        private string GenerateNewCode(string oldCode)
        {
            // Thành công
            // lấy ra code của đối tượng asset 
            string newCode = "";
            bool check = false;
            var regex = new Regex(@"([a-zA-Z]+)(\d+)");
            // vòng lặp lấy 
            while (!check)
            {
                // Tách phần số và phần chữ của code ra
                string alphaPart = regex.Match(oldCode).Groups[1].Value;
                string numberPart = regex.Match(oldCode).Groups[2].Value;
                // Tăng phần số lên một đơn vị
                int numberCode = int.Parse(numberPart) + 1;
                // Tạo ra code mới bằng cách nối phần chữ và phần số mới 
                string strNumberCode = "" + numberCode;
                while (strNumberCode.Length < 5)
                {
                    strNumberCode = '0' + strNumberCode;
                }
                newCode = alphaPart + strNumberCode;
                // Kiểm tra xem code mới có bị trùng không
                // Lấy ra số bản ghi tài sản có code bằng code mới
                var result = IsSameCode(newCode,Guid.NewGuid());
                check = result.IsSuccess;
            }

            return newCode;
        }

        /// <summary>
        /// Hàm validate chung dữ liệu 
        /// </summary>
        /// <param name="record">bản ghi cần validate</param>
        /// <param name="recordId">Id của bản ghi</param>
        /// <returns>Kết quả validate dữ liệu</returns>
        protected virtual ValidateResult ValidateRequesData(T record,Guid recordId)
        {
            
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.IsDefined(typeof(HcsnCodeAttribute), false))
                {
                    string recordCode = property.GetValue(record).ToString();
                    var isSameCode = IsSameCode(recordCode, recordId);      
                    if (!isSameCode.IsSuccess)
                    {
                        return new ValidateResult
                        {
                            IsSuccess = false,
                            ValidateCode = ValidateCode.DuplicateCode,
                            Message = ValidateResource.DuplicateCode
                        };
                    }
                }
                
                var propValue = property.GetValue(record);
                var propName = property.Name;
                if(property.IsDefined(typeof(HcsnNameAttribute), false))
                {
                    var attHcsnName = property.GetCustomAttributes(typeof(HcsnNameAttribute), false).FirstOrDefault();
                    propName = (attHcsnName as HcsnNameAttribute).PropName;
                }
                if (property.IsDefined(typeof(HcsnRequiredAttribute), false) 
                    && (propValue == null || propValue.ToString() == String.Empty))
                {
                    return new ValidateResult
                    {
                        IsSuccess = false,
                        ValidateCode = ValidateCode.Empty,
                        Message = propName + ValidateResource.Empty
                    };
                }
                if(property.IsDefined(typeof(HcsnNumberAttribute), false))
                {
                    string propNumber = propValue.ToString();
                    var attHcsnNumber = property.GetCustomAttributes(typeof(HcsnNumberAttribute), false).FirstOrDefault();
                    string propType = (attHcsnNumber as HcsnNumberAttribute).PropType;
                    if (propNumber.Count() > 14)
                    {
                        return new ValidateResult
                        {
                            IsSuccess = false,
                            ValidateCode = ValidateCode.OutMaxLength,
                            Message = propName + ValidateResource.OutMaxLength
                        };
                    }else if(propType == "rate" && ((float)propValue < 0 || (float)propValue > 100))
                    {
                        return new ValidateResult
                        {
                            IsSuccess = false,
                            ValidateCode = ValidateCode.OutOfRate,
                            Message = propName + ValidateResource.OutOfRate
                        };
                    }
                    if (property.IsDefined(typeof(HcsnGreateThanZeroAttribute), false))
                    {
                        bool check = true;
                        if (propType == "decimal" && (decimal)propValue <= 0)
                        {
                            check = false;
                        }
                        else if (propType == "int" && (int)propValue <= 0)
                        {
                            check = false;
                        }
                        else if ((propType == "rate" || propType == "float") && (float)propValue <= 0)
                        {
                            check = false;
                        }
                        if (!check)
                        {
                            return new ValidateResult
                            {
                                IsSuccess = false,
                                ValidateCode = ValidateCode.NumberEqual0,
                                Message = propName + ValidateResource.NumberEqual0
                            };
                        }
                    }
                }

                
            }
            var result = ValidateCustom(record);
            if (!result.IsSuccess)
            {
                return result;
            }
            return new ValidateResult { IsSuccess = true };

        }

        /// <summary>
        /// Hàm validate riêng dữ liệu 
        /// </summary>
        /// <param name="record">bản ghi cần validate</param>
        /// <returns>Kết quả validate dữ liệu</returns>
        protected virtual ValidateResult ValidateCustom(T record)
        {
            return new ValidateResult() { IsSuccess = true };
        }

		


		#endregion
	}
}
