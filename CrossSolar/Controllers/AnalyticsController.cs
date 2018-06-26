using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossSolar.Controllers
{
    [Route("panel")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        private readonly IPanelRepository _panelRepository;

        public AnalyticsController(IAnalyticsRepository analyticsRepository, IPanelRepository panelRepository)
        {
            _analyticsRepository = analyticsRepository;
            _panelRepository = panelRepository;
        }

        // GET panel/XXXX1111YYYY2222/analytics
        [HttpGet("{panelId}/[controller]")]
        public async Task<IActionResult> Get([FromRoute]int panelId)
        {
            var panel = await _panelRepository.Query()
                .FirstOrDefaultAsync(x => x.Serial.Equals(panelId));

            if (panel == null)
            {
                return NotFound();
            }

            var analytics = await _analyticsRepository.Query()
                .Where(x => x.PanelId.Equals(panelId)).ToListAsync();

            var result = new OneHourElectricityListModel
            {
                OneHourElectricitys = analytics.Select(c => new OneHourElectricityModel
                {
                    Id = c.Id,
                    KiloWatt = c.KiloWatt,
                    DateTime = c.DateTime
                })
            };

            return Ok(result);
        }

        // GET panel/XXXX1111YYYY2222/analytics/day
        [HttpGet("{panelId}/[controller]/{day}")]
        public async Task<IActionResult> DayResults([FromRoute]int panelId, [FromRoute] DateTime day)
        {
            var datePart = day.Date.ToString("d");
            var hourlyReultsqury = _analyticsRepository.Query().Where(p => p.PanelId == panelId
            && p.DateTime.Date.ToString("d") == datePart).ToList();

            if (hourlyReultsqury == null || hourlyReultsqury.Count() == 0)
            {
                return NotFound();
            }
            IEnumerable<OneDayElectricityModel> dailyResults = from res in hourlyReultsqury
                                                               group (res.KiloWatt) by new
                                                               {
                                                                   res.DateTime.Day
                                                               }
                                                            into gs
                                                               select new OneDayElectricityModel()
                                                               {
                                                                   Sum = gs.Sum(),
                                                                   Minimum = gs.Min(),
                                                                   Maximum = gs.Max(),
                                                                   Average = gs.Average(),
                                                                   DateTime = DateTime.Parse(datePart)
                                                               };


            return Ok(dailyResults);
        }

        // POST panel/XXXX1111YYYY2222/analytics
        [HttpPost("{panelId}/[controller]")]
        public async Task<IActionResult> Post([FromRoute]int panelId, [FromBody]OneHourElectricityModel value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_panelRepository.Query().Where(p => p.Id == panelId).SingleOrDefault() == null)
            {
                return new UnauthorizedResult();

            }
            var oneHourElectricityContent = new OneHourElectricity
            {
                PanelId = panelId,
                KiloWatt = value.KiloWatt,
                DateTime = value.DateTime
            };

            await _analyticsRepository.InsertAsync(oneHourElectricityContent);

            var result = new OneHourElectricityModel
            {
                Id = oneHourElectricityContent.Id,
                KiloWatt = oneHourElectricityContent.KiloWatt,
                DateTime = oneHourElectricityContent.DateTime
            };

            return Created($"panel/{panelId}/analytics/{result.Id}", result);
        }
    }
}
