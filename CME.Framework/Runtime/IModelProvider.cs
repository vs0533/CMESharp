using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Runtime
{
    public interface IModelProvider
    {
        Type GetType(Guid modelId);
        Type[] GetTypes();
    }
}
