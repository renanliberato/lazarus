using NUnit.Framework;
using UnityEngine;
using Lazarus;

namespace Lazarus.Tests
{
    /// <summary>
    /// Tests that verify consumers can provide custom filtering logic through the public interface.
    /// </summary>
    public class CustomViewFilterTests
    {
        /// <summary>
        /// Custom filter implementation for testing - accepts all GameObjects with "Custom" suffix.
        /// </summary>
        private class CustomViewFilter : IViewFilter
        {
            public bool ShouldTrack(GameObject gameObject)
            {
                if (gameObject == null)
                    return false;
                return gameObject.name.EndsWith("Custom");
            }
        }

        [Test]
        public void CustomFilter_Can_Provide_Custom_Filtering_Logic()
        {
            // Arrange: Create custom filter and GameObjects
            var filter = new CustomViewFilter();
            var trackedObject = new GameObject("MyCustom");
            var notTrackedObject = new GameObject("Other");

            // Act: Check filtering behavior
            bool shouldTrack1 = filter.ShouldTrack(trackedObject);
            bool shouldTrack2 = filter.ShouldTrack(notTrackedObject);

            // Assert: Custom filtering logic should work
            Assert.IsTrue(shouldTrack1, "Custom filter should accept objects matching custom criteria");
            Assert.IsFalse(shouldTrack2, "Custom filter should reject objects not matching custom criteria");
        }

        [Test]
        public void IDismissalPolicy_Interface_Can_Be_Implemented_Custom()
        {
            // Arrange: Create custom policy that only accepts abrupt dismissal
            class CustomDismissalPolicy : IDismissalPolicy
            {
                public DismissalValidationResult ValidateDismissal(GameObject gameObject, DismissalIntent intent)
                {
                    return new DismissalValidationResult
                    {
                        IsValid = intent == DismissalIntent.Abrupt,
                        RejectionReason = intent == DismissalIntent.Abrupt ? null : "Only abrupt dismissal allowed"
                    };
                }
            }

            var policy = new CustomDismissalPolicy();
            var gameObject = new GameObject("TestView");

            // Act: Validate different dismissal intents
            var abruptResult = policy.ValidateDismissal(gameObject, DismissalIntent.Abrupt);
            var scaleResult = policy.ValidateDismissal(gameObject, DismissalIntent.Scale);

            // Assert: Custom policy logic should work
            Assert.IsTrue(abruptResult.IsValid, "Custom policy should accept abrupt dismissal");
            Assert.IsFalse(scaleResult.IsValid, "Custom policy should reject non-abrupt dismissal");
            Assert.IsNotNull(scaleResult.RejectionReason, "Custom policy should provide rejection reason");
        }
    }
}