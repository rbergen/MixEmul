# .NET 10.0 Upgrade Plan - MixEmul Solution

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
  - [MixLib.csproj](#mixlibcsproj)
  - [MixAssembler.csproj](#mixassemblercsproj)
  - [MixEmul.csproj](#mixemulcsproj)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
Upgrade MixEmul solution from .NET 8.0 (net8.0-windows7.0) to .NET 10.0 (net10.0-windows) to leverage the latest Long Term Support (LTS) release, improved performance, and enhanced Windows Forms capabilities.

### Scope
**Projects Affected**: 3 projects
1. **src\MixLib\MixLib.csproj** - Core library (7,641 LOC)
2. **src\MixAssembler\MixAssembler.csproj** - Assembler library (1,710 LOC)  
3. **src\MixEmul\MixEmul.csproj** - Windows Forms application (15,059 LOC)

**Current State**: All projects target net8.0-windows7.0  
**Target State**: All projects will target .NET 10.0 with appropriate Windows platform versioning

### Discovered Metrics
| Metric | Value | Status |
|--------|-------|--------|
| Total Projects | 3 | All require upgrade |
| Total LOC | 24,410 | Medium codebase |
| Dependency Depth | 2 levels | Simple linear chain |
| NuGet Packages | 0 | No package updates needed ✅ |
| API Issues | 13,035 | Concentrated in MixEmul.csproj |
| Binary Incompatible APIs | 12,661 | Windows Forms (non-breaking) |
| Source Incompatible APIs | 374 | System.Drawing |
| Security Vulnerabilities | 0 | None ✅ |

### Complexity Classification
**MEDIUM** - Small solution with concentrated API compatibility challenges

**Rationale**:
- ✅ Small project count (3 projects)
- ✅ Simple dependency structure (no circular dependencies)
- ✅ All projects on modern .NET baseline (net8.0)
- ✅ No NuGet package updates required
- ✅ No security vulnerabilities
- ⚠️ High API incompatibility count (13k+) in main application
- ⚠️ Estimated 86.6% LOC impact in MixEmul.csproj
- ℹ️ API issues primarily Windows Forms binary incompatibilities (typically non-breaking)

### Critical Issues
**None** - No blocking security vulnerabilities or critical compatibility issues identified.

**Key Consideration**: The 13,035 API "issues" reported are predominantly Windows Forms binary incompatibility flags. These are expected when upgrading Windows Forms applications and typically do not require code changes—the APIs exist and function identically in .NET 10.0. Verification through compilation will confirm minimal actual code changes needed.

### Selected Strategy
**All-At-Once Strategy** - All projects upgraded simultaneously in single atomic operation.

**Rationale**:
- Solution meets all criteria for All-At-Once approach:
  - Small solution (3 projects, well below 30-project threshold)
  - All projects currently on modern .NET (8.0)
  - Homogeneous codebase (all Windows desktop projects)
  - No NuGet package updates needed
  - Clear dependency structure
  - Single development team working on unified codebase
- Benefits:
  - Fastest completion time
  - No multi-targeting complexity
  - Single comprehensive testing phase
  - Clean dependency resolution
  - Minimal coordination overhead
- Trade-offs:
  - All API compatibility issues addressed in one pass
  - Full solution testing required before completion
  - Short period of build instability during upgrade

### Iteration Strategy
**Fast Batch (5 iterations total)**
- Phase 1: Foundation (3 iterations) - Strategy, dependency analysis, project stubs
- Phase 2: Detail Generation (1 iteration) - All project details in single batch
- Phase 3: Finalization (1 iteration) - Risk, testing, success criteria

---

## Migration Strategy

### Approach Selection: All-At-Once Strategy

**Decision**: Upgrade all 3 projects simultaneously in a single coordinated operation.

### Justification

**Why All-At-Once**:

1. **Solution Size**: 3 projects (well below 30-project threshold for All-At-Once)
2. **Modern Baseline**: All projects currently on .NET 8.0 (modern .NET)
3. **Homogeneous Stack**: All Windows desktop projects with consistent technology patterns
4. **No Package Complexity**: Zero NuGet packages to update
5. **Clear Structure**: Simple linear dependency chain with no circular references
6. **Assessment Compatibility**: All projects have known target frameworks compatible with .NET 10.0
7. **Unified Codebase**: Single team working on cohesive solution

**Why Not Incremental**:
- Solution too small to benefit from phased approach overhead
- No complex external dependencies requiring staged validation
- All projects owned by same team, allowing coordinated upgrade
- No multi-targeting requirements or mixed framework scenarios
- Testing entire solution at once is feasible

### All-At-Once Strategy Rationale

The All-At-Once strategy provides:
- **Fastest Time to Completion**: Single upgrade operation vs. multiple phases
- **No Multi-Targeting Complexity**: Projects never need to support multiple framework versions
- **Unified Testing**: One comprehensive test cycle vs. multiple phase validations
- **Clean Dependency Resolution**: No intermediate states with mixed framework versions
- **Simplified Coordination**: Single atomic change vs. coordinated multi-phase rollout

**Acceptable Trade-offs**:
- Higher initial risk (all projects change simultaneously) - mitigated by thorough testing
- Larger testing surface - acceptable given small solution size
- Brief build instability during upgrade - expected and planned for

### Dependency-Based Ordering Rationale

While All-At-Once updates all projects simultaneously, the **validation order** follows dependency hierarchy:

**Logical Validation Sequence**:
1. **MixLib.csproj** (Level 0 - Leaf node)
   - No dependencies, validates independently
   - Any compilation errors here affect downstream projects

2. **MixAssembler.csproj** (Level 1)
   - Depends on MixLib
   - Validates after MixLib builds successfully

3. **MixEmul.csproj** (Level 2 - Root)
   - Depends on both MixLib and MixAssembler
   - Final validation point for entire solution

**Build Strategy**: Use `dotnet build MixEmul.sln` to build entire solution, which automatically respects dependency order.

### Execution Approach

**Single Atomic Operation**:
1. Update all TargetFramework properties across all 3 projects simultaneously
2. Restore dependencies for entire solution
3. Build entire solution to identify compilation errors
4. Fix all compilation errors found (expected to be minimal based on assessment)
5. Rebuild solution to verify fixes
6. Execute comprehensive testing

**No Intermediate States**: Solution moves directly from "all .NET 8" to "all .NET 10" without partial states.

### Parallel vs Sequential Execution

**Sequential Execution** within the atomic operation:
- Framework updates: Can be done in parallel (independent file edits)
- Dependency restoration: Single `dotnet restore` for solution
- Build: Automatically sequential (MSBuild respects dependencies)
- Error fixing: Dependency order (fix leaf nodes first)
- Testing: Comprehensive solution-wide testing

**Parallelization Opportunities**:
- Project file editing (3 files can be updated simultaneously)
- Code analysis for breaking changes (can analyze all projects concurrently)

**Sequential Requirements**:
- Build must respect dependency order (MixLib → MixAssembler → MixEmul)
- Error fixing should follow dependency order (fix dependencies before dependants)

### Risk Management Alignment

The All-At-Once strategy is appropriate given:
- **Low Package Risk**: Zero package updates eliminates compatibility conflicts
- **Predictable API Risk**: Windows Forms binary incompatibilities are well-understood and typically non-breaking
- **Good Test Coverage**: Solution structure suggests comprehensive testing capability
- **Rollback Simplicity**: Single atomic change = single rollback point (Git branch)

### Alternative Considered: Incremental Migration

**Why Rejected**:
- Would require 3 separate phases (one per project)
- Each phase would need separate testing cycle
- Multi-targeting would be needed if maintaining .NET 8 compatibility
- Coordination overhead exceeds solution complexity
- No significant risk reduction benefit for this small solution

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The MixEmul solution has a clean, linear dependency structure with no circular dependencies:

```
MixEmul.csproj (Application)
├── MixAssembler.csproj (Library)
│   └── MixLib.csproj (Core Library)
└── MixLib.csproj (Core Library)
```

**Dependency Levels**:
- **Level 0 (Leaf)**: MixLib.csproj - No project dependencies, 2 dependants
- **Level 1**: MixAssembler.csproj - Depends on MixLib, 1 dependant
- **Level 2 (Root)**: MixEmul.csproj - Depends on both MixLib and MixAssembler

### Project Groupings by Migration Phase

Since we're using **All-At-Once Strategy**, all projects are upgraded simultaneously in a single atomic operation:

**Single Atomic Upgrade Phase**:
- **MixLib.csproj** - Core library (net8.0-windows7.0 → net10.0-windows7.0)
- **MixAssembler.csproj** - Assembler library (net8.0-windows7.0 → net10.0-windows7.0)
- **MixEmul.csproj** - Windows Forms app (net8.0-windows7.0 → net10.0-windows)

All three projects will have their TargetFramework properties updated simultaneously, followed by a unified build and testing phase.

### Critical Path Identification

**Critical Path**: MixLib.csproj → MixAssembler.csproj → MixEmul.csproj

While the All-At-Once strategy updates all projects simultaneously, the dependency chain establishes the logical validation order:
1. MixLib must compile cleanly (no dependencies)
2. MixAssembler must compile cleanly (depends on MixLib)
3. MixEmul must compile cleanly (depends on both)

Any compilation errors will be addressed in dependency order during the single atomic upgrade phase.

### Circular Dependency Analysis

**None detected** - The solution has a clean acyclic dependency graph.

### Target Framework Assignments

| Project | Current TFM | Proposed TFM | Rationale |
|---------|-------------|--------------|-----------|
| MixLib.csproj | net8.0-windows7.0 | net10.0-windows7.0 | Core library maintaining Windows 7 compatibility baseline |
| MixAssembler.csproj | net8.0-windows7.0 | net10.0-windows7.0 | Library maintaining Windows 7 compatibility baseline |
| MixEmul.csproj | net8.0-windows7.0 | net10.0-windows | Windows Forms application, simplified Windows TFM |

**Note**: The proposed TFM for MixEmul.csproj changes from `net10.0--windows7.0` (assessment notation with double hyphen) to `net10.0-windows` (standard notation). This follows .NET 10 conventions where Windows Forms applications typically target `net10.0-windows` without explicit platform version unless specific Windows API levels are required.

---

## Project-by-Project Plans

This section provides detailed migration specifications for each project in dependency order.

### MixLib.csproj

**Current State**: 
- Target Framework: net8.0-windows7.0
- Project Type: ClassLibrary (SDK-style)
- Dependencies: 0 project dependencies, 0 NuGet packages
- Dependants: 2 (MixAssembler, MixEmul)
- Lines of Code: 7,641
- Files: 93 code files
- API Compatibility Issues: 0

**Target State**:
- Target Framework: net10.0-windows7.0
- Updated Packages: None (no packages)

#### Migration Steps

##### 1. Prerequisites
- ✅ .NET 10 SDK installed (verified during assessment initialization)
- ✅ Project already SDK-style (no conversion needed)
- ✅ No NuGet package prerequisites

##### 2. Framework Update

**File**: `src\MixLib\MixLib.csproj`

**Change Required**:
```xml
<!-- Current -->
<TargetFramework>net8.0-windows7.0</TargetFramework>

<!-- Updated -->
<TargetFramework>net10.0-windows7.0</TargetFramework>
```

**Rationale**: Core library maintains Windows 7 compatibility baseline (`-windows7.0` platform version) for maximum compatibility.

##### 3. Package/Module/Dependency Updates

**None required** - Project has zero NuGet packages.

##### 4. Expected Breaking Changes

**None anticipated** - Assessment found zero API compatibility issues in this project.

**Verification Required**:
- Confirm clean compilation after framework update
- Verify no unexpected API changes in code files

##### 5. Code Modifications

**Expected Changes**: None

**Rationale**:
- No API compatibility issues flagged in assessment
- Standard class library with 4,525 compatible APIs
- No deprecated framework features identified

**If Compilation Errors Occur**:
1. Review error messages for specific API changes
2. Consult .NET 10 breaking changes documentation
3. Apply minimal fixes to restore compilation
4. Document any unexpected breaking changes for future reference

##### 6. Testing Strategy

**Unit Testing**:
- If MixLib has unit tests, execute them after upgrade
- Verify all tests pass with .NET 10 runtime

**Integration Testing**:
- Indirect testing through dependent projects (MixAssembler, MixEmul)
- Verify MixAssembler builds successfully against upgraded MixLib
- Verify MixEmul builds successfully against upgraded MixLib

**Manual Testing**:
- Not applicable (library project, no direct UI)

##### 7. Validation Checklist

- [ ] Project file TargetFramework updated to `net10.0-windows7.0`
- [ ] Project builds without errors: `dotnet build src\MixLib\MixLib.csproj`
- [ ] Project builds without warnings
- [ ] All unit tests pass (if present)
- [ ] No API compatibility warnings
- [ ] Dependent projects (MixAssembler, MixEmul) can reference upgraded library

**Success Criteria**: Clean build with zero errors and zero warnings.

---

### MixAssembler.csproj

**Current State**:
- Target Framework: net8.0-windows7.0
- Project Type: ClassLibrary (SDK-style)
- Dependencies: 1 (MixLib)
- Dependants: 1 (MixEmul)
- Lines of Code: 1,710
- Files: 32 code files
- API Compatibility Issues: 0

**Target State**:
- Target Framework: net10.0-windows7.0
- Updated Packages: None (no packages)

#### Migration Steps

##### 1. Prerequisites
- ✅ .NET 10 SDK installed
- ✅ Project already SDK-style (no conversion needed)
- ✅ MixLib.csproj upgraded to net10.0-windows7.0 (dependency)
- ✅ No NuGet package prerequisites

##### 2. Framework Update

**File**: `src\MixAssembler\MixAssembler.csproj`

**Change Required**:
```xml
<!-- Current -->
<TargetFramework>net8.0-windows7.0</TargetFramework>

<!-- Updated -->
<TargetFramework>net10.0-windows7.0</TargetFramework>
```

**Rationale**: Library maintains Windows 7 compatibility baseline consistent with MixLib dependency.

##### 3. Package/Module/Dependency Updates

**None required** - Project has zero NuGet packages.

**Project Reference Updates**:
- ProjectReference to MixLib remains unchanged (path-based reference automatically uses upgraded version)
- No version constraints to update

##### 4. Expected Breaking Changes

**None anticipated** - Assessment found zero API compatibility issues in this project.

**Verification Required**:
- Confirm clean compilation after framework update
- Verify compatibility with upgraded MixLib dependency

##### 5. Code Modifications

**Expected Changes**: None

**Rationale**:
- No API compatibility issues flagged in assessment
- Standard class library with 1,057 compatible APIs
- Depends only on MixLib (also has zero API issues)
- No deprecated framework features identified

**If Compilation Errors Occur**:
1. Distinguish between MixLib integration issues vs. framework API issues
2. Review MixLib public API for any changes affecting MixAssembler
3. Apply minimal fixes to restore compilation
4. Document any unexpected breaking changes

##### 6. Testing Strategy

**Unit Testing**:
- If MixAssembler has unit tests, execute them after upgrade
- Verify all tests pass with .NET 10 runtime
- Verify tests correctly integrate with upgraded MixLib

**Integration Testing**:
- Indirect testing through MixEmul (dependent application)
- Verify MixEmul builds successfully against upgraded MixAssembler
- Test assembly functionality through MixEmul UI

**Manual Testing**:
- Not applicable (library project, no direct UI)

##### 7. Validation Checklist

- [ ] Project file TargetFramework updated to `net10.0-windows7.0`
- [ ] Project builds without errors: `dotnet build src\MixAssembler\MixAssembler.csproj`
- [ ] Project builds without warnings
- [ ] All unit tests pass (if present)
- [ ] No API compatibility warnings
- [ ] Successfully integrates with upgraded MixLib
- [ ] Dependent project (MixEmul) can reference upgraded library

**Success Criteria**: Clean build with zero errors and zero warnings, successful integration with MixLib.

---

### MixEmul.csproj

**Current State**:
- Target Framework: net8.0-windows7.0
- Project Type: WinForms (SDK-style)
- Dependencies: 2 (MixLib, MixAssembler)
- Dependants: 0 (root application)
- Lines of Code: 15,059
- Files: 93 code files (51 with API incidents)
- API Compatibility Issues: 13,035
  - Binary Incompatible: 12,661 (Windows Forms)
  - Source Incompatible: 374 (System.Drawing)

**Target State**:
- Target Framework: net10.0-windows
- Updated Packages: None (no packages)

#### Migration Steps

##### 1. Prerequisites
- ✅ .NET 10 SDK installed
- ✅ Project already SDK-style (no conversion needed)
- ✅ MixLib.csproj upgraded to net10.0-windows7.0 (dependency)
- ✅ MixAssembler.csproj upgraded to net10.0-windows7.0 (dependency)
- ✅ No NuGet package prerequisites

##### 2. Framework Update

**File**: `src\MixEmul\MixEmul.csproj`

**Change Required**:
```xml
<!-- Current -->
<TargetFramework>net8.0-windows7.0</TargetFramework>

<!-- Updated -->
<TargetFramework>net10.0-windows</TargetFramework>
```

**Rationale**: 
- Windows Forms application targets standard `net10.0-windows` TFM
- Removes explicit platform version (Windows 7) as .NET 10 establishes modern Windows baseline
- Simplified TFM aligns with .NET 10 Windows desktop conventions

**Alternative** (if Windows 7 compatibility required):
```xml
<TargetFramework>net10.0-windows7.0</TargetFramework>
```
Use this if maintaining explicit Windows 7 API compatibility is required.

##### 3. Package/Module/Dependency Updates

**None required** - Project has zero NuGet packages.

**Project Reference Updates**:
- ProjectReference to MixLib remains unchanged (automatically uses net10.0-windows7.0 version)
- ProjectReference to MixAssembler remains unchanged (automatically uses net10.0-windows7.0 version)
- No version constraints to update

##### 4. Expected Breaking Changes

**Assessment Finding**: 13,035 API compatibility issues

**Reality Check**: High flag count, **minimal actual breaking changes expected**

#### 4.1 Windows Forms Binary Incompatibilities (12,661 flagged)

**Nature of Issues**: 
- Assessment flags these as "binary incompatible" because Windows Forms assemblies have different binary signatures between .NET 8 and .NET 10
- **Critical Understanding**: The APIs themselves are **functionally identical**—same method signatures, same behavior, same capabilities

**Expected Code Changes**: **Zero**

**Top Flagged APIs** (all expected to work without changes):
- `System.Windows.Forms.Button` (850 occurrences)
- `System.Windows.Forms.Label` (842 occurrences)
- `System.Windows.Forms.AnchorStyles` (744 occurrences)
- `System.Windows.Forms.Control.*` properties and methods (thousands of occurrences)
- `System.Windows.Forms.ToolStripMenuItem` (296 occurrences)
- `System.Windows.Forms.CheckBox`, `TextBox`, `GroupBox`, `ComboBox`, `ListView`, `Panel`, `NumericUpDown`, `RichTextBox`, etc.

**Why Flagged But Not Breaking**:
1. Assembly version change triggers "binary incompatible" classification
2. Recompilation against .NET 10 Windows Forms assemblies resolves the "incompatibility"
3. No source code changes needed—APIs work identically

**Verification Approach**:
1. Update TargetFramework
2. Build solution
3. **Expected Result**: Clean build with zero or minimal errors
4. **If Errors Occur**: Address specific compilation errors (unlikely to be widespread)

#### 4.2 System.Drawing Source Incompatibilities (374 flagged)

**Affected APIs**:
- `System.Drawing.ContentAlignment` (135 occurrences)
- `System.Drawing.Font` (70 occurrences)
- Other GDI+ types

**Expected Code Changes**: **Zero to minimal**

**Rationale**:
- System.Drawing types are available in .NET 10 via built-in Windows support
- Desktop applications automatically have access (no NuGet package needed for Windows targets)
- Source incompatibility flags may indicate namespace or assembly changes, not API removal

**Verification Approach**:
1. Compilation will reveal any actual incompatibilities
2. Most common fixes (if needed):
   - Ensure `using System.Drawing;` directives present
   - Verify property access patterns match .NET 10 conventions

**Likelihood of Changes**: Very low

#### 4.3 Windows Forms Legacy Controls (7 flagged)

**Potentially Removed Controls**:
- `StatusBar` → Replace with `StatusStrip`
- `ContextMenu` → Replace with `ContextMenuStrip`
- `MainMenu` / `MenuItem` → Replace with `MenuStrip` / `ToolStripMenuItem`
- `ToolBar` → Replace with `ToolStrip`
- `DataGrid` → Replace with `DataGridView`

**Assessment**: Project shows 7 references to legacy control features

**Verification Required**:
1. Determine if project actually uses these legacy controls
2. If used, plan replacements during compilation error fixing phase
3. Modern .NET 8 codebase unlikely to use these (deprecated since .NET Core 3.1)

**Expected Impact**: Low (likely false positives or minimal usage)

##### 5. Code Modifications

**Expected Changes**: **Minimal to none**

**Primary Modification Areas** (if compilation errors occur):

#### 5.1 Namespace Adjustments (if needed)
```csharp
// Ensure System.Drawing usings present
using System.Drawing;
using System.Windows.Forms;
```

#### 5.2 Legacy Control Replacements (if used)

**IF** StatusBar is used:
```csharp
// Old (removed)
StatusBar statusBar = new StatusBar();

// New
StatusStrip statusStrip = new StatusStrip();
ToolStripStatusLabel label = new ToolStripStatusLabel();
statusStrip.Items.Add(label);
```

**IF** ContextMenu is used:
```csharp
// Old (removed)
ContextMenu contextMenu = new ContextMenu();

// New
ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
```

**IF** MainMenu/MenuItem is used:
```csharp
// Old (removed)
MainMenu mainMenu = new MainMenu();
MenuItem menuItem = new MenuItem();

// New
MenuStrip menuStrip = new MenuStrip();
ToolStripMenuItem menuItem = new ToolStripMenuItem();
```

**Likelihood**: Low (modern codebase probably uses modern controls)

#### 5.3 Property or Method Signature Changes (if any)

**Approach**:
1. Let compilation reveal specific errors
2. Consult .NET 10 Windows Forms breaking changes documentation
3. Apply targeted fixes

**Expected Count**: Zero to single-digit changes

##### 6. Testing Strategy

**Critical**: Windows Forms UI requires comprehensive functional testing

#### 6.1 Compilation Testing
- Build MixEmul.csproj: `dotnet build src\MixEmul\MixEmul.csproj`
- **Expected**: Clean build or minimal errors
- Fix any compilation errors found
- Rebuild to verify fixes

#### 6.2 Unit Testing
- If MixEmul has unit tests, execute after upgrade
- Verify all tests pass with .NET 10 runtime
- Verify tests correctly integrate with upgraded dependencies

#### 6.3 Integration Testing
- Verify project successfully references upgraded MixLib and MixAssembler
- Confirm no runtime assembly loading issues
- Test cross-project functionality

#### 6.4 UI Functional Testing (Critical)

**Application Startup**:
- [ ] Application launches without errors
- [ ] Main form renders correctly
- [ ] All UI controls visible and positioned correctly

**Control Functionality**:
- [ ] Buttons respond to clicks
- [ ] Text boxes accept input
- [ ] Labels display text correctly
- [ ] ComboBoxes/ListViews populate and respond to selection
- [ ] Menus open and execute commands
- [ ] Toolbars/ToolStrips display and function
- [ ] Status bar updates correctly (if present)
- [ ] CheckBoxes/RadioButtons toggle state

**Emulator-Specific Testing** (based on project name):
- [ ] Core emulation functionality works
- [ ] Assembler integration functions correctly
- [ ] File I/O operations succeed
- [ ] Debug/trace features functional
- [ ] Any plugin or extension systems load correctly

**Visual Verification**:
- [ ] Fonts render correctly
- [ ] Icons/images display properly
- [ ] Layout/anchoring/docking behavior correct
- [ ] Form resizing works as expected
- [ ] Dialog boxes appear and function correctly

**Performance Testing**:
- [ ] Application startup time acceptable
- [ ] UI responsiveness maintained
- [ ] No unexpected delays or freezes
- [ ] Memory usage comparable to .NET 8 version

#### 6.5 Regression Testing
- Test all major user workflows end-to-end
- Verify no behavioral changes from .NET 8 version
- Confirm all features work identically

##### 7. Validation Checklist

**Build Validation**:
- [ ] Project file TargetFramework updated to `net10.0-windows`
- [ ] Project builds without errors: `dotnet build src\MixEmul\MixEmul.csproj`
- [ ] Project builds without warnings
- [ ] Solution builds without errors: `dotnet build MixEmul.sln`
- [ ] Solution builds without warnings

**Dependency Validation**:
- [ ] Successfully references upgraded MixLib (net10.0-windows7.0)
- [ ] Successfully references upgraded MixAssembler (net10.0-windows7.0)
- [ ] No assembly version conflicts
- [ ] No runtime assembly loading errors

**Code Validation**:
- [ ] All Windows Forms APIs compile cleanly
- [ ] All System.Drawing APIs compile cleanly
- [ ] No legacy control errors (or replacements implemented)
- [ ] No unexpected breaking changes discovered

**Testing Validation**:
- [ ] All unit tests pass (if present)
- [ ] Application launches successfully
- [ ] UI rendering correct across all forms
- [ ] All controls functional
- [ ] All user workflows operational
- [ ] No performance regressions
- [ ] No visual regressions

**Success Criteria**: 
- Clean build with zero errors and zero warnings
- All automated tests pass
- Full UI functionality verified through manual testing
- No regressions in features, performance, or visual appearance
- Application ready for end-to-end validation

---

## Risk Management

### High-Risk Changes

| Project | Risk Level | Description | Mitigation |
|---------|------------|-------------|------------|
| MixEmul.csproj | 🟡 Medium | 13,035 API compatibility issues (86.6% LOC impact) | Issues are primarily Windows Forms binary incompatibilities which are non-breaking; verify through compilation; comprehensive testing of UI functionality |
| MixLib.csproj | 🟢 Low | Core library with no API issues | Standard framework upgrade; validate through dependent projects |
| MixAssembler.csproj | 🟢 Low | Library with no API issues | Standard framework upgrade; validate through dependent projects |

### Risk Analysis by Category

#### API Compatibility Risk: 🟡 Medium (Appears High, Actually Low)

**Assessment Finding**: 13,035 API compatibility issues in MixEmul.csproj
- 12,661 Binary Incompatible (Windows Forms)
- 374 Source Incompatible (System.Drawing)

**Actual Risk Assessment**: **Low-to-Medium**

**Rationale**:
1. **Windows Forms Binary Incompatibilities (12,661 issues)**:
   - These are flagged as "binary incompatible" because Windows Forms assemblies changed between .NET 8 and .NET 10
   - **However**: The APIs themselves are functionally identical—same signatures, same behavior
   - **Reality**: These typically require **zero code changes**
   - **Examples**: `System.Windows.Forms.Button`, `System.Windows.Forms.Label`, `System.Windows.Forms.AnchorStyles`
   - **Mitigation**: Verify through compilation; expect clean build with no actual changes needed

2. **System.Drawing Source Incompatibilities (374 issues)**:
   - Types like `System.Drawing.Font`, `System.Drawing.ContentAlignment`
   - Available via `System.Drawing.Common` package or built-in Windows support
   - May require minimal namespace adjustments or property access changes
   - **Mitigation**: Address compilation errors if they arise; likely minimal or zero changes

3. **Legacy Control Risk (7 issues)**:
   - Assessment mentions legacy controls (StatusBar, DataGrid, ContextMenu, MainMenu, MenuItem, ToolBar)
   - **Only if used**: Replacement controls required (ToolStrip, MenuStrip, ContextMenuStrip, DataGridView)
   - **Mitigation**: Identify usage during compilation; plan replacements if needed

**Expected Reality**: Despite 13k+ flagged issues, actual code changes expected to be **minimal to none**.

#### Package Dependency Risk: ✅ None

- **No NuGet packages** in any project
- Zero package update conflicts
- Zero security vulnerabilities
- No transitive dependency issues

#### Framework Breaking Changes Risk: 🟢 Low

- Upgrading from .NET 8 (LTS) to .NET 10 (LTS)
- Well-documented upgrade path
- Windows Forms is fully supported in .NET 10
- No deprecated framework features in use (based on assessment)

#### Build System Risk: 🟢 Low

- All projects already SDK-style
- No project file transformation needed
- Standard MSBuild integration

### Security Vulnerabilities

**None identified** - No packages with security vulnerabilities.

### Contingency Plans

#### If Compilation Errors Exceed Expectations

**Scenario**: More breaking changes than anticipated in Windows Forms or System.Drawing APIs

**Contingency**:
1. Catalog all compilation errors by category (API changes, namespace changes, behavior changes)
2. Address in dependency order: MixLib → MixAssembler → MixEmul
3. Consult .NET 10 breaking changes documentation: https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0
4. For Windows Forms changes: https://learn.microsoft.com/en-us/dotnet/core/compatibility/windows-forms

**Rollback**: Revert to main branch (all changes on `upgrade-to-NET10` branch)

#### If Legacy Controls Require Replacement

**Scenario**: Project uses removed legacy controls (StatusBar, ContextMenu, MainMenu, MenuItem, ToolBar)

**Contingency**:
1. Identify all usages via compilation errors
2. Create replacement mapping:
   - StatusBar → StatusStrip
   - ContextMenu → ContextMenuStrip  
   - MainMenu/MenuItem → MenuStrip
   - ToolBar → ToolStrip
   - DataGrid → DataGridView
3. Update designer files and code-behind
4. Verify UI functionality matches original behavior

**Estimated Impact**: Low (likely zero usages given modern codebase on .NET 8)

#### If Performance Degrades

**Scenario**: .NET 10 runtime or Windows Forms changes cause performance regression

**Contingency**:
1. Profile application to identify regression areas
2. Consult .NET 10 performance changes documentation
3. Apply targeted optimizations or workarounds
4. If critical: Defer upgrade and report issue to Microsoft

**Likelihood**: Very low (. NET 10 generally improves performance)

#### If Testing Reveals Functional Regressions

**Scenario**: Application builds but behavior changes in .NET 10

**Contingency**:
1. Document specific regression scenarios
2. Check .NET 10 behavioral changes documentation
3. Implement compensating code changes
4. If unresolvable: Rollback and reassess upgrade timeline

**Mitigation**: Comprehensive testing strategy (see Testing & Validation section)

### Rollback Strategy

**Simple Rollback** (before merge to main):
```bash
git checkout main
git branch -D upgrade-to-NET10
```

**If Already Merged** (post-deployment issues):
```bash
git revert <merge-commit-sha>
# Or full rollback:
git reset --hard <pre-upgrade-commit-sha>
git push --force-with-lease
```

**Pre-Merge Validation**: Do not merge `upgrade-to-NET10` branch until all success criteria met (see Success Criteria section).

---

## Testing & Validation Strategy

### Multi-Level Testing Approach

Given the All-At-Once strategy, testing occurs in a unified phase after all projects are upgraded simultaneously.

---

### Level 1: Per-Project Build Validation

Execute immediately after framework updates applied to all projects.

#### MixLib.csproj - Build Validation
```bash
dotnet build src\MixLib\MixLib.csproj --configuration Release
```

**Validation Criteria**:
- [ ] Build succeeds with exit code 0
- [ ] Zero errors
- [ ] Zero warnings
- [ ] Output assembly generated: `bin\Release\net10.0-windows7.0\MixLib.dll`

#### MixAssembler.csproj - Build Validation
```bash
dotnet build src\MixAssembler\MixAssembler.csproj --configuration Release
```

**Validation Criteria**:
- [ ] Build succeeds with exit code 0
- [ ] Zero errors
- [ ] Zero warnings
- [ ] Output assembly generated: `bin\Release\net10.0-windows7.0\MixAssembler.dll`
- [ ] Successfully references upgraded MixLib

#### MixEmul.csproj - Build Validation
```bash
dotnet build src\MixEmul\MixEmul.csproj --configuration Release
```

**Validation Criteria**:
- [ ] Build succeeds with exit code 0
- [ ] Zero errors
- [ ] Zero warnings (or only acceptable warnings documented)
- [ ] Output executable generated: `bin\Release\net10.0-windows\MixEmul.exe`
- [ ] Successfully references upgraded MixLib and MixAssembler

---

### Level 2: Solution-Wide Build Validation

Execute after individual project builds succeed.

```bash
dotnet build MixEmul.sln --configuration Release
```

**Validation Criteria**:
- [ ] Solution build succeeds
- [ ] All 3 projects build in correct dependency order
- [ ] Zero errors across entire solution
- [ ] Zero warnings across entire solution
- [ ] No dependency resolution conflicts
- [ ] All output assemblies generated

**Dependency Order Verification**:
MSBuild should automatically build in this order:
1. MixLib.csproj (no dependencies)
2. MixAssembler.csproj (depends on MixLib)
3. MixEmul.csproj (depends on both)

---

### Level 3: Unit Testing (If Tests Exist)

Execute after solution build succeeds.

```bash
# Discover test projects
dotnet sln MixEmul.sln list

# Run all tests in solution
dotnet test MixEmul.sln --configuration Release --no-build
```

**If MixLib has tests**:
```bash
dotnet test src\MixLib\MixLib.csproj --configuration Release --no-build
```

**If MixAssembler has tests**:
```bash
dotnet test src\MixAssembler\MixAssembler.csproj --configuration Release --no-build
```

**If MixEmul has tests**:
```bash
dotnet test src\MixEmul\MixEmul.csproj --configuration Release --no-build
```

**Validation Criteria**:
- [ ] All tests discovered successfully
- [ ] All tests execute under .NET 10 runtime
- [ ] 100% tests pass
- [ ] Zero test failures
- [ ] Zero test errors
- [ ] No performance regressions in test execution time

---

### Level 4: Application Runtime Validation

Execute after all builds and tests pass.

#### 4.1 Application Launch Testing

```bash
# Run application
dotnet run --project src\MixEmul\MixEmul.csproj --configuration Release
```

**Validation Criteria**:
- [ ] Application launches without errors
- [ ] No runtime assembly loading failures
- [ ] Main window appears correctly
- [ ] Application doesn't crash on startup
- [ ] No unhandled exceptions in event log

#### 4.2 Smoke Testing

Quick validation of core functionality:

**Critical Path Tests**:
1. **Application Lifecycle**
   - [ ] Application starts
   - [ ] Main form loads and displays
   - [ ] Application closes cleanly
   - [ ] No errors during shutdown

2. **Core UI Elements**
   - [ ] Menus render and respond
   - [ ] Toolbars display correctly
   - [ ] Status bar updates (if present)
   - [ ] All controls visible and positioned correctly

3. **Basic Functionality**
   - [ ] File operations (Open/Save if applicable)
   - [ ] Core emulation features execute
   - [ ] Assembler integration works
   - [ ] Settings/preferences load correctly

#### 4.3 Comprehensive Functional Testing

**Windows Forms Controls Validation**:
- [ ] **Buttons**: Click events fire, visual feedback works
- [ ] **Labels**: Text displays correctly, font rendering accurate
- [ ] **TextBoxes**: Accept input, text retrieval works, events fire
- [ ] **CheckBoxes**: State toggles, checked/unchecked events work
- [ ] **ComboBoxes**: Dropdown displays, selection works, events fire
- [ ] **ListViews**: Items display, selection works, columns render
- [ ] **GroupBoxes/Panels**: Contain controls correctly, layout maintained
- [ ] **NumericUpDown**: Value changes work, min/max respected
- [ ] **RichTextBox**: Formatted text displays, editing works
- [ ] **TabControl**: Tab switching works, tab pages display
- [ ] **ToolStrip**: Buttons display and respond, layout correct
- [ ] **MenuStrip**: Menus open, menu items execute, shortcuts work
- [ ] **ContextMenuStrip**: Right-click menus appear and function

**Visual Validation**:
- [ ] **Fonts**: All text renders with correct fonts (System.Drawing.Font)
- [ ] **Layout**: AnchorStyles maintain correct positioning on resize
- [ ] **Alignment**: ContentAlignment properties position elements correctly
- [ ] **Margins/Padding**: Spacing between elements correct
- [ ] **Docking**: Docked controls behave correctly
- [ ] **Images**: Icons and images display properly (if used)

**Emulator-Specific Functionality** (adapt to actual MixEmul features):
- [ ] **Load Assembly Code**: File loading works
- [ ] **Execute Instructions**: Core emulation runs
- [ ] **Debug Features**: Breakpoints, stepping, inspection work
- [ ] **Memory Display**: Memory viewer shows correct data
- [ ] **Registers**: Register display updates correctly
- [ ] **Output**: Program output displays correctly

#### 4.4 Performance Validation

**Startup Performance**:
- [ ] Application startup time comparable to .NET 8 version (±10%)
- [ ] No significant delays during initialization

**Runtime Performance**:
- [ ] UI responsiveness maintained (no lag on button clicks)
- [ ] Emulation execution speed comparable to .NET 8
- [ ] No unexpected CPU spikes
- [ ] Memory usage comparable to .NET 8 (±15%)

**Measurement** (optional but recommended):
```bash
# Compare startup times
Measure-Command { & "src\MixEmul\bin\Release\net10.0-windows\MixEmul.exe"; Start-Sleep -Seconds 2; Stop-Process -Name MixEmul }
```

#### 4.5 Regression Testing

**Compare .NET 10 vs .NET 8 Behavior**:
- [ ] All user workflows produce identical results
- [ ] File formats remain compatible (saved files load correctly)
- [ ] Configuration/settings migrate correctly
- [ ] No visual changes (unless intentional .NET 10 improvements)
- [ ] No behavioral changes in core functionality

---

### Level 5: Error Handling & Edge Case Testing

**Exception Handling**:
- [ ] Application handles errors gracefully (no crashes)
- [ ] Error dialogs display correctly
- [ ] Logging/diagnostics work correctly

**Edge Cases**:
- [ ] Large files load correctly
- [ ] Long-running operations don't freeze UI
- [ ] Rapid user input handled correctly
- [ ] Window resizing doesn't cause layout issues

---

### Testing Execution Order

**Sequence**:
1. ✅ Per-Project Build Validation (Level 1)
2. ✅ Solution-Wide Build Validation (Level 2)
3. ✅ Unit Testing (Level 3) - if tests exist
4. ✅ Application Launch Testing (Level 4.1)
5. ✅ Smoke Testing (Level 4.2)
6. ✅ Comprehensive Functional Testing (Level 4.3)
7. ✅ Performance Validation (Level 4.4)
8. ✅ Regression Testing (Level 4.5)
9. ✅ Error Handling & Edge Cases (Level 5)

**Stop Criteria**: If any level fails, stop and fix issues before proceeding to next level.

---

### Test Environment

**Platform**: Windows (Windows Forms requires Windows)
**Configuration**: Release build (closest to production)
**.NET Runtime**: .NET 10.0 runtime
**Comparison Baseline**: Current .NET 8 version for regression testing

---

### Test Reporting

**Document All Issues**:
- Compilation errors (should be zero or minimal)
- Runtime errors or exceptions
- Visual regressions (layout, fonts, rendering)
- Functional regressions (behavior changes)
- Performance regressions (timing, memory)

**Issue Template**:
```
Issue: [Brief description]
Severity: [Critical/High/Medium/Low]
Category: [Build/Runtime/Visual/Functional/Performance]
Steps to Reproduce: [Steps]
Expected Behavior: [.NET 8 behavior]
Actual Behavior: [.NET 10 behavior]
Proposed Fix: [If known]
```

---

### Acceptance Criteria (All Levels Must Pass)

- ✅ All builds succeed with zero errors
- ✅ All builds complete with zero warnings (or documented acceptable warnings)
- ✅ All unit tests pass (if tests exist)
- ✅ Application launches and runs without errors
- ✅ All UI controls function correctly
- ✅ All core features operational
- ✅ No visual regressions
- ✅ No functional regressions
- ✅ No performance regressions (>20% degradation)
- ✅ All edge cases handled correctly

---

## Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | Dependencies | Risk | Rationale |
|---------|------------|--------------|------|-----------|
| MixLib.csproj | 🟢 Low | 0 projects, 0 packages | Low | Simple framework update; no API issues; no dependencies |
| MixAssembler.csproj | 🟢 Low | 1 project, 0 packages | Low | Simple framework update; no API issues; depends only on low-risk MixLib |
| MixEmul.csproj | 🟡 Medium | 2 projects, 0 packages | Medium | Framework update + high API flag count (non-breaking); UI testing required |

### Phase Complexity Assessment

**Single Atomic Phase**: All projects upgraded simultaneously

**Complexity Factors**:
- ✅ **Framework Updates**: Simple (3 TargetFramework property changes)
- ✅ **Package Updates**: None required
- ⚠️ **API Compatibility**: High flag count, low actual impact expected
- ✅ **Build System**: No changes (already SDK-style)
- ⚠️ **Testing**: Comprehensive UI testing needed for Windows Forms application

**Overall Phase Complexity**: 🟡 **Medium**
- Simple structure and dependencies
- Moderate due to Windows Forms API verification needs
- No package or security complexity

### Relative Complexity by Dependency Order

**Validation Order** (follows dependency chain):

1. **MixLib.csproj** - 🟢 **Low** (Baseline validation)
   - Simplest project (no dependencies, no API issues)
   - First validation checkpoint
   - Success criteria: Clean build, no warnings

2. **MixAssembler.csproj** - 🟢 **Low** (Dependent validation)
   - Depends on validated MixLib
   - No API issues
   - Success criteria: Clean build, no warnings, works with upgraded MixLib

3. **MixEmul.csproj** - 🟡 **Medium** (Comprehensive validation)
   - Depends on both validated libraries
   - Windows Forms UI requires functional testing
   - High API flag count requires verification (expect minimal changes)
   - Success criteria: Clean build, no warnings, full UI functionality

### Resource Requirements

**Skill Levels Required**:
- **.NET Migration Experience**: Moderate (familiar with framework upgrades)
- **Windows Forms Knowledge**: Required (for UI testing and any potential fixes)
- **MSBuild/Project System**: Basic (standard SDK-style project editing)
- **Git/Source Control**: Basic (branch management, commits)

**Parallel Execution Capacity**:
- **Project File Editing**: Can parallelize (3 independent file edits)
- **Code Analysis**: Can parallelize (analyze all projects simultaneously)
- **Build & Validation**: Sequential (respects dependency order automatically)
- **Testing**: Sequential (validate MixLib → MixAssembler → MixEmul)

**Recommended Team Size**: 1-2 developers
- Single developer sufficient given All-At-Once atomic approach
- Second developer optional for accelerated testing or code review

---

## Source Control Strategy

### Branching Strategy

**Main Branch**: `main` (production baseline, .NET 8)
**Upgrade Branch**: `upgrade-to-NET10` (currently active, migration work)
**Merge Target**: `main` (after all success criteria met)

#### Branch Workflow

```
main (net8.0)
  │
  └─── upgrade-to-NET10 (net10.0) ← All upgrade work happens here
         │
         └─── (After validation success) → Merge back to main
```

**Branch Protection**:
- `upgrade-to-NET10` branch already created and checked out ✅
- All migration changes isolated from main branch
- Main branch remains stable during upgrade process
- Rollback is simple: `git checkout main`

---

### Commit Strategy

#### All-At-Once Single Commit Approach (Recommended)

Given the All-At-Once strategy, **prefer a single comprehensive commit** for the entire upgrade:

**Rationale**:
- Atomic upgrade operation logically maps to atomic commit
- Simplifies rollback (single commit revert)
- Clean Git history without intermediate broken states
- All changes are interdependent (projects won't build individually with mixed frameworks)

**Recommended Commit Structure**:

```bash
# After all changes complete and validated
git add .
git commit -m "Upgrade solution to .NET 10.0

- Update all projects from net8.0-windows7.0 to net10.0
- MixLib.csproj: net8.0-windows7.0 → net10.0-windows7.0
- MixAssembler.csproj: net8.0-windows7.0 → net10.0-windows7.0
- MixEmul.csproj: net8.0-windows7.0 → net10.0-windows

All builds pass, all tests pass, full UI validation complete.

Fixes: [List any code changes if needed]
Breaking Changes: None
API Issues: 13k+ Windows Forms flags resolved via recompilation"
```

#### Alternative: Multi-Commit Approach

If preferred, break into logical commits (all on `upgrade-to-NET10` branch):

**Commit 1: Project File Updates**
```bash
git add src/MixLib/MixLib.csproj src/MixAssembler/MixAssembler.csproj src/MixEmul/MixEmul.csproj
git commit -m "Update TargetFramework to .NET 10.0 for all projects"
```

**Commit 2: Code Changes** (if any compilation fixes needed)
```bash
git add src/
git commit -m "Fix compilation errors from .NET 10 upgrade

- [Describe specific changes]
- [List files modified]"
```

**Commit 3: Configuration Updates** (if any)
```bash
git add *.config *.json
git commit -m "Update configuration files for .NET 10 compatibility"
```

**Note**: Multi-commit approach is less recommended for All-At-Once strategy since intermediate commits may not build successfully.

---

### Commit Message Format

**Template**:
```
<type>: <short summary>

<detailed description>

[optional] Breaking Changes: <list>
[optional] Fixes: <issue references>
```

**Example**:
```
chore: Upgrade solution to .NET 10.0

- All projects upgraded from net8.0-windows7.0 to net10.0
- Zero code changes required (Windows Forms binary incompatibilities resolved via recompilation)
- All builds pass with zero warnings
- All tests pass
- Full UI functional validation complete

Breaking Changes: None
Tested-on: Windows 11, .NET 10.0.0 SDK
```

---

### Review and Merge Process

#### Pre-Merge Checklist

**Before creating Pull Request**:
- [ ] All commits pushed to `upgrade-to-NET10` branch
- [ ] All success criteria met (see Success Criteria section)
- [ ] All builds pass
- [ ] All tests pass
- [ ] Full functional validation complete
- [ ] No warnings in build output
- [ ] Performance validated
- [ ] Documentation updated (README, if needed)

#### Pull Request Process

**Create PR**:
```bash
# Push upgrade branch
git push origin upgrade-to-NET10

# Create PR via GitHub/Azure DevOps/GitLab UI
```

**PR Title**: "Upgrade solution to .NET 10.0"

**PR Description Template**:
```markdown
## Overview
Upgrades MixEmul solution from .NET 8.0 to .NET 10.0 (LTS).

## Changes
- **MixLib.csproj**: net8.0-windows7.0 → net10.0-windows7.0
- **MixAssembler.csproj**: net8.0-windows7.0 → net10.0-windows7.0
- **MixEmul.csproj**: net8.0-windows7.0 → net10.0-windows

## Code Changes
- [List any code modifications, or state "None required"]

## Testing
- ✅ All builds pass (zero errors, zero warnings)
- ✅ All unit tests pass
- ✅ Full UI functional testing complete
- ✅ Performance validated (no regressions)
- ✅ Regression testing complete

## API Compatibility
- 13,035 API issues flagged by assessment (Windows Forms binary incompatibilities)
- **Actual breaking changes**: [Number, likely zero]
- All resolved via recompilation

## Breaking Changes
[List any, or state "None"]

## Rollback Plan
Revert this PR or checkout `main` branch.

## Checklist
- [ ] Builds without errors
- [ ] Builds without warnings
- [ ] Tests pass
- [ ] Functionality validated
- [ ] Performance acceptable
- [ ] Documentation updated
```

#### PR Review Checklist

**Reviewers Should Verify**:
- [ ] All project files show correct TargetFramework updates
- [ ] No unintended changes (check diff carefully)
- [ ] Commit messages clear and descriptive
- [ ] All CI/CD builds pass (if automated)
- [ ] Test results attached or referenced
- [ ] No leftover debugging code or commented sections
- [ ] Code changes (if any) are minimal and justified

#### Merge Criteria

**Merge to `main` only when**:
1. ✅ All technical success criteria met (see Success Criteria section)
2. ✅ PR approved by required reviewers
3. ✅ All CI/CD checks pass
4. ✅ No unresolved review comments
5. ✅ Documentation updated
6. ✅ Release notes prepared (if applicable)

**Merge Method**: Squash merge or merge commit (team preference)

```bash
# Squash merge (recommended for All-At-Once)
git checkout main
git merge --squash upgrade-to-NET10
git commit -m "Upgrade solution to .NET 10.0"

# Or standard merge
git checkout main
git merge upgrade-to-NET10 --no-ff -m "Merge .NET 10 upgrade"
```

---

### Post-Merge Activities

**After Merge to Main**:
1. Tag the release:
   ```bash
   git tag -a v2.0-net10 -m "Version 2.0 - .NET 10.0 upgrade"
   git push origin v2.0-net10
   ```

2. Delete upgrade branch (optional):
   ```bash
   git branch -d upgrade-to-NET10
   git push origin --delete upgrade-to-NET10
   ```

3. Update CI/CD pipelines to target .NET 10 SDK

4. Notify team of upgrade completion

5. Deploy to test/staging environment

6. Monitor for issues in production

---

### Rollback Procedure

#### Pre-Merge Rollback (Simple)

If issues discovered before merging to `main`:

```bash
# Option 1: Abandon upgrade branch
git checkout main
git branch -D upgrade-to-NET10

# Option 2: Reset upgrade branch
git checkout upgrade-to-NET10
git reset --hard main
```

#### Post-Merge Rollback (If Deployed)

If critical issues discovered after merge:

```bash
# Option 1: Revert merge commit
git checkout main
git revert -m 1 <merge-commit-sha>
git push origin main

# Option 2: Hard reset (use with caution)
git checkout main
git reset --hard <pre-upgrade-commit-sha>
git push origin main --force-with-lease
```

**Deployment Rollback**:
- Redeploy previous .NET 8 version
- Restore .NET 8 runtime environment
- Verify application functionality

---

### Branch Housekeeping

**Keep Clean History**:
- Don't commit IDE-generated files (.vs/, bin/, obj/)
- Ensure `.gitignore` excludes build artifacts
- Avoid committing temporary or experimental changes
- Use meaningful commit messages

**Protect Against Accidents**:
- Never force-push to `main`
- Always work on `upgrade-to-NET10` branch during migration
- Keep local and remote branches synchronized

---

## Success Criteria

The .NET 10.0 upgrade is considered successful when ALL criteria below are met.

---

### Technical Criteria (Mandatory)

#### All Projects Migrated
- [x] MixLib.csproj TargetFramework = `net10.0-windows7.0`
- [x] MixAssembler.csproj TargetFramework = `net10.0-windows7.0`
- [x] MixEmul.csproj TargetFramework = `net10.0-windows`
- [ ] All projects build successfully on .NET 10 SDK
- [ ] No projects remain on .NET 8 or earlier

#### Package Updates Applied
- [x] **N/A** - No NuGet packages in solution (zero updates required)

#### Build Success
- [ ] **MixLib.csproj** builds without errors: `dotnet build src\MixLib\MixLib.csproj`
- [ ] **MixAssembler.csproj** builds without errors: `dotnet build src\MixAssembler\MixAssembler.csproj`
- [ ] **MixEmul.csproj** builds without errors: `dotnet build src\MixEmul\MixEmul.csproj`
- [ ] **Solution** builds without errors: `dotnet build MixEmul.sln`
- [ ] Exit code 0 for all build commands

#### Zero Warnings
- [ ] **MixLib.csproj**: Zero build warnings
- [ ] **MixAssembler.csproj**: Zero build warnings
- [ ] **MixEmul.csproj**: Zero build warnings
- [ ] **Solution**: Zero warnings across entire build

**Acceptable Exceptions** (document if present):
- [ ] [List any acceptable warnings with justification, or mark N/A]

#### Test Success
- [ ] All unit tests pass (if tests exist): `dotnet test MixEmul.sln`
- [ ] 100% test pass rate
- [ ] Zero test failures
- [ ] Zero test errors
- [ ] [x] **N/A if no tests exist** - Document test coverage status

#### No Vulnerabilities
- [x] Zero security vulnerabilities (confirmed by assessment)
- [ ] No new vulnerabilities introduced
- [ ] `dotnet list package --vulnerable` returns no results

#### Runtime Success
- [ ] **MixEmul.exe** launches without errors
- [ ] No assembly loading failures
- [ ] No unhandled exceptions during startup
- [ ] Application runs on .NET 10 runtime

---

### Quality Criteria (Mandatory)

#### Code Quality Maintained
- [ ] No code smells introduced during upgrade
- [ ] Code readability maintained
- [ ] No commented-out code left in codebase
- [ ] All breaking changes properly addressed (not bypassed with hacks)

#### Test Coverage Maintained
- [ ] Unit test count unchanged (or increased)
- [ ] Test coverage percentage maintained or improved
- [ ] All existing tests still relevant and executing
- [ ] [x] **N/A if no existing tests** - Document as technical debt

#### Documentation Updated
- [ ] README.md updated to reflect .NET 10 requirement
- [ ] Build instructions updated (if SDK version specified)
- [ ] Deployment documentation updated (if runtime version specified)
- [ ] CHANGELOG.md or release notes document the upgrade
- [ ] Any .NET 8-specific documentation removed or updated

#### No Regressions
- [ ] **Functional**: All features work identically to .NET 8 version
- [ ] **Visual**: No UI rendering differences (unless intentional improvements)
- [ ] **Performance**: No degradation >20% in startup time, runtime performance, or memory usage
- [ ] **Behavioral**: No unexpected behavior changes

---

### Process Criteria (Mandatory)

#### All-At-Once Strategy Followed
- [x] All projects upgraded simultaneously (not incrementally)
- [ ] Single atomic upgrade operation completed
- [ ] No intermediate states with mixed framework versions
- [ ] Dependency order respected during validation

#### Source Control Strategy Followed
- [x] All work done on `upgrade-to-NET10` branch
- [ ] Commits follow agreed format (single commit or logical multi-commit)
- [ ] Commit messages descriptive and accurate
- [ ] No force-pushes to main branch
- [ ] Branch ready for PR and merge

#### All-At-Once Strategy Principles Applied
- [ ] Project file updates applied simultaneously
- [ ] Package updates applied simultaneously (N/A - no packages)
- [ ] Build and validation performed on entire solution
- [ ] No phased or incremental migration approach used

#### Validation Complete
- [ ] All levels of testing complete (Build → Unit → Integration → Functional)
- [ ] Smoke testing passed
- [ ] Regression testing passed
- [ ] Performance validation passed
- [ ] Edge case testing passed

---

### API Compatibility Resolution

#### Windows Forms Binary Incompatibilities (12,661 flagged)
- [ ] Verified as non-breaking via clean compilation
- [ ] **OR** All actual breaking changes identified and fixed
- [ ] No runtime API exceptions related to Windows Forms

#### System.Drawing Source Incompatibilities (374 flagged)
- [ ] Verified as non-breaking via clean compilation
- [ ] **OR** All actual incompatibilities identified and fixed
- [ ] No compilation errors related to System.Drawing types

#### Legacy Controls (7 flagged)
- [ ] Verified not in use (clean build)
- [ ] **OR** Replacements implemented (StatusBar→StatusStrip, ContextMenu→ContextMenuStrip, etc.)
- [ ] No compilation errors related to removed legacy controls

---

### Deployment Readiness (Pre-Production)

#### Environment Validated
- [ ] .NET 10 runtime installed on target deployment environment
- [ ] Application tested on target OS version (Windows)
- [ ] Dependencies verified on deployment platform
- [ ] Deployment scripts updated for .NET 10

#### Performance Baseline Established
- [ ] Startup time measured and acceptable
- [ ] Memory usage measured and acceptable
- [ ] CPU usage measured and acceptable
- [ ] Comparison to .NET 8 baseline documented

#### Rollback Plan Ready
- [ ] Rollback procedure documented (see Source Control Strategy)
- [ ] Previous .NET 8 version tagged and accessible
- [ ] Rollback tested (if critical application)

---

### Final Acceptance Checklist

**Before declaring upgrade complete, verify ALL items checked**:

#### Build & Compilation
- [ ] ✅ All projects target .NET 10
- [ ] ✅ Solution builds without errors
- [ ] ✅ Solution builds without warnings
- [ ] ✅ All dependencies resolved correctly

#### Testing & Validation
- [ ] ✅ All automated tests pass
- [ ] ✅ Application launches successfully
- [ ] ✅ Full UI functional testing complete
- [ ] ✅ No visual regressions
- [ ] ✅ No functional regressions
- [ ] ✅ No performance regressions

#### Code Quality
- [ ] ✅ Code quality maintained
- [ ] ✅ No security vulnerabilities
- [ ] ✅ Documentation updated

#### Process Compliance
- [ ] ✅ All-At-Once strategy followed
- [ ] ✅ Source control strategy followed
- [ ] ✅ Testing strategy executed
- [ ] ✅ Success criteria reviewed

#### Deployment Preparation
- [ ] ✅ Environment validated
- [ ] ✅ Performance baselined
- [ ] ✅ Rollback plan ready

---

### Definition of Done

**The .NET 10.0 upgrade is DONE when**:

1. ✅ All 3 projects successfully targeting .NET 10 with appropriate platform versions
2. ✅ Solution builds cleanly (zero errors, zero warnings)
3. ✅ All tests pass (or N/A documented)
4. ✅ MixEmul application launches and runs without errors
5. ✅ Full UI functional validation complete with no regressions
6. ✅ Performance validated (no >20% degradation)
7. ✅ No security vulnerabilities present
8. ✅ Code quality maintained
9. ✅ Documentation updated
10. ✅ All commits on `upgrade-to-NET10` branch
11. ✅ PR created and approved (if using PR workflow)
12. ✅ **Ready to merge to `main` branch**

**Only after ALL criteria met**: Merge to main, tag release, deploy to staging/production.

---

### Post-Upgrade Monitoring

**After deployment to production** (outside scope of this plan, but important):
- Monitor application logs for .NET 10-related errors
- Track performance metrics vs. baseline
- Gather user feedback on functionality
- Watch for memory leaks or resource issues
- Be prepared to execute rollback if critical issues discovered

**Success Duration**: Upgrade considered fully successful after 30 days in production with no critical .NET 10-related issues.
