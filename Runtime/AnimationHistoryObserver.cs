using UnityEngine;
using System.Collections.Generic;

namespace Lazarus
{
    /// <summary>
    /// Sample of animation state at a specific time.
    /// </summary>
    internal struct AnimationSample
    {
        public float time;
        public Vector3 scale;
        public float alpha;

        public AnimationSample(float time, Vector3 scale, float alpha)
        {
            this.time = time;
            this.scale = scale;
            this.alpha = alpha;
        }
    }

    /// <summary>
    /// Passive observer that tracks animation history for UI views.
    /// Samples scale and CanvasGroup alpha over time to infer dismissal intent.
    /// Deep module: simple interface, complex implementation hidden inside.
    /// </summary>
    public class AnimationHistoryObserver
    {
        private class ViewHistory
        {
            public LinkedList<AnimationSample> samples = new LinkedList<AnimationSample>();
            public float lastSampleTime;
        }

        private Dictionary<GameObject, ViewHistory> _historyMap = new Dictionary<GameObject, ViewHistory>();
        private float _maxHistoryDuration = 1.0f; // Keep 1 second of history by default
        private float _scaleChangeThreshold = 0.3f; // 30% scale change qualifies by default
        private float _alphaChangeThreshold = 0.5f; // 50% alpha change qualifies by default

        /// <summary>
        /// Gets or sets the observation window duration in seconds.
        /// Animation samples older than this duration are pruned and not used for dismissal inference.
        /// Default is 1.0 second.
        /// </summary>
        public float ObservationWindowDuration
        {
            get => _maxHistoryDuration;
            set => _maxHistoryDuration = Mathf.Max(0.01f, value); // Minimum 10ms to prevent issues
        }

        /// <summary>
        /// Gets or sets the scale change threshold for dismissal inference.
        /// A scale decrease greater than or equal to this value qualifies as dismissal evidence.
        /// Default is 0.3 (30% scale change).
        /// </summary>
        public float ScaleDismissalThreshold
        {
            get => _scaleChangeThreshold;
            set => _scaleChangeThreshold = Mathf.Clamp01(value); // Between 0 and 1
        }

        /// <summary>
        /// Gets or sets the alpha change threshold for dismissal inference.
        /// An alpha decrease greater than or equal to this value qualifies as dismissal evidence.
        /// Default is 0.5 (50% alpha change).
        /// </summary>
        public float AlphaDismissalThreshold
        {
            get => _alphaChangeThreshold;
            set => _alphaChangeThreshold = Mathf.Clamp01(value); // Between 0 and 1
        }

        /// <summary>
        /// Samples the current animation state of a view.
        /// </summary>
        public void Sample(GameObject view)
        {
            if (view == null)
                return;

            var currentTime = Time.time;
            ViewHistory history;

            if (!_historyMap.TryGetValue(view, out history))
            {
                history = new ViewHistory();
                _historyMap[view] = history;
            }

            // Get current scale
            Vector3 scale = view.transform.localScale;

            // Get current alpha (if CanvasGroup exists)
            float alpha = 1.0f;
            var canvasGroup = view.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                alpha = canvasGroup.alpha;
            }

            // Add sample
            var sample = new AnimationSample(currentTime, scale, alpha);
            history.samples.AddLast(sample);
            history.lastSampleTime = currentTime;

            // Prune old samples
            PruneOldSamples(history, currentTime);
        }

        /// <summary>
        /// Checks if a view has recent animation history.
        /// </summary>
        public bool HasRecentHistory(GameObject view)
        {
            if (view == null)
                return false;

            ViewHistory history;
            if (!_historyMap.TryGetValue(view, out history))
                return false;

            return history.samples.Count > 0;
        }

        /// <summary>
        /// Gets the scale history for a view.
        /// </summary>
        public List<Vector3> GetScaleHistory(GameObject view)
        {
            if (view == null)
                return null;

            ViewHistory history;
            if (!_historyMap.TryGetValue(view, out history))
                return null;

            var result = new List<Vector3>();
            foreach (var sample in history.samples)
            {
                result.Add(sample.scale);
            }
            return result;
        }

        /// <summary>
        /// Gets the alpha history for a view.
        /// </summary>
        public List<float> GetAlphaHistory(GameObject view)
        {
            if (view == null)
                return null;

            ViewHistory history;
            if (!_historyMap.TryGetValue(view, out history))
                return null;

            var result = new List<float>();
            foreach (var sample in history.samples)
            {
                result.Add(sample.alpha);
            }
            return result;
        }

        /// <summary>
        /// Determines if a view has dismissal evidence based on recent animation history.
        /// </summary>
        public bool HasDismissalEvidence(GameObject view)
        {
            if (view == null)
                return false;

            ViewHistory history;
            if (!_historyMap.TryGetValue(view, out history))
                return false;

            if (history.samples.Count < 2)
                return false;

            // Check for significant scale decrease
            if (HasSignificantScaleDecrease(history))
                return true;

            // Check for significant alpha decrease (only if CanvasGroup)
            if (view.GetComponent<CanvasGroup>() != null && HasSignificantAlphaDecrease(history))
                return true;

            return false;
        }

        /// <summary>
        /// Determines if a view has significant scale decrease in recent history.
        /// </summary>
        public bool HasScaleDecrease(GameObject view)
        {
            if (view == null)
                return false;

            ViewHistory history;
            if (!_historyMap.TryGetValue(view, out history))
                return false;

            if (history.samples.Count < 2)
                return false;

            return HasSignificantScaleDecrease(history);
        }

        /// <summary>
        /// Determines if a view has significant alpha decrease in recent history.
        /// </summary>
        public bool HasAlphaDecrease(GameObject view)
        {
            if (view == null)
                return false;

            ViewHistory history;
            if (!_historyMap.TryGetValue(view, out history))
                return false;

            if (history.samples.Count < 2)
                return false;

            return HasSignificantAlphaDecrease(history);
        }

        private void PruneOldSamples(ViewHistory history, float currentTime)
        {
            var node = history.samples.First;
            while (node != null)
            {
                var next = node.Next;
                if (currentTime - node.Value.time > _maxHistoryDuration)
                {
                    history.samples.Remove(node);
                }
                node = next;
            }
        }

        private bool HasSignificantScaleDecrease(ViewHistory history)
        {
            var first = history.samples.First.Value;
            var last = history.samples.Last.Value;

            // Check if scale decreased significantly in any dimension
            float maxDecrease = 0f;
            for (int i = 0; i < 3; i++)
            {
                float decrease = first.scale[i] - last.scale[i];
                if (decrease > maxDecrease)
                    maxDecrease = decrease;
            }

            return maxDecrease >= _scaleChangeThreshold;
        }

        private bool HasSignificantAlphaDecrease(ViewHistory history)
        {
            var first = history.samples.First.Value;
            var last = history.samples.Last.Value;

            float decrease = first.alpha - last.alpha;
            return decrease >= _alphaChangeThreshold;
        }

        /// <summary>
        /// Clears history for a specific view.
        /// </summary>
        public void ClearHistory(GameObject view)
        {
            if (view != null && _historyMap.ContainsKey(view))
            {
                _historyMap.Remove(view);
            }
        }

        /// <summary>
        /// Clears all history.
        /// </summary>
        public void ClearAllHistory()
        {
            _historyMap.Clear();
        }
    }
}