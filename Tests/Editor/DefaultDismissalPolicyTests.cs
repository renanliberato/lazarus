using NUnit.Framework;
using UnityEngine;
using Lazarus;

namespace Lazarus.Tests
{
    /// <summary>
    /// Tests for the default dismissal policy behavior with observed animation history.
    /// These are behavior-first tests that verify the public API contracts
    /// for validating dismissals based on passive observation.
    /// </summary>
    public class DefaultDismissalPolicyTests
    {
        [Test]
        public void DefaultDismissalPolicy_Accepts_Canvas_Backend_View_With_Observed_Scale_Animation()
        {
            // Arrange: Create a GameObject with Canvas component and observer with history
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // Full scale
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Scale down
            observer.Sample(gameObject); // Sample after animation

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: The policy should accept dismissal with observed scale animation
            Assert.IsTrue(result.IsValid, "Policy should accept dismissal with observed scale animation");
            Assert.IsTrue(result.ScaleAnimationObserved, "Scale animation should be marked as observed");
            Assert.IsFalse(result.AlphaAnimationObserved, "Alpha animation should not be marked as observed for Canvas");
            Assert.IsNull(result.RejectionReason, "Rejection reason should be null when valid");
        }

        [Test]
        public void DefaultDismissalPolicy_Rejects_Canvas_Backend_View_Without_Observed_Animation()
        {
            // Arrange: Create a GameObject with Canvas component but no animation history
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // One sample, no change
            observer.Sample(gameObject); // Another sample, still no change

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: The policy should reject abrupt deactivation
            Assert.IsFalse(result.IsValid, "Policy should reject dismissal without observed animation");
            Assert.IsFalse(result.ScaleAnimationObserved, "Scale animation should not be marked as observed");
            Assert.IsFalse(result.AlphaAnimationObserved, "Alpha animation should not be marked as observed");
            Assert.IsNotNull(result.RejectionReason, "Policy should provide rejection reason");
            Assert.IsTrue(result.RejectionReason.Contains("scale"), "Rejection reason should mention scale animation requirement");
        }

        [Test]
        public void DefaultDismissalPolicy_Accepts_CanvasGroup_Backend_View_With_Observed_Alpha_Animation()
        {
            // Arrange: Create a GameObject with CanvasGroup component and observer with alpha history
            var gameObject = new GameObject("TestView");
            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // Full alpha
            canvasGroup.alpha = 0.0f; // Fade out
            observer.Sample(gameObject); // Sample after animation

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: The policy should accept dismissal with observed alpha animation
            Assert.IsTrue(result.IsValid, "Policy should accept dismissal with observed alpha animation");
            Assert.IsFalse(result.ScaleAnimationObserved, "Scale animation should not be marked as observed (no scale change)");
            Assert.IsTrue(result.AlphaAnimationObserved, "Alpha animation should be marked as observed");
            Assert.IsNull(result.RejectionReason, "Rejection reason should be null when valid");
        }

        [Test]
        public void DefaultDismissalPolicy_Accepts_CanvasGroup_Backend_View_With_Observed_Scale_Animation()
        {
            // Arrange: Create a GameObject with CanvasGroup component and observer with scale history
            var gameObject = new GameObject("TestView");
            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // Full scale
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Scale down
            observer.Sample(gameObject); // Sample after animation

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: The policy should accept dismissal with observed scale animation
            Assert.IsTrue(result.IsValid, "Policy should accept dismissal with observed scale animation");
            Assert.IsTrue(result.ScaleAnimationObserved, "Scale animation should be marked as observed");
            Assert.IsFalse(result.AlphaAnimationObserved, "Alpha animation should not be marked as observed (no alpha change)");
            Assert.IsNull(result.RejectionReason, "Rejection reason should be null when valid");
        }

