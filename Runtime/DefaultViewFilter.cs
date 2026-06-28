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
}