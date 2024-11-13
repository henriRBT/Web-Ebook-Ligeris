﻿using Ligeris.Modelss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ligeris.DataAcess.Repository.IRepository
{
	public interface ICategoryRepository :IRepository<Category>
	{
		void Update(Category obj);
	}
}