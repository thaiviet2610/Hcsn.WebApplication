using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hcsn.WebApplication.API.Controllers
{
	public class AssetIncrementDetailsController : BasesController<FixedAssetIncrementDetail>
	{
		public AssetIncrementDetailsController(IBaseBL<FixedAssetIncrementDetail> baseBL) : base(baseBL)
		{
		}

		
	}
}
