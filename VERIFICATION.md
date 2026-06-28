# Lazarus - Unity Package Verification

## Installation

To install this package in a Unity project:

1. Add the package to your project's `Packages/manifest.json`:
   ```json
   {
     "dependencies": {
       "com.renanliberato.lazarus": "file:/path/to/lazarus"
     }
   }
   ```

2. Or use the Unity Package Manager window:
   - Window > Package Manager
   - Click the "+" icon
   - Select "Add package from disk..."
   - Navigate to the `lazarus` directory

## Running Tests

### Edit-Mode Tests (Test Runner)

1. Open Unity Test Runner:
   - Window > General > Test Runner

2. Select "Edit Mode" tab

3. Click "Run All" to execute all edit-mode tests

 4. Expected results:
    - All tests in `Lazarus.Tests.PackageStructureTests` should pass
    - All tests in `Lazarus.Tests.DismissalMarkerSmokeTest` should pass
    - All tests in `Lazarus.Tests.DefaultViewFilterTests` should pass
    - All tests in `Lazarus.Tests.DefaultDismissalPolicyTests` should pass
    - All tests in `Lazarus.Tests.CustomFilterTests` should pass
    - All tests in `Lazarus.Tests.UIViewWatcherTests` should pass

### Command Line Tests

If you have Unity's test runner command line tools available:

```bash
# Run edit-mode tests
/Applications/Unity/Hub/Editor/2021.3.x/Unity.app/Contents/MacOS/Unity \
  -runTests \
  -batchmode \
  -projectPath /path/to/your/unity/project \
  -testResults /path/to/results.xml \
  -testPlatform EditMode \
  -assemblyNames com.renanliberato.lazarus.Tests
```

## Verification Checklist

### Package Structure
- [x] `package.json` exists at package root with valid JSON
- [x] `package.json` contains required fields: `name`, `version`, `displayName`
- [x] Runtime assembly definition exists: `Runtime/com.renanliberato.lazarus.asmdef`
- [x] Test assembly definition exists: `Tests/Editor/com.renanliberato.lazarus.Tests.asmdef`

### Assembly Configuration
- [x] Runtime assembly references no other assemblies (standalone)
- [x] Test assembly references runtime assembly
- [x] Test assembly is configured for Editor-only execution
- [x] Test assembly includes NUnit framework reference

### Public API
- [x] `Lazarus.IDismissalMarker` interface exists
- [x] `Lazarus.DismissalMarker` MonoBehaviour implements `IDismissalMarker`
- [x] Public API methods: `MarkDismissalPlanned()`, `IsDismissalPlanned()`
- [x] `Lazarus.IViewFilter` interface for custom filtering logic
- [x] `Lazarus.DefaultViewFilter` implements `IViewFilter` with default behavior
- [x] `Lazarus.IDismissalPolicy` interface for dismissal validation
- [x] `Lazarus.DefaultDismissalPolicy` implements `IDismissalPolicy` with default behavior
- [x] `Lazarus.DismissalIntent` enum (Scale, Alpha, Both, Abrupt)
- [x] `Lazarus.DismissalValidationResult` struct (IsValid, RejectionReason)
- [x] `Lazarus.UIViewWatcher` MonoBehaviour for lifecycle monitoring
- [x] `Lazarus.UIViewWatcher.DiscoveryInterval` property (configurable throttling)
- [x] `Lazarus.UIViewWatcher.Tick()` method (explicit discovery control)

### Test Coverage
- [x] Structural tests verify package.json and assembly definitions
- [x] Smoke test creates GameObject with DismissalMarker
- [x] Smoke test verifies public interface implementation
- [x] Smoke test tests behavior: MarkDismissalPlanned updates state
- [x] Smoke test tests interface: IDismissalMarker provides IsDismissalPlanned
- [x] Default view filter tests: accepts/rejects based on name and components
- [x] Default dismissal policy tests: validates Canvas/CanvasGroup dismissal rules
- [x] Custom filter/policy tests: verifies extensibility through public interfaces
- [x] UIView watcher tests: persistence across scene loads
- [x] UIView watcher tests: discovery interval configuration and limits
- [x] UIView watcher tests: explicit Tick() method for controlled discovery
- [x] UIView watcher tests: discovery of existing and newly created UI GameObjects
- [x] UIView watcher tests: dismissal evidence acceptance
- [x] UIView watcher tests: abrupt deactivation detection
- [x] UIView watcher tests: public interface safety without casting

### Non-Unity Validation
- [x] package.json is valid JSON
- [x] Assembly definition files are valid JSON
- [x] All C# files compile without syntax errors

## Expected Test Output

When tests pass, you should see:
- 3 structural tests passing (PackageStructureTests)
- 4 behavior tests passing (DismissalMarkerSmokeTest)
- 6 default view filter tests passing (DefaultViewFilterTests)
- 6 default dismissal policy tests passing (DefaultDismissalPolicyTests)
- 2 custom implementation tests passing (CustomFilterTests)
- 11 UIView watcher tests passing (UIViewWatcherTests)
- Total: 32 tests passing

## Known Limitations

This is the complete implementation for issues #2, #3, and #4.

## Issue #3 Completion Status

**Implemented (via strict TDD):**
- ✅ Public `IViewFilter` interface with `bool ShouldTrack(GameObject gameObject)` method
- ✅ `DefaultViewFilter` implementation matching "*View" suffix with Canvas/CanvasGroup components
- ✅ Public `IDismissalPolicy` interface with `DismissalValidationResult ValidateDismissal(GameObject, DismissalIntent)` method
- ✅ `DefaultDismissalPolicy` implementation with Canvas scale-only and CanvasGroup alpha/scale rules
- ✅ `DismissalIntent` enum (Scale, Alpha, Both, Abrupt)
- ✅ `DismissalValidationResult` struct with `IsValid` and `RejectionReason` fields
- ✅ All behaviors covered test-first through public APIs (20 total tests)
- ✅ Custom filtering/policy interfaces verified extensible

## Issue #4 Completion Status

**Implemented (via strict TDD with hardened implementation):**
- ✅ `UIViewWatcher` MonoBehaviour that persists across scene loads with DontDestroyOnLoad
- ✅ Watcher discovers existing UI GameObjects at startup using default filter
- ✅ Watcher discovers newly created UI GameObjects during runtime with **throttled discovery** (configurable interval, default 0.5s)
- ✅ **Performance optimization**: Does NOT call FindObjectsOfType every Update; uses throttled interval-based discovery
- ✅ **Explicit Tick() method** for controlled discovery without relying on Update loop
- ✅ Watcher records/accepts dismiss evidence through `IDismissalMarker` before deactivation
- ✅ **Interface safety**: Extended `IDismissalMarker` to include `IsDismissalPlanned()` method, eliminating unsafe casting
- ✅ Watcher emits diagnostics containing object context and rejection reason for abrupt deactivation
- ✅ Lifecycle behavior covered test-first through public APIs (11 total tests)
- ✅ Public API: `UIViewWatcher` component can be attached to GameObjects in boot scenes
- ✅ Public API: `DiscoveryInterval` property for configurable throttling (minimum 0.1s)
- ✅ Public API: `Tick()` method for explicit discovery control
- ✅ **Strengthened tests**: Assert diagnostics behavior, observable state changes, and interface safety

**Performance Characteristics:**
- Discovery occurs at configurable intervals (default 0.5s, minimum 0.1s)
- No unbounded full hierarchy scanning every frame
- Explicit Tick() method available for manual control when needed
- Initial discovery at startup ensures existing views are tracked immediately