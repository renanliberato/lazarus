using NUnit.Framework;
using UnityEngine;
using Lazarus;

namespace Lazarus.Tests
{
    /// <summary>
    /// Tests for the default dismissal policy behavior.
    /// These are behavior-first tests that verify the public API contracts
    /// for validating dismissal animations.
    /// </summary>
    public class DefaultDismissalPolicyTests
    {
        [Test]
        public void DefaultDismissalPolicy_Accepts_Canvas_Backend_View_With_Scale_Dismissal()
        {
            // Arrange: Create a GameObject with Canvas component
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();

            var policy = new DefaultDismissalPolicy();
            var intent = DismissalIntent.Scale;

            // Act: Validate the dismissal intent
            var result = policy.ValidateDismissal(gameObject, intent);

            // Assert: The policy should accept scale dismissal for Canvas-backed views
            Assert.IsTrue(result.IsValid, "Policy should accept scale dismissal for Canvas-backed views");
        }

        [Test]
        public void DefaultDismissalPolicy_Rejects_Abrupt_Deactivation_For_Canvas_Views_With_Reason()
        {
            // Arrange: Create a GameObject with Canvas component
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();

            var policy = new DefaultDismissalPolicy();
            var intent = DismissalIntent.Abrupt;

            // Act: Validate the dismissal intent
            var result = policy.ValidateDismissal(gameObject, intent);

            // Assert: The policy should reject abrupt deactivation with specific reason
            Assert.IsFalse(result.IsValid, "Policy should reject abrupt deactivation for Canvas-backed views");
            Assert.IsNotNull(result.RejectionReason, "Policy should provide rejection reason");
            Assert.IsTrue(result.RejectionReason.Contains("scale"), "Rejection reason should mention scale dismissal requirement");
        }

        [Test]
        public void DefaultDismissalPolicy_Accepts_CanvasGroup_Backend_View_With_Alpha_Dismissal()
        {
            // Arrange: Create a GameObject with CanvasGroup component
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<CanvasGroup>();

            var policy = new DefaultDismissalPolicy();
            var intent = DismissalIntent.Alpha;

            // Act: Validate the dismissal intent
            var result = policy.ValidateDismissal(gameObject, intent);

            // Assert: The policy should accept alpha dismissal for CanvasGroup-backed views
            Assert.IsTrue(result.IsValid, "Policy should accept alpha dismissal for CanvasGroup-backed views");
        }

        [Test]
        public void DefaultDismissalPolicy_Accepts_CanvasGroup_Backend_View_With_Scale_Dismissal()
        {
            // Arrange: Create a GameObject with CanvasGroup component
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<CanvasGroup>();

            var policy = new DefaultDismissalPolicy();
            var intent = DismissalIntent.Scale;

            // Act: Validate the dismissal intent
            var result = policy.ValidateDismissal(gameObject, intent);

            // Assert: The policy should accept scale dismissal for CanvasGroup-backed views
            Assert.IsTrue(result.IsValid, "Policy should accept scale dismissal for CanvasGroup-backed views");
        }

        [Test]
        public void DefaultDismissalPolicy_Accepts_CanvasGroup_Backend_View_With_Both_Dismissal()
        {
            // Arrange: Create a GameObject with CanvasGroup component
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<CanvasGroup>();

            var policy = new DefaultDismissalPolicy();
            var intent = DismissalIntent.Both;

            // Act: Validate the dismissal intent
            var result = policy.ValidateDismissal(gameObject, intent);

            // Assert: The policy should accept both dismissal for CanvasGroup-backed views
            Assert.IsTrue(result.IsValid, "Policy should accept both alpha and scale dismissal for CanvasGroup-backed views");
        }

        [Test]
        public void DefaultDismissalPolicy_Rejects_Abrupt_Deactivation_For_CanvasGroup_Views_With_Reason()
        {
            // Arrange: Create a GameObject with CanvasGroup component
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<CanvasGroup>();

            var policy = new DefaultDismissalPolicy();
            var intent = DismissalIntent.Abrupt;

            // Act: Validate the dismissal intent
            var result = policy.ValidateDismissal(gameObject, intent);

            // Assert: The policy should reject abrupt deactivation with specific reason
            Assert.IsFalse(result.IsValid, "Policy should reject abrupt deactivation for CanvasGroup-backed views");
            Assert.IsNotNull(result.RejectionReason, "Policy should provide rejection reason");
            Assert.IsTrue(result.RejectionReason.Contains("alpha") || result.RejectionReason.Contains("scale"),
                "Rejection reason should mention alpha or scale dismissal requirement");
        }
    }
}