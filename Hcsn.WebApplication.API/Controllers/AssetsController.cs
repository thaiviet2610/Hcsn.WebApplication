using Dapper;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Data.Common;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Constants;
using System.Reflection;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.BL.AssetBL;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Hcsn.WebApplication.Common.Resource;

namespace Hcsn.WebApplication.API.Controllers
{
    
    public class AssetsController : BasesController<Asset>
    {
        #region Field
        private IAssetBL _assetBL;
        #endregion
        public AssetsController(IAssetBL assetBL) : base(assetBL)
        {
            _assetBL = assetBL;
        }

        /// <summary>
        /// API phân trang, lọc danh sách tài sản
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <param name="departmentId">Id phòng ban tìm kiếm</param>
        /// <param name="fixedAssetCatagortId">Id loại tài sản tìm kiếm</param>
        /// <param name="pageSize">Số bản ghi trong 1 trang</param>
        /// <param name="pageNumber">Vị trí trang hiện tại</param>
        /// <returns>Danh sách các tài sản phù hợp</returns>
        [HttpGet("Filter")]
        public IActionResult GetPaging(
            [FromQuery] string? keyword,
            [FromQuery] Guid? departmentId,
            [FromQuery] Guid? fixedAssetCatagortId,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = _assetBL.GetPaging(keyword, departmentId, fixedAssetCatagortId, pageSize, pageNumber);
                if (!result.IsSuccess)
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
                return StatusCode(200, result.Data);
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

        [HttpGet("test")]
        public IActionResult get()
        {
            string store = "Proc_Asset_CheckSameCode";
            var parmeters = new DynamicParameters();
            parmeters.Add("p_code", "TS74451");
            parmeters.Add("p_id", Guid.NewGuid());
            string connec = "Server=localhost;Port=3306;Database=test;Uid=root;Pwd=123456;";
            using(var mySQL = new MySqlConnection(connec))
            {
                mySQL.Open();
                var a = mySQL.QueryFirstOrDefault<int>(store, parmeters, commandType:CommandType.StoredProcedure);
                return StatusCode(200, a);
            }
        }
       
    }
}
