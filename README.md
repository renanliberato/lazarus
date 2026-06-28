# Lazarus

Lazarus is a Unity Package Manager package that helps detect UI `GameObject`s being deactivated without a dismissal animation.

It is intended for UI views such as canvases and canvas groups that should fade or scale out before `SetActive(false)`.

## Install

In Unity Package Manager, choose **Add package from git URL...** and use:

```text
https://github.com/renanliberato/lazarus.git
```

Or add it to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.renanliberato.lazarus": "https://github.com/renanliberato/lazarus.git"
  }
}
```

## Usage

Attach `UIViewWatcher` to a GameObject in your boot scene. The watcher calls `DontDestroyOnLoad`, discovers matching UI views, and logs warnings when a tracked view is deactivated abruptly without a dismissal animation.

Lazarus passively observes UI view animations and infers dismissals from scale and opacity changes. Simply play your scale/fade animations as normal before calling `SetActive(false)` - no manual marking is required.

```csharp
using Lazarus;
using UnityEngine;

public class Boot : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AddComponent<UIViewWatcher>();
    }
}
```

## How It Works

Lazarus is a passive observer - it watches your UI views and infers whether a deactivation was preceded by a proper dismissal animation.

- **Canvas-backed views**: Pass when local scale decreases by at least 30% before deactivation
- **CanvasGroup-backed views**: Pass when either alpha fades by 50% OR scale decreases by 30% (or both)
- **Abrupt deactivation**: Emits diagnostic warnings when views are deactivated without recent qualifying motion

The watcher samples tracked views at regular intervals and validates dismissals based on this observed history. No manual integration is required beyond installing the watcher.

## Default tracking rules

`DefaultViewFilter` tracks GameObjects that:

- have a name ending in `View`, and
- have a `Canvas` or `CanvasGroup` component.

`DefaultDismissalPolicy` accepts:

- scale dismissals for `Canvas` views
- alpha, scale, or both for `CanvasGroup` views

## Performance

`UIViewWatcher` does not scan the hierarchy every frame. Discovery is throttled with `DiscoveryInterval` (default `0.5s`, minimum `0.1s`) and can also be triggered manually with `Tick()`.

## Tests

See [VERIFICATION.md](VERIFICATION.md) for Unity edit-mode test instructions and the verification checklist.
