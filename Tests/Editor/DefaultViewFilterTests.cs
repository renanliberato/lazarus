using NUnit.Framework;
using UnityEngine;
using Lazarus;

namespace Lazarus.Tests
{
    /// <summary>
    /// Tests for the default view filtering behavior.
    /// These are behavior-first tests that verify the public API contracts
    /// for deciding which GameObjects should be tracked.
    /// </summary>
    public class DefaultViewFilterTests
    {
        [Test]
        public void DefaultViewFilter_Accepts_GameObject_With_ViewSuffix_And_Canvas()
        {
            // Arrange: Create a GameObject with "View" suffix and Canvas component
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();

            var filter = new DefaultViewFilter();

            // Act: Check if the filter accepts this GameObject
            bool shouldTrack = filter.ShouldTrack(gameObject);

            // Assert: The filter should accept it
            Assert.IsTrue(shouldTrack, "Default view filter should accept GameObjects ending with 'View' that have Canvas");
        }

        [Test]
        public void DefaultViewFilter_Rejects_GameObject_Without_ViewSuffix()
        {
            // Arrange: Create a GameObject without "View" suffix
            var gameObject = new GameObject("TestPanel");
            gameObject.AddComponent<Canvas>();

            var filter = new DefaultViewFilter();

            // Act: Check if the filter accepts this GameObject
            bool shouldTrack = filter.ShouldTrack(gameObject);

            // Assert: The filter should reject it
            Assert.IsFalse(shouldTrack, "Default view filter should reject GameObjects not ending with 'View'");
        }

        [Test]
        public void DefaultViewFilter_Rejects_GameObject_With_ViewSuffix_But_No_Canvas_Or_CanvasGroup()
        {
            // Arrange: Create a GameObject with "View" suffix but no Canvas or CanvasGroup
            var gameObject = new GameObject("TestView");

            var filter = new DefaultViewFilter();

            // Act: Check if the filter accepts this GameObject
            bool shouldTrack = filter.ShouldTrack(gameObject);

            // Assert: The filter should reject it
            Assert.IsFalse(shouldTrack, "Default view filter should reject GameObjects ending with 'View' but without Canvas or CanvasGroup");
        }

        [Test]
        public void DefaultViewFilter_Accepts_GameObject_With_ViewSuffix_And_CanvasGroup()
        {
            // Arrange: Create a GameObject with "View" suffix and CanvasGroup component
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<CanvasGroup>();

            var filter = new DefaultViewFilter();

            // Act: Check if the filter accepts this GameObject
            bool shouldTrack = filter.ShouldTrack(gameObject);

            // Assert: The filter should accept it
            Assert.IsTrue(shouldTrack, "Default view filter should accept GameObjects ending with 'View' that have CanvasGroup");
        }

        [Test]
        public void DefaultViewFilter_Accepts_GameObject_With_ViewSuffix_And_Canvas_And_CanvasGroup()
        {
            // Arrange: Create a GameObject with "View" suffix and both Canvas and CanvasGroup components
            var gameObject = new GameObject("TestView");
            gameObject.AddComponent<Canvas>();
            gameObject.AddComponent<CanvasGroup>();

            var filter = new DefaultViewFilter();

            // Act: Check if the filter accepts this GameObject
            bool shouldTrack = filter.ShouldTrack(gameObject);

            // Assert: The filter should accept it
            Assert.IsTrue(shouldTrack, "Default view filter should accept GameObjects ending with 'View' that have both Canvas and CanvasGroup");
        }

        [Test]
        public void DefaultViewFilter_Rejects_Null_GameObject()
        {
            // Arrange: Create filter
            var filter = new DefaultViewFilter();

            // Act: Check if the filter accepts null
            bool shouldTrack = filter.ShouldTrack(null);

            // Assert: The filter should reject it
            Assert.IsFalse(shouldTrack, "Default view filter should reject null GameObject");
        }
    }
}