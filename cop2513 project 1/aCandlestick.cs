using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cop2513_project_1
{
    internal class aCandlestick
    {
        public DateTime Date { get; set; }
        public Decimal Open { get; set; }
        public Decimal Close { get; set; }
        public Decimal High { get; set; }
        public Decimal Low { get; set; }
        public long Volume { get; set; }

        public aCandlestick()
        {
            Date = DateTime.Now;
            Open = 0; Close= 0; High = 0; Low = 0; Volume = 0;
        }

        public aCandlestick(DateTime date, decimal open, decimal close, decimal high, decimal low, long volume)
        {
            Date = date;
            Open = open;
            Close = close;
            High = high;
            Low = low;
            Volume = volume;
        }
    }
}