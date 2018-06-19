using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Domain;
using CrossSolar.Models;

namespace CrossSolar.Domain
{
    public class OneHourElectricity
    {
        public int Id { get; set; }

        public int PanelId { get; set; }

        public long KiloWatt { get; set; }

        public DateTime DateTime { get; set; }
    }
}
