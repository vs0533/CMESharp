using CME.Data.Infrastructure;
using CME.Framework.Model;
using CME.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace CME.Data.Repositories
{
    public class DynamicEntityRepository: RepositoryDynamic,IDynamicEntityRepository
    {
        public DynamicEntityRepository(CMEDBContext context,IModelProvider modelprovider):base(context,modelprovider)
        {

        }
    }
    public interface IDynamicEntityRepository : IRepositoryDynamic
    {
    }
}
