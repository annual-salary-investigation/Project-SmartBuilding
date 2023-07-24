using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appTemplate.Models
{
    internal class Weather
    {
        /* API 검색결과
         "item": [
          {
            "baseDate": "20230712",
            "baseTime": "1200",
            "category": "PTY",
            "nx": 98,
            "ny": 74,
            "obsrValue": "1"
          },
         */

        public int BaseDate { get; set; }
        public int BaseTime { get; set; }
        public string Category { get; set; }
        public int NX { get; set; }
        public int NY { get; set; }
        public double ObsrValue { get; set; }
    }
}
