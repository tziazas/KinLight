namespace KinLight.Modules.Portal.Client.Arrange;

/// <summary>A position or size change reported by gridstack.</summary>
/// <param name="Id">The placement id (gs-id).</param>
/// <param name="X">Column.</param>
/// <param name="Y">Row.</param>
/// <param name="W">Width.</param>
/// <param name="H">Height.</param>
public sealed record GridItemChange(Guid Id, int X, int Y, int W, int H);