        [Test]
        public void DefaultDismissalPolicy_Accepts_CanvasGroup_Backend_View_With_Both_Observed_Animations()
        {
            // Arrange: Create a GameObject with CanvasGroup component and observer with both animations
            var gameObject = new GameObject("TestView");
            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // Full alpha and scale
            canvasGroup.alpha = 0.0f; // Fade out
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Scale down
            observer.Sample(gameObject); // Sample after both animations

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: The policy should accept dismissal with both animations observed
            Assert.IsTrue(result.IsValid, "Policy should accept dismissal with both alpha and scale animations");
            Assert.IsTrue(result.ScaleAnimationObserved, "Scale animation should be marked as observed");
            Assert.IsTrue(result.AlphaAnimationObserved, "Alpha animation should be marked as observed");
            Assert.IsNull(result.RejectionReason, "Rejection reason should be null when valid");
        }

        [Test]
        public void DefaultDismissalPolicy_Rejects_CanvasGroup_Backend_View_Without_Observed_Animation()
        {
            // Arrange: Create a GameObject with CanvasGroup component but no animation history
            var gameObject = new GameObject("TestView");
            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // One sample, no change
            observer.Sample(gameObject); // Another sample, still no change

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: The policy should reject abrupt deactivation
            Assert.IsFalse(result.IsValid, "Policy should reject dismissal without observed animation");
            Assert.IsFalse(result.ScaleAnimationObserved, "Scale animation should not be marked as observed");
            Assert.IsFalse(result.AlphaAnimationObserved, "Alpha animation should not be marked as observed");
            Assert.IsNotNull(result.RejectionReason, "Policy should provide rejection reason");
            Assert.IsTrue(result.RejectionReason.Contains("alpha") || result.RejectionReason.Contains("scale"),
                "Rejection reason should mention alpha or scale animation requirement");
        }

        [Test]
        public void DefaultDismissalPolicy_Canvas_View_Shows_ScaleObserved_Flag_When_Scale_Animated()
        {
            // Arrange: Create a GameObject with Canvas component and scale animation
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // Full scale
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Scale down
            observer.Sample(gameObject); // Sample after animation

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: ScaleAnimationObserved should be true, AlphaAnimationObserved should be false
            Assert.IsTrue(result.ScaleAnimationObserved, "Scale animation should be marked as observed");
            Assert.IsFalse(result.AlphaAnimationObserved, "Alpha animation should not be marked as observed (Canvas has no CanvasGroup)");
        }

        [Test]
        public void DefaultDismissalPolicy_CanvasGroup_View_Shows_ScaleObserved_Flag_When_Scale_Animated()
        {
            // Arrange: Create a GameObject with CanvasGroup component and scale animation only
            var gameObject = new GameObject("TestView");
            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // Full scale, full alpha
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Scale down
            observer.Sample(gameObject); // Sample after scale animation

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: ScaleAnimationObserved should be true, AlphaAnimationObserved should be false
            Assert.IsTrue(result.ScaleAnimationObserved, "Scale animation should be marked as observed");
            Assert.IsFalse(result.AlphaAnimationObserved, "Alpha animation should not be marked as observed (no alpha change)");
        }

        [Test]
        public void DefaultDismissalPolicy_CanvasGroup_View_Shows_AlphaObserved_Flag_When_Alpha_Animated()
        {
            // Arrange: Create a GameObject with CanvasGroup component and alpha animation only
            var gameObject = new GameObject("TestView");
            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // Full scale, full alpha
            canvasGroup.alpha = 0.0f; // Fade out
            observer.Sample(gameObject); // Sample after alpha animation

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: AlphaAnimationObserved should be true, ScaleAnimationObserved should be false
            Assert.IsTrue(result.AlphaAnimationObserved, "Alpha animation should be marked as observed");
            Assert.IsFalse(result.ScaleAnimationObserved, "Scale animation should not be marked as observed (no scale change)");
        }

