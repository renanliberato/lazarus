using UnityEngine;

namespace Lazarus
{
    /// <summary>
    /// Marker interface for registering dismissal intent on a GameObject.
    /// This is the minimal public API seam for the smoke test.
    /// </summary>
    public interface IDismissalMarker
    {
        /// <summary>
        /// Marks the associated GameObject as having a planned dismissal animation.
        /// </summary>
        void MarkDismissalPlanned();

        /// <summary>
        /// Checks whether the associated GameObject has a planned dismissal animation.
        /// </summary>
        /// <returns>True if dismissal is planned; otherwise, false.</returns>
        bool IsDismissalPlanned();
    }

    /// <summary>
    /// Minimal implementation of dismissal marker for testing purposes.
    /// </summary>
    public class DismissalMarker : MonoBehaviour, IDismissalMarker
    {
        private bool _dismissalPlanned = false;

        public void MarkDismissalPlanned()
        {
            _dismissalPlanned = true;
        }

        public bool IsDismissalPlanned()
        {
            return _dismissalPlanned;
        }
    }
}