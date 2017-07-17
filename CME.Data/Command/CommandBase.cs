using CME.Framework.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CME.Data.Command
{
    public abstract class CommandBase
    {
        public IEnumerable<DynamicEntity> Entitys { get; set; }
        public int MyProperty { get; set; }
    }
}
