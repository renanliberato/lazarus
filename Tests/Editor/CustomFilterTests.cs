using NUnit.Framework;
using UnityEngine;
using Lazarus;

namespace Lazarus.Tests
{
    /// <summary>
    /// Tests that verify consumers can provide custom filtering and policy logic through the public interface.
    /// </summary>
    public class CustomFilterTests
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
                public DismissalValidationResult ValidateDismissal(GameObject gameObject, AnimationHistoryObserver observer)
                {
                    return new DismissalValidationResult
                    {
                        IsValid = true, // Accept all dismissals
                        RejectionReason = null
                    };
                }
            }

            var policy = new CustomDismissalPolicy();
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();
            var observer = new AnimationHistoryObserver();

            // Act: Validate dismissal with custom policy
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: Custom policy logic should work
            Assert.IsTrue(result.IsValid, "Custom policy should accept dismissals");
        }

        [Test]
        public void CustomPolicy_Can_Query_AnimationHistoryObserver()
        {
            // Arrange: Create custom policy that checks for specific animation patterns
            class StrictDismissalPolicy : IDismissalPolicy
            {
                public DismissalValidationResult ValidateDismissal(GameObject gameObject, AnimationHistoryObserver observer)
                {
                    // Only accept dismissal if there's animation evidence
                    if (observer.HasDismissalEvidence(gameObject))
                    {
                        return new DismissalValidationResult
                        {
                            IsValid = true,
                            RejectionReason = null,
                            ScaleAnimationObserved = true,
                            AlphaAnimationObserved = true
                        };
                    }

                    return new DismissalValidationResult
                    {
                        IsValid = false,
                        RejectionReason = "Strict policy requires animation evidence",
                        ScaleAnimationObserved = false,
                        AlphaAnimationObserved = false
                    };
                }
            }

            var policy = new StrictDismissalPolicy();
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();
            gameObject.transform.localScale = Vector3.one;
            var observer = new AnimationHistoryObserver();

            // Act: Validate without animation evidence
            var resultWithoutEvidence = policy.ValidateDismissal(gameObject, observer);

            // Add animation evidence
            observer.Sample(gameObject);
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            observer.Sample(gameObject);
            var resultWithEvidence = policy.ValidateDismissal(gameObject, observer);

            // Assert: Custom policy should correctly query observer
            Assert.IsFalse(resultWithoutEvidence.IsValid, "Strict policy should reject without evidence");
            Assert.IsNotNull(resultWithoutEvidence.RejectionReason, "Strict policy should provide reason");
            Assert.IsTrue(resultWithEvidence.IsValid, "Strict policy should accept with evidence");
        }
    }
}