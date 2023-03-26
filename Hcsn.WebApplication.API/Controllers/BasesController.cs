using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hcsn.WebApplication.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasesController<T> : ControllerBase
    {
        #region Field
        private IBaseBL<T> _baseBL;
        #endregion

        #region Constructor
        public BasesController(IBaseBL<T> baseBL)
        {
            _baseBL = baseBL;
        }
        #endregion

        #region Method
        /// <summary>
        /// API Lấy ra danh sách tất cả các bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả các bản ghi</returns>
        /// Created by: LTVIET (20/03/2023)
        [HttpGet]
        public IActionResult GetAllRecord()
        {
            try
            {
                var result = _baseBL.GetAllRecord();
                if (result.IsSuccess)
                {
                    return StatusCode(200, result.Data);
                }
                else
                {
                    return StatusCode(500, new ErrorResult
                    {
                        ErrorCode = ErrorCode.NotFound,
                        DevMsg = ErrorResource.DevMsg_NotFound,
                        UserMsg = ErrorResource.UserMsg_NotFound,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
            }
            catch (Exception ex)
            {
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
					sws.WriteLine(HttpContext.TraceIdentifier);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.Exception,
                    DevMsg = ErrorResource.DevMsg_Exception,
                    UserMsg = ErrorResource.UserMsg_Exception,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = "Xảy ra exception",
                });
            }
        }

        /// <summary>
        /// API Lấy thông tin chi tiết 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID bản ghi muốn lấy</param>
        /// <returns>Bản ghi muốn lấy</returns>
        /// Created by: LTVIET (20/03/2023)
        [HttpGet("{recordId}")]
        public IActionResult GetRecordById([FromRoute]Guid recordId)
        {
            try
            {
                var result = _baseBL.GetRecordById(recordId);
                if (result.IsSuccess)
                {
					return StatusCode(200, result.Data);
                }
                return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.NotFound,
                    DevMsg = ErrorResource.DevMsg_NotFound,
                    UserMsg = ErrorResource.UserMsg_NotFound,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = result.Data,
                });
            }
            catch (Exception ex)
            {
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
					sws.WriteLine(HttpContext.TraceIdentifier);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.Exception,
                    DevMsg = ErrorResource.DevMsg_Exception,
                    UserMsg = ErrorResource.UserMsg_Exception,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = "Xảy ra exception",
                });
            }
        }

        /// <summary>
        /// Hàm thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn thêm</param>
        /// <returns>
        /// 1: Nếu insert thành công
        /// 2: Nếu insert thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        [HttpPost]
        public IActionResult InsertRecord([FromBody]T record)
        {
            try
            {
                var result = _baseBL.InsertRecord(record);
                if (result.IsSuccess)
                {
                    return StatusCode(201);
                }
                else if (!result.IsSuccess && result.ErrorCode == ErrorCode.InvalidateData)
                {
                    return StatusCode(400, new ErrorResult
                    {
                        ErrorCode = ErrorCode.InvalidateData,
                        DevMsg = ErrorResource.DevMsg_InvalidData,
                        UserMsg = ErrorResource.UserMsg_InvalideData,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
                else if (!result.IsSuccess && result.ErrorCode == ErrorCode.DuplicateCode)
                {
                    return StatusCode(400, new ErrorResult
                    {
                        ErrorCode = ErrorCode.DuplicateCode,
                        DevMsg = ErrorResource.DevMsg_DuplicateCode,
                        UserMsg = ErrorResource.UserMsg_DuplicateCode,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
                else
                {
					return StatusCode(500, new ErrorResult
                    {
                        ErrorCode = ErrorCode.InsertFailed,
                        DevMsg = ErrorResource.DevMsg_InsertFailed,
                        UserMsg = ErrorResource.UserMsg_InsertFailed,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
            }
            catch (Exception ex)
            {
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
					sws.WriteLine(HttpContext.TraceIdentifier);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.Exception,
                    DevMsg = ErrorResource.DevMsg_Exception,
                    UserMsg = ErrorResource.UserMsg_Exception,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = "Xảy ra exception",
                });
            }

        }

        /// <summary>
        /// Hàm sửa đổi 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn sửa đổi</param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 2: Nếu update thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        [HttpPut("{recordId}")]
        public IActionResult UpdateRecordById([FromRoute]Guid recordId, [FromBody]T record)
        {
            try
            {
                var result = _baseBL.UpdateRecord(recordId, record);
                if (result.IsSuccess)
                {
                    return StatusCode(200);
                }
                else if (!result.IsSuccess && result.ErrorCode == ErrorCode.InvalidateData)
                {
                    return StatusCode(400, new ErrorResult
                    {
                        ErrorCode = ErrorCode.InvalidateData,
                        DevMsg = ErrorResource.DevMsg_InvalidData,
                        UserMsg = ErrorResource.UserMsg_InvalideData,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
                else if (!result.IsSuccess && result.ErrorCode == ErrorCode.DuplicateCode)
                {
                    return StatusCode(400, new ErrorResult
                    {
                        ErrorCode = ErrorCode.DuplicateCode,
                        DevMsg = ErrorResource.DevMsg_DuplicateCode,
                        UserMsg = ErrorResource.UserMsg_DuplicateCode,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
                else
                {
                    return StatusCode(500, new ErrorResult
                    {
                        ErrorCode = ErrorCode.UpdateFailed,
                        DevMsg = ErrorResource.DevMsg_UpdateFailed,
                        UserMsg = ErrorResource.UserMsg_UpdateFailed,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
            }
            catch (Exception ex)
            {
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
					sws.WriteLine(HttpContext.TraceIdentifier);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}

				return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.Exception,
                    DevMsg = ErrorResource.DevMsg_Exception,
                    UserMsg = ErrorResource.UserMsg_Exception,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = "Xảy ra exception",
                });

				//System.IO.writeline(ex.ToString(), "D\:log.txt");
				

			}
        }

        /// <summary>
        /// API xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">Id bản ghi muốn xóa</param>
        /// <returns>
        /// 1: Nếu delete thành công
        /// 2: Nếu delete thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        [HttpDelete("{recordId}")]
        public ActionResult DeleteRecordById([FromRoute]Guid recordId)
        {
            try
            {
                var result = _baseBL.DeleteRecord(recordId);
                if (result.IsSuccess)
                {
                    return StatusCode(200);
                }
                return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.DeleteFailed,
                    DevMsg = ErrorResource.DevMsg_DeleteFailed,
                    UserMsg = ErrorResource.UserMsg_DeleteFailed,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = result.Data,
                });
                
            }
            catch (Exception ex)   
            {
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
					sws.WriteLine(HttpContext.TraceIdentifier);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.Exception,
                    DevMsg = ErrorResource.DevMsg_Exception,
                    UserMsg = ErrorResource.UserMsg_Exception,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = "Xảy ra exception",
                });
            }
        }

        /// <summary>
        /// API lấy ra code mới dựa theo code ở lần nhập gần nhất + 1
        /// </summary>
        /// <returns>Mã code mới</returns>
        /// Created by: LTViet (20/03/2023)
        [HttpGet("GetNewCode")]
        public IActionResult GetNewCode()
        {
            try
            {
                var result = _baseBL.GetNewCode();
                if (result.IsSuccess)
                {
                    return StatusCode(200, result.Data);
                }

                return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.GenerateNewCodefailed,
                    DevMsg = ErrorResource.DevMsg_GetNewCodeFailed,
                    UserMsg = ErrorResource.UserMsg_GetNewCodeFailed,
                    MoreInfo= result.Data,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                string traceId = HttpContext.TraceIdentifier;
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
                    
					sws.WriteLine(traceId);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(500, new ErrorResult
                {
                    ErrorCode = ErrorCode.Exception,
                    DevMsg = ErrorResource.DevMsg_Exception,
                    UserMsg = ErrorResource.UserMsg_Exception,
                    TraceId = traceId,
                    MoreInfo = "Xảy ra exception",
                });
            }
            
        }
        #endregion
    }
}
