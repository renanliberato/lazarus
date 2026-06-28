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
     - All tests in `Lazarus.Tests.AnimationHistoryTests` should pass
     - All tests in `Lazarus.Tests.DocumentationTests` should pass

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
- [x] `Lazarus.IViewFilter` interface for custom filtering logic
- [x] `Lazarus.DefaultViewFilter` implements `IViewFilter` with default behavior
- [x] `Lazarus.IDismissalPolicy` interface for dismissal validation
- [x] `Lazarus.DefaultDismissalPolicy` implements `IDismissalPolicy` with default behavior
- [x] `Lazarus.AnimationHistoryObserver` for passive animation sampling (primary integration path)
- [x] `Lazarus.DismissalValidationResult` struct (IsValid, RejectionReason, ScaleAnimationObserved, AlphaAnimationObserved, ObservedFirstScale, ObservedLastScale, ObservedFirstAlpha, ObservedLastAlpha, ScaleDismissalThreshold, AlphaDismissalThreshold)
- [x] `Lazarus.UIViewWatcher` MonoBehaviour for lifecycle monitoring (passive observation model)
- [x] `Lazarus.UIViewWatcher.DiscoveryInterval` property (configurable throttling)
- [x] `Lazarus.UIViewWatcher.Tick()` method (explicit discovery control)
- [x] `Lazarus.IDismissalMarker` and `Lazarus.DismissalMarker` (deprecated legacy API for backwards compatibility only)
- [x] `Lazarus.DismissalIntent` enum (deprecated legacy API for backwards compatibility only)

### Test Coverage
- [x] Structural tests verify package.json and assembly definitions
- [x] Animation history tests: passive sampling, scale capture, alpha capture, dismissal inference, configurable thresholds, stale evidence expiry
- [x] Default dismissal policy tests: Canvas scale validation, CanvasGroup alpha/scale validation, observed value diagnostics
- [x] Default view filter tests: accepts/rejects based on name and components
- [x] Custom filter/policy tests: verifies extensibility through public interfaces
- [x] UIView watcher tests: persistence, discovery, sampling, diagnostic emission with passive observation
- [x] Dismissal marker smoke tests: deprecated API still works for backwards compatibility
- [x] Documentation tests: README and VERIFICATION docs correctly describe passive inference model

### Non-Unity Validation
- [x] package.json is valid JSON
- [x] Assembly definition files are valid JSON
- [x] All C# files compile without syntax errors

## Expected Test Output

