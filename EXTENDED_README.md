# DataTables.AspNet - Extended for .NET 9.0

## Overview
This is an extended fork of [DataTables.AspNet](https://github.com/ALMMa/datatables.aspnet) with enhancements for modern .NET applications and full support for DataTables extensions like columnControl.

## What's New in Extended Version

### 1. **Updated to .NET 9.0**
- Upgraded from `netstandard1.1`/`net6.0` to `net9.0`
- Full compatibility with latest ASP.NET Core features

### 2. **Automatic columnControl Parsing**
- **No more manual parsing!** The ModelBinder now automatically extracts columnControl data from form requests
- Supports `columns[i][columnControl][list][j]` pattern for dropdown selections
- Supports any columnControl property automatically
- Available in `request.AdditionalParameters["columnControl"]`

### 3. **Structure of Parsed columnControl Data**

```csharp
// request.AdditionalParameters["columnControl"] contains:
{
  "identityType": {
    "list": ["IdentityContract", "IdentityInternal"]
  },
  "status": {
    "list": ["active", "released"]
  }
}
```

## Usage

### Before (Manual Parsing Required)

```csharp
[HttpPost("load-data")]
public async Task<IActionResult> LoadData(IDataTablesRequest request)
{
    var columnFilters = new List<ColumnFilter>();
    
    // Manual parsing of Request.Form for columnControl data
    foreach (var key in Request.Form.Keys)
    {
        if (key.Contains("[columnControl][list]"))
        {
            var match = Regex.Match(key, @"columns\[(\d+)\]\[columnControl\]\[list\]\[(\d+)\]");
            // ... lots of manual parsing code ...
        }
    }
}
```

### After (Automatic Parsing)

```csharp
[HttpPost("load-data")]
public async Task<IActionResult> LoadData(IDataTablesRequest request)
{
    var columnFilters = new List<ColumnFilter>();
    
    // Automatically parsed by ModelBinder!
    if (request.AdditionalParameters?.ContainsKey("columnControl") == true)
    {
        var columnControl = request.AdditionalParameters["columnControl"] as IDictionary<string, object>;
        
        foreach (var kvp in columnControl)
        {
            var fieldName = kvp.Key; // e.g., "identityType"
            var controlData = kvp.Value as IDictionary<string, object>;
            
            if (controlData?.ContainsKey("list") == true)
            {
                var listValues = controlData["list"] as IEnumerable<object>;
                foreach (var value in listValues)
                {
                    columnFilters.Add(new ColumnFilter
                    {
                        Field = fieldName,
                        SearchValue = value.ToString(),
                        Multiple = true
                    });
                }
            }
        }
    }
}
```

## Installation in Your Project

### Option 1: Project Reference (Current Setup)
The projects are already added to your solution as project references:
- `Libraries/DataTables.AspNet/src/DataTables.AspNet.Core/DataTables.AspNet.Core.csproj`
- `Libraries/DataTables.AspNet/src/DataTables.AspNet.AspNetCore/DataTables.AspNet.AspNetCore.csproj`

Your `IAM.Web.Mvc` project now references these local projects instead of NuGet packages.

### Option 2: Publish to Private NuGet Feed
1. Build the projects:
   ```powershell
   dotnet pack Libraries/DataTables.AspNet/src/DataTables.AspNet.Core/DataTables.AspNet.Core.csproj -c Release
   dotnet pack Libraries/DataTables.AspNet/src/DataTables.AspNet.AspNetCore/DataTables.AspNet.AspNetCore.csproj -c Release
   ```

2. Publish to your private feed:
   ```powershell
   dotnet nuget push Libraries/DataTables.AspNet/src/DataTables.AspNet.Core/bin/Release/*.nupkg -s https://your-private-feed
   dotnet nuget push Libraries/DataTables.AspNet/src/DataTables.AspNet.AspNetCore/bin/Release/*.nupkg -s https://your-private-feed
   ```

### Option 3: Publish to NuGet.org (If Open Source)
1. Update package names in `.csproj` files to avoid conflicts:
   - `DataTables.AspNet.Core.Extended`
   - `DataTables.AspNet.AspNetCore.Extended`

2. Get your NuGet API key from nuget.org

3. Publish:
   ```powershell
   dotnet nuget push *.nupkg -k YOUR_API_KEY -s https://api.nuget.org/v3/index.json
   ```

## Key Files Modified

1. **DataTables.AspNet.Core.csproj**
   - Updated `TargetFramework` to `net9.0`
   - Changed `PackageId` to avoid conflicts

2. **DataTables.AspNet.AspNetCore.csproj**
   - Updated `TargetFramework` to `net9.0`
   - Changed `PackageId` to avoid conflicts

3. **ModelBinder.cs**
   - Added `ParseColumnControlData()` method
   - Automatically extracts columnControl data from form
   - Populates `AdditionalParameters["columnControl"]`

## License
MIT License - Same as original DataTables.AspNet

Original Copyright (c) 2014-2022 Anderson Matos  
Extended (c) 2026

## Credits
- Original library: [ALMMa/datatables.aspnet](https://github.com/ALMMa/datatables.aspnet)
- Extended for IAM project with .NET 9.0 and columnControl support
