using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrossSolar.Domain
{
    public class Panel
    {
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [Required]

        public string Serial { get; set; }

        public string Brand { get; set; }

        [ForeignKey("PanelId")]
        public ICollection<OneHourElectricity> HourlyRecords { get; set; }

    }
}
