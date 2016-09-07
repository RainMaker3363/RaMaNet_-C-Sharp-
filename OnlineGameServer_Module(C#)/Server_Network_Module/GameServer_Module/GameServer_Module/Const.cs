using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer_Module
{
    public struct Const<T>
    {
        public T Value { get; private set; }

        public Const(T value) : this()
        {
            this.Value = value;
        }
    }
}
