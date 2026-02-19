using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Constants.Swagger;

public class DataExamples
{
    public const string ROLCREATEDATA = @" {
    ""roleId"": 1,
    ""roleName"": ""ADMIN"",
    ""isMandatory"": false,
    ""rolePurpose"": ""Admin description"",
    ""platformId"": 2,
    ""active"": true,
    ""deleted"": false,
    ""createdDate"": ""2026-02-03T20:12:00.204"",
    ""createdUser"": ""Mithun N"",
    ""modifiedDate"": null,
    ""modifiedUser"": "" "",
    ""deletedUser"": "" "",
    ""deletedDate"": null
  }";
    public const string ROLEBYIDDATA = @" {
""planRoleActionLink"": [
      {
        ""planRoleLinkId"": 14,
        ""planId"": 1,
        ""planActionLink"": [
          {
            ""actionLinkId"": 5,
            ""actionName"": ""Editt"",
            ""active"": true,
            ""createdUser"": ""Mithun N"",
            ""createdDate"": ""2026-02-03T20:12:00.238"",
            ""modifiedUser"": "" "",
            ""modifiedDate"": null
          }
        ]
      }
    ],
    ""roleId"": 1,
    ""roleName"": ""ADMIN"",
    ""isMandatory"": false,
    ""rolePurpose"": ""Admin description"",
    ""platformId"": 2,
    ""active"": true,
    ""deleted"": false,
    ""createdDate"": ""2026-02-03T20:12:00.204"",
    ""createdUser"": ""Mithun N"",
    ""modifiedDate"": null,
    ""modifiedUser"": "" "",
    ""deletedUser"": "" "",
    ""deletedDate"": null
  }";
    public const string ROLUPDATEDATA = @" {
    ""roleId"": 1,
    ""roleName"": ""ADMIN"",
    ""isMandatory"": false,
    ""rolePurpose"": ""Admin description"",
    ""platformId"": 2,
    ""active"": true,
    ""deleted"": false,
    ""createdDate"": ""2026-02-03T20:12:00.204"",
    ""createdUser"": ""Mithun N"",
    ""modifiedDate"": ""2026-02-03T20:12:00.204"",
    ""modifiedUser"": ""Mithun N"",
    ""deletedUser"": "" "",
    ""deletedDate"": null
  }";
    public const string ROLGETALLDATA = @" [{
    ""roleId"": 1,
    ""roleName"": ""ADMIN"",
    ""isMandatory"": false,
    ""rolePurpose"": ""Admin description"",
    ""platformId"": 2,
    ""active"": true,
    ""deleted"": false,
    ""createdDate"": ""2026-02-03T20:12:00.204"",
    ""createdUser"": ""Mithun N"",
    ""modifiedDate"": ""2026-02-03T20:12:00.204"",
    ""modifiedUser"": ""Mithun N"",
    ""deletedUser"": "" "",
    ""deletedDate"": null
  },{
    ""roleId"": 2,
    ""roleName"": ""USER"",
    ""isMandatory"": false,
    ""rolePurpose"": ""User description"",
    ""platformId"": 2,
    ""active"": true,
    ""deleted"": false,
    ""createdDate"": ""2026-02-03T20:12:00.204"",
    ""createdUser"": ""Mithun N"",
    ""modifiedDate"": ""2026-02-03T20:12:00.204"",
    ""modifiedUser"": ""Mithun N"",
    ""deletedUser"": "" "",
    ""deletedDate"": null
  }]";
    public const string ROLLOOKUPATA = @" [{
    ""roleId"": 1,
    ""roleName"": ""ADMIN""
  },{
    ""roleId"": 2,
    ""roleName"": ""USER""
  }]";
}

