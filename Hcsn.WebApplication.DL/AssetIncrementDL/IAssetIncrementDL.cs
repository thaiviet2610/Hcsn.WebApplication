using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.DL.AssetIncrementDL
{
	public interface IAssetIncrementDL : IBaseDL<FixedAssetIncrement>
	{
		public int InsertAssetIncrement(AssetIncrementInsertDTO entity);
	}
}
