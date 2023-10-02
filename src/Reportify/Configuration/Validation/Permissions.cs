using System.Text.Json.Serialization;

namespace Reportify.Configuration.Validation;

internal sealed record Permissions([property: JsonPropertyName("permissions")]
  BrowseProjectsPermission BrowseProjectsPermission);

internal sealed record BrowseProjectsPermission([property: JsonPropertyName("BROWSE_PROJECTS")]
  Permission Permission);

internal sealed record Permission([property: JsonPropertyName("havePermission")]
  bool HavePermission);