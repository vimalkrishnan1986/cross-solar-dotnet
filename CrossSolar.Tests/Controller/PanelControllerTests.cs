using System.Threading.Tasks;
using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Net;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace CrossSolar.Tests.Controller
{
    public class PanelControllerTests:UnitTestBase
    {
        private PanelController _panelController;

        private IPanelRepository _panelRepository;

        public PanelControllerTests():base()
        {
            var builder = new DbContextOptionsBuilder<CrossSolarDbContext>().UseSqlServer(ConnectionString);
            var context = new CrossSolarDbContext(builder.Options);
            _panelRepository = new PanelRepository(context);
            _panelController = new PanelController(_panelRepository);
        }

        [Fact]
        public async Task Register_ShouldInsertPanel()
        {

            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            };

            // Arrange

            // Act
            var result = await _panelController.Register(panel);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);

        }

        [Fact]
        public async Task Register_PerformValidations()
        {

            //with  empty serial number

            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = ""
            };

            var result = await _panelController.Register(panel);
            // Assert
            Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal<int>((int)HttpStatusCode.BadRequest, (int)createdResult.StatusCode);


            //  with invalid serial number
            panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "1212121212121212121212"
            };

            result = await _panelController.Register(panel);
            Assert.IsType<CreatedResult>(result);
            createdResult = result as CreatedResult;
            Assert.Equal<int>((int)HttpStatusCode.BadRequest, (int)createdResult.StatusCode);
        }

    }
}
