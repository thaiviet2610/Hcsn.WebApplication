using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class AssetIncrementInsertDTO
	{
        public FixedAssetIncrement fixedAssetIncrement { get; set; }

        public List<FixedAsset> assets { get; set; }
    }
}
