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

        

        public ServiceResult UpdateRecord(Guid recordId, T record)
        {
            // Kiểm tra có trung code hay không ?
            //string recordCode = "";
            //var properties = typeof(T).GetProperties();
            //foreach (var property in properties)
            //{
            //    if (property.IsDefined(typeof(HcsnCodeAttribute), false))
            //    {
            //        recordCode = property.GetValue(record).ToString();
            //        break;
            //    }
            //}
            //var isSameCode = IsSameCode(recordCode, recordId);
            //if (!isSameCode.IsSuccess)
            //{
            //    // Thất bại
            //    return new ServiceResult
            //    {
            //        IsSuccess = false,
            //        ErrorCode = ErrorCode.DuplicateCode,
            //        Message = ServiceResource.DuplicateCode,
            //        Data = isSameCode
            //    };
            //}
            // Thành công
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

        protected virtual ValidateResult ValidateRequesData(T record,Guid recordId)
        {
            string recordCode = "";
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.IsDefined(typeof(HcsnCodeAttribute), false))
                {
                    recordCode = property.GetValue(record).ToString();
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
                if (property.IsDefined(typeof(HcsnRequiredAttribute), false) && (propValue == null || propValue.ToString() == String.Empty))
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

        protected virtual ValidateResult ValidateCustom(T record)
        {
            return new ValidateResult() { IsSuccess = true };
        }


        #endregion
    }
}
