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
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CrossSolar.Tests.Controller
{
    public class PanelControllerTests : UnitTestBase
    {
        private PanelController _panelController;

        private IPanelRepository _panelRepository;

        public PanelControllerTests() : base()
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
        public void Register_PerformValidations()
        {

            //with  empty serial number

            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "1212121212121212"
            };


            var context = new ValidationContext(panel, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(panel, context, result, true);
            Assert.True(valid);

            //inavlid record (blank serial number

            panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = ""
            };

            context = new ValidationContext(panel, null, null);
            result = new List<ValidationResult>();

            valid = Validator.TryValidateObject(panel, context, result, true);
            Assert.True(!valid);

            //invalid serial number 

            panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "1123132`12`12`12`12"
            };

            context = new ValidationContext(panel, null, null);
            result = new List<ValidationResult>();

            valid = Validator.TryValidateObject(panel, context, result, true);
            Assert.True(!valid);
        }

    }
}
