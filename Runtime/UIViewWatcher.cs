using UnityEngine;
using System.Collections.Generic;

namespace Lazarus
{
    /// <summary>
    /// MonoBehaviour that persists across scene loads and watches UI activation lifecycle.
    /// Discovers UI GameObjects, tracks their state, and emits diagnostics for abrupt deactivations.
    /// </summary>
    public class UIViewWatcher : MonoBehaviour
    {
        private IViewFilter _viewFilter;
        private IDismissalPolicy _dismissalPolicy;
        private HashSet<GameObject> _trackedViews;
        private float _discoveryInterval = 0.5f; // Configurable interval in seconds
        private float _lastDiscoveryTime;

        /// <summary>
        /// Gets or sets the interval between discovery scans in seconds.
        /// Default is 0.5 seconds.
        /// </summary>
        public float DiscoveryInterval
        {
            get => _discoveryInterval;
            set => _discoveryInterval = Mathf.Max(0.1f, value); // Minimum 0.1 seconds
        }

        private void Awake()
        {
            // Persist this GameObject across scene loads
            DontDestroyOnLoad(gameObject);

            // Initialize with default filter and policy
            _viewFilter = new DefaultViewFilter();
            _dismissalPolicy = new DefaultDismissalPolicy();
            _trackedViews = new HashSet<GameObject>();
            _lastDiscoveryTime = -_discoveryInterval; // Force immediate discovery on first frame
        }

        private void Start()
        {
            // Discover existing UI GameObjects at startup
            DiscoverExistingViews();
        }

        private void Update()
        {
            // Throttled discovery - only scan at configurable intervals
            if (Time.time - _lastDiscoveryTime >= _discoveryInterval)
            {
                Tick();
                _lastDiscoveryTime = Time.time;
            }
        }

        /// <summary>
        /// Explicitly performs a single tick of discovery and checking.
        /// This allows for manual control instead of relying on Update.
        /// </summary>
        public void Tick()
        {
            DiscoverNewViews();
            CheckForAbruptDeactivations();
        }

        private void DiscoverExistingViews()
        {
            // Find all GameObjects in the scene (only done once at startup)
            var allObjects = FindObjectsOfType<GameObject>(true);

            foreach (var obj in allObjects)
            {
                if (_viewFilter.ShouldTrack(obj))
                {
                    _trackedViews.Add(obj);
                }
            }
        }

        private void DiscoverNewViews()
        {
            // Find all GameObjects in the scene
            var allObjects = FindObjectsOfType<GameObject>(true);

            foreach (var obj in allObjects)
            {
                // Only add if not already tracked
                if (_viewFilter.ShouldTrack(obj) && !_trackedViews.Contains(obj))
                {
                    _trackedViews.Add(obj);
                }
            }
        }

        private void CheckForAbruptDeactivations()
        {
            // Check each tracked view for abrupt deactivation
            var objectsToRemove = new List<GameObject>();

            foreach (var view in _trackedViews)
            {
                if (view == null)
                {
                    // Object was destroyed - this is expected
                    objectsToRemove.Add(view);
                    continue;
                }

                // Check if the view is being deactivated without dismissal evidence
                if (!view.activeSelf && !HasDismissalEvidence(view))
                {
                    // Emit diagnostic for abrupt deactivation
                    EmitAbruptDeactivationDiagnostic(view);
                    objectsToRemove.Add(view);
                }
            }

            // Remove destroyed or deactivated views from tracking
            foreach (var obj in objectsToRemove)
            {
                _trackedViews.Remove(obj);
            }
        }

        private bool HasDismissalEvidence(GameObject view)
        {
            // Check if the view has a dismissal marker with planned dismissal
            var dismissalMarker = view.GetComponent<IDismissalMarker>();
            return dismissalMarker != null && dismissalMarker.IsDismissalPlanned();
        }

        private void EmitAbruptDeactivationDiagnostic(GameObject view)
        {
            // Validate the abrupt dismissal using the policy
            var validationResult = _dismissalPolicy.ValidateDismissal(view, DismissalIntent.Abrupt);

            // Emit diagnostic with object context and rejection reason
            Debug.LogWarning($"[Lazarus] Abrupt UI deactivation detected: {view.name}\n" +
                            $"Reason: {validationResult.RejectionReason}\n" +
                            $"Path: {GetGameObjectPath(view)}");
        }

        private string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
    }
}