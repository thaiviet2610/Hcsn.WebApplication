using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class PriceAssetDTO 
	{
        public Guid budget_id { get; set; }

        public string budget_code { get; set; }

        public string budget_name { get; set; }

        public decimal mount { get; set; }

	}
}
