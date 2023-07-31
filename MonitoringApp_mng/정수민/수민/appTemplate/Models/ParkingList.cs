using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appTemplate
{
    internal class ParkingList
    {
        public int Number { get; set; }
        public string CarName { get; set; }
        public DateTime EntranceTime { get; set; }
        public DateTime ExitTime { get; set; }
        public string Fee { get; set; }
        public string CarId { get; set; }
        public string IsExit { get; set; }
        public string Reason { get; set; }
    }
}
