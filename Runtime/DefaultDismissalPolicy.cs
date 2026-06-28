using UnityEngine;

namespace Lazarus
{
    /// <summary>
    /// Result of validating a dismissal.
    /// </summary>
    public struct DismissalValidationResult
    {
        /// <summary>Whether the dismissal is valid</summary>
        public bool IsValid;

        /// <summary>Reason for rejection, if any</summary>
        public string RejectionReason;

        /// <summary>Whether scale animation was observed</summary>
        public bool ScaleAnimationObserved;

        /// <summary>Whether alpha animation was observed</summary>
        public bool AlphaAnimationObserved;

        /// <summary>Observed scale values (first and last in history window)</summary>
        public Vector3 ObservedFirstScale;
        public Vector3 ObservedLastScale;

        /// <summary>Observed alpha values (first and last in history window)</summary>
        public float ObservedFirstAlpha;
        public float ObservedLastAlpha;

        /// <summary>Thresholds used for validation</summary>
        public float ScaleDismissalThreshold;
        public float AlphaDismissalThreshold;
    }

    /// <summary>
    /// Public interface for dismissal policy logic.
    /// Consumers can implement this interface to provide custom dismissal validation behavior.
    /// </summary>
    public interface IDismissalPolicy
    {
        /// <summary>
        /// Validates whether the deactivation of the specified GameObject is acceptable
        /// based on observed animation history.
        /// </summary>
        /// <param name="gameObject">The GameObject being deactivated.</param>
        /// <param name="observer">The animation history observer with recent samples.</param>
        /// <returns>A validation result indicating acceptance and any rejection reason.</returns>
        DismissalValidationResult ValidateDismissal(GameObject gameObject, AnimationHistoryObserver observer);
    }

    /// <summary>
    /// Default implementation of dismissal policy.
    /// - Accepts deactivation when scale animation is observed (for Canvas views)
    /// - Accepts deactivation when alpha or scale animation is observed (for CanvasGroup views)
    /// - Rejects abrupt deactivation without observed animation evidence
    /// </summary>
    public class DefaultDismissalPolicy : IDismissalPolicy
    {
        public DismissalValidationResult ValidateDismissal(GameObject gameObject, AnimationHistoryObserver observer)
        {
            var result = new DismissalValidationResult
            {
                IsValid = false,
                RejectionReason = null,
                ScaleAnimationObserved = false,
                AlphaAnimationObserved = false,
                ObservedFirstScale = Vector3.zero,
                ObservedLastScale = Vector3.zero,
                ObservedFirstAlpha = 0f,
                ObservedLastAlpha = 0f,
                ScaleDismissalThreshold = observer.ScaleDismissalThreshold,
                AlphaDismissalThreshold = observer.AlphaDismissalThreshold
            };

            if (gameObject == null)
            {
                result.RejectionReason = "GameObject is null";
                return result;
            }

            // Capture observed values for diagnostics
            var scaleHistory = observer.GetScaleHistory(gameObject);
            if (scaleHistory != null && scaleHistory.Count > 0)
            {
                result.ObservedFirstScale = scaleHistory[0];
                result.ObservedLastScale = scaleHistory[scaleHistory.Count - 1];
            }

            var alphaHistory = observer.GetAlphaHistory(gameObject);
            if (alphaHistory != null && alphaHistory.Count > 0)
            {
                result.ObservedFirstAlpha = alphaHistory[0];
                result.ObservedLastAlpha = alphaHistory[alphaHistory.Count - 1];
            }

            // Check if it has Canvas component
            var canvas = gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Canvas-backed views require scale dismissal animation
                if (observer.HasScaleDecrease(gameObject))
                {
                    result.IsValid = true;
                    result.RejectionReason = null;
                    result.ScaleAnimationObserved = true;
                    result.AlphaAnimationObserved = false;
                }
                else
                {
                    result.IsValid = false;
                    result.RejectionReason = $"Canvas-backed views require scale dismissal animation (no recent scale change observed). First scale: {result.ObservedFirstScale}, Last scale: {result.ObservedLastScale}, Threshold: {result.ScaleDismissalThreshold:F2}";
                    result.ScaleAnimationObserved = false;
                    result.AlphaAnimationObserved = false;
                }
                return result;
            }

            // Check if it has CanvasGroup component
            var canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // CanvasGroup-backed views accept alpha and/or scale animation
                bool hasScale = observer.HasScaleDecrease(gameObject);
                bool hasAlpha = observer.HasAlphaDecrease(gameObject);

                if (hasScale || hasAlpha)
                {
                    result.IsValid = true;
                    result.RejectionReason = null;
                    result.ScaleAnimationObserved = hasScale;
                    result.AlphaAnimationObserved = hasAlpha;
                }
                else
                {
                    result.IsValid = false;
                    result.RejectionReason = $"CanvasGroup-backed views require alpha or scale dismissal animation (no recent alpha or scale change observed). First alpha: {result.ObservedFirstAlpha:F2}, Last alpha: {result.ObservedLastAlpha:F2}, First scale: {result.ObservedFirstScale}, Last scale: {result.ObservedLastScale}, Alpha threshold: {result.AlphaDismissalThreshold:F2}, Scale threshold: {result.ScaleDismissalThreshold:F2}";
                    result.ScaleAnimationObserved = false;
                    result.AlphaAnimationObserved = false;
                }
                return result;
            }

            // No Canvas or CanvasGroup - this shouldn't happen if filtering works correctly
            result.IsValid = false;
            result.RejectionReason = "GameObject is not a valid UI view (no Canvas or CanvasGroup)";
            return result;
        }
    }

    /// <summary>
    /// Legacy dismissal intent enum - deprecated in favor of passive observation.
    /// Kept for backwards compatibility only.
    /// </summary>
    [System.Obsolete("Use AnimationHistoryObserver for passive animation inference instead")]
    public enum DismissalIntent
    {
        /// <summary>Dismissal using scale animation</summary>
        Scale,
        /// <summary>Dismissal using alpha (fade) animation</summary>
        Alpha,
        /// <summary>Dismissal using both scale and alpha animations</summary>
        Both,
        /// <summary>Abrupt deactivation without animation</summary>
        Abrupt
    }
}