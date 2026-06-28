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

Attach `UIViewWatcher` to a GameObject in your boot scene. The watcher calls `DontDestroyOnLoad`, discovers matching UI views, and logs warnings when a tracked view is deactivated abruptly.

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

Before hiding a tracked view, add or use a `DismissalMarker` and mark the dismissal as planned:

```csharp
var marker = view.GetComponent<DismissalMarker>() ?? view.AddComponent<DismissalMarker>();
marker.MarkDismissalPlanned();

// Play your scale/fade animation, then deactivate.
view.SetActive(false);
```

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
