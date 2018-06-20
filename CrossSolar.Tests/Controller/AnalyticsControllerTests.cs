using System.Threading.Tasks;
using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Net;
using System;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;

namespace CrossSolar.Tests.Controller
{
    public class AnalyticsControllerTests
    {
        //mock framework

        private AnalyticsController _anlyticsController;
        private PanelController _panelController;
        private IAnalyticsRepository _anlticsReposiitory;
        private IPanelRepository _panelRepository;
        private string connectionString
        {
            get

            {
                return DbHelper.GetConnectionString();

            }
        }
        public AnalyticsControllerTests()
        {
            var contextOpions = new DbContextOptionsBuilder<CrossSolarDbContext>().UseSqlServer(connectionString).Options;
            var context = new CrossSolarDbContext(contextOpions);
            _anlticsReposiitory = new AnalyticsRepository(context);
            _panelRepository = new PanelRepository(context);
            _anlyticsController = new AnalyticsController(_anlticsReposiitory, _panelRepository);
            _panelController = new PanelController(_panelRepository);
        }

        [Fact]

        public async void AnalyticsControllerTests_Day()
        {

            //insert pannel

            DateTime date1 = new DateTime(2017, 11, 05);
            DateTime date1_t1 = date1.AddHours(1);
            DateTime date1_t2 = date1.AddHours(2);


            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            };


            var result = await _panelController.Register(panel);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
            var createdpanel = createdResult.Value as Panel;
            int panelId = createdpanel.Id;

            //inserting  records  to table

            await _anlyticsController.Post(panelId, new OneHourElectricityModel()
            {
                KiloWatt = 100,
                DateTime = date1_t1
            });


            await _anlyticsController.Post(panelId, new OneHourElectricityModel()
            {
                KiloWatt = 200,
                DateTime = date1_t2
            });


            var day1Result = _anlyticsController.DayResults(panelId, date1).Result;
            Assert.IsType<OkObjectResult>(day1Result);
            var day1records = day1Result as OkObjectResult;
            Assert.True(day1records.StatusCode == (int)HttpStatusCode.OK);
            Assert.IsAssignableFrom<IEnumerable<OneDayElectricityModel>>(day1records.Value);
            Assert.True((day1records.Value as IEnumerable<OneDayElectricityModel>).Count() == 1);
            var day1Record = (day1records.Value as IEnumerable<OneDayElectricityModel>).Single();
            Assert.Equal(150, day1Record.Average);
            Assert.Equal(100, day1Record.Minimum);
            Assert.Equal(200, day1Record.Maximum);


        }
    }
}
