using CME.Framework.Enumeration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CME.Framework.Model
{
    public class RelationshipInfo
    {
        public string Reference { get; set; }
        public string Relationship { get; set; }

        public RelationshipType Type { get; set; }
        public string ForeignKey { get; set; }
    }
}