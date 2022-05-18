using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.TimeTravel.Shared
{
    public class READ_DATABASE<TInternal, TEnvelop>
    {
        public List<TInternal> InternalEntities { get; set; }
        public List<TEnvelop> EntityEnvelops { get; set; }

        public void RESET()
        {
            InternalEntities = new List<TInternal>();
            EntityEnvelops = new List<TEnvelop>();
        }

    }
}
