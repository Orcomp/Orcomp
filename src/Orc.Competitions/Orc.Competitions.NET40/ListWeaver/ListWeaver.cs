﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orc.Competitions.ListWeaver
{
    using System;
    using System.Collections.Generic;

    public class ListWeaver<T>
    {
        public ListWeaver()
        {
            this.Yarns = new List<IList<T>>();
        }

        public List<IList<T>> Yarns { get; set; }

        public bool CanWeave(IList<T> yarn)
        {
            throw new NotImplementedException();
        }

        public List<T> Weave()
        {
            throw new NotImplementedException();
        }
    }
}
