using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hcsn.WebApplication.API.Controllers
{

	public class BudgetsController : BasesController<Budget>
	{
		public BudgetsController(IBaseBL<Budget> baseBL) : base(baseBL)
		{
		}
	}
}
