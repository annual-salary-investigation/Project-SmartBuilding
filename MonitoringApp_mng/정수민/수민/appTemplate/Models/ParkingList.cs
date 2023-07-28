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
        public int CarId { get; set; }
        public int IsExit { get; set; }
    }
}
