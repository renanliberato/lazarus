using UnityEngine;

namespace Lazarus
{
    /// <summary>
    /// Public interface for custom filtering logic to determine which GameObjects should be tracked.
    /// Consumers can implement this interface to provide custom filtering behavior.
    /// </summary>
    public interface IViewFilter
    {
        /// <summary>
        /// Determines whether the specified GameObject should be tracked.
        /// </summary>
        /// <param name="gameObject">The GameObject to evaluate.</param>
        /// <returns>True if the GameObject should be tracked; otherwise, false.</returns>
        bool ShouldTrack(GameObject gameObject);
    }

    /// <summary>
    /// Default implementation of view filtering.
    /// Accepts GameObjects whose names end with "View" and have Canvas and/or CanvasGroup components.
    /// </summary>
    public class DefaultViewFilter : IViewFilter
    {
        public bool ShouldTrack(GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            // Check if name ends with "View"
            if (!gameObject.name.EndsWith("View"))
                return false;

            // Check if it has Canvas or CanvasGroup component
            return gameObject.GetComponent<Canvas>() != null || gameObject.GetComponent<CanvasGroup>() != null;
        }
    }

    /// <summary>
    /// Represents the type of dismissal animation being used.
    /// </summary>
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

    /// <summary>
    /// Result of validating a dismissal intent.
    /// </summary>
    public struct DismissalValidationResult
    {
        /// <summary>Whether the dismissal is valid</summary>
        public bool IsValid;

        /// <summary>Reason for rejection, if any</summary>
        public string RejectionReason;
    }

    /// <summary>
    /// Public interface for dismissal policy logic.
    /// Consumers can implement this interface to provide custom dismissal validation behavior.
    /// </summary>
    public interface IDismissalPolicy
    {
        /// <summary>
        /// Validates whether the specified dismissal intent is acceptable for the given GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject being dismissed.</param>
        /// <param name="intent">The type of dismissal being performed.</param>
        /// <returns>A validation result indicating acceptance and any rejection reason.</returns>
        DismissalValidationResult ValidateDismissal(GameObject gameObject, DismissalIntent intent);
    }

    /// <summary>
    /// Default implementation of dismissal policy.
    /// - Accepts scale dismissal for Canvas-backed views
    /// - Accepts alpha and/or scale dismissal for CanvasGroup-backed views
    /// - Rejects abrupt deactivation with specific reasons
    /// </summary>
    public class DefaultDismissalPolicy : IDismissalPolicy
    {
        public DismissalValidationResult ValidateDismissal(GameObject gameObject, DismissalIntent intent)
        {
            var result = new DismissalValidationResult { IsValid = true, RejectionReason = null };

            // Check if it has Canvas component
            var canvas = gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Canvas-backed views require scale dismissal
                if (intent == DismissalIntent.Scale)
                {
                    result.IsValid = true;
                    result.RejectionReason = null;
                }
                else
                {
                    result.IsValid = false;
                    result.RejectionReason = "Canvas-backed views require scale dismissal animation";
                }
                return result;
            }

            // Check if it has CanvasGroup component
            var canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // CanvasGroup-backed views accept alpha and/or scale
                if (intent == DismissalIntent.Alpha || intent == DismissalIntent.Scale || intent == DismissalIntent.Both)
                {
                    result.IsValid = true;
                    result.RejectionReason = null;
                }
                else
                {
                    result.IsValid = false;
                    result.RejectionReason = "CanvasGroup-backed views require alpha or scale dismissal animation";
                }
                return result;
            }

            // No Canvas or CanvasGroup - this shouldn't happen if filtering works correctly
            result.IsValid = false;
            result.RejectionReason = "GameObject is not a valid UI view (no Canvas or CanvasGroup)";
            return result;
        }
    }
}