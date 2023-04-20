using Dapper;
using Hcsn.WebApplication.Common.Constants.ProcedureName;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.DL.AssetIncrementDL;
using Hcsn.WebApplication.DL.BaseDL;
using Hcsn.WebApplication.DL.DBConfig;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.DL.AssetIncrement
{
	public class AssetIncrementDL : BaseDL<FixedAssetIncrement>, IAssetIncrementDL
	{
		#region Field
		private IRepositoryDB _assetIncrementRepository;
		#endregion
		public AssetIncrementDL(IRepositoryDB repositoryDB) : base(repositoryDB)
		{
			_assetIncrementRepository = repositoryDB;
		}

		public int InsertAssetIncrement(AssetIncrementInsertDTO entity)
		{
			string storedProcedureNameInsertAssetIncrement = String.Format(ProcedureName.Insert, typeof(FixedAssetIncrement).Name);
			string storedProcedureNameInsertAssetIncrementDetail = String.Format(ProcedureName.Insert, typeof(FixedAssetIncrementDetail).Name);
			string storedProcedureNameUpdateAssetActive = String.Format(ProcedureNameAsset.UpdateActive, typeof(FixedAsset).Name);

			var parametersAssetIncrement = new DynamicParameters();
			var parametersAssetIncrementDetail = new DynamicParameters();
			var parametersAsset = new DynamicParameters();

			var assetIncrement = entity.fixedAssetIncrement;
			assetIncrement.voucher_id = Guid.NewGuid();
			var assets = entity.assets;
			var propertiesAssetIncrement = typeof(FixedAssetIncrement).GetProperties();
			AddParametersValue(propertiesAssetIncrement, parametersAssetIncrement, assetIncrement);
			var assetToString = new List<string>();
			foreach (var asset in assets)
			{
				var value = new List<string>();
				value.Add(Guid.NewGuid().ToString());
				value.Add(assetIncrement.voucher_id.ToString());
				value.Add(asset.fixed_asset_id.ToString());
				value.Add("LTVIET");
				value.Add(DateTime.Now.ToString("yyyy-MM-dd"));
				value.Add("LTVIET");
				value.Add(DateTime.Now.ToString("yyyy-MM-dd"));
				assetToString.Add($"('{string.Join("','", value)}')");
			}
			var assetsToString = $"{string.Join(",", assetToString)}";
			parametersAssetIncrementDetail.Add("p_values",assetsToString);

			var ids = new List<Guid>();
			foreach (var asset in assets)
			{
				ids.Add(asset.fixed_asset_id);
			}
			var idsToString = $"('{string.Join("','", ids)}')";
			parametersAsset.Add("p_ids", idsToString);
			int numberOfAffectedRowsInsertAssetIncrement = 0;
			var dbConnection = _assetIncrementRepository.GetOpenConnection();
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					numberOfAffectedRowsInsertAssetIncrement = 0;
					numberOfAffectedRowsInsertAssetIncrement = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameInsertAssetIncrement, parametersAssetIncrement, transaction: transaction, commandType: CommandType.StoredProcedure);
					

					int numberOfAffectedRowsInsertAssetIncrementDetail = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameInsertAssetIncrementDetail, parametersAssetIncrementDetail, transaction: transaction, commandType: CommandType.StoredProcedure);
					int numberOfAffectedRowsUpdateAsset = _assetIncrementRepository.Execute(dbConnection, storedProcedureNameUpdateAssetActive, parametersAsset, transaction: transaction, commandType: CommandType.StoredProcedure);

					if (numberOfAffectedRowsInsertAssetIncrement == 0)
					{
						transaction.Rollback();
					}
					else
					{
						transaction.Commit();
					}




				}
				catch (Exception)
				{
					numberOfAffectedRowsInsertAssetIncrement = 0;
					transaction.Rollback();
				}
			}
			dbConnection.Close();
			return numberOfAffectedRowsInsertAssetIncrement;


		}
	}
}