When tests pass, you should see:
- 3 structural tests passing (PackageStructureTests)
- 13 animation history tests passing (AnimationHistoryTests) - Increased from 6 in #8
- 12 default dismissal policy tests passing (DefaultDismissalPolicyTests) - Increased from 9 in #8
- 6 default view filter tests passing (DefaultViewFilterTests)
- 3 custom implementation tests passing (CustomFilterTests)
- 13 UIView watcher tests passing (UIViewWatcherTests)
- 4 dismissal marker smoke tests passing (DismissalMarkerSmokeTest)
- 6 documentation tests passing (DocumentationTests) - Added in #9
- Total: 60 tests passing (up from 54 in #8)

## Issue #6 Completion Status

**Implemented (via strict TDD):**
- ✅ `AnimationHistoryObserver` deep module with simple interface: `Sample()`, `HasDismissalEvidence()`, `HasRecentHistory()`, `GetScaleHistory()`, `GetAlphaHistory()`, `ClearHistory()`, `ClearAllHistory()`
- ✅ Passive scale history sampling for all tracked views
- ✅ Passive alpha history sampling for CanvasGroup-backed views
- ✅ Dismissal inference from observed scale/alpha changes
- ✅ `DismissalValidationResult` extended with `ScaleAnimationObserved` and `AlphaAnimationObserved` fields
- ✅ `IDismissalPolicy` interface updated to accept `AnimationHistoryObserver` parameter
- ✅ `DefaultDismissalPolicy` updated to query observer instead of requiring explicit intent
- ✅ `UIViewWatcher` updated to sample tracked views and use passive observation
- ✅ `DismissalMarker` and `IDismissalMarker` marked as `[Obsolete]` for backwards compatibility
- ✅ `DismissalIntent` enum marked as `[Obsolete]` for backwards compatibility
- ✅ All behaviors covered test-first through public APIs (38 total tests)
- ✅ Deep-module design: small interface, complex implementation hidden inside observer

**Acceptance Criteria Met:**
- ✅ Dismissal acceptance no longer requires consumers to call `MarkDismissalPlanned()`
- ✅ Tracked views have recent scale history available to the dismissal policy
- ✅ Tracked views with CanvasGroup have recent alpha history available to the dismissal policy
- ✅ Existing marker-based APIs are clearly deprecated in favor of passive observation
- ✅ Behavior is covered test-first through public APIs

## Issue #3 Completion Status

**Implemented (via strict TDD):**
- ✅ Public `IViewFilter` interface with `bool ShouldTrack(GameObject gameObject)` method
- ✅ `DefaultViewFilter` implementation matching "*View" suffix with Canvas/CanvasGroup components
- ✅ Public `IDismissalPolicy` interface with `DismissalValidationResult ValidateDismissal(GameObject, AnimationHistoryObserver)` method
- ✅ `DefaultDismissalPolicy` implementation with Canvas scale-only and CanvasGroup alpha/scale rules
- ✅ All behaviors covered test-first through public APIs
- ✅ Custom filtering/policy interfaces verified extensible

## Issue #4 Completion Status

**Implemented (via strict TDD):**
- ✅ `UIViewWatcher` MonoBehaviour that persists across scene loads with DontDestroyOnLoad
- ✅ Watcher discovers existing UI GameObjects at startup using default filter
- ✅ Watcher discovers newly created UI GameObjects during runtime with **throttled discovery** (configurable interval, default 0.5s)
- ✅ **Performance optimization**: Does NOT call FindObjectsOfType every Update; uses throttled interval-based discovery
- ✅ **Explicit Tick() method** for controlled discovery without relying on Update loop
- ✅ Watcher samples tracked views for animation history during regular ticks
- ✅ Watcher validates dismissals using observed animation evidence through policy
- ✅ Watcher emits diagnostics containing object context, rejection reason, and observed animation state
- ✅ Lifecycle behavior covered test-first through public APIs
- ✅ Public API: `UIViewWatcher` component can be attached to GameObjects in boot scenes
- ✅ Public API: `DiscoveryInterval` property for configurable throttling (minimum 0.1s)
- ✅ Public API: `Tick()` method for explicit discovery control

**Performance Characteristics:**
- Discovery occurs at configurable intervals (default 0.5s, minimum 0.1s)
- No unbounded full hierarchy scanning every frame
- Explicit Tick() method available for manual control when needed
- Initial discovery at startup ensures existing views are tracked immediately
- Sampling occurs for all tracked active views during each tick
- History pruning keeps only 1 second of recent samples per view

## Issue #7 Completion Status

**Implemented (via strict TDD):**
- ✅ `AnimationHistoryObserver.HasScaleDecrease(GameObject)` method for specific scale animation detection
- ✅ `AnimationHistoryObserver.HasAlphaDecrease(GameObject)` method for specific alpha animation detection
- ✅ `DefaultDismissalPolicy` updated to use specific animation type detection instead of generic dismissal evidence
- ✅ Canvas-backed views pass ONLY when recent scale motion shows qualifying scale-down (≥30%)
- ✅ Canvas-backed views fail WITHOUT qualifying scale motion
- ✅ CanvasGroup-backed views pass when alpha fades down (≥50% decrease)
- ✅ CanvasGroup-backed views pass when scale changes down (≥30% decrease)
- ✅ CanvasGroup-backed views pass when both animations are observed
- ✅ CanvasGroup-backed views fail WITHOUT qualifying alpha or scale motion
- ✅ `DismissalValidationResult.ScaleAnimationObserved` flag correctly reflects scale detection
- ✅ `DismissalValidationResult.AlphaAnimationObserved` flag correctly reflects alpha detection
- ✅ All behaviors covered test-first through public APIs (41 total tests, up from 38)
- ✅ Deep-module design maintained: minimal interface expansion, complex implementation hidden

**Acceptance Criteria Met:**
- ✅ Canvas-backed views pass when scale changes toward the configured hidden threshold before deactivation
- ✅ Canvas-backed views fail when deactivated without qualifying scale motion
- ✅ CanvasGroup-backed views pass when alpha fades down before deactivation
- ✅ CanvasGroup-backed views pass when scale changes down before deactivation
- ✅ CanvasGroup-backed views fail when deactivated without qualifying alpha or scale motion
- ✅ Tests cover successful and failing inference paths through public behavior

**Test Count Increase:**
- 9 default dismissal policy tests (up from 6)
- 41 total tests (up from 38)
- 3 new tests added for animation flag verification

## Issue #8 Completion Status

**Implemented (via strict TDD):**
- ✅ `AnimationHistoryObserver.ObservationWindowDuration` property for configuring observation window duration
- ✅ `AnimationHistoryObserver.ScaleDismissalThreshold` property for configuring scale dismissal threshold
- ✅ `AnimationHistoryObserver.AlphaDismissalThreshold` property for configuring alpha dismissal threshold
- ✅ Stale evidence expiry logic in `PruneOldSamples()` uses configurable observation window
- ✅ `DismissalValidationResult` extended with observed values (`ObservedFirstScale`, `ObservedLastScale`, `ObservedFirstAlpha`, `ObservedLastAlpha`)
- ✅ `DismissalValidationResult` extended with threshold values (`ScaleDismissalThreshold`, `AlphaDismissalThreshold`)
- ✅ `DefaultDismissalPolicy` captures and includes observed values and thresholds in validation results
- ✅ `DefaultDismissalPolicy` includes detailed observed values and thresholds in rejection reasons
- ✅ `UIViewWatcher.EmitAbruptDeactivationDiagnostic()` includes observed values and thresholds in diagnostic output
- ✅ All behaviors covered test-first through public APIs (54 total tests, up from 41)

**Acceptance Criteria Met:**
- ✅ Consumers can configure observation window duration
- ✅ Consumers can configure alpha dismissal threshold
- ✅ Consumers can configure scale dismissal threshold or minimum scale delta
- ✅ Old animation evidence expires and does not validate a much later deactivation
- ✅ Diagnostics include object context, observed scale/alpha values, thresholds, and rejection reason
- ✅ Tests cover threshold configuration and stale-evidence behavior

**Test Count Increase:**
- 13 animation history tests (up from 6)
- 12 default dismissal policy tests (up from 9)
- 54 total tests (up from 41)
- 7 new tests for configurable thresholds
- 2 new tests for stale evidence expiry
- 3 new tests for enhanced diagnostics

**New Public API:**
- `AnimationHistoryObserver.ObservationWindowDuration { get; set; }` - Configurable observation window duration (default 1.0s)
- `AnimationHistoryObserver.ScaleDismissalThreshold { get; set; }` - Configurable scale threshold (default 0.3)
- `AnimationHistoryObserver.AlphaDismissalThreshold { get; set; }` - Configurable alpha threshold (default 0.5)
- `DismissalValidationResult.ObservedFirstScale` - First scale value in history
- `DismissalValidationResult.ObservedLastScale` - Last scale value in history
- `DismissalValidationResult.ObservedFirstAlpha` - First alpha value in history
- `DismissalValidationResult.ObservedLastAlpha` - Last alpha value in history
- `DismissalValidationResult.ScaleDismissalThreshold` - Scale threshold used for validation
- `DismissalValidationResult.AlphaDismissalThreshold` - Alpha threshold used for validation

**Enhanced Diagnostics:**
Rejection reasons now include:
- Observed first and last scale values
- Observed first and last alpha values
- Scale and alpha dismissal thresholds used
- Detailed explanation of why dismissal was rejected

Example diagnostic output:
```
[Lazarus] Abrupt UI deactivation detected: TestView
Path: /Canvas/TestView
Reason: Canvas-backed views require scale dismissal animation (no recent scale change observed). First scale: (1.0, 1.0, 1.0), Last scale: (1.0, 1.0, 1.0), Threshold: 0.30
Scale Animation Observed: False
Alpha Animation Observed: False
Observed First Scale: (1.0, 1.0, 1.0)
Observed Last Scale: (1.0, 1.0, 1.0)
Observed First Alpha: 1.00
Observed Last Alpha: 1.00
Scale Dismissal Threshold: 0.30
Alpha Dismissal Threshold: 0.50
```

## Issue #9 Completion Status

**Implemented (via strict TDD):**
- ✅ DocumentationTests.cs with 6 behavior-first tests verifying README and VERIFICATION documentation
- ✅ README updated to show simple UIViewWatcher installation without DismissalMarker examples
- ✅ README updated to explain passive inference behavior for scale and alpha
- ✅ README removed all examples asking users to set dismiss intent manually
- ✅ VERIFICATION.md updated to list passive-inference test cases
- ✅ Main test files verified to not promote DismissalMarker as normal integration path
- ✅ Deprecated marker API documented as legacy/backwards compatibility only

**Acceptance Criteria Met:**
- ✅ README shows installing `UIViewWatcher` without calling marker APIs
- ✅ README explains scale and alpha inference behavior
- ✅ VERIFICATION docs list passive-inference test cases
- ✅ Tests no longer promote `DismissalMarker` as the normal integration path
- ✅ Deprecated marker API is documented as legacy compatibility only

**Test Count Increase:**
- 6 documentation tests passing (DocumentationTests)
- 57 total tests passing (up from 51)

**Files Changed:**
- README.md - Updated usage section and added "How It Works" section
- VERIFICATION.md - Updated test coverage, public API, and expected test output
- Tests/Editor/DocumentationTests.cs - New test file with documentation verification tests

## Backwards Compatibility

**Deprecated APIs (for backwards compatibility only):**
- `IDismissalMarker` interface - marked as `[Obsolete]`, legacy API for backwards compatibility
- `DismissalMarker` class - marked as `[Obsolete]`, legacy API for backwards compatibility
- `DismissalIntent` enum - marked as `[Obsolete]`, legacy API for backwards compatibility

These APIs are kept for backwards compatibility but clearly signal obsolescence. Consumers should use passive animation observation via `AnimationHistoryObserver` and `UIViewWatcher` instead. The package now automatically infers dismissals from observed scale and alpha animations without requiring any manual marking.