        [Test]
        public void DefaultDismissalPolicy_Diagnostics_Include_Observed_Values_For_Canvas_Rejection()
        {
            // Arrange: Create a GameObject with Canvas component but no animation
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // One sample at scale (1,1,1)
            observer.Sample(gameObject); // Another sample at scale (1,1,1)

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: Result should include observed values and thresholds
            Assert.IsFalse(result.IsValid, "Policy should reject dismissal without observed animation");
            Assert.AreEqual(Vector3.one, result.ObservedFirstScale, "Should capture first scale value");
            Assert.AreEqual(Vector3.one, result.ObservedLastScale, "Should capture last scale value");
            Assert.AreEqual(0.3f, result.ScaleDismissalThreshold, 0.001f, "Should include scale threshold");
            Assert.IsNotNull(result.RejectionReason, "Should provide rejection reason");
            Assert.IsTrue(result.RejectionReason.Contains("scale"), "Rejection reason should mention scale");
        }

        [Test]
        public void DefaultDismissalPolicy_Diagnostics_Include_Observed_Values_For_CanvasGroup_Rejection()
        {
            // Arrange: Create a GameObject with CanvasGroup component but no animation
            var gameObject = new GameObject("TestView");
            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // One sample at scale (1,1,1) and alpha 1.0
            observer.Sample(gameObject); // Another sample at scale (1,1,1) and alpha 1.0

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: Result should include observed values and thresholds
            Assert.IsFalse(result.IsValid, "Policy should reject dismissal without observed animation");
            Assert.AreEqual(Vector3.one, result.ObservedFirstScale, "Should capture first scale value");
            Assert.AreEqual(Vector3.one, result.ObservedLastScale, "Should capture last scale value");
            Assert.AreEqual(1.0f, result.ObservedFirstAlpha, 0.001f, "Should capture first alpha value");
            Assert.AreEqual(1.0f, result.ObservedLastAlpha, 0.001f, "Should capture last alpha value");
            Assert.AreEqual(0.3f, result.ScaleDismissalThreshold, 0.001f, "Should include scale threshold");
            Assert.AreEqual(0.5f, result.AlphaDismissalThreshold, 0.001f, "Should include alpha threshold");
            Assert.IsNotNull(result.RejectionReason, "Should provide rejection reason");
            Assert.IsTrue(result.RejectionReason.Contains("alpha") || result.RejectionReason.Contains("scale"),
                "Rejection reason should mention alpha or scale");
        }

        [Test]
        public void DefaultDismissalPolicy_Diagnostics_Include_Observed_Values_For_Accepted_Dismissal()
        {
            // Arrange: Create a GameObject with CanvasGroup and valid animation
            var gameObject = new GameObject("TestView");
            var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;
            gameObject.transform.localScale = Vector3.one;

            var observer = new AnimationHistoryObserver();
            observer.Sample(gameObject); // Full scale and alpha
            canvasGroup.alpha = 0.0f; // Fade out
            observer.Sample(gameObject); // Sample after animation

            var policy = new DefaultDismissalPolicy();

            // Act: Validate the dismissal
            var result = policy.ValidateDismissal(gameObject, observer);

            // Assert: Result should include observed values for accepted dismissal
            Assert.IsTrue(result.IsValid, "Policy should accept dismissal with observed animation");
            Assert.AreEqual(Vector3.one, result.ObservedFirstScale, "Should capture first scale value");
            Assert.AreEqual(Vector3.one, result.ObservedLastScale, "Should capture last scale value");
            Assert.AreEqual(1.0f, result.ObservedFirstAlpha, 0.001f, "Should capture first alpha value");
            Assert.AreEqual(0.0f, result.ObservedLastAlpha, 0.001f, "Should capture last alpha value");
            Assert.AreEqual(0.3f, result.ScaleDismissalThreshold, 0.001f, "Should include scale threshold");
            Assert.AreEqual(0.5f, result.AlphaDismissalThreshold, 0.001f, "Should include alpha threshold");
        }
    }
}