using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Strive.Core.Domain.Entities;
using Strive.Core.Extensions;
using Strive.Core.Services.ConferenceManagement;
using Strive.Core.Services.Permissions;
using Xunit;

namespace Strive.Core.Tests.Services.ConferenceManagement
{
    public class ConferenceDataValidatorTests
    {
        private readonly ConferenceDataValidator _validator = new();

        private ConferenceData GetValidConferenceData()
        {
            var data = new ConferenceData();
            data.Configuration.Moderators.Add("Michael");

            return data;
        }

        [Fact]
        public void Validate_ValidMinimalData_Valid()
        {
            // arrange
            var data = GetValidConferenceData();

            // act
            var result = _validator.Validate(data);

            // assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_ModeratorsEmpty_Invalid()
        {
            // arrange
            var data = new ConferenceData();

            // act
            var result = _validator.Validate(data);

            // assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_InvalidCronSchedule_Invalid()
        {
            // arrange
            var data = GetValidConferenceData();
            data.Configuration.ScheduleCron = "test 123";

            // act
            var result = _validator.Validate(data);

            // assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_ValidCron_Valid()
        {
            // arrange
            var data = GetValidConferenceData();
            data.Configuration.ScheduleCron = "0 0 16 ? * MON";

            // act
            var result = _validator.Validate(data);

            // assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_InvalidPermission_Invalid()
        {
            // arrange
            var data = GetValidConferenceData();
            data.Permissions[PermissionType.Conference] =
                new Dictionary<string, JValue> {{"yolo", (JValue) JToken.FromObject(true)}};

            // act
            var result = _validator.Validate(data);

            // assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_InvalidPermissionGroup_Invalid()
        {
            // arrange
            var data = GetValidConferenceData();
            data.Permissions[(PermissionType) 5] = new Dictionary<string, JValue>();

            // act
            var result = _validator.Validate(data);

            // assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_ValidPermission_Valid()
        {
            // arrange
            var data = GetValidConferenceData();
            data.Permissions[PermissionType.Conference] =
                new Dictionary<string, JValue>(new[] {DefinedPermissions.Conference.CanOpenAndClose.Configure(true)});

            // act
            var result = _validator.Validate(data);

            // assert
            Assert.True(result.IsValid);
        }
    }
}
