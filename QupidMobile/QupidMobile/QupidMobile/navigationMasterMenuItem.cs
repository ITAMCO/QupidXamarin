using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QupidMobile
{

    public class navigationMasterMenuItem
    {
        public navigationMasterMenuItem()
        {
            TargetType = typeof(navigationMasterMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}