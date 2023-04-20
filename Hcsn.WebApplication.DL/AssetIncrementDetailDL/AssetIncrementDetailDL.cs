using Dapper;
using Hcsn.WebApplication.Common.Constants.ProcedureName;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.DL.BaseDL;
using Hcsn.WebApplication.DL.DBConfig;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.DL.AssetIncrementDetailDL
{
    public class AssetIncrementDetailDL : BaseDL<FixedAssetIncrementDetail>, IAssetIncrementDetailDL
	{
		#region Field
		private IRepositoryDB _assetIncrementDetailRepository;
		#endregion
		public AssetIncrementDetailDL(IRepositoryDB repositoryDB) : base(repositoryDB)
		{
			_assetIncrementDetailRepository = repositoryDB;
		}


	}
}
