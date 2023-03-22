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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
        #endregion
    }
}
