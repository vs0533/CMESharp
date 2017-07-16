using CME.Framework.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CME.Framework.Runtime
{
    public interface IRelationshipProvider
    {
        IEnumerable<RelationshipInfo> GetRelationship(string typeName); 
    }
}
