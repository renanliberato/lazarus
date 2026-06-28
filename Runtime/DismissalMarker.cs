using UnityEngine;
using System;

namespace Lazarus
{
    /// <summary>
    /// Marker interface for registering dismissal intent on a GameObject.
    /// DEPRECATED: Use AnimationHistoryObserver for passive animation inference instead.
    /// This interface is kept for backwards compatibility only.
    /// </summary>
    [Obsolete("Use AnimationHistoryObserver for passive animation inference instead of explicit dismissal markers")]
    public interface IDismissalMarker
    {
        /// <summary>
        /// Marks the associated GameObject as having a planned dismissal animation.
        /// DEPRECATED: Animation history is now observed passively.
        /// </summary>
        [Obsolete("Animation history is now observed passively - do not call MarkDismissalPlanned")]
        void MarkDismissalPlanned();

        /// <summary>
        /// Checks whether the associated GameObject has a planned dismissal animation.
        /// DEPRECATED: Always returns false in passive observation mode.
        /// </summary>
        [Obsolete("Animation history is now observed passively")]
        bool IsDismissalPlanned();
    }

    /// <summary>
    /// Minimal implementation of dismissal marker.
    /// DEPRECATED: Use AnimationHistoryObserver for passive animation inference instead.
    /// </summary>
    [Obsolete("Use AnimationHistoryObserver for passive animation inference instead")]
    public class DismissalMarker : MonoBehaviour, IDismissalMarker
    {
        private bool _dismissalPlanned = false;

        /// <summary>
        /// Marks dismissal as planned.
        /// DEPRECATED: This method no longer affects dismissal validation.
        /// </summary>
        [Obsolete("This method no longer affects dismissal validation - use passive animation observation")]
        public void MarkDismissalPlanned()
        {
            _dismissalPlanned = true;
        }

        /// <summary>
        /// Checks if dismissal is planned.
        /// DEPRECATED: This method no longer affects dismissal validation.
        /// </summary>
        [Obsolete("This method no longer affects dismissal validation - use passive animation observation")]
        public bool IsDismissalPlanned()
        {
            return _dismissalPlanned;
        }
    }
}