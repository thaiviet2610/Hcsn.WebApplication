﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities
{
	public class FixedAssetIncrement : BaseEntity
	{
        public Guid voucher_id { get; set; }

        public string voucher_code { get; set; }

		public DateTime voucher_date { get; set; }

		public DateTime increment_date { get; set; }

		public string price { get; set; }


		public string description { get; set; }



	}
}
