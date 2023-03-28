using Dapper;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.BL.BaseBL;

namespace Hcsn.WebApplication.API.Controllers
{

    public class AssetCategoriesController : BasesController<FixedAssetCategory>
    {
        public AssetCategoriesController(IBaseBL<FixedAssetCategory> baseBL) : base(baseBL)
        {
        }

    }

}
